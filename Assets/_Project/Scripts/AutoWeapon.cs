using UnityEngine;
using UnityEngine.Pool;

public class AutoWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private int defaultPoolSize = 20;
    [SerializeField] private int maxPoolSize = 50;

    private float fireCountdown = 0f;
    private IObjectPool<Projectile> projectilePool;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        projectilePool = new ObjectPool<Projectile>(
            createFunc: CreateProjectile,
            actionOnGet: OnGetProjectile,
            actionOnRelease: OnReleaseProjectile,
            actionOnDestroy: OnDestroyProjectile,
            collectionCheck: true,
            defaultCapacity: defaultPoolSize,
            maxSize: maxPoolSize
        );
    }

    private Projectile CreateProjectile()
    {
        GameObject projGO = Instantiate(projectilePrefab);
        Projectile projectile = projGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetPool(projectilePool);
        }

        // Изначально выключаем
        projGO.SetActive(false);

        return projectile;
    }

    private void OnGetProjectile(Projectile projectile)
    {
        // Объект уже активируется в методе Seek
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.identity;
    }

    private void OnReleaseProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    private void Update()
    {
        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            Transform nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                Shoot(nearestEnemy);
                fireCountdown = fireRate;
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            Enemy[] enemyComponents = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            enemies = new GameObject[enemyComponents.Length];
            for (int i = 0; i < enemyComponents.Length; i++)
                enemies[i] = enemyComponents[i].gameObject;
        }

        Transform closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeInHierarchy) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance && distanceToEnemy <= attackRange)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    private void Shoot(Transform enemyTarget)
    {
        Projectile projectile = projectilePool.Get();

        if (projectile != null)
        {
            projectile.Seek(enemyTarget, damage, projectileSpeed);
        }
    }

    public void UpgradeDamage(float amount)
    {
        damage += amount;
        Debug.Log($"Урон оружия повышен! Текущий урон: {damage}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}