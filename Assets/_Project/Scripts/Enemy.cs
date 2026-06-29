using UnityEngine;
using System;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private EnemyData data;
    private Transform playerTransform;
    private float currentHealth;
    private Rigidbody2D rb;

    public event Action<Enemy> OnEnemyDied;

    [Header("Drop Settings")]
    [SerializeField] private ExperienceGem gemPrefab; // Теперь ссылаемся на ExperienceGem
    [SerializeField] private int defaultGemPoolSize = 20;
    [SerializeField] private int maxGemPoolSize = 50;

    // Статический пул для всех врагов
    private static IObjectPool<ExperienceGem> gemPool;
    private static ExperienceGem gemPrefabStatic;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Инициализируем пул гемов, если он еще не создан
        if (gemPool == null && gemPrefab != null)
        {
            gemPrefabStatic = gemPrefab;
            gemPool = new ObjectPool<ExperienceGem>(
                createFunc: CreateGem,
                actionOnGet: OnGetGem,
                actionOnRelease: OnReleaseGem,
                actionOnDestroy: OnDestroyGem,
                collectionCheck: true,
                defaultCapacity: defaultGemPoolSize,
                maxSize: maxGemPoolSize
            );
        }
    }

    private ExperienceGem CreateGem()
    {
        ExperienceGem gem = Instantiate(gemPrefabStatic);
        gem.gameObject.SetActive(false);
        gem.SetPool(gemPool);
        return gem;
    }

    private void OnGetGem(ExperienceGem gem)
    {
        gem.gameObject.SetActive(true);
    }

    private void OnReleaseGem(ExperienceGem gem)
    {
        gem.gameObject.SetActive(false);
        gem.transform.position = Vector3.zero;
        gem.transform.rotation = Quaternion.identity;
    }

    private void OnDestroyGem(ExperienceGem gem)
    {
        Destroy(gem.gameObject);
    }

    public void Initialize(EnemyData enemyData, Transform targetPlayer)
    {
        data = enemyData;
        playerTransform = targetPlayer;
        currentHealth = data.maxHealth;

        if (TryGetComponent<SpriteRenderer>(out var renderer))
        {
            renderer.color = Color.red;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform == null || data == null) return;

        float distance = Vector2.Distance(rb.position, playerTransform.position);

        if (distance > 0.1f)
        {
            Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * data.moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Спавним гем из пула
        if (gemPool != null)
        {
            ExperienceGem gem = gemPool.Get();
            gem.transform.position = transform.position;
            gem.transform.rotation = Quaternion.identity;
        }

        OnEnemyDied?.Invoke(this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (data == null) return;

        if (collision.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.TakeDamage(data.damage * Time.deltaTime);
        }
    }
}