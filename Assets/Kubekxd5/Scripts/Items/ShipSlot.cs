using UnityEngine;

public class ShipSlot : MonoBehaviour
{
    public enum WeaponMount
    {
        Primary,
        Secondary,
        Hangar,
        Special
    }

    [Header("Weapon Mount:")] public WeaponMount weaponMount;

    [Header("Equipment:")] public WeaponController weaponController;

    public void AddNewWeapon()
    {
        var slotsManager = transform.GetComponentInParent<SlotsManager>();
        switch (weaponMount)
        {
            case WeaponMount.Primary:
                slotsManager.AddPrimaryWeapon(weaponController);
                break;
            case WeaponMount.Secondary:
                slotsManager.AddSecondaryWeapon(weaponController);
                break;
            case WeaponMount.Hangar:
                slotsManager.AddHangarBay(weaponController);
                break;
            case WeaponMount.Special:
                slotsManager.AddSpecialWeapon(weaponController);
                break;
            default:
                return;
        }
    }

    public void RemoveWeapon()
    {
        if (weaponController != null)
        {
            Destroy(weaponController.gameObject);
            weaponController = null;
        }
    }
}