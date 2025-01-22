using UnityEngine;

public class ShipController : MonoBehaviour
{
    public enum ShipClass
    {
        Interceptor,
        Assault,
        Tank,
        Bomber,
        Stealth
    }

    public SlotsManager slotManagerRef;

    [Header("Ship Stats:")] public string shipName;

    public ShipClass shipClass;
    public float health;
    public float hullLevel, energyShield, damageReduction;
    public float shieldRegenRate, healthRegenRate;
    public float speed, maxSpeed, maneuverability, boostCharge;

    [Header("Sprint Stats:")] public float sprintMultiplier = 2f; // Boost multiplier during sprint
    public float sprintCooldownTime = 3f; // Cooldown duration

    public ParticleSystem sprintParticles; // Particle system to visualize sprint
    private CameraController _mainCamera;

    private Rigidbody _rb;

    private bool isSprinting;
    private bool isOnCooldown; // Tracks if sprint is on cooldown
    private float sprintCooldownTimer; // Tracks remaining cooldown time

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            var enemyTower = enemy.GetComponent<EnemyTower>();
            if (enemyTower != null) enemyTower.player = transform;
        }
    }

    public void HandleMovement()
    {
        var forwardInput = Input.GetAxis("Vertical");
        var turnInput = Input.GetAxis("Horizontal");

        var currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        transform.Translate(Vector3.forward * (forwardInput * currentSpeed * Time.deltaTime));

        transform.Rotate(Vector3.up, turnInput * (maneuverability * 10) * Time.deltaTime);
    }

    public void HandleSprint()
    {
        if (isOnCooldown)
        {
            sprintCooldownTimer -= Time.deltaTime;
            if (sprintCooldownTimer <= 0)
            {
                isOnCooldown = false;
                Debug.Log("Sprint is ready to use again!");
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && boostCharge > 0 && !isOnCooldown)
        {
            isSprinting = true;
            if (sprintParticles != null && !sprintParticles.isPlaying) sprintParticles.Play();
        }
        else
        {
            isSprinting = false;
            if (sprintParticles != null && sprintParticles.isPlaying)
            {
                sprintParticles.Stop();
                sprintParticles.Clear();
            }
        }

        if (isSprinting)
        {
            boostCharge -= Time.deltaTime * 10f;
            boostCharge = Mathf.Max(boostCharge, 0);

            if (boostCharge == 0)
            {
                isOnCooldown = true;
                sprintCooldownTimer = sprintCooldownTime;
                Debug.Log("Sprint depleted. Cooldown started.");
            }
        }
        else
        {
            if (!isOnCooldown)
            {
                boostCharge += Time.deltaTime * 5f;
                boostCharge = Mathf.Min(boostCharge, 100f);
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"{shipName} took {damageAmount} damage. Remaining health: {health}");

        if (health <= 0)
        {
            health = 0;
            Debug.Log($"{shipName} has been destroyed!");
        }
    }
}
