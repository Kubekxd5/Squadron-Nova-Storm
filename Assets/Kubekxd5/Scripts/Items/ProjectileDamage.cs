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
            var calculatedLifetime = weaponController.projectileSpeed > 0
                ? weaponController.range / weaponController.projectileSpeed
                : 0.1f;

            var mainModule = _particleSystem.main;
            mainModule.startLifetime = calculatedLifetime;
            mainModule.startSpeed = weaponController.projectileSpeed;

            var emissionModule = _particleSystem.emission;
            emissionModule.rateOverTime = weaponController.projectileInterval;
            emissionModule.burstCount = weaponController.projectileAmount;

            if (subEmmiters != null && subEmmiters.Length > 0)
                foreach (var subEmitter in subEmmiters)
                    if (subEmitter != null)
                    {
                        var subMain = subEmitter.main;
                        subMain.startSizeMultiplier += weaponController.explosionRadius;
                    }
                    else
                    {
                        Debug.LogWarning("Projectile: One of the sub-emitters is null.");
                    }
            else
                Debug.Log("Projectile: No sub-emitters assigned or array is empty.");
        }
        else
        {
            Debug.LogWarning("Projectile: ParticleSystem component is missing.");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"Particle collision detected with {other.name} on layer {LayerMask.LayerToName(other.layer)}");

        if (weaponController == null) return;

        if (other.layer == LayerMask.NameToLayer("Player"))
        {
            var ship = other.GetComponent<ShipController>();

            if (ship != null)
            {
                var actualDamage = weaponController.damageValue * weaponController.damageMultiplier;
                ship.TakeDamage(actualDamage);
                Debug.Log($"Particle hit player {ship.shipName} and dealt {actualDamage} damage.");
            }
            else
            {
                Debug.LogWarning("Projectile: ShipController component not found on collided object.");
            }
        }

        if (other.layer == LayerMask.NameToLayer("Enemy"))
        {
            HandleEnemyCollision(other, "Enemy");
        }

        if (other.layer == LayerMask.NameToLayer("GroundEnemy"))
        {
            HandleEnemyCollision(other, "GroundEnemy");
        }

        if (other.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Hit Ground layer");
        }
    }

    private void HandleEnemyCollision(GameObject other, string enemyType)
    {
        // Traverse up the hierarchy to find the object with the EnemyClass component
        var enemy = other.GetComponentInParent<EnemyClass>();

        if (enemy != null)
        {
            var hitCollider = other.GetComponent<Collider>();
            string bodyPartName = null;

            // Check if the hit object corresponds to a body part
            foreach (var part in enemy.bodyParts)
            {
                if (part.GetComponent<Renderer>() != null && part.GetComponent<Renderer>().gameObject == hitCollider.gameObject)
                {
                    bodyPartName = part.partName;
                    break;
                }
            }

            var actualDamage = weaponController.damageValue * weaponController.damageMultiplier;

            // Pass damage to the enemy with the body part name
            enemy.TakeDamage(actualDamage, bodyPartName);

            if (!string.IsNullOrEmpty(bodyPartName))
            {
                Debug.Log($"Particle hit {enemyType} {enemy.name}'s body part {bodyPartName} and dealt {actualDamage} damage.");
            }
            else
            {
                Debug.Log($"Particle hit {enemyType} {enemy.name} and dealt {actualDamage} damage.");
            }
        }
        else
        {
            Debug.LogWarning($"Projectile: EnemyClass component not found on {enemyType} object {other.name} or its parent.");
        }
    }
}