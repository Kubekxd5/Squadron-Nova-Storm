using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Components")]
    public Transform inventoryMenu;
    public GameObject inventoryItemPrefab;

    [Header("References")]
    public GarageShipManager garageManager;

    public SlotsManager slotsManager;
    public List<GameObject> currentInventorySlots = new List<GameObject>();

    [Header("Offset Settings")]
    public float slotVerticalOffset = 50f;
    private float currentYOffset = 0f;

    public void SelectNewShip()
    {
        currentYOffset = 0;

        foreach (var slot in currentInventorySlots)
        {
            Destroy(slot);
        }
        currentInventorySlots.Clear();

        GameObject currentShip = garageManager.currentEquippedShip;
        if (currentShip == null)
        {
            Debug.LogError("No ship currently equipped!");
            return;
        }

        slotsManager = currentShip.GetComponent<ShipController>().slotManagerRef;
        if (slotsManager == null)
        {
            Debug.LogError("No SlotsManager found on the current ship!");
            return;
        }

        StartCoroutine(RefreshInventoryAfterDelay());
    }

    private IEnumerator RefreshInventoryAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        PopulateInventory(slotsManager);
    }

    private void PopulateInventory(SlotsManager slotsManager)
    {
        PopulateCategory(slotsManager.primarySlot, "Primary");
        PopulateCategory(slotsManager.secondarySlot, "Secondary");
        PopulateCategory(slotsManager.hangarBaySlot, "Hangar");
        PopulateCategory(slotsManager.specialSlot, "Special");
    }

    private void PopulateCategory(Transform[] slots, string category)
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogWarning($"{category} has no slots available.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            ShipSlot shipSlot = slots[i].GetComponent<ShipSlot>();
            if (shipSlot == null)
            {
                Debug.LogError($"Slot {i + 1} in {category} does not have a ShipSlot component.");
                continue;
            }

            Debug.Log($"{category} Slot {i + 1}: {(shipSlot.weaponController != null ? shipSlot.weaponController.weaponName : "Empty")}");

            GameObject newSlot = Instantiate(inventoryItemPrefab, inventoryMenu);
            newSlot.transform.localScale = Vector3.one;
            newSlot.SetActive(true);
            currentInventorySlots.Add(newSlot);

            TextMeshProUGUI slotText = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            slotText.text = $"{category} Slot {i + 1}";

            RectTransform rectTransform = newSlot.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -currentYOffset);

            currentYOffset += slotVerticalOffset;

            TMP_Dropdown weaponDropdown = newSlot.GetComponentInChildren<TMP_Dropdown>();
            PopulateWeaponDropdown(weaponDropdown, shipSlot, category);
        }
    }

    private void PopulateWeaponDropdown(TMP_Dropdown dropdown, ShipSlot shipSlot, string category)
    {
        dropdown.ClearOptions();

        // Populate dropdown with matching weapon types
        List<string> options = new List<string> { "None" };
        foreach (var weapon in garageManager.availableWeapons)
        {
            WeaponController weaponController = weapon.GetComponent<WeaponController>();
            if (weaponController.weaponMount.ToString() == category)
            {
                options.Add(weaponController.weaponName);
            }
        }

        dropdown.AddOptions(options);

        if (shipSlot.weaponController != null)
        {
            int index = options.IndexOf(shipSlot.weaponController.weaponName);
            dropdown.value = index >= 0 ? index : 0;
        }

        dropdown.onValueChanged.AddListener((selectedIndex) =>
        {
            Debug.Log($"Selected weapon for {category}: {dropdown.options[selectedIndex].text}");
            HandleWeaponSelection(shipSlot, dropdown.options[selectedIndex].text);
        });
    }

    private void HandleWeaponSelection(ShipSlot shipSlot, string selectedWeaponName)
    {
        if (selectedWeaponName == "None")
        {
            Debug.Log("Slot cleared.");
            if (shipSlot.weaponController != null)
            {
                Destroy(shipSlot.weaponController.gameObject);
                shipSlot.weaponController = null;
            }
            return;
        }

        WeaponController selectedWeapon = garageManager.availableWeapons
            .Find(w => w.GetComponent<WeaponController>().weaponName == selectedWeaponName)
            ?.GetComponent<WeaponController>();

        GameObject weaponPrefab = garageManager.availableWeapons
            .Find(w => w.GetComponent<WeaponController>().weaponName == selectedWeaponName);

        if (selectedWeapon != null)
        {
            if (shipSlot.weaponController != null)
            {
                Destroy(shipSlot.weaponController.gameObject);
            }

            GameObject weaponInstance = Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity);
            weaponInstance.GetComponent<WeaponController>().EquipWeapon();
            weaponInstance.transform.SetParent(shipSlot.transform);
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            shipSlot.weaponController = weaponInstance.GetComponent<WeaponController>();
        }
    }
}
