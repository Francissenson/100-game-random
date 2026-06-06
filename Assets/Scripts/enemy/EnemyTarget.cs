using UnityEngine;

public sealed class EnemyTarget : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage)
    {
        Debug.Log($"{name} took {damage} damage.");
    }
}