using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Сообщаем всем подписчикам (например, будущему UI), что здоровье изменилось
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"Игроку нанесено {damageAmount} урона. Текущее здоровье: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            UIManager uiManager = FindFirstObjectByType<UIManager>(); 

            if (uiManager != null)
            {
                uiManager.TriggerGameOver();
            }

            // Выключаем игрока или логику его перемещения, чтобы враги его больше не били
            gameObject.SetActive(false);
            Die();
        }
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
        // Ограничиваем здоровье, чтобы оно не стало больше максимального
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"Игрок вылечен на {amount} HP. Текущее здоровье: {currentHealth}/{maxHealth}");
    }
    private void Die()
    {
        Debug.Log("Игра окончена! Вы погибли.");
        OnPlayerDied?.Invoke();

        // Временная жесткая остановка игры при поражении (ставим время на паузу)
        Time.timeScale = 0f;
    }
}