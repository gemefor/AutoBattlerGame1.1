using UnityEngine;
using UnityEngine.Pool;

public class ExperienceGem : MonoBehaviour
{
    [Header("Gem Settings")]
    [SerializeField] private int expValue = 1; // Сколько опыта дает этот кристалл

    private IObjectPool<ExperienceGem> pool;
    private float lifetime = 10f; // Время жизни гема на сцене
    private float timer;
    private bool isCollected = false;

    // Метод для установки пула
    public void SetPool(IObjectPool<ExperienceGem> pool)
    {
        this.pool = pool;
    }

    private void OnEnable()
    {
        // Сбрасываем состояние при активации
        isCollected = false;
        timer = lifetime;
    }

    private void Update()
    {
        // Автоматический возврат в пул через время
        if (!isCollected)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ReturnToPool();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.TryGetComponent<PlayerController>(out var player))
        {
            player.AddExperience(expValue);
            isCollected = true;
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            // Безопасное падение
            Destroy(gameObject);
        }
    }

    // Метод для установки значения опыта (если нужно менять)
    public void SetExpValue(int value)
    {
        expValue = value;
    }
}