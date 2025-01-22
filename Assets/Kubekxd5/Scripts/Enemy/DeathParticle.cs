using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    private ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("No ParticleSystem found on the DeathParticle prefab.");
            Destroy(gameObject);
            return;
        }

        particleSystem.Play();

        Destroy(gameObject, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
    }
}
