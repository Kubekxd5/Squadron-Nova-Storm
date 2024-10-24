using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsManager : MonoBehaviour
{
    [Header("Weapons:")]
    public List<WeaponController> primaryWeapons = new List<WeaponController>();
    public List<WeaponController> secondaryWeapons = new List<WeaponController>();
    public List<WeaponController> hangarBayWeapons = new List<WeaponController>();
    public List<WeaponController> specialWeapons = new List<WeaponController>();

    private void Start()
    {
        StartCoroutine(WeaponInitialization());
    }
    
    private IEnumerator WeaponInitialization()
    {
        yield return new WaitForSeconds(1f);

        _InitializeWeapons();
    }
    
    private void _InitializeWeapons()
    {
        ShipSlot[] shipSlots = GetComponentsInChildren<ShipSlot>();
        foreach (ShipSlot slot in shipSlots)
        {
            if (slot.weaponController != null)
            {
                switch (slot.weaponMount)
                {
                    case ShipSlot.WeaponMount.Primary:
                        primaryWeapons.Add(slot.weaponController);
                        break;
                    case ShipSlot.WeaponMount.Secondary:
                        secondaryWeapons.Add(slot.weaponController);
                        break;
                    case ShipSlot.WeaponMount.Hangar:
                        hangarBayWeapons.Add(slot.weaponController);
                        break;
                    case ShipSlot.WeaponMount.Special:
                        specialWeapons.Add(slot.weaponController);
                        break;
                }
            }
        }
    }

    public void FirePrimaryWeapons()
    {
        foreach (var weapon in primaryWeapons)
        {
            weapon.Shoot();
        }
    }

    public void FireSecondaryWeapons()
    {
        foreach (var weapon in secondaryWeapons)
        {
            weapon.Shoot();
        }
    }
}

