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

    [Header("Death Settings")]
    public GameObject deathParticlePrefab;
    public List<GameObject> itemObjectsPrefabs = new List<GameObject>();
    public float itemSpawnHeight;
    [Range(0f, 1f)]
    public float itemSpawnChance = 0.5f;

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

        Debug.Log($"{gameObject.name} took {finalDamage} damage. Remaining currentHealth: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    public void ApplyDamageToMainBody(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} main body took {damage} damage. Remaining currentHealth: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed! Awarding {pointsValue} points.");

        // Instantiate death particles
        if (deathParticlePrefab != null)
        {
            Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        }

        // Attempt to spawn an item object
        if (itemObjectsPrefabs.Count > 0 && Random.value <= itemSpawnChance)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, itemSpawnHeight, transform.position.z);
            GameObject selectedItem = itemObjectsPrefabs[Random.Range(0, itemObjectsPrefabs.Count)];
            Instantiate(selectedItem, spawnPosition, Quaternion.identity);
        }

        // Destroy the enemy
        Destroy(gameObject);

        // Award points
        GameManager.Instance.IncreaseScore(pointsValue);
    }
}