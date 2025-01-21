using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ShipController shipController;
    public SlotsManager slotsManager;
    
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerShipSpawned += OnPlayerShipSpawned;
        }
    }
    private void OnPlayerShipSpawned()
    {
        StartCoroutine(LookForPlayer());
    }
    private IEnumerator LookForPlayer()
    {
        yield return new WaitForSeconds(1f);
        AssignPlayerShip();
    }
    
    public void AssignPlayerShip()
    {
        gameObject.transform.SetParent(transform);

        shipController = gameObject.GetComponentInChildren<ShipController>();
        slotsManager = gameObject.GetComponentInChildren<SlotsManager>();

        if (shipController == null)
        {
            Debug.LogWarning("PlayerController: ShipController not found on the spawned player ship.");
        }
        else
        {
            Debug.Log("PlayerController: ShipController successfully assigned.");
        }

        if (slotsManager == null)
        {
            Debug.LogWarning("PlayerController: SlotsManager not found on the spawned player ship.");
        }
        else
        {
            Debug.Log("PlayerController: SlotsManager successfully assigned.");
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
            shipController.HandleSprint();
            shipController.HandleMovement();
        }

        if (slotsManager != null)
        {
            if (Input.GetButton("Fire1"))
            {
                slotsManager.FirePrimaryWeapons();
            }

            if (Input.GetButton("Fire2"))
            {
                slotsManager.FireSecondaryWeapons();
            }

            if (Input.GetButton("Jump"))
            {
                slotsManager.FireHangarBay();
            }

            if (Input.GetButton("Fire3"))
            {
                slotsManager.FireSpecialWeapon();
            }
        }
    }
}