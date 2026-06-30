using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed = 10f;
    private float damage = 25f;
    private bool isActive = false;

    // Ссылка на пул для возврата
    private IObjectPool<Projectile> pool;

    // Метод для инициализации пули при спавне
    public void Seek(Transform _target, float _damage, float _speed)
    {
        target = _target;
        damage = _damage;
        speed = _speed;
        isActive = true;

        // Активируем объект
        gameObject.SetActive(true);
    }

    // Устанавливаем пул для возврата
    public void SetPool(IObjectPool<Projectile> pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        if (!isActive) return;

        if (target == null)
        {
            ReturnToPool(); // Возвращаем в пул вместо Destroy
            return;
        }

        // Направление к цели
        Vector3 direction = (target.position - transform.position).normalized;

        // Двигаем пулю вперед
        transform.position += direction * speed * Time.deltaTime;

        // Поворачиваем пулю по направлению движения (для визуала)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(damage);
            ReturnToPool(); // Возвращаем в пул вместо Destroy
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
            // Если пула нет (безопасное падение)
            Destroy(gameObject);
        }
    }

    // Сброс состояния при возврате в пул
    private void OnDisable()
    {
        isActive = false;
        target = null;
    }
}