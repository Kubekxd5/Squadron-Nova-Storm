using UnityEngine;

using UnityEngine;

public class ShipSlots : MonoBehaviour
{
    public enum WeaponMount
    {
        Primary,
        Secondary,
        Hangar,
        Special
    };
    
    [Header("Weapon Mount:")]
    public WeaponMount weaponMount;
    
    [Header("Equipment:")]
    public WeaponController weaponController;

    private void Start()
    {
        if (transform.childCount > 0)
        {
            weaponController = transform.GetChild(0).GetComponent<WeaponController>();
            if (weaponController != null)
            {
                Debug.Log($"{weaponController.weaponName} equipped in {weaponMount} slot.");
            }
        }
    }
}

