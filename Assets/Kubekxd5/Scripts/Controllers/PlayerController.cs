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

        if (shipController == null)
        {
            Debug.LogWarning("PlayerController: ShipController not found.");
        }

        if (slotsManager == null)
        {
            Debug.LogWarning("PlayerController: SlotsManager not found.");
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (shipController != null)
        {
            shipController.HandleMovement();
        }

        if (slotsManager != null)
        {
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
        else
        {
            Debug.LogWarning("PlayerController: SlotsManager is null. Cannot handle weapon inputs.");
        }
    }
}