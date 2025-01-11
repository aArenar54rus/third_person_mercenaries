using System;


namespace Arenar
{
    [Serializable]
    public enum WeaponType : byte
    {
        None = 0,
        Melee = 1,
        Firearm = 2,
        Throwing = 3,
    }
}