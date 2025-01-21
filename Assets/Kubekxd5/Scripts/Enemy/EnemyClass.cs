using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Points Settings")]
    public int pointsValue = 10;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed! Awarding {pointsValue} points.");
        Destroy(gameObject);
    }
}