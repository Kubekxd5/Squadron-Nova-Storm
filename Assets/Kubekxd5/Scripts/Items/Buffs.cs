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

    private void Start()
    {
        // Start a timer to despawn the buff after the specified time
        Invoke(nameof(DespawnBuff), despawnTime);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Collided with: " + other.name);
        if (other.CompareTag("PlayerShip"))
        {
            ShipController playerShip = other.GetComponent<ShipController>();
            Debug.Log("got with: " + playerShip);
            if (playerShip != null)
            {
                Debug.Log("for sure: " + playerShip);
                ApplyBuff(playerShip);
                Destroy(gameObject);
            }
        }
    }

    private void ApplyBuff(ShipController ship)
    {
        switch (buffType)
        {
            case BuffType.HealthRestore:
                ship.currentHealth = Mathf.Min(ship.currentHealth + buffValue, ship.maxHealth);
                Debug.Log($"Health restored by {buffValue}. Current health: {ship.currentHealth}");
                break;

            case BuffType.SpeedBoost:
                StartCoroutine(TemporarySpeedBoost(ship));
                break;

            case BuffType.DamageReduction:
                ship.damageReduction += buffValue;
                Debug.Log($"Damage reduction increased by {buffValue}%. Current reduction: {ship.damageReduction}%");
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
        float originalSpeed = ship.speed;
        ship.speed *= 1 + (buffValue / 100f);
        Debug.Log($"Speed boosted by {buffValue}%. New speed: {ship.speed}");

        yield return new WaitForSeconds(duration);

        ship.speed = originalSpeed;
        Debug.Log($"Speed boost expired. Speed reverted to: {ship.speed}");
    }

    private System.Collections.IEnumerator TemporaryImmortality(ShipController ship)
    {
        ship.isImmortal = true;
        Debug.Log($"Immortality granted for {duration} seconds.");

        yield return new WaitForSeconds(duration);

        ship.isImmortal = false;
        Debug.Log("Immortality expired.");
    }

    private System.Collections.IEnumerator TemporaryScoreMultiplier()
    {
        int defaultMultiplier = GameManager.Instance.scoreMultiplier;
        GameManager.Instance.scoreMultiplier = buffValue; // Apply multiplier
        Debug.Log($"Score multiplier set to {GameManager.Instance.scoreMultiplier} for {duration} seconds.");

        yield return new WaitForSeconds(duration);

        GameManager.Instance.scoreMultiplier = defaultMultiplier; // Revert multiplier
        Debug.Log("Score multiplier expired.");
    }

    private void DespawnBuff()
    {
        Debug.Log($"Buff {buffType} despawned after {despawnTime} seconds.");
        Destroy(gameObject);
    }
}
