using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drop Table", menuName = "Scriptable Objects/Drop Table", order = 2)]
public class ItemDropTable : ScriptableObject
{
    public int id;

    [Space]
    [NonReorderable]
    public List<DropItem> drops = new List<DropItem>();
}
