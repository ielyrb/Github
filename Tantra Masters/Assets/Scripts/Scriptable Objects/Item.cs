using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item", order = 1)]
public class Item : ScriptableObject
{
    //public string itemName;
    public string itemDesc;
    [Space]
    public ItemGrade itemGrade;
    public ItemType itemType;
    public WeaponType weaponType;
    [Space]
    public bool canStack;
    public bool canEquip;
    public bool canExpire;
    public DateTime expiryDate;
    [Space]
    public ItemStats stats;

    public enum WeaponType
    {
        None,
        Sword,
        Blade,
        Dagger,
        Bow,
        Shield,
        Spear,
        Axe,
        HeavyAxe,
        Cane,
        Staff
    }

    public enum ItemGrade
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        DemiGod,
    }

    public enum ItemType
    {
        Collectible,
        Event,
        Special,
        Weapon,
        Offhand,
        Helm,
        Chest,
        Pants,
        Gloves,
        Belt,
        Boots,
        Necklace,
        Charm,
        Earring,
        Bracelet,
        Ring,
        Mount,
        Eyewear,
        Headdress
    }
}

[System.Serializable]
public class ItemStats
{
    public FlatStats flat;
    public PercentStats percent;
}

[System.Serializable]
public class FlatStats
{
    public int hp;
    public int tp;
    public int patk;
    public int matk;
    public int pdef;
    public int mdef;
    public int hit;
    public int dodge;
    public int crit;
    public int critEva;
}

[System.Serializable]
public class PercentStats
{
    public float hp;
    public float tp;
    public float patk;
    public float matk;
    public float pdef;
    public float mdef;
    public float critDamageBoost;
    public float critDamageReduc;
    public float dropChance;
}
