using System;
using System.Collections.Generic;

[System.Serializable]
public class ShipData
{
    public string shipName;
    public List<WeaponData> weapons;
}

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public string weaponMount;
    public int slotIndex;
}
