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

    [Header("Equipment:")] public GameObject item;
    public WeaponController weaponController;

    private void Start()
    {
        if (transform.childCount > 0)
        {
            item = transform.GetChild(0).gameObject;
            weaponController = item.GetComponent<WeaponController>();
            Debug.Log("Item assigned: " + item.name + " - found in: " + name);
        }
        else
        {
            Debug.Log("No item found in: " + name + " - " + tag);
        }
    }
}
