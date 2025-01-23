using UnityEngine;

public class Buff : MonoBehaviour
{
    public enum BuffType
    {
        HealthRestore,
        SpeedBoost,
        DamageReduction,
        Immortality,
        ScoreMultiplier // New buff type
    }

    [Header("Buff Settings")]
    public BuffType buffType;
    public int buffValue; // The value of the buff (e.g., +50 currentHealth, +20 speed, 2x score multiplier)
    public float duration;  // Duration of temporary buffs (e.g., speed boost, immortality, score multiplier)

    [Header("Buff Lifetime")]
    public float despawnTime = 15f; // Time before the buff despawns if not collected
    public float detectionRadius = 1f; // The detection radius for nearby ships

    private MeshRenderer meshRenderer;
    private Collider buffCollider;
    private bool isEffectActive = false;

    private void Start()
    {
        // Cache the MeshRenderer and Collider
        meshRenderer = GetComponent<MeshRenderer>();
        buffCollider = GetComponent<Collider>();

        // Start a timer to despawn the buff after the specified time
        Invoke(nameof(DespawnBuff), despawnTime);
    }

    private void Update()
    {
        if (!isEffectActive) // Only check if the effect is not already active
        {
            CheckForPlayerShip();
        }
    }

    private void CheckForPlayerShip()
    {
        // Use Physics.OverlapSphere to detect nearby colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("PlayerShip"))
            {
                ShipController playerShip = collider.GetComponent<ShipController>();
                if (playerShip != null)
                {
                    Debug.Log($"Buff detected PlayerShip: {playerShip.shipName}");
                    ApplyBuff(playerShip);

                    DisableBuffVisuals(collider.gameObject);

                    CancelInvoke(nameof(DespawnBuff));
                    break;
                }
            }
        }
    }

    private void ApplyBuff(ShipController ship)
    {
        switch (buffType)
        {
            case BuffType.HealthRestore:
                ship.currentHealth = Mathf.Clamp(ship.currentHealth + buffValue, 0, ship.maxHealth);
                Debug.Log($"Health restored by {buffValue}. Current health: {ship.currentHealth}");
                Destroy(gameObject); // Destroy immediately for non-temporary buffs
                break;

            case BuffType.SpeedBoost:
                StartCoroutine(TemporarySpeedBoost(ship));
                break;

            case BuffType.DamageReduction:
                ship.damageReduction += buffValue;
                Debug.Log($"Damage reduction increased by {buffValue}%. Current reduction: {ship.damageReduction}%");
                Destroy(gameObject); // Destroy immediately for non-temporary buffs
                break;

            case BuffType.Immortality:
                StartCoroutine(TemporaryImmortality(ship));
                break;

            case BuffType.ScoreMultiplier:
                StartCoroutine(TemporaryScoreMultiplier());
                break;
        }
    }

    private System.Collections.IEnumerator TemporarySpeedBoost(ShipController ship)
    {
        isEffectActive = true; // Mark effect as active
        float originalSpeed = ship.speed;
        ship.speed *= 1 + (buffValue / 100f);
        ship.maneuverability *= 1 + (buffValue / 100f);
        Debug.Log($"Speed boosted by {buffValue}%. New speed: {ship.speed}");

        yield return new WaitForSeconds(duration);

        ship.speed = originalSpeed;
        Debug.Log($"Speed boost expired. Speed reverted to: {ship.speed}");
        Destroy(gameObject); // Destroy buff after effect ends
    }

    private System.Collections.IEnumerator TemporaryImmortality(ShipController ship)
    {
        isEffectActive = true;
        ship.isImmortal = true;
        Debug.Log($"Immortality granted for {duration} seconds.");

        yield return new WaitForSeconds(duration);

        ship.isImmortal = false;
        Debug.Log("Immortality expired.");
        Destroy(gameObject); // Destroy buff after effect ends
    }

    private System.Collections.IEnumerator TemporaryScoreMultiplier()
    {
        isEffectActive = true; // Mark effect as active
        int defaultMultiplier = GameManager.Instance.scoreMultiplier;
        GameManager.Instance.scoreMultiplier = buffValue; // Apply multiplier
        Debug.Log($"Score multiplier set to {GameManager.Instance.scoreMultiplier} for {duration} seconds.");

        yield return new WaitForSeconds(duration);

        GameManager.Instance.scoreMultiplier = defaultMultiplier; // Revert multiplier
        Debug.Log("Score multiplier expired.");
        Destroy(gameObject);
    }

    private void DisableBuffVisuals(GameObject shipPosition)
    {
        transform.parent = shipPosition.transform;
        transform.localPosition = Vector3.zero;
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (buffCollider != null) buffCollider.enabled = false;
        Debug.Log("Buff visuals and collider disabled.");
    }

    private void DespawnBuff()
    {
        Debug.Log($"Buff {buffType} despawned after {despawnTime} seconds.");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}