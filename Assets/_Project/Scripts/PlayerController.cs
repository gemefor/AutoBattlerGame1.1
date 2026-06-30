using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Experience & Levels")]
    private int currentExperience = 0;
    private int experienceToNextLevel = 10;
    private int currentLevel = 1;
    
    [Header("Experience Settings")]
    [SerializeField] private float levelUpMultiplier = 1.5f; 

   
    public event Action<int, int, int> OnExperienceChanged;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on PlayerController!");
        }

        // Инициализация UI
        OnExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
    }

    private void Update()
    {
        // Получение ввода
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        
    }

    private void FixedUpdate()
    {
        // Движение через Rigidbody2D с нормализацией вектора
        if (moveInput.magnitude > 0.1f) 
        {
            rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0) // Добавил проверку на положительное значение
        {
            Debug.LogWarning("Experience amount must be positive!");
            return;
        }

        currentExperience += amount;
        Debug.Log($"Получено опыта: +{amount}. Текущий прогресс: {currentExperience}/{experienceToNextLevel}");

        // Проверка на повышение уровня
        while (currentExperience >= experienceToNextLevel) // Изменил if на while для множественных уровней
        {
            LevelUp();
        }

        // Обновление UI
        OnExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;

        // Увеличение требуемого опыта с использованием настраиваемого множителя
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * levelUpMultiplier);

        Debug.Log($"Повышение уровня! Текущий уровень: {currentLevel}. Нужно опыта для следующего: {experienceToNextLevel}");
        
        // Можно добавить эффект повышения уровня
        // Например, небольшое увеличение скорости при каждом уровне
        // UpgradeSpeed(1.05f); // Разкомментировать если нужно
    }

    public void UpgradeSpeed(float multiplier)
    {
        if (multiplier <= 0) 
        {
            Debug.LogWarning("Speed multiplier must be positive!");
            return;
        }
        
        moveSpeed *= multiplier;
        Debug.Log($"Скорость увеличена! Текущая скорость: {moveSpeed}");
    }
}