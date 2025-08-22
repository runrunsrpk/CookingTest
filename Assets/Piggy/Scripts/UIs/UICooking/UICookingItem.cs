using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICookingItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemAmountText;

    private ItemSO itemData;
    private Sprite itemSprite;
    private int itemRequire;
    private int playerItemAmount;

    public void SetItem(int id, int require)
    {
        itemData = Database.GetItem(id);
        itemSprite = Database.GetItemIcon(id);
        itemRequire = require;

        PlayerItem playerItem = PlayerManager.Instance.GetPlayerItemById(id);
        playerItemAmount = (playerItem == null) ? 0 : playerItem.ItemAmount;

        itemImage.sprite = itemSprite;
        itemImage.SetNativeSize();

        if(playerItemAmount == 0)
        {
            itemAmountText.text = $"<color=#FF0000>{playerItemAmount}</color>/{require}";
        }
        else
        {
            itemAmountText.text = $"{playerItemAmount}/{require}";
        }
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsItemEnough()
    {
        return playerItemAmount >= itemRequire;
    }
}
