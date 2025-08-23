using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public static PlayerData PlayerData => playerData;
    private static PlayerData playerData;

    //private PlayerSave playerSave;
    private UICooking uiCooking;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }    
    }

    private void Start()
    {
        //playerSave = GetComponent<PlayerSave>();

        Debug.Log($"PlayerEnergy: {playerData.PlayerEnergy.CurrentEnergy}/{playerData.PlayerEnergy.MaxEnergy}");
        Debug.Log($"PlayerInventory: {playerData.PlayerInventory.Items.Count}");

        BackgroundRuntime.Instance.UpdateTimeDiff();

        uiCooking = UILoader.Instance.LoadUI("UICooking/UICooking").GetComponent<UICooking>();
        uiCooking.Show();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (uiCooking != null)
                return;

            uiCooking = UILoader.Instance.LoadUI("UICooking/UICooking").GetComponent<UICooking>();
            uiCooking.Show();
        }
    }

    public PlayerItem GetPlayerItemById(int id)
    {
        return playerData.PlayerInventory.Items.FirstOrDefault(player => player.ItemID == id);
    }

    public void SetPlayerItemById(int id, int amount)
    {

        PlayerItem playerItem = GetPlayerItemById(id);
        playerItem.ItemAmount = amount;
    }
    

    public void SetPlayerLastedActive(DateTime dateTime)
    {
        playerData.PlayerLastedActive = dateTime.ToString("o");
    }

    public void SetPlayerData(PlayerData data)
    {
        playerData = data;
    }
}
