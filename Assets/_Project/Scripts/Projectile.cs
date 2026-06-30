using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed = 10f;
    private float damage = 25f;
    private bool isActive = false;
    private IObjectPool<Projectile> pool;

    [Header("Settings")]
    [SerializeField] private float maxLifeTime = 5f;
    private float lifeTimer = 0f;

    public void Seek(Transform _target, float _damage, float _speed)
    {
        target = _target;
        damage = _damage;
        speed = _speed;
        isActive = true;
        lifeTimer = 0f;
        gameObject.SetActive(true);
    }

    public void SetPool(IObjectPool<Projectile> pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        if (!isActive) return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifeTime)
        {
            ReturnToPool();
            return;
        }

        if (target == null)
        {
            ReturnToPool();
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(damage);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            isActive = false;
            target = null;
            pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        isActive = false;
        target = null;
    }
}