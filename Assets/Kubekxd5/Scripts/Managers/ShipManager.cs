using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public Transform shipSlot;
    public GameObject shipList, weaponList;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found. Ensure GameManager is present in the scene.");
            return;
        }

        GameObject playerShip = GameManager.Instance.currentEquippedShip;

        if (playerShip == null)
        {
            Debug.LogWarning("No ship was instantiated by the GameManager. Spawning default ship.");
            GameManager.Instance.SpawnPlayerShip();
            playerShip = GameManager.Instance.currentEquippedShip;
        }

        if (playerShip != null)
        {
            PositionShipInSlot(playerShip);
        }

        GameObject playerCamera = GameObject.FindWithTag("MainCamera");
        if (playerCamera != null)
        {
            playerCamera.GetComponent<CameraController>()?.FindPlayerShip();
        }
    }

    private void PositionShipInSlot(GameObject ship)
    {
        ship.transform.SetParent(shipSlot);
        ship.transform.localPosition = Vector3.zero;
        ship.transform.localRotation = Quaternion.identity;
        ship.transform.localScale = new Vector3(5, 5, 5);
    }
}