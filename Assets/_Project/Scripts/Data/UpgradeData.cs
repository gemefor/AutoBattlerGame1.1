using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Weapons/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName = "Damage Upgrade";
    public UpgradeType upgradeType;
    public float value = 5f;
}

public enum UpgradeType
{
    Damage,
    FireRate,
    Range,
    ProjectileSpeed,
    MoveSpeed
}