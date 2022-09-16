using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tantra_Masters_GM_Tool
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class InventoryId
    {
        public int itemId { get; set; }
        public string itemName { get; set; }
        public string itemDescription { get; set; }
        public string grade { get; set; }
        public Stats stats { get; set; }
        public int upgrade { get; set; }
        public Enchant enchant { get; set; }
        public string rawText { get; set; }
    }

    public class Enchant
    {
        public Flat flat { get; set; }
        public Percent percent { get; set; }
    }

    public class Flat
    {
        public int hp { get; set; }
        public int tp { get; set; }
        public int patk { get; set; }
        public int matk { get; set; }
        public int pdef { get; set; }
        public int mdef { get; set; }
        public int hit { get; set; }
        public int dodge { get; set; }
        public int crit { get; set; }
        public int critEva { get; set; }
    }

    public class Percent
    {
        public int hp { get; set; }
        public int tp { get; set; }
        public int patk { get; set; }
        public int matk { get; set; }
        public int pdef { get; set; }
        public int mdef { get; set; }
        public int critDmgBoost { get; set; }
        public int critDmgReduc { get; set; }
        public int dropChance { get; set; }
    }

    public class InventoryItemAPI
    {
        public Dictionary<int,InventoryId> inventoryId { get; set; }
    }

    public class Stats
    {
        public Flat flat { get; set; }
        public Percent percent { get; set; }
    }

}
