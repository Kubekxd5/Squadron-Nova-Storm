using System.Collections.Generic;
using UnityEngine;

public class SlotsManager : MonoBehaviour
{
    [Header("Weapons:")] public List<WeaponController> primaryWeapons = new();

    public List<WeaponController> secondaryWeapons = new();
    public List<WeaponController> hangarBayWeapons = new();
    public List<WeaponController> specialWeapons = new();

    [Header("Slots transform")] public Transform[] primarySlot;

    public Transform[] secondarySlot;
    public Transform[] hangarBaySlot;
    public Transform[] specialSlot;

    public IEnumerable<Transform> allSlots
    {
        get
        {
            var slots = new List<Transform>();
            if (primarySlot != null) slots.AddRange(primarySlot);
            if (secondarySlot != null) slots.AddRange(secondarySlot);
            if (hangarBaySlot != null) slots.AddRange(hangarBaySlot);
            if (specialSlot != null) slots.AddRange(specialSlot);
            return slots;
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

    public void AddPrimaryWeapon(WeaponController weapon)
    {
        AddWeaponToCategory(weapon, "Primary");
    }

    public void AddSecondaryWeapon(WeaponController weapon)
    {
        AddWeaponToCategory(weapon, "Secondary");
    }

    public void AddHangarBay(WeaponController weapon)
    {
        AddWeaponToCategory(weapon, "Hangar");
    }

    public void AddSpecialWeapon(WeaponController weapon)
    {
        AddWeaponToCategory(weapon, "Special");
    }

    public void FirePrimaryWeapons()
    {
        foreach (var weapon in primaryWeapons) weapon.Shoot();
    }

    public void FireSecondaryWeapons()
    {
        foreach (var weapon in secondaryWeapons) weapon.Shoot();
    }

    public void FireHangarBay()
    {
        foreach (var hangar in hangarBayWeapons) hangar.Shoot();
    }

    public void FireSpecialWeapon()
    {
        foreach (var special in specialWeapons) special.Shoot();
    }
}