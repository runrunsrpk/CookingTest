using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public PlayerEnergy PlayerEnergy;
    public PlayerInventory PlayerInventory;
    public PlayerCooking PlayerCooking;
    public string PlayerLastedActive;
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
    public PlayerItem(int itemId, int itemAmount)
    {
        ItemID = itemId;
        ItemAmount = itemAmount;
    }

    public int ItemID;
    public int ItemAmount;
}

[System.Serializable]
public class PlayerCooking
{
    public int FoodId;
    public int FoodCookingTimer;
}
