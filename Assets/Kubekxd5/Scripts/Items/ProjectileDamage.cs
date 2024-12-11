using UnityEngine;

public class Projectile : MonoBehaviour
{
    public WeaponController weaponController;
    private ParticleSystem _particleSystem;

    private void Start()
    {
        weaponController = gameObject.GetComponentInParent<WeaponController>();
        _particleSystem = gameObject.GetComponent<ParticleSystem>();

        if (weaponController != null)
        {
            float calculatedLifetime = weaponController.projectileSpeed > 0 ? weaponController.range / weaponController.projectileSpeed : 0.1f;

            var mainModule = _particleSystem.main;
            mainModule.startLifetime = calculatedLifetime;
            mainModule.startSpeed = weaponController.projectileSpeed;

            var emissionModule = _particleSystem.emission;
            emissionModule.rateOverTime = weaponController.projectileInterval;
            
            emissionModule.burstCount = weaponController.projectileAmount;
        }
        else
        {
            Debug.LogWarning("WeaponController reference is missing in Projectile script.");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"Particle collision detected with {other.name} on layer {LayerMask.LayerToName(other.layer)}");
        if (weaponController != null && weaponController.isEquippedByPlayer && other.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyTower enemy = other.GetComponent<EnemyTower>();
            if (enemy != null)
            {
                float actualDamage = weaponController.damageValue * weaponController.damageMultiplier;
                enemy.TakeDamage(actualDamage);
                Debug.Log($"Particle hit {other.name} and dealt {actualDamage} damage.");
            }
        }
    }
}