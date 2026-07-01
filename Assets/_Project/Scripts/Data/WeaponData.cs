using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float damage = 25f;

    [Header("Weapon Settings")]
    public float fireRate = 1.5f;
    public float attackRange = 7f;

    [Header("Pool Settings")]
    public int defaultPoolSize = 20;
    public int maxPoolSize = 50;

    [Header("Upgrade Settings")]
    public float damageUpgradeAmount = 5f;
    public float fireRateUpgradeMultiplier = 0.9f;
    public float rangeUpgradeAmount = 1f;
    public float speedUpgradeAmount = 2f;
}