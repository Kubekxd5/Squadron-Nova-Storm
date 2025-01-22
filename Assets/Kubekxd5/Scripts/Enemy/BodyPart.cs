using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public string partName;
    public EnemyClass enemyClassParent; // Reference to the parent EnemyClass
    public Renderer renderer;
    public Material damagedMaterial;
    public float damageThreshold = 50f;
    public float damageMultiplier = 1.5f;
    public float damageTransferRatio = 0.5f; // Percentage of damage transferred to the main body (50% in this case)

    private float currentDamage;
    private bool isDamaged;

    public void Initialize()
    {
        currentDamage = 0;
        isDamaged = false;
    }

    public float ApplyDamage(float damage)
    {
        currentDamage += damage;

        if (!isDamaged && currentDamage >= damageThreshold)
        {
            isDamaged = true;

            if (renderer != null && damagedMaterial != null)
            {
                var materials = renderer.materials; // Copy the materials array
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = damagedMaterial;
                }
                renderer.materials = materials;
            }

            Debug.Log($"{partName} is now damaged and takes {damageMultiplier}x damage!");
        }

        // Damage multiplier applies if the part is damaged
        float effectiveDamage = isDamaged ? damage * damageMultiplier : damage;

        // Transfer a portion of damage to the main body
        float transferredDamage = effectiveDamage * damageTransferRatio;
        enemyClassParent.ApplyDamageToMainBody(transferredDamage);

        return isDamaged ? damageMultiplier : 1f;
    }
}