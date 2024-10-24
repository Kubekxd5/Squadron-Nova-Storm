using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Info:")] 
    public string playerName;
    public int score, scoreMultiplier;

    private ShipController _shipController;
    private SlotsManager _slotsManager;
    private ShipManager _shipManager;

    private void Start()
    {
        _shipController = GetComponentInChildren<ShipController>();
        _slotsManager = GetComponentInChildren<SlotsManager>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        _shipController.HandleMovement();
        
        if (Input.GetButtonDown("Fire1"))
        {
            _slotsManager.FirePrimaryWeapons();
        }
        
        if (Input.GetButtonDown("Fire2"))
        {
            _slotsManager.FireSecondaryWeapons();
        }
    }
}