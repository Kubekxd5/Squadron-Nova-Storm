using System.Collections.Generic;
using UnityEngine;

public class SlotsManager : MonoBehaviour
{
    [Header("Weapons:")]
    public List<WeaponController> primaryWeapons = new List<WeaponController>();
    public List<WeaponController> secondaryWeapons = new List<WeaponController>();
    public List<WeaponController> hangarBayWeapons = new List<WeaponController>();
    public List<WeaponController> specialWeapons = new List<WeaponController>();
    
    [Header("Slots transform")]
    public Transform[] primarySlot;
    public Transform[] secondarySlot;
    public Transform[] hangarBaySlot;
    public Transform[] specialSlot;
    
    public Transform GetSlotTransform(string category, int index)
    {
        switch (category)
        {
            case "Primary":
                return primarySlot[index];
            case "Secondary":
                return secondarySlot[index];
            case "Hangar":
                return hangarBaySlot[index];
            case "Special":
                return specialSlot[index];
            default:
                return null;
        }
    }
    
    public void AddWeaponToCategory(WeaponController weapon, string category)
    {
        switch (category)
        {
            case "Primary":
                primaryWeapons.Add(weapon);
                break;
            case "Secondary":
                secondaryWeapons.Add(weapon);
                break;
            case "Hangar":
                hangarBayWeapons.Add(weapon);
                break;
            case "Special":
                specialWeapons.Add(weapon);
                break;
        }
    }
    
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

