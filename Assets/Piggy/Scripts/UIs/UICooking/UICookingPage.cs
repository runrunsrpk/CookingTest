using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICookingPage : MonoBehaviour
{
    [Header("UICookingPage")]
    [SerializeField] private Image pageImage;
    [Header("PageSprite")]
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite deselectedSprite;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PageSelect()
    {
        pageImage.sprite = selectedSprite;
    }

    public void PageDeselect()
    {
        pageImage.sprite = deselectedSprite;
    }
}
