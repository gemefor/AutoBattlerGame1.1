using UnityEngine;
using UnityEngine.Pool; // Обязательно добавляем это пространство имен!

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy 1 (Standard)")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Enemy 2 (Fast / Advanced)")]
    [SerializeField] private Enemy enemyPrefab2;     // Префаб второго монстра
    [SerializeField] private EnemyData enemyData2;     // Данные второго монстра (ScriptableObject)
    [SerializeField] private float spawnInterval2 = 1.5f; // Интервал спавна для второго монстра
    [SerializeField] private float enemy2Delay = 15f;    // Задержка перед началом спавна (15 секунд)

    [Header("Spawn Zone (Custom Arena Bounds)")]
    [SerializeField] private float minX = -9f;
    [SerializeField] private float maxX = 9f;
    [SerializeField] private float minY = -9f;
    [SerializeField] private float maxY = 9f;

    private float spawnTimer;
    private float spawnTimer2;
    private float totalGameTime; // Общий таймер игры

    // Два раздельных пула под каждого монстра
    private IObjectPool<Enemy> enemyPool1;
    private IObjectPool<Enemy> enemyPool2;

    private void Awake()
    {
        // Инициализируем пул для первого врага
        enemyPool1 = new ObjectPool<Enemy>(
            createFunc: CreateEnemy1Instance,
            actionOnGet: OnTakeEnemyFromPool,
            actionOnRelease: OnReturnEnemyToPool,
            actionOnDestroy: OnDestroyEnemy1InPool,
            collectionCheck: true,
            defaultCapacity: 20,
            maxSize: 50
        );

        enemyPool2 = new ObjectPool<Enemy>(
            createFunc: CreateEnemy2Instance,
            actionOnGet: OnTakeEnemyFromPool,
            actionOnRelease: OnReturnEnemyToPool,
            actionOnDestroy: OnDestroyEnemy2InPool,
            collectionCheck: true,
            defaultCapacity: 15,
            maxSize: 40
        );
    }

    private void Update()
    {
        // Наращиваем общее время игры каждый кадр
        totalGameTime += Time.deltaTime;

        // Логика спавна первого врага (работает с самого начала)
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy(enemyPool1, enemyData);
            spawnTimer = 0f;
        }

        // Логика спавна второго врага (включается строго после 15 секунд игры)
        if (totalGameTime >= enemy2Delay)
        {
            spawnTimer2 += Time.deltaTime;
            if (spawnTimer2 >= spawnInterval2)
            {
                SpawnEnemy(enemyPool2, enemyData2);
                spawnTimer2 = 0f;
            }
        }
    }

    private Enemy CreateEnemy1Instance()
    {
        Enemy enemy = Instantiate(enemyPrefab);
        enemy.OnEnemyDied += HandleEnemy1Death;
        return enemy;
    }
    private void OnDestroyEnemy1InPool(Enemy enemy)
    {
        enemy.OnEnemyDied -= HandleEnemy1Death;
        Destroy(enemy.gameObject);
    }
    private void HandleEnemy1Death(Enemy enemy)
    {
        enemyPool1.Release(enemy);
    }

    private Enemy CreateEnemy2Instance()
    {
        Enemy enemy = Instantiate(enemyPrefab2);
        enemy.OnEnemyDied += HandleEnemy2Death;
        return enemy;
    }
    private void OnDestroyEnemy2InPool(Enemy enemy)
    {
        enemy.OnEnemyDied -= HandleEnemy2Death;
        Destroy(enemy.gameObject);
    }
    private void HandleEnemy2Death(Enemy enemy)
    {
        enemyPool2.Release(enemy);
    }

    private void OnTakeEnemyFromPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);

        if (enemy.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero; // или rb.velocity для старых версий
            rb.angularVelocity = 0f;
        }
    }

    private void OnReturnEnemyToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void SpawnEnemy(IObjectPool<Enemy> pool, EnemyData data)
    {
        // Проверяем, назначены ли префабы, чтобы избежать ошибок
        if (enemyPrefab == null || enemyPrefab2 == null) return;

        // Расчет случайной позиции внутри твоей зоны
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        // Достаем готового врага из нужного пула
        Enemy enemy = pool.Get();

        // Перемещаем его в нужную точку
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = Quaternion.identity;

        // Находим игрока на сцене
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Инициализируем конкретными данными (первого или второго типа)
        if (playerTransform != null && data != null)
        {
            enemy.Initialize(data, playerTransform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 1f);
        Gizmos.DrawWireCube(center, size);
    }
}