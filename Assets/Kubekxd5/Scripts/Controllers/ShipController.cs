using UnityEngine;

public class ShipController : MonoBehaviour
{
    public SlotsManager slotManagerRef;
    
    [Header("Ship Stats:")] public string shipName;
    public ShipClass shipClass;
    public enum ShipClass {Interceptor, Assault, Tank, Bomber, Stealth};
    public float health;
    public float hullIntegrity, energyShield, damageReduction;
    public float shieldRegenRate, healthRegenRate;
    public float speed, maxSpeed, maneuverability, boostCharge;
    
    private Rigidbody _rb;
    private CameraController _mainCamera;
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
        
        transform.Translate(Vector3.forward * (forwardInput * speed * Time.deltaTime));
        
        if (Mathf.Abs(forwardInput) < 0.01f)
        {
            _rb.linearVelocity *= 0.99f;
        }
        
        transform.Rotate(Vector3.up, turnInput * (maneuverability * 10) * Time.deltaTime);
    }
}

