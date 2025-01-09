using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ListManager shipsList, weaponList;
    public GameObject currentEquippedShip;
    [Header("Selected Ship Configuration")]
    public ShipConfigObject selectedShipConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (IsGameScene())
        {
            SpawnPlayerShip();
        }
    }

    public void SpawnPlayerShip()
    {
        if (selectedShipConfig == null || shipsList.listOfObjects.Length <= selectedShipConfig.shipIndex)
        {
            Debug.LogWarning("Invalid ship configuration or ship index!");
            return;
        }

        currentEquippedShip = Instantiate(shipsList.listOfObjects[selectedShipConfig.shipIndex]);
        currentEquippedShip.transform.localPosition = new Vector3(0, 5, 0);
        currentEquippedShip.transform.localRotation = Quaternion.identity;
        currentEquippedShip.transform.localScale = new Vector3(5, 5, 5);
        currentEquippedShip.tag = "PlayerShip";

        AttachWeaponsToShip();
    }

    private void AttachWeaponsToShip()
    {
        if (selectedShipConfig == null || currentEquippedShip == null)
        {
            Debug.LogWarning("Cannot attach weapons: Missing ship configuration or ship instance.");
            return;
        }

        foreach (var slotConfig in selectedShipConfig.weaponSlots)
        {
            Transform slotTransform = FindSlotOnShip(slotConfig.slotName);
            if (slotTransform == null)
            {
                Debug.LogWarning($"Slot '{slotConfig.slotName}' not found on the ship!");
                continue;
            }

            GameObject weaponPrefab = weaponList.GetObjectByName(slotConfig.weaponName);
            if (weaponPrefab == null)
            {
                Debug.LogWarning($"Weapon '{slotConfig.weaponName}' not found in weapon list!");
                continue;
            }

            GameObject weaponInstance = Instantiate(weaponPrefab, slotTransform);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    private Transform FindSlotOnShip(string slotName)
    {
        foreach (Transform child in currentEquippedShip.GetComponentsInChildren<Transform>())
        {
            if (child.name == slotName)
            {
                return child;
            }
        }
        return null;
    }

    public void ClearSelectedShip()
    {
        selectedShipConfig = null;
    }

    public int GetSelectedShipPrefab()
    {
        return selectedShipConfig != null ? selectedShipConfig.shipIndex : 0;
    }

    private bool IsGameScene()
    {
        return SceneManager.GetActiveScene().buildIndex == 1;
    }
}
