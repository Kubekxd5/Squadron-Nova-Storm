using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;

public class GarageShipManager : MonoBehaviour
{
    [Header("UI_Components")] public TMP_Dropdown shipDropdown;
    public TextMeshProUGUI shipTypeTextField;
    
    [Header("Lists")] public GameObject shipsList;
    public GameObject weaponList;
    
    public Transform equippedShipParent;
    public InventoryManager InventoryManager;
    
    public List<GameObject> availableWeapons = new List<GameObject>();
    private List<GameObject> _availableShips = new List<GameObject>();
    public GameObject currentEquippedShip;
    
    private void Start()
    {
        PopulateDropdown();
        PopulateAvailableWeapons();
        shipDropdown.onValueChanged.AddListener(OnShipSelected);
    }

    private void PopulateDropdown()
    {
        _availableShips.Clear();
        shipDropdown.options.Clear();
        
        foreach (Transform ship in shipsList.transform)
        {
            _availableShips.Add(ship.gameObject);
            shipDropdown.options.Add(new TMP_Dropdown.OptionData(ship.gameObject.GetComponent<ShipController>().shipName));
        }
        
        if (_availableShips.Count > 0)
        {
            shipDropdown.value = 0;
            shipDropdown.captionText.text = _availableShips[0].GetComponent<ShipController>().shipName;
            EquipShip(_availableShips[0]);
        }
        else
        {
            Debug.LogWarning("No _availableShips available in the _availableShips list.");
        }
    }

    private void PopulateAvailableWeapons()
    {
        availableWeapons.Clear();
        
        foreach (Transform child in weaponList.transform)
        {
            availableWeapons.Add(child.gameObject);
        }
    }
    
    private void OnShipSelected(int index)
    {
        EquipShip(_availableShips[index]);
        SaveShipData(_availableShips[index]);
    }

    private void EquipShip(GameObject selectedShip)
    {
        if (currentEquippedShip != null)
        {
            Destroy(currentEquippedShip);
        }

        currentEquippedShip = Instantiate(selectedShip, equippedShipParent);
        currentEquippedShip.transform.localPosition = Vector3.zero;
        currentEquippedShip.transform.localRotation = Quaternion.identity;
        currentEquippedShip.transform.localScale = new Vector3(5, 5, 5);
        currentEquippedShip.tag = "PlayerShip";
        shipTypeTextField.text = currentEquippedShip.GetComponent<ShipController>().shipClass.ToString();
        
        InventoryManager.SelectNewShip();
    }

    private void SaveShipData(GameObject selectedShip)
    {
        ShipData shipData = new ShipData
        {
            shipName = selectedShip.GetComponent<ShipController>().shipName,
            weapons = new List<WeaponData>()
        };

        // Add equipped weapons
        SlotsManager slotsManager = selectedShip.GetComponent<SlotsManager>();
        if (slotsManager != null)
        {
            AddWeaponsToShipData(slotsManager.primaryWeapons, "Primary", shipData.weapons);
            AddWeaponsToShipData(slotsManager.secondaryWeapons, "Secondary", shipData.weapons);
            AddWeaponsToShipData(slotsManager.hangarBayWeapons, "Hangar", shipData.weapons);
            AddWeaponsToShipData(slotsManager.specialWeapons, "Special", shipData.weapons);
        }

        // Serialize and store in PlayerPrefs
        string jsonData = JsonUtility.ToJson(shipData);
        PlayerPrefs.SetString("SelectedShipData", jsonData);
        PlayerPrefs.Save();
        Debug.Log($"Saved Ship Data: {jsonData}");
    }


    private void AddWeaponsToShipData(List<WeaponController> weaponList, string weaponMount, List<WeaponData> shipWeaponData)
    {
        for (int i = 0; i < weaponList.Count; i++)
        {
            if (weaponList[i] != null)
            {
                shipWeaponData.Add(new WeaponData
                {
                    weaponName = weaponList[i].weaponName,
                    weaponMount = weaponMount,
                    slotIndex = i
                });
            }
        }
    }

}
