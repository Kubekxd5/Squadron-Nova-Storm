using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GarageShipManager : MonoBehaviour
{
    [Header("UI_Components")] public TMP_Dropdown shipDropdown;

    public TextMeshProUGUI shipTypeTextField;

    [Header("Lists")] public ListManager shipsList;
    public ListManager weaponList;

    public Transform equippedShipParent;
    public InventoryManager InventoryManager;

    public List<GameObject> availableWeapons = new();
    public GameObject currentEquippedShip;

    [Header("Config Object")] public ShipConfigObject shipConfig;

    private readonly List<GameObject> _availableShips = new();

    private void Start()
    {
        shipsList = GameManager.Instance.shipsList;
        weaponList = GameManager.Instance.weaponList;
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
            shipDropdown.options.Add(
                new TMP_Dropdown.OptionData(ship.gameObject.GetComponent<ShipController>().shipName));
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

        foreach (Transform child in weaponList.transform) availableWeapons.Add(child.gameObject);
    }

    private void UpdateShipConfig()
    {
        if (shipConfig == null) return;

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
            currentEquippedShip = null;
        }

        if (selectedShip == null)
        {
            Debug.LogWarning("Selected ship is null!");
            return;
        }

        currentEquippedShip = Instantiate(selectedShip, equippedShipParent);
        currentEquippedShip.transform.localPosition = Vector3.zero;
        currentEquippedShip.transform.localRotation = Quaternion.identity;
        currentEquippedShip.transform.localScale = new Vector3(5, 5, 5);
        currentEquippedShip.tag = "PlayerShip";

        if (currentEquippedShip.TryGetComponent(out ShipController shipController))
        {
            shipTypeTextField.text = shipController.shipClass.ToString();
            InventoryManager.SelectNewShip();
        }
        else
        {
            Debug.LogWarning("Selected ship does not have a ShipController component!");
        }
    }
    
    public void SaveConfig(int index)
    {
        if (shipConfig == null) return;

        if (currentEquippedShip == null) return;

        shipConfig.shipIndex = index;

        var shipController = currentEquippedShip.GetComponent<ShipController>();
        if (shipController == null) return;

        if (shipController.slotManagerRef == null) return;

        if (shipController.slotManagerRef.allSlots == null) return;

        var equippedWeapons = new List<GameObject>();

        foreach (var slot in shipController.slotManagerRef.allSlots)
        {
            if (slot == null) continue;

            var shipSlot = slot.GetComponent<ShipSlot>();
            if (shipSlot != null && shipSlot.weaponController != null)
                equippedWeapons.Add(shipSlot.weaponController.gameObject);
            else
                Debug.LogWarning("Empty or unequipped slot: " + slot.name);
        }
    }
}