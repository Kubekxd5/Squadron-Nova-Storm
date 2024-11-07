using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public GameObject currentShip;
    public Transform shipSlot;
    private PlayerController _playerController;
    private GameObject _previousShip;
    private void Start()
    {
        _playerController = gameObject.GetComponent<PlayerController>();
        SwapShip(currentShip);
    }

    public void SwapShip(GameObject newShipPrefab)
    {
        if (_previousShip != null)
        {
            Destroy(_previousShip);
        }
        
        
        if (currentShip != null && currentShip.scene.isLoaded)
        {
            Destroy(currentShip);
        }
    
        currentShip = Instantiate(newShipPrefab, shipSlot.position, shipSlot.rotation, shipSlot);
        currentShip.tag = "PlayerShip";
        _playerController.shipController = currentShip.GetComponent<ShipController>();
        _playerController.slotsManager = currentShip.GetComponentInChildren<SlotsManager>();
        
        _previousShip = currentShip;
    }
}

