using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    public void SavePlayer(PlayerData playerData)
    {
        string jsonString = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerDataJson", jsonString);
        PlayerPrefs.Save();
    }

    public void LoadPlayer()
    {
        //PlayerPrefs.DeleteKey("PlayerDataJson");

        string loadedJsonString = PlayerPrefs.GetString("PlayerDataJson", "");
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(loadedJsonString);

        if (string.IsNullOrEmpty(loadedJsonString))
        {
            CreateNewPlayerData();
            return;
        }

        if (playerData != null)
        {
            PlayerManager.Instance.SetPlayerData(playerData);
        }
        else
        {
            CreateNewPlayerData();
        }
    }

    private void CreateNewPlayerData()
    {
        PlayerData newData = new PlayerData();
        newData.PlayerEnergy = new PlayerEnergy();
        newData.PlayerEnergy.CurrentEnergy = 20;
        newData.PlayerEnergy.MaxEnergy = 30;

        newData.PlayerInventory = new PlayerInventory();
        List<PlayerItem> playerItems = new List<PlayerItem>();
        playerItems.Add(new PlayerItem(1, 25));
        playerItems.Add(new PlayerItem(2, 100));
        playerItems.Add(new PlayerItem(3, 150));
        playerItems.Add(new PlayerItem(4, 200));
        newData.PlayerInventory.Items = playerItems;

        newData.PlayerCooking = new PlayerCooking();
        newData.PlayerCooking.FoodId = 0;
        newData.PlayerCooking.FoodCookingTimer = 0;
       
        newData.PlayerLastedActive = System.DateTime.Now.ToString("o");

        PlayerManager.Instance.SetPlayerData(newData);
        SavePlayer(newData);
    }

    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            LoadPlayer();

            if(Database.IsDatabaseLoaded)
                BackgroundRuntime.Instance.UpdateTimeDiff();
        }
        else
        {
            PlayerManager.Instance.SetPlayerLastedActive(System.DateTime.Now);
            SavePlayer(PlayerManager.PlayerData);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerManager.Instance.SetPlayerLastedActive(System.DateTime.Now);
        SavePlayer(PlayerManager.PlayerData);
    }
}
