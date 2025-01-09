using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GarageShipManager : MonoBehaviour
{
    [Header("UI_Components")]
    public TMP_Dropdown shipDropdown;
    public TextMeshProUGUI shipTypeTextField;

    [Header("Lists")]
    public GameObject shipsList;
    public GameObject weaponList;

    public Transform equippedShipParent;
    public InventoryManager InventoryManager;

    public List<GameObject> availableWeapons = new List<GameObject>();
    private List<GameObject> _availableShips = new List<GameObject>();
    public GameObject currentEquippedShip;

    [Header("Config Object")]
    public ShipConfigObject shipConfig;

    private void Start()
    {
        PopulateDropdown();
        PopulateAvailableWeapons();
        UpdateShipConfig();
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
            Debug.LogWarning("No available ships in the list.");
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

    private void UpdateShipConfig()
    {
        if (shipConfig == null)
        {
            return;
        }
        
        shipConfig.ships = new List<GameObject>(_availableShips);
        shipConfig.equippedWeapons = new List<GameObject>(availableWeapons);
    }

    private void OnShipSelected(int index)
    {
        EquipShip(_availableShips[index]);
        SaveConfig(index);
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

    public void SaveConfig(int index)
    {
        if (shipConfig == null)
        {
            return;
        }

        if (currentEquippedShip == null)
        {
            return;
        }

        shipConfig.shipIndex = index;

        ShipController shipController = currentEquippedShip.GetComponent<ShipController>();
        if (shipController == null)
        {
            return;
        }

        if (shipController.slotManagerRef == null)
        {
            return;
        }

        if (shipController.slotManagerRef.allSlots == null)
        {
            return;
        }

        List<GameObject> equippedWeapons = new List<GameObject>();

        foreach (Transform slot in shipController.slotManagerRef.allSlots)
        {
            if (slot == null)
            {
                continue;
            }

            ShipSlot shipSlot = slot.GetComponent<ShipSlot>();
            if (shipSlot != null && shipSlot.weaponController != null)
            {
                equippedWeapons.Add(shipSlot.weaponController.gameObject);
            }
            else
            {
                Debug.LogWarning("Empty or unequipped slot: " + slot.name);
            }
        }
    }
}
