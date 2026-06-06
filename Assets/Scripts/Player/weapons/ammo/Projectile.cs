using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 15f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 3f;

    [Header("Damage")]
    [SerializeField] private int damage = 10;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable =
            other.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(damage);

        Destroy(gameObject);
    }
}