using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    public Transform player;
    public GameObject weapon;
    public ParticleSystem gunfireVfx;
    public AudioSource gunfireSfx;
    public float shootInterval = 1f;
    public float rotationSpeed = 5f;
    private float _timer;

    private void Update()
    {
        RotateTowardsPlayer();
        HandleShooting();
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - weapon.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleShooting()
    {
        _timer += Time.deltaTime;
        if (_timer >= shootInterval)
        {
            gunfireSfx.Play();
            gunfireVfx.Play();
            _timer = 0f;
        }
    }
}