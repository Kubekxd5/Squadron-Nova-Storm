using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    public Transform player;
    public GameObject weapon;
    public ParticleSystem[] gunfireVfx;
    public AudioSource gunfireSfx;
    public float shootInterval = 1f;
    public float rotationSpeed = 5f;
    private float _timer;
    public float health=10;
    private float _currentHealth;

    private void Start()
    {
        player = GameObject.FindWithTag("PlayerShip")?.transform;
        _currentHealth = health;
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("PlayerShip")?.transform;
            if (player == null) return;
        }

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
            foreach (var gun in gunfireVfx)
            {
                gun.Play();
            }
            _timer = 0f;
        }
    }
    
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {_currentHealth}");
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed.");
        Destroy(gameObject);
    }
}