using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    public PlayerEnergy PlayerEnergy;
    public PlayerInventory PlayerInventory;
}

[System.Serializable]
public class PlayerEnergy
{
    public int CurrentEnergy;
    public int MaxEnergy;
}

[System.Serializable]
public class PlayerInventory
{
    public List<PlayerItem> Items;
}

[System.Serializable]
public class PlayerItem
{
    public int ItemID;
    public int ItemAmount;
}
