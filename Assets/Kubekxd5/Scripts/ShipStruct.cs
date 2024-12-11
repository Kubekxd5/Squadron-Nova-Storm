using System;
using System.Collections.Generic;

[Serializable]
public class ShipData
{
    public string shipName;
    public List<WeaponData> weapons;
}

[Serializable]
public class WeaponData
{
    public string weaponName;
    public string weaponMount;
}