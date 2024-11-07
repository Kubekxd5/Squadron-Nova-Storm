using System.Collections.Generic;
using UnityEngine;

public class SlotsManager : MonoBehaviour
{
    [Header("Weapons:")]
    public List<WeaponController> primaryWeapons = new List<WeaponController>();
    public List<WeaponController> secondaryWeapons = new List<WeaponController>();
    public List<WeaponController> hangarBayWeapons = new List<WeaponController>();
    public List<WeaponController> specialWeapons = new List<WeaponController>();
    
    /*public void InitializeWeapons()
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
    }*/

    public void FirePrimaryWeapons()
    {
        foreach (var weapon in primaryWeapons)
        {
            weapon.Shoot();
        }
    }

    public void AddPrimaryWeapon(WeaponController weapon)
    {
        primaryWeapons.Add(weapon);
        foreach (var wa in primaryWeapons)
        {
            wa.EquipWeapon();
        }
    }
    
    public void FireSecondaryWeapons()
    {
        foreach (var weapon in secondaryWeapons)
        {
            weapon.Shoot();
        }
    }
    
    public void AddSecondaryWeapon(WeaponController weapon)
    {
        secondaryWeapons.Add(weapon);
        foreach (var wa in secondaryWeapons)
        {
            wa.EquipWeapon();
        }
    }
    
    public void FireHangarBay()
    {
        foreach (var hangar in hangarBayWeapons)
        {
            hangar.Shoot();
        }
    }
    
    public void AddHangarBay(WeaponController hangar)
    {
        hangarBayWeapons.Add(hangar);
        foreach (var wa in hangarBayWeapons)
        {
            wa.EquipWeapon();
        }
    }
    
    public void FireSpecialWeapon()
    {
        foreach (var special in specialWeapons)
        {
            special.Shoot();
        }
    }
    
    public void AddSpecialWeapon(WeaponController weapon)
    {
        specialWeapons.Add(weapon);
        foreach (var wa in specialWeapons)
        {
            wa.EquipWeapon();
        }
    }
}

