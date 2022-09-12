using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int charLevel { get; set; }
    public int exp { get; set; }
    public int tribe { get; set; }
    public int job { get; set; }
    public int rupiahs { get; set; }
    public int maxInventorySlot { get; set; }
    public int expRequired { get; set; }

    public int expToTwoLevels { get; set; }
}

public class CharInfoAPI
{
    public Data data { get; set; }
}
