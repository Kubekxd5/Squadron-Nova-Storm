using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Info:")] 
    public string playerName;
    public int score, scoreMultiplier;

    public ShipController shipController;
    public SlotsManager slotsManager;
    private ShipManager _shipManager;

    private void Start()
    {
        shipController = GetComponentInChildren<ShipController>();
        slotsManager = GetComponentInChildren<SlotsManager>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        shipController.HandleMovement();
        
        if (Input.GetButtonDown("Fire1"))
        {
            slotsManager.FirePrimaryWeapons();
        }
        
        if (Input.GetButtonDown("Fire2"))
        {
            slotsManager.FireSecondaryWeapons();
        }

        if (Input.GetButtonDown("Jump"))
        {
            slotsManager.FireHangarBay();
        }

        if (Input.GetButtonDown("Fire3"))
        {
            slotsManager.FireSpecialWeapon();
        }
    }
}