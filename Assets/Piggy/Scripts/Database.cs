using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static bool IsDatabaseLoaded = false;

    public static Dictionary<int, FoodSO> FoodDatas => foodDatas;
    private static Dictionary<int, FoodSO> foodDatas;

    public static Dictionary<int, ItemSO> ItemDatas => itemDatas;
    private static Dictionary<int, ItemSO> itemDatas;

    private static Dictionary<int, Sprite> foodSprites;
    private static Dictionary<int, Sprite> itemSprites;

    private void Start()
    {
        LoadDatabase();
    }

    public void LoadDatabase()
    {
        LoadFoodData();
        LoadItemData();

        LoadFoodSprite();
        LoadItemSprite();

        IsDatabaseLoaded = true;
    }

    #region FoodDatabase
    private void LoadFoodData()
    {
        foodDatas = new Dictionary<int, FoodSO>();

        FoodSO[] foodRsc = Resources.LoadAll<FoodSO>("Database/FoodDB");

        foreach(FoodSO food in foodRsc)
        {
            if(!foodDatas.ContainsKey(food.Id))
            {
                foodDatas.Add(food.Id, food);
            }
        }

        Debug.Log($"FoodData count: {foodDatas.Count}");
    }

    private void LoadFoodSprite()
    {
        foodSprites = new Dictionary<int, Sprite>();

        Sprite[] foodRsc = Resources.LoadAll<Sprite>("Icons/Food");

        int index = 0;
        foreach (Sprite food in foodRsc)
        {
            if (!foodSprites.ContainsKey(index))
            {
                foodSprites.Add(index, food);
                index++;
            }
        }

        Debug.Log($"FoodIcons count: {foodSprites.Count}");
    }

    public static FoodSO GetFood(int id)
    {
        if(foodDatas.ContainsKey(id))
            return foodDatas[id];
        
        return null;
    }

    public static List<FoodSO> GetAllFoods()
    {
        return foodDatas.Values.ToList();
    }

    public static Sprite GetFoodIcon(int id)
    {
        if (foodSprites.ContainsKey(id))
            return foodSprites[id];

        return null;
    }
    #endregion

    #region ItemDatabase
    private void LoadItemData()
    {
        itemDatas = new Dictionary<int, ItemSO>();

        ItemSO[] itemRsc = Resources.LoadAll<ItemSO>("Database/ItemDB");

        foreach (ItemSO item in itemRsc)
        {
            if (!itemDatas.ContainsKey(item.Id))
            {
                itemDatas.Add(item.Id, item);
            }
        }

        Debug.Log($"ItemData count: {itemDatas.Count}");
    }

    private void LoadItemSprite()
    {
        itemSprites = new Dictionary<int, Sprite>();

        Sprite[] itemRsc = Resources.LoadAll<Sprite>("Icons/Item");

        int index = 0;
        foreach (Sprite item in itemRsc)
        {
            if (!itemSprites.ContainsKey(index))
            {
                itemSprites.Add(index, item);
                index++;
            }
        }

        Debug.Log($"ItemIcons count: {itemSprites.Count}");
    }

    public static ItemSO GetItem(int id)
    {
        if (itemDatas.ContainsKey(id))
            return itemDatas[id];

        return null;
    }

    public static Sprite GetItemIcon(int id)
    {
        if (itemSprites.ContainsKey(id))
            return itemSprites[id];

        return null;
    }
    #endregion
}