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

        itemImage.sprite = itemSprite;
        itemImage.SetNativeSize();

        UpdateItem();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public int GetItemId()
    {
        return itemData.Id;
    }

    public int GetItemRequire()
    {
        return itemRequire;
    }

    public bool IsItemEnough()
    {
        return playerItemAmount >= itemRequire;
    }

    public void UpdateItem()
    {
        PlayerItem playerItem = PlayerManager.Instance.GetPlayerItemById(itemData.Id);
        playerItemAmount = (playerItem == null) ? 0 : playerItem.ItemAmount;

        if (playerItemAmount < itemRequire)
        {
            itemAmountText.text = $"<color=#FF0000>{playerItemAmount}</color>/{itemRequire}";
        }
        else
        {
            itemAmountText.text = $"{playerItemAmount}/{itemRequire}";
        }
    }
}
