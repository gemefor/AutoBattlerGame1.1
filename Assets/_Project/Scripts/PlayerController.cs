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

        OnExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Experience amount must be positive!");
            return;
        }

        currentExperience += amount;
        Debug.Log($"Получено опыта: +{amount}. Текущий прогресс: {currentExperience}/{experienceToNextLevel}");

        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }

        OnExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * levelUpMultiplier);

        Debug.Log($"Повышение уровня! Текущий уровень: {currentLevel}. Нужно опыта для следующего: {experienceToNextLevel}");
    }

    // Применение апгрейдов
    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogWarning("UpgradeData is null!");
            return;
        }

        switch (upgrade.upgradeType)
        {
            case UpgradeType.MoveSpeed:
                moveSpeed += upgrade.value;
                Debug.Log($"Speed upgraded! Current speed: {moveSpeed}");
                break;

            case UpgradeType.Damage:
                AutoWeapon weapon = FindFirstObjectByType<AutoWeapon>();
                if (weapon != null)
                {
                    weapon.ApplyUpgrade(upgrade);
                }
                break;

            default:
                Debug.LogWarning($"Unhandled upgrade type: {upgrade.upgradeType}");
                break;
        }
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