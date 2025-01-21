using UnityEngine;

public class Projectile : MonoBehaviour
{
    public WeaponController weaponController;
    public ParticleSystem[] subEmmiters;
    private ParticleSystem _particleSystem;

    private void Start()
    {
        weaponController = gameObject.GetComponentInParent<WeaponController>();
        _particleSystem = gameObject.GetComponent<ParticleSystem>();

        if (weaponController == null)
        {
            Debug.LogWarning("Projectile: WeaponController reference is missing.");
            return;
        }

        if (_particleSystem != null)
        {
            float calculatedLifetime = weaponController.projectileSpeed > 0 ? weaponController.range / weaponController.projectileSpeed : 0.1f;

            var mainModule = _particleSystem.main;
            mainModule.startLifetime = calculatedLifetime;
            mainModule.startSpeed = weaponController.projectileSpeed;

            var emissionModule = _particleSystem.emission;
            emissionModule.rateOverTime = weaponController.projectileInterval;
            emissionModule.burstCount = weaponController.projectileAmount;

            if (subEmmiters != null && subEmmiters.Length > 0)
            {
                foreach (var subEmitter in subEmmiters)
                {
                    if (subEmitter != null)
                    {
                        var subMain = subEmitter.main;
                        subMain.startSizeMultiplier += weaponController.explosionRadius;
                    }
                    else
                    {
                        Debug.LogWarning("Projectile: One of the sub-emitters is null.");
                    }
                }
            }
            else
            {
                Debug.Log("Projectile: No sub-emitters assigned or array is empty.");
            }
        }
        else
        {
            Debug.LogWarning("Projectile: ParticleSystem component is missing.");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"Particle collision detected with {other.name} on layer {LayerMask.LayerToName(other.layer)}");

        if (weaponController == null || !weaponController.isEquippedByPlayer) return;

        if (other.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyClass enemy = other.GetComponent<EnemyClass>();

            if (enemy != null)
            {
                float actualDamage = weaponController.damageValue * weaponController.damageMultiplier;
                enemy.TakeDamage(actualDamage);
                Debug.Log($"Particle hit {other.name} and dealt {actualDamage} damage.");
            }
            else
            {
                Debug.LogWarning("Projectile: EnemyClass component not found on collided object.");
            }
        }
    }
}