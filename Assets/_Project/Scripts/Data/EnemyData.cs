using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Base Stats")]
    public string enemyName;
    public float maxHealth;
    public float moveSpeed;

    [Header("Combat Stats")]
    public float damage;
    public float attackCooldown;

}