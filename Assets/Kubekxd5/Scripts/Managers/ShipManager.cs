using System.Collections;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public Transform shipSlot;
    public GameObject shipList, weaponList;

    private void Start()
    {
        StartCoroutine(InitializeShip());
    }

    private IEnumerator InitializeShip()
    {
        yield return new WaitUntil(() =>
            GameManager.Instance != null && GameManager.Instance.currentEquippedShip != null);

        var playerShip = GameManager.Instance.currentEquippedShip;

        if (playerShip != null)
            PositionShipInSlot(playerShip);
        else
            Debug.LogWarning("ShipManager: No ship found even after waiting. Check GameManager logic.");

        var playerCamera = GameObject.FindWithTag("MainCamera");
        if (playerCamera != null) playerCamera.GetComponent<CameraController>()?.FindPlayerShip();
    }

    private void PositionShipInSlot(GameObject ship)
    {
        ship.transform.SetParent(shipSlot);
        ship.transform.localPosition = Vector3.zero;
        ship.transform.localRotation = Quaternion.identity;
        ship.transform.localScale = new Vector3(5, 5, 5);
    }
}