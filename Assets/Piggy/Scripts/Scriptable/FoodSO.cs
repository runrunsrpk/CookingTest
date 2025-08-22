using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Database/FoodData", order = 1)]
public class FoodSO : ScriptableObject
{
    public int Id;
    public string Name;
    public int Star;
    public int CookingTime;
    public List<FoodIngredient> Ingredients;
}

[System.Serializable]
public class FoodIngredient
{
    public int Id;
    public int Amount;
}
