using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public GameObject currentShip;
    public Transform shipSlot;

    private void Start()
    {
        SwapShip(currentShip);
    }

    public void SwapShip(GameObject newShipPrefab)
    {
        if (currentShip != null && currentShip.scene.isLoaded)
        {
            Destroy(currentShip);
        }
    
        currentShip = Instantiate(newShipPrefab, shipSlot.position, shipSlot.rotation, shipSlot);
        currentShip.tag = "PlayerShip";
    }

}

