using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewShipConfig", menuName = "Configs/ShipConfig")]
public class ShipConfigObject : ScriptableObject
{
    [Header("Possible Weapons")] 
    public List<GameObject> equippedWeapons;
    [Header("Possible Ships")] 
    public List<GameObject> ships;

    [Header("Ship Selection")]
    public int shipIndex;

    [Header("Weapon Configuration")]
    public List<ShipSlotConfig> weaponSlots = new List<ShipSlotConfig>();

    [System.Serializable]
    public class ShipSlotConfig
    {
        public string slotName;
        public string weaponName;
    }
}
