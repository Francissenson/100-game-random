using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Debug.Log("EnemyProjectile Awake");
    }

    public void Initialize(
        Vector2 direction,
        float speed,
        int damage,
        float lifetime)
    {
        this.damage = damage;

        rb.linearVelocity =
            direction.normalized * speed;

        Destroy(gameObject, lifetime);

        Debug.Log("Enemy Projectile Fired");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile Hit: " + other.name);

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                other.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogWarning("PlayerHealth Missing");
            }
            else
            {
                playerHealth.TakeDamage(damage);

                Debug.Log("Enemy Projectile Hit Player");
            }
        }

        Debug.Log("Enemy Projectile Destroyed On Collision");

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Debug.Log("Enemy Projectile Destroyed");
    }
}