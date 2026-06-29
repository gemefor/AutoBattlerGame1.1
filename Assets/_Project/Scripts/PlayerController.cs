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

    // Событие, которое будет сообщать UIManager'у об изменениях
    // Передает: Текущий Уровень, Текущий Опыт, Опыт для следующего уровня
    public event Action<int, int, int> OnExperienceChanged;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // При старте игры сразу обновляем UI стартовыми значениями (1 уровень, 0 опыта)
        OnExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
    }

    private void Update()
    {
        // Сбор ввода от игрока (WASD / Стрелочки)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        // Физическое движение через Rigidbody2D
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log($"Опыт получен: +{amount}. Всего: {currentExperience}/{experienceToNextLevel}");

        // Если набрали нужное количество опыта — повышаем уровень
        if (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }

        // Кликаем по событию, чтобы UI перерисовал цифры на экране
        OnExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;

        // Увеличиваем планку опыта для следующего уровня на 50%
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

        Debug.Log($"УРОВЕНЬ ПОВЫШЕН! Теперь у тебя {currentLevel} уровень.");

        // Сюда мы в следующем шаге добавим появление меню выбора перков
    }
    public void UpgradeSpeed(float multiplier)
    {
        moveSpeed *= multiplier; // Увеличиваем текущую скорость игрока
        Debug.Log($"Скорость игрока увеличена! Новая скорость: {moveSpeed}");
    }

}