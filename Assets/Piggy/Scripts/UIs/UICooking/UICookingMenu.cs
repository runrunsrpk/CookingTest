using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICookingMenu : MonoBehaviour
{
    [Header("CookingMenuPanel")]
    [SerializeField] private Button panelBtn;
    [SerializeField] private Image menuImage;
    [SerializeField] private Image menuImageOutline;
    [SerializeField] private TMP_Text menuText;
    [SerializeField] private Image menuBG;
    [SerializeField] private List<Sprite> menuBGs = new List<Sprite>();
    [Header("FoodItem")]
    [SerializeField] private GameObject itemParent;

    private UICooking uiCooking;
    private FoodSO foodData;

    private void Awake()
    {
        uiCooking = GetComponentInParent<UICooking>();
    }

    private void Start()
    {
        panelBtn.onClick.AddListener(OnClickSelect);
    }

    public void SetMenu(int id)
    {
        FoodSO food = Database.GetFood(id);
        Sprite foodSprite = Database.GetFoodIcon(id);

        foodData = food;

        menuImage.sprite = foodSprite;
        menuText.text = food.Name;
        menuBG.sprite = menuBGs[food.Star - 1]; 
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }

    public void SelecFood()
    {
        uiCooking.DeselectAllFoods();
        uiCooking.UpdateFoodItem(foodData);

        menuImageOutline.gameObject.SetActive(true);

        Debug.Log($"Select food: {foodData.Id}");
    }

    public void DeselectFood()
    {
        menuImageOutline.gameObject.SetActive(false);
    }

    private void OnClickSelect()
    {
        SelecFood();
    }

    
}
