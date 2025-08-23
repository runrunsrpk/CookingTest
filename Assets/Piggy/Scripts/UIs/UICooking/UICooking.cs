using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICooking : MonoBehaviour
{
    [Header("CookingPanel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;

    [Header("CookingMenu")]
    [SerializeField] private TMP_InputField menuSearch;
    [SerializeField] private TMP_Dropdown menuFilter;
    [SerializeField] private GameObject menuParent;
    [SerializeField] private GameObject pageParent;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private Button menuLeftArrow;
    [SerializeField] private Button menuRightArrow;

    [Header("CookingItem")]
    [SerializeField] private GameObject itemParent;

    [Header("CookingLimit")]
    [SerializeField] private Slider energySlider;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text cookingTimer;
    [SerializeField] private Button cookingBtn;

    [Header("CookingAnimation")]
    [SerializeField] private SkeletonGraphic cookingAnimation;

    [Header("CookingPopup")]
    [SerializeField] private UICookingPopup cookingPopup;

    private Spine.AnimationState cookingAnimationState;

    private int currentPage;
    private int maxPage;

    private List<FoodSO> foodMenus;
    private List<FoodSO> currentFoodMenus;
    private FoodSO currentFood;

    public void Show()
    {
        Debug.Log("Show Cooking Panel");

        panel.SetActive(true);

        InitPanel();
        InitListeners();
        InitMenuPage();
        InitSpine();

        BackgroundRuntime.OnEnergyChanged += UpdatePlayerEnergy;
        BackgroundRuntime.OnCookingTimerChanged += UpdateCookingTimer;
        BackgroundRuntime.OnCookingCompleted += UpdateCookingCompleted;
        BackgroundRuntime.Instance.CheckCookingTimer();
    }

    public void Hide()
    {
        //panel.SetActive(false);
        BackgroundRuntime.OnEnergyChanged -= UpdatePlayerEnergy;
        BackgroundRuntime.OnCookingTimerChanged -= UpdateCookingTimer;
        BackgroundRuntime.OnCookingCompleted -= UpdateCookingCompleted;

        Destroy(gameObject);
    }

    private void InitPanel()
    {
        foodMenus = Database.GetAllFoods();
        foodMenus.RemoveAt(0);
        currentFoodMenus = foodMenus;

        currentPage = 1;
        maxPage = GetMaxPage(currentFoodMenus.Count);

        UpdatePlayerEnergy();
    }

    private void InitListeners()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);

        menuLeftArrow.onClick.AddListener(OnClickLeftArrow);
        menuRightArrow.onClick.AddListener(OnClickRightArrow);

        cookingBtn.onClick.AddListener(OnClickCookingStart);

        menuSearch.onValueChanged.AddListener(OnSearchValueChanged);
        menuFilter.onValueChanged.AddListener(OnFilterValueChanged);
    }

    private void InitSpine()
    {
        cookingAnimationState = cookingAnimation.AnimationState;
        cookingAnimationState.SetAnimation(0, "idle", true);
    }

    #region CookingPanel
    private void OnClickCloseBtn()
    {
        Hide();
    }
    #endregion

    #region CookingMenu
    private int GetMaxPage(int menuAmount)
    {
        int maxPage = menuAmount / 4;
        int divPage = menuAmount % 4;
        if (divPage > 0) maxPage += 1;

        Debug.Log($"MaxPage: {maxPage}");
        return maxPage;
    }

    private void InitMenuPage()
    {
        CreatePageChildren();
        SetPageSelected(currentPage, true);
        UpdateArrow();
        UpdateMenuPage(currentPage, currentFoodMenus);
    }

    private void CreatePageChildren()
    {
        int pageCount = pageParent.transform.childCount;
        if (pageCount < maxPage)
        {
            int amount = maxPage - pageCount;
            for (int i = 0; i < amount; i++)
            {
                Instantiate(pagePrefab, pageParent.transform);
            }
        }
    }

    private void UpdatePageChildren()
    {
        for(int i = 0; i < pageParent.transform.childCount; i++)
        {
            UICookingPage uiPage = pageParent.transform.GetChild(i).gameObject.GetComponent<UICookingPage>();
            if (i < maxPage)
            {
                uiPage.Show();
                uiPage.PageDeselect();
            }
            else
            {
                uiPage.Hide();
            }
        }
    }

    private void SetPageSelected(int page, bool isSelect)
    {
        UICookingPage uiPage = pageParent.transform.GetChild(page - 1).gameObject.GetComponent<UICookingPage>();
        if (isSelect)
            uiPage.PageSelect();
        else
            uiPage.PageDeselect();
    }

    private void UpdateArrow()
    {
        SetActiveButton(menuLeftArrow, !(currentPage == 1));
        SetActiveButton(menuRightArrow, !(currentPage == maxPage));
    }

    private void OnClickLeftArrow()
    {
        if (currentPage > 1)
        {
            SetPageSelected(currentPage, false);
            currentPage--;
            SetPageSelected(currentPage, true);
        }

        UpdateArrow();
        UpdateMenuPage(currentPage, currentFoodMenus);
    }

    private void OnClickRightArrow()
    {
        if (currentPage < maxPage)
        {
            SetPageSelected(currentPage, false);
            currentPage++;
            SetPageSelected(currentPage, true);
        }

        UpdateArrow();
        UpdateMenuPage(currentPage, currentFoodMenus);
    }

    private void UpdateMenuPage(int page, List<FoodSO> foods)
    {
        DeselectAllFoods();

        int startIndex = ((page - 1) * 4);

        for(int i = 0; i < menuParent.transform.childCount; i++)
        {
            UICookingMenu food = menuParent.transform.GetChild(i).gameObject.GetComponent<UICookingMenu>();
            int index = startIndex + i;

            if (index >= foods.Count)
            {
                food.Hide();
                continue;
            }

            food.SetMenu(foods[index].Id);
            food.Show();

            if(i == 0)
                food.SelecFood();
        }
    }

    public void UpdateFoodItem(FoodSO food)
    {
        currentFood = food;

        for (int i = 0; i < itemParent.transform.childCount; i++)
        {
            UICookingItem foodItem = itemParent.transform.GetChild(i).gameObject.GetComponent<UICookingItem>();

            if (i > food.Ingredients.Count - 1)
            {
                foodItem.Hide();
                continue;
            }

            foodItem.SetItem(food.Ingredients[i].Id, food.Ingredients[i].Amount);
            foodItem.Show();
        }

        CheckCookingAllow();
    }

    public void DeselectAllFoods()
    {
        for (int i = 0; i < menuParent.transform.childCount; i++)
        {
            menuParent.transform.GetChild(i).gameObject.GetComponent<UICookingMenu>().DeselectFood();
        }
    }

    private void OnSearchValueChanged(string text)
    {
        currentFoodMenus = foodMenus.Where(food => food.Name.StartsWith(text)).ToList();

        currentPage = 1;
        maxPage = GetMaxPage(currentFoodMenus.Count);

        UpdatePageChildren();
        SetPageSelected(currentPage, true);

        UpdateArrow();
        UpdateMenuPage(currentPage, currentFoodMenus);
    }

    private void OnFilterValueChanged(int value)
    {
        switch (value)
        {
            case 0:
                currentFoodMenus = foodMenus;
                break;
            case 1:
                currentFoodMenus = foodMenus.Where(food => food.Star == 3).ToList();
                break;
            case 2:
                currentFoodMenus = foodMenus.Where(food => food.Star == 2).ToList();
                break;
            case 3:
                currentFoodMenus = foodMenus.Where(food => food.Star == 1).ToList();
                break;
        }

        currentPage = 1;
        maxPage = GetMaxPage(currentFoodMenus.Count);

        UpdatePageChildren();
        SetPageSelected(currentPage, true);

        UpdateArrow();
        UpdateMenuPage(currentPage, currentFoodMenus);
    }

    #endregion

    #region CookingLimit
    private void UpdateEnergy(float current, float max)
    {
        float energy = current / max;
        energySlider.value = energy;
        energyText.text = $"{current}/{max}";
    }

    private void UpdatePlayerEnergy()
    {
        UpdateEnergy(PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy, PlayerManager.PlayerData.PlayerEnergy.MaxEnergy);
    }

    private bool IsEnergyEnough()
    {
        return PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy >= 10;
    }

    private void ReduecPlayerEnergy()
    {
        PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy -= 10;
        UpdatePlayerEnergy();
    }

    private void ReducePlayerItems()
    {

        for (int i = 0; i < itemParent.transform.childCount; i++)
        {
            UICookingItem foodItem = itemParent.transform.GetChild(i).gameObject.GetComponent<UICookingItem>();

            if (!foodItem.gameObject.activeSelf)
                break;

            int itemId = foodItem.GetItemId();
            int itemAmount = PlayerManager.Instance.GetPlayerItemById(itemId).ItemAmount - foodItem.GetItemRequire();
            PlayerManager.Instance.SetPlayerItemById(itemId, itemAmount);

            foodItem.UpdateItem();
        }
    }

    private void CheckCookingAllow()
    {
        if (!IsEnergyEnough())
        {
            SetCookingBtn(false);
            return;
        }

        for (int i = 0; i < itemParent.transform.childCount; i++)
        {
            UICookingItem foodItem = itemParent.transform.GetChild(i).gameObject.GetComponent<UICookingItem>();

            if (!foodItem.gameObject.activeSelf)
                break;

            if(!foodItem.IsItemEnough())
            {
                SetCookingBtn(false);
                return;
            }    
        }

        SetCookingBtn(true);
    }

    private void SetCookingBtn(bool isAllow)
    {
        cookingBtn.interactable = isAllow;
    }

    private void SetCookingTimer(string text)
    {
        cookingTimer.text = text;
    }

    private void UpdateCookingTimer(int timer)
    {
        if(timer > 0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            SetCookingTimer(string.Format("{0:0}:{1:00}", minutes, seconds));
            return;
        }

        SetCookingTimer(string.Format("{0:0}:{1:00}", 0, 0));

        cookingAnimationState.SetAnimation(0, "success", false);
        cookingAnimationState.AddAnimation(0, "success-idle", true, 0f);

        //TODO: Popup result and reset cooking
        cookingPopup.SetImage(currentFood.Id);
        cookingPopup.Show();
    }

    private void UpdateCookingCompleted(int foodId)
    {
        cookingAnimationState.AddAnimation(0, "success-idle", true, 0f);

        //TODO: Popup result and reset cooking
        cookingPopup.SetImage(foodId);
        cookingPopup.Show();
    }

    private void OnClickCookingStart()
    {
        SetCookingBtn(false);

        //TODO: Play cooking animation
        cookingAnimationState.SetAnimation(0, "idle-boiled", true);

        //TODO: Reduce energy and items
        ReduecPlayerEnergy();
        ReducePlayerItems();

        //TODO: Start cooking timer
        BackgroundRuntime.Instance.StartCooking(currentFood);
        BackgroundRuntime.Instance.CheckPlayerEnergy();
    }

    public void UpdateCookingLimit()
    {
        cookingAnimationState.SetAnimation(0, "idle", true);

        CheckCookingAllow();
    }
    #endregion

    #region Helper
    private void SetActiveButton(Button button, bool isActive)
    {
        button.gameObject.SetActive(isActive);
    }
    #endregion
}
