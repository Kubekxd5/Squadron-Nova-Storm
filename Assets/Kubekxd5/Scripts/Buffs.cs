using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [Header("Buff skibidi")]
    public BuffType buffType;
    public Sprite icon;
    public float dropChance;
    public float value;

    private void OnValidate()
    {
        buffName = buffType.ToString();
    }

    [HideInInspector]
    public string buffName;
}

public enum BuffType
{
    Shield,
    SpeedBoost,
    ShieldRegenRate,
    DamageReduction,
    WeaponDamage,
    FireRate,
    Ammo,
    ProjectileAmount,
    PiercingDamage,
    GodMode
}

public class BuffManager : MonoBehaviour
{
    [Header("Buff Config")]
    public Buff[] buffs;
    public GameObject buffPrefab;
    public Transform buffDropPoint;

    private void DropBuff()
    {
        foreach (var buff in buffs)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= buff.dropChance)
            {
                SpawnBuff(buff);
            }
        }
    }

    private void SpawnBuff(Buff buff)
    {
        if (buffPrefab == null || buffDropPoint == null) return;

        GameObject buffInstance = Instantiate(buffPrefab, buffDropPoint.position, Quaternion.identity);
        BuffDisplay buffDisplay = buffInstance.GetComponent<BuffDisplay>();

        if (buffDisplay != null)
        {
            buffDisplay.SetBuff(buff);
        }
    }

    public void OnEnemyDefeated()
    {
        DropBuff();
    }
}

public class BuffDisplay : MonoBehaviour
{
    [Header("Buff UI")]
    public SpriteRenderer iconRenderer;

    private Buff currentBuff;

    public void SetBuff(Buff buff)
    {
        currentBuff = buff;
        if (iconRenderer != null)
        {
            iconRenderer.sprite = buff.icon;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyBuff(other.gameObject);
            ApplyGod(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void ApplyBuff(GameObject player)
    {
        if (currentBuff == null) return;

        ShipController shipController = player.GetComponent<ShipController>();
        WeaponController weaponController = player.GetComponentInChildren<WeaponController>();

        if (shipController != null)
        {
            ApplyBuffToShip(shipController);
        }

        if (weaponController != null)
        {
            ApplyBuffToWeapon(weaponController);
        }
    }

    private void ApplyBuffToShip(ShipController shipController)
    {
        switch (currentBuff.buffType)
        {
            case BuffType.Shield:
                shipController.energyShield += currentBuff.value;
                break;

            case BuffType.SpeedBoost:
                shipController.speed = Mathf.Min(shipController.speed + currentBuff.value, shipController.maxSpeed);
                break;

            case BuffType.ShieldRegenRate:
                shipController.shieldRegenRate += currentBuff.value;
                break;

            case BuffType.DamageReduction:
                shipController.damageReduction += currentBuff.value;
                break;

            default:
                Debug.LogWarning($"Oj soooooory nie dziala: {currentBuff.buffName}");
                break;
        }
    }

    private void ApplyBuffToWeapon(WeaponController weaponController)
    {
        switch (currentBuff.buffType)
        {
            case BuffType.WeaponDamage:
                weaponController.damageValue += currentBuff.value;
                break;

            case BuffType.FireRate:
                weaponController.fireRate = Mathf.Max(0.1f, weaponController.fireRate - currentBuff.value);
                break;

            case BuffType.Ammo:
                weaponController.ammoMax += Mathf.RoundToInt(currentBuff.value);
                break;

            case BuffType.ProjectileAmount:
                weaponController.projectileAmount += Mathf.RoundToInt(currentBuff.value);
                break;

            case BuffType.PiercingDamage:
                weaponController.piercing += currentBuff.value;
                break;

            default:
                Debug.LogWarning($"Oj soooooory nie dziala: {currentBuff.buffName}");
                break;
        }
    }

    private void ApplyGod(GameObject player)
    {
        if (currentBuff == null) return;

        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            switch (currentBuff.buffType)
            {
                case BuffType.GodMode:
                    playerStats.EnableGodMode(currentBuff.value);
                    break;
                default:
                    Debug.LogWarning($"Oj soooooory nie dziala: {currentBuff.buffName}");
                    break;
            }
        }
    }
}

public class PlayerStats : MonoBehaviour
{
    [Header("god mode?")]
    private bool isGodModeActive = false;
    public bool isDead { get; private set; } = false;
    private bool godModeEnabled => isGodModeActive;
    public GameObject deathUI;

    public void EnableGodMode(float duration)
    {
        if (!isGodModeActive)
        {
            isGodModeActive = true;
            StartCoroutine(GodModeCoroutine(duration));
        }
    }

    private System.Collections.IEnumerator GodModeCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        isGodModeActive = false;
    }

    public void TakeDamage(float amount)
    {
        if (godModeEnabled) return;
        ShipController shipController = GetComponent<ShipController>();
        shipController.health -= amount;
        shipController.health = Mathf.Max(shipController.health, 0);
        if (shipController.health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }
    }
}
