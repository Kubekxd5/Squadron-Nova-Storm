using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Components")] public Transform inventoryMenu;

    public GameObject inventoryItemPrefab;

    [Header("References")] public GarageShipManager garageManager;

    public SlotsManager slotsManager;
    public List<GameObject> currentInventorySlots = new();

    [Header("Offset Settings")] public float slotVerticalOffset = 50f;

    private float currentYOffset;

    public void SelectNewShip()
    {
        currentYOffset = 0;

        foreach (var slot in currentInventorySlots) Destroy(slot);
        currentInventorySlots.Clear();

        var currentShip = garageManager.currentEquippedShip;
        if (currentShip == null) return;

        slotsManager = currentShip.GetComponent<ShipController>().slotManagerRef;
        if (slotsManager == null) return;

        StartCoroutine(RefreshInventoryAfterDelay());
    }

    private IEnumerator RefreshInventoryAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        PopulateInventory(slotsManager);
    }

    private void PopulateInventory(SlotsManager slotsManager)
    {
        PopulateCategory(slotsManager.primarySlot, ShipSlot.WeaponMount.Primary);
        PopulateCategory(slotsManager.secondarySlot, ShipSlot.WeaponMount.Secondary);
        PopulateCategory(slotsManager.hangarBaySlot, ShipSlot.WeaponMount.Hangar);
        PopulateCategory(slotsManager.specialSlot, ShipSlot.WeaponMount.Special);
    }

    private void PopulateCategory(Transform[] slots, ShipSlot.WeaponMount weaponMount)
    {
        if (slots == null || slots.Length == 0) return;

        for (var i = 0; i < slots.Length; i++)
        {
            var shipSlot = slots[i].GetComponent<ShipSlot>();
            if (shipSlot == null) continue;

            var newSlot = Instantiate(inventoryItemPrefab, inventoryMenu);
            newSlot.transform.localScale = Vector3.one;
            newSlot.SetActive(true);
            currentInventorySlots.Add(newSlot);

            var slotText = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            slotText.text = $"{weaponMount} Slot {i + 1}";

            var rectTransform = newSlot.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -currentYOffset);

            currentYOffset += slotVerticalOffset;

            var weaponDropdown = newSlot.GetComponentInChildren<TMP_Dropdown>();
            PopulateWeaponDropdown(weaponDropdown, shipSlot, weaponMount);
        }
    }

    private void PopulateWeaponDropdown(TMP_Dropdown dropdown, ShipSlot shipSlot, ShipSlot.WeaponMount weaponMount)
    {
        dropdown.ClearOptions();

        var options = new List<string> { "None" };
        foreach (var weapon in garageManager.availableWeapons)
        {
            var weaponController = weapon.GetComponent<WeaponController>();

            // Convert WeaponController.WeaponMount to ShipSlot.WeaponMount
            var convertedMount = (ShipSlot.WeaponMount)(int)weaponController.weaponMount;

            if (convertedMount == weaponMount) options.Add(weaponController.weaponName);
        }

        dropdown.AddOptions(options);

        if (shipSlot.weaponController != null)
        {
            var index = options.IndexOf(shipSlot.weaponController.weaponName);
            dropdown.value = index >= 0 ? index : 0;
        }

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(selectedIndex =>
        {
            HandleWeaponSelection(shipSlot, dropdown.options[selectedIndex].text);
            RefreshInventory();
        });
    }

    private void HandleWeaponSelection(ShipSlot shipSlot, string selectedWeaponName)
    {
        if (selectedWeaponName == "None")
        {
            shipSlot.RemoveWeapon();

            if (garageManager.shipConfig != null)
            {
                var config = garageManager.shipConfig;
                var slotConfig = config.weaponSlots.FirstOrDefault(slot => slot.slotName == shipSlot.name);
                if (slotConfig != null)
                {
                    slotConfig.slotName = null;
                    slotConfig.weaponName = null;
                }
            }
        }
        else
        {
            var weaponPrefab = garageManager.shipConfig.equippedWeapons.FirstOrDefault(w =>
                w.GetComponent<WeaponController>().weaponName == selectedWeaponName);

            if (weaponPrefab != null)
            {
                if (shipSlot.weaponController != null) Destroy(shipSlot.weaponController.gameObject);

                var weaponInstance = Instantiate(weaponPrefab, shipSlot.transform);
                var weaponController = weaponInstance.GetComponent<WeaponController>();
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;
                weaponInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                shipSlot.weaponController = weaponController;

                if (garageManager.shipConfig != null)
                {
                    var newSlotConfig = new ShipConfigObject.ShipSlotConfig
                    {
                        slotName = shipSlot.name,
                        weaponName = shipSlot.weaponController.weaponName
                    };

                    var existingIndex =
                        garageManager.shipConfig.weaponSlots.FindIndex(slot => slot.slotName == shipSlot.name);
                    if (existingIndex >= 0)
                        garageManager.shipConfig.weaponSlots[existingIndex] = newSlotConfig;
                    else
                        garageManager.shipConfig.weaponSlots.Add(newSlotConfig);
                }
            }
            else
            {
                Debug.LogError($"Weapon '{selectedWeaponName}' not found in Possible Weapons.");
            }
        }
    }


    private void RefreshInventory()
    {
        foreach (var slot in currentInventorySlots)
        {
            var dropdown = slot.GetComponentInChildren<TMP_Dropdown>();
            var shipSlot = slot.GetComponentInParent<ShipSlot>();

            if (shipSlot != null) PopulateWeaponDropdown(dropdown, shipSlot, shipSlot.weaponMount);
        }
    }
}