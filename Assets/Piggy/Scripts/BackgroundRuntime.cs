using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRuntime : MonoBehaviour
{
    public static Action OnEnergyChanged;
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

    private void Start()
    {
        CheckPlayerEnergy();
    }

    public void CheckPlayerEnergy()
    {
        if(PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy < PlayerManager.PlayerData.PlayerEnergy.MaxEnergy)
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
            OnCookingTimerChanged?.Invoke(cookingTimer);
        }
        else if(cookedFood != null)
        {
            OnCookingCompleted?.Invoke(cookedFood.Id);
            cookedFood = null;
        }
    }

    public void StartCooking(FoodSO food)
    {
        if(cookingCoroutine == null)
        {
            cookingFood = food;
            cookingTimer = food.CookingTime;
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
    #endregion
}
