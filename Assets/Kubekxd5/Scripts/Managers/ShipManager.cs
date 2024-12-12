using UnityEngine;
using System.Collections.Generic;

public class ShipManager : MonoBehaviour
{
    public Transform shipSlot;
    public GameObject shipList, weaponList;
    
    private List<GameObject> availableShips = new List<GameObject>(); // All detected ship prefabs
    private List<GameObject> availableWeapons = new List<GameObject>(); // All detected weapon prefabs

    private GameObject currentShip;

    private void Start()
    {
        PopulateAvailableShips();
        PopulateAvailableWeapons();

        if (PlayerPrefs.HasKey("SelectedShipData"))
        {
            LoadShipData();
        }
        else
        {
            Debug.LogWarning("No saved ship data found. Default ship will be used.");
        }

        GameObject playerCamera = GameObject.FindWithTag("MainCamera");
        playerCamera.GetComponent<CameraController>().FindPlayerShip();
    }


    private void PopulateAvailableShips()
    {
        foreach (Transform child in shipList.transform)
        {
            availableShips.Add(child.gameObject);
        }
    }

    private void PopulateAvailableWeapons()
    {
        foreach (Transform child in weaponList.transform)
        {
            availableWeapons.Add(child.gameObject);
        }
    }

    private void LoadShipData()
    {
        string jsonData = PlayerPrefs.GetString("SelectedShipData", null);
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogWarning("No ship data found, using a default ship.");
            currentShip = InstantiateDefaultShip();
            return;
        }

        ShipData shipData = JsonUtility.FromJson<ShipData>(jsonData);
        Debug.Log($"Loaded Ship Data: {jsonData}");

        GameObject selectedShipPrefab = availableShips.Find(ship =>
            ship.GetComponent<ShipController>().shipName == shipData.shipName);

        if (selectedShipPrefab == null)
        {
            Debug.LogError($"Ship prefab '{shipData.shipName}' not found, using default.");
            currentShip = InstantiateDefaultShip();
            return;
        }

        currentShip = Instantiate(selectedShipPrefab, shipSlot.position, shipSlot.rotation, shipSlot);
        currentShip.transform.localPosition = Vector3.zero;
        currentShip.transform.localRotation = Quaternion.identity;
        currentShip.tag = "PlayerShip";

        EquipWeapons(currentShip, shipData);
    }

    private GameObject InstantiateDefaultShip()
    {
        GameObject defaultShipPrefab = availableShips[0]; // Assuming the first ship is the default.
        return Instantiate(defaultShipPrefab, shipSlot.position, shipSlot.rotation, shipSlot);
    }


    private void EquipWeapons(GameObject ship, ShipData shipData)
    {
        SlotsManager slotsManager = ship.GetComponent<SlotsManager>();
        if (slotsManager == null) return;

        foreach (var weaponData in shipData.weapons)
        {
            GameObject weaponPrefab = availableWeapons.Find(weapon => weapon.name == weaponData.weaponName);
            if (weaponPrefab == null)
            {
                Debug.LogError($"Weapon prefab '{weaponData.weaponName}' not found!");
                continue;
            }

            WeaponController weaponInstance = Instantiate(weaponPrefab).GetComponent<WeaponController>();
            switch (weaponData.weaponMount)
            {
                case "Primary":
                    slotsManager.AddPrimaryWeapon(weaponInstance);
                    break;
                case "Secondary":
                    slotsManager.AddSecondaryWeapon(weaponInstance);
                    break;
                case "Hangar":
                    slotsManager.AddHangarBay(weaponInstance);
                    break;
                case "Special":
                    slotsManager.AddSpecialWeapon(weaponInstance);
                    break;
            }
        }
    }
}
