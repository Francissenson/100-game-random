using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Debug.Log("Projectile Awake");
    }

    public void Initialize(
        Vector2 direction,
        int damage,
        float speed,
        float lifeTime)
    {
        this.damage = damage;

        rb.linearVelocity =
            direction.normalized * speed;

        Destroy(gameObject, lifeTime);

        Debug.Log(
            $"Projectile Initialized | Damage:{damage}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Projectile Hit: {other.name}");

        if (other.gameObject == gameObject)
            return;

        if (other.CompareTag("Player"))
            return;

        if (other.GetComponent<Projectile>() != null)
            return;

        IDamageable damageable =
            other.GetComponent<IDamageable>();

        if (damageable == null)
        {
            damageable =
                other.GetComponentInParent<IDamageable>();
        }

        if (damageable != null)
        {
            Debug.Log($"Damaging {other.name}");

            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Debug.Log("Projectile Destroyed");
    }
}