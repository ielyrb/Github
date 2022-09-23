using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData
{ 
    public string itemName { get; set; }
}

public class EquippedItemAPI
{
    public Dictionary<string, EquipmentData> equipmentdata { get; set; }
}
