using UnityEngine;

public class ShipController : MonoBehaviour
{
    public SlotsManager slotManagerRef;

    [Header("Ship Stats:")] 
    public string shipName;
    public ShipClass shipClass;
    public enum ShipClass { Interceptor, Assault, Tank, Bomber, Stealth }
    public float health;
    public float hullLevel, energyShield, damageReduction;
    public float shieldRegenRate, healthRegenRate;
    public float speed, maxSpeed, maneuverability, boostCharge;

    [Header("Sprint Stats:")]
    public float sprintMultiplier = 2f; // Boost multiplier during sprint
    public ParticleSystem sprintParticles; // Particle system to visualize sprint

    private Rigidbody _rb;
    private CameraController _mainCamera;

    private bool isSprinting = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (transform.parent.GetComponent<PlayerController>() == true)
        {
            _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
            _mainCamera.FindPlayerShip();
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            EnemyTower enemyTower = enemy.GetComponent<EnemyTower>();
            if (enemyTower != null)
            {
                enemyTower.player = transform;
            }
        }
    }

    public void HandleMovement()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        transform.Translate(Vector3.forward * (forwardInput * currentSpeed * Time.deltaTime));

        transform.Rotate(Vector3.up, turnInput * (maneuverability * 10) * Time.deltaTime);
    }

    public void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && boostCharge > 0)
        {
            isSprinting = true;
            if (sprintParticles != null && !sprintParticles.isPlaying)
            {
                sprintParticles.Play();
            }
        }
        else
        {
            isSprinting = false;
            if (sprintParticles != null && sprintParticles.isPlaying)
            {
                sprintParticles.Stop();
            }
        }

        if (isSprinting)
        {
            boostCharge -= Time.deltaTime * 10f;
            boostCharge = Mathf.Max(boostCharge, 0);
        }
        else
        {
            boostCharge += Time.deltaTime * 5f;
            boostCharge = Mathf.Min(boostCharge, 100f);
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
