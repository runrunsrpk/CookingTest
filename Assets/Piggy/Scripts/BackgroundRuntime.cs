using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRuntime : MonoBehaviour
{
    public static Action OnEnergyChanged;
    public static Action<bool> OnCookingStarted;
    public static Action<int> OnCookingTimerChanged;
    public static Action<int> OnCookingCompleted;

    public static BackgroundRuntime Instance;

    private IEnumerator playerEnergyCoroutine;

    private IEnumerator cookingCoroutine;
    private int cookingTimer;
    private FoodSO cookingFood;
    private FoodSO cookedFood;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateTimeDiff()
    {
        UpdatePlayerEnergyDiff();
        UpdateCookingTimerDiff();
    }

    private void UpdatePlayerEnergyDiff()
    {
        if (playerEnergyCoroutine != null)
        {
            StopCoroutine(playerEnergyCoroutine);
            playerEnergyCoroutine = null;
        }

        int secondTimeDiff = GetSecondTimeDiff();
        int addedEnergy = secondTimeDiff / 5;
        PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy = PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy + addedEnergy;

        if (PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy > PlayerManager.PlayerData.PlayerEnergy.MaxEnergy)
            PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy = PlayerManager.PlayerData.PlayerEnergy.MaxEnergy;

        CheckPlayerEnergy();
    }

    public void CheckPlayerEnergy()
    {
        OnEnergyChanged?.Invoke();

        if (PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy < PlayerManager.PlayerData.PlayerEnergy.MaxEnergy)
        {
            if(playerEnergyCoroutine == null)
            {
                playerEnergyCoroutine = EnumPlayerEnergy();
                StartCoroutine(playerEnergyCoroutine);
            }
        }
    }

    private IEnumerator EnumPlayerEnergy()
    {
        while (PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy < PlayerManager.PlayerData.PlayerEnergy.MaxEnergy)
        {
            yield return new WaitForSecondsRealtime(5f);

            PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy++;
            OnEnergyChanged?.Invoke();
        }
        
        StopCoroutine(playerEnergyCoroutine);
        playerEnergyCoroutine = null;
    }

    #region Cooking Background
    public void CheckCookingTimer()
    {
        if(cookingTimer > 0 && cookingCoroutine != null)
        {
            OnCookingStarted?.Invoke(true);
            OnCookingTimerChanged?.Invoke(cookingTimer);
        }
        else if(cookedFood != null)
        {
            OnCookingTimerChanged?.Invoke(0);
            OnCookingCompleted?.Invoke(cookedFood.Id);
            cookedFood = null;

            PlayerManager.PlayerData.PlayerCooking.FoodId = 0;
            PlayerManager.PlayerData.PlayerCooking.FoodCookingTimer = 0;

            if (cookingCoroutine != null)
            {
                StopCoroutine (cookingCoroutine);
                cookingCoroutine = null;
            }
        }
    }

    public void StartCooking(FoodSO food)
    {
        if(cookingCoroutine == null)
        {
            cookingFood = food;
            cookingTimer = food.CookingTime;
            PlayerManager.PlayerData.PlayerCooking.FoodId = food.Id;
            PlayerManager.PlayerData.PlayerCooking.FoodCookingTimer = cookingTimer;
            cookingCoroutine = EnumCooking(cookingTimer);
            StartCoroutine(cookingCoroutine);
        }
    }

    private void ContinueCooking(FoodSO food, int remainingTime)
    {
        cookingFood = food;
        cookingTimer = remainingTime;
        OnCookingTimerChanged?.Invoke(remainingTime);
        PlayerManager.PlayerData.PlayerCooking.FoodId = food.Id;
        PlayerManager.PlayerData.PlayerCooking.FoodCookingTimer = remainingTime;

        if (cookingCoroutine == null)
        {
            cookingCoroutine = EnumCooking(cookingTimer);
            StartCoroutine(cookingCoroutine);
        }
    }

    private IEnumerator EnumCooking(int timer)
    {
        while (timer > 0)
        {
            OnCookingTimerChanged?.Invoke(timer);
            yield return new WaitForSecondsRealtime(1f);
            timer--;
            cookingTimer = timer;
            PlayerManager.PlayerData.PlayerCooking.FoodCookingTimer = timer;
        }

        GameObject loadedUI = UILoader.Instance.GetLoadedUI();
        UICooking uiCooking = null;
        if (loadedUI != null)
        {
            uiCooking = loadedUI.GetComponent<UICooking>();
        }

        if (uiCooking != null)
        {
            OnCookingTimerChanged?.Invoke(0);
        }
        else
        {
            cookedFood = cookingFood;
        }

        StopCoroutine(cookingCoroutine);
        cookingCoroutine = null;
        cookingFood = null;
    }

    private void UpdateCookingTimerDiff()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
        }

        // Player Cooking
        int foodId = PlayerManager.PlayerData.PlayerCooking.FoodId;
        if (foodId > 0)
        {
            int secondTimeDiff = GetSecondTimeDiff();
            int remainingTime = PlayerManager.PlayerData.PlayerCooking.FoodCookingTimer - secondTimeDiff;
            
            FoodSO food = Database.GetFood(foodId);

            if (remainingTime <= 0)
            {
                cookingTimer = 0;
                cookingFood = null;
                cookedFood = food;

                CheckCookingTimer();
            }
            else
            {
                ContinueCooking(food, remainingTime);
            }

        }
    }
    #endregion

    #region Helper
    private int GetSecondTimeDiff()
    {
        DateTime startTime = DateTime.Parse(PlayerManager.PlayerData.PlayerLastedActive);
        DateTime endTime = DateTime.Now;

        // Calculate the TimeSpan difference
        TimeSpan timeDifference = endTime - startTime;

        // Get the total difference in seconds
        int diffInSeconds = (int)timeDifference.TotalSeconds;

        return diffInSeconds;
    }
    #endregion
}
