using UnityEngine;
using UnityEngine.Pool;

public class AutoWeapon : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private UpgradeData[] availableUpgrades;

    private float fireCountdown = 0f;
    private IObjectPool<Projectile> projectilePool;

    // Текущие значения (могут меняться через апгрейды)
    private float currentDamage;
    private float currentFireRate;
    private float currentAttackRange;
    private float currentProjectileSpeed;

    private void Awake()
    {
        if (weaponData == null)
        {
            Debug.LogError("WeaponData is not assigned!", this);
            return;
        }

        InitializeStats();
        InitializePool();
    }

    private void InitializeStats()
    {
        currentDamage = weaponData.damage;
        currentFireRate = weaponData.fireRate;
        currentAttackRange = weaponData.attackRange;
        currentProjectileSpeed = weaponData.projectileSpeed;
    }

    private void InitializePool()
    {
        projectilePool = new ObjectPool<Projectile>(
            createFunc: CreateProjectile,
            actionOnGet: OnGetProjectile,
            actionOnRelease: OnReleaseProjectile,
            actionOnDestroy: OnDestroyProjectile,
            collectionCheck: true,
            defaultCapacity: weaponData.defaultPoolSize,
            maxSize: weaponData.maxPoolSize
        );
    }

    private Projectile CreateProjectile()
    {
        GameObject projGO = Instantiate(weaponData.projectilePrefab);
        Projectile projectile = projGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetPool(projectilePool);
        }

        projGO.SetActive(false);
        return projectile;
    }

    private void OnGetProjectile(Projectile projectile)
    {
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
                fireCountdown = currentFireRate;
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

            if (distanceToEnemy < shortestDistance && distanceToEnemy <= currentAttackRange)
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
            projectile.Seek(enemyTarget, currentDamage, currentProjectileSpeed);
        }
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return;

        switch (upgrade.upgradeType)
        {
            case UpgradeType.Damage:
                currentDamage += upgrade.value;
                Debug.Log($"Damage upgraded! Current damage: {currentDamage}");
                break;

            case UpgradeType.FireRate:
                currentFireRate *= upgrade.value;
                Debug.Log($"Fire Rate upgraded! Current fire rate: {currentFireRate}");
                break;

            case UpgradeType.Range:
                currentAttackRange += upgrade.value;
                Debug.Log($"Range upgraded! Current range: {currentAttackRange}");
                break;

            case UpgradeType.ProjectileSpeed:
                currentProjectileSpeed += upgrade.value;
                Debug.Log($"Projectile Speed upgraded! Current speed: {currentProjectileSpeed}");
                break;
        }
    }

    // Геттеры для UI
    public float GetDamage() => currentDamage;
    public float GetFireRate() => currentFireRate;
    public float GetRange() => currentAttackRange;
    public float GetProjectileSpeed() => currentProjectileSpeed;

    public UpgradeData[] GetAvailableUpgrades() => availableUpgrades;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentAttackRange);
    }
}