using System.Collections.Generic;
public class InventoryId
{
    public string itemName { get; set; }
    public int upgrade { get; set; }
    public Enchant enchant { get; set; }
}

public class Enchant
{
    public Flat flat { get; set; }
    public Percent percent { get; set; }
}

public class Flat
{
    public float hp { get; set; }
    public float tp { get; set; }
    public float patk { get; set; }
    public float matk { get; set; }
    public float pdef { get; set; }
    public float mdef { get; set; }
    public float hit { get; set; }
    public float dodge { get; set; }
    public float crit { get; set; }
    public float critEva { get; set; }
}

public class Percent
{
    public float hp { get; set; }
    public float tp { get; set; }
    public float patk { get; set; }
    public float matk { get; set; }
    public float pdef { get; set; }
    public float mdef { get; set; }
    public float critDmgBoost { get; set; }
    public float critDmgReduc { get; set; }
    public float dropChance { get; set; }
}

public class InventoryAPI
{
    public Dictionary<int,InventoryId> inventoryId { get; set; }
}




