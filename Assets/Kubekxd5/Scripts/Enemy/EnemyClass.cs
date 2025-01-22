using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;

    [Header("Points Settings")]
    public int pointsValue = 10;

    [Header("Body Parts")]
    public List<BodyPart> bodyParts = new List<BodyPart>();

    public GameObject deathParticlePrefab;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        foreach (var part in bodyParts)
        {
            part.Initialize();
            part.enemyClassParent = this;
        }
    }

    public void TakeDamage(float damage, string bodyPartName = null)
    {
        float finalDamage = damage;

        if (!string.IsNullOrEmpty(bodyPartName))
        {
            var part = bodyParts.Find(p => p.partName == bodyPartName);
            if (part != null)
            {
                finalDamage *= part.ApplyDamage(damage);
            }
            else
            {
                Debug.LogWarning($"Body part {bodyPartName} not found on {gameObject.name}.");
            }
        }

        // Apply remaining damage to the main body (if no body part was hit or body part multiplier is applied)
        ApplyDamageToMainBody(finalDamage);

        Debug.Log($"{gameObject.name} took {finalDamage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    public void ApplyDamageToMainBody(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} main body took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed! Awarding {pointsValue} points.");

        if (deathParticlePrefab != null)
        {
            Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);

        GameManager.Instance.IncreaseScore(pointsValue);
    }
}
