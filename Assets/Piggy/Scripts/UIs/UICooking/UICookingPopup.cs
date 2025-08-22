using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICookingPopup : MonoBehaviour
{
    [SerializeField] private Image foodImage;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button fadeBtn;


    private UICooking uiCooking;

    private void Awake()
    {
        uiCooking = GetComponentInParent<UICooking>();
    }

    private void Start()
    {
        confirmBtn.onClick.AddListener(OnClickConfirm);
        fadeBtn.onClick.AddListener(OnClickConfirm);
    }

    public void SetImage(int foodId)
    {
        foodImage.sprite = Database.GetFoodIcon(foodId);
    }

    public void Show()
    {
        transform.gameObject.SetActive(true);
    }

    public void Hide()
    {
        transform.gameObject.SetActive(false);
    }
    
    private void OnClickConfirm()
    {
        Hide();
        uiCooking.UpdateCookingLimit();
    }
}
