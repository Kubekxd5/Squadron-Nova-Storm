using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GarageShipManager : MonoBehaviour
{
    public TMP_Dropdown shipDropdown;
    public TextMeshProUGUI shipTypeTextField;
    public GameObject shipsList, weaponList;
    public Transform equippedShipParent;
    
    private List<GameObject> availableWeapons = new List<GameObject>();
    private List<GameObject> ships = new List<GameObject>();
    private GameObject currentEquippedShip;

    private void Start()
    {
        PopulateDropdown();
        foreach (Transform child in weaponList.transform)
        {
            availableWeapons.Add(child.gameObject);
        }
        shipDropdown.onValueChanged.AddListener(OnShipSelected);
    }

    private void PopulateDropdown()
    {
        foreach (Transform ship in shipsList.transform)
        {
            ships.Add(ship.gameObject);
            shipDropdown.options.Add(new TMP_Dropdown.OptionData(ship.gameObject.GetComponent<ShipController>().shipName));
        }
        
        shipDropdown.value = 0;
        shipDropdown.captionText.text = ships[0].GetComponent<ShipController>().shipName;
        EquipShip(ships[0]);
    }

    private void OnShipSelected(int index)
    {
        EquipShip(ships[index]);
        SaveShipData(ships[index]);
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
    }

    private void SaveShipData(GameObject selectedShip)
    {
        ShipData shipData = new ShipData
        {
            shipName = selectedShip.GetComponent<ShipController>().shipName,
            weapons = new List<WeaponData>()
        };

        SlotsManager slotsManager = selectedShip.GetComponent<SlotsManager>();
        if (slotsManager != null)
        {
            foreach (var weapon in slotsManager.primaryWeapons)
            {
                shipData.weapons.Add(new WeaponData { weaponName = weapon.weaponName, weaponMount = "Primary" });
            }
            foreach (var weapon in slotsManager.secondaryWeapons)
            {
                shipData.weapons.Add(new WeaponData { weaponName = weapon.weaponName, weaponMount = "Secondary" });
            }
            foreach (var weapon in slotsManager.hangarBayWeapons)
            {
                shipData.weapons.Add(new WeaponData { weaponName = weapon.weaponName, weaponMount = "Hangar" });
            }
            foreach (var weapon in slotsManager.specialWeapons)
            {
                shipData.weapons.Add(new WeaponData { weaponName = weapon.weaponName, weaponMount = "Special" });
            }
        }

        string jsonData = JsonUtility.ToJson(shipData);
        PlayerPrefs.SetString("SelectedShipData", jsonData);
        PlayerPrefs.Save();
        Debug.Log($"Saved Ship Data: {jsonData}");
    }
}
