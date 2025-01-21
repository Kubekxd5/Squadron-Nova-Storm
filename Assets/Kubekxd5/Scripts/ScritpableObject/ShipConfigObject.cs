using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShipConfig", menuName = "Configs/ShipConfig")]
public class ShipConfigObject : ScriptableObject
{
    [Header("Possible Weapons")] public List<GameObject> equippedWeapons;

    [Header("Possible Ships")] public List<GameObject> ships;

    [Header("Ship Selection")] public int shipIndex;

    [Header("Weapon Configuration")] public List<ShipSlotConfig> weaponSlots = new();

    [Serializable]
    public class ShipSlotConfig
    {
        public string slotName;
        public string weaponName;
    }
}