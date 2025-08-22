using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Database/ItemData", order = 2)]
public class ItemSO : ScriptableObject
{
    public int Id;
    public string Name;
    public string Description;
}
