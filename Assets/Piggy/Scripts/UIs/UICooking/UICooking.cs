using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
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
        InitButtons();
        InitMenuPage();
        InitSpine();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void InitPanel()
    {
        foodMenus = Database.GetAllFoods();
        currentFoodMenus = Database.GetAllFoods();
        currentFoodMenus.RemoveAt(0);

        currentPage = 1;
        maxPage = GetMaxPage(currentFoodMenus.Count);

        UpdateEnergy(PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy, PlayerManager.PlayerData.PlayerEnergy.MaxEnergy);
    }

    private void InitButtons()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);

        menuLeftArrow.onClick.AddListener(OnClickLeftArrow);
        menuRightArrow.onClick.AddListener(OnClickRightArrow);

        cookingBtn.onClick.AddListener(OnClickCookingStart);
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
        SetActiveButton(menuLeftArrow, !(currentPage == 1));
        SetActiveButton(menuRightArrow, !(currentPage == maxPage));

        UpdateMenuPage(currentPage, currentFoodMenus);
    }

    private void OnClickLeftArrow()
    {
        if (currentPage > 1)
        {
            currentPage--;
        }

        SetActiveButton(menuLeftArrow, !(currentPage == 1));
    }

    private void OnClickRightArrow()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
        }

        SetActiveButton(menuRightArrow, !(currentPage == maxPage));
    }

    private void UpdateMenuPage(int page, List<FoodSO> foods)
    {
        DeselectAllFoods();

        int startIndex = ((page - 1) * 4) + 1;

        for(int i = 0; i < menuParent.transform.childCount; i++)
        {
            UICookingMenu food = menuParent.transform.GetChild(i).gameObject.GetComponent<UICookingMenu>();
            int index = startIndex + i;

            if (index > foods.Count)
            {
                food.Hide();
                continue;
            }

            food.SetMenu(index);
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

    private void UpdateSearch()
    {

    }

    private void UpdateFilter()
    {

    }

    #endregion

    #region CookingLimit
    private void UpdateEnergy(float current, float max)
    {
        float energy = current / max;
        energySlider.value = energy;
        energyText.text = $"{current}/{max}";
    }

    private bool IsEnergyEnough()
    {
        return PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy >= 10;
    }

    private void ReduecPlayerEnergy()
    {
        int energy = PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy - 10;
        PlayerManager.PlayerData.PlayerEnergy.CurrentEnergy = energy;
        UpdateEnergy(energy, PlayerManager.PlayerData.PlayerEnergy.MaxEnergy);
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

    private void OnClickCookingStart()
    {
        SetCookingBtn(false);

        //TODO: Play cooking animation
        cookingAnimationState.SetAnimation(0, "idle-boiled", true);

        //TODO: Reduce energy and items
        ReduecPlayerEnergy();
        ReducePlayerItems();

        //TODO: Start cooking timer
        StartCoroutine(EnumCookingTime(currentFood.CookingTime));
    }

    private IEnumerator EnumCookingTime(int cookingTime)
    {
        while (cookingTime > 0)
        {
            int minutes = Mathf.FloorToInt(cookingTime / 60);
            int seconds = Mathf.FloorToInt(cookingTime % 60);
            cookingTimer.text = string.Format("{0:0}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1f);

            cookingTime--;
        }

        cookingTimer.text = string.Format("{0:0}:{1:00}", 0, 0);
        cookingAnimationState.SetAnimation(0, "success", false);
        cookingAnimationState.AddAnimation(0, "success-idle", true, 0f);

        //TODO: Popup result and reset cooking
        cookingPopup.SetImage(currentFood.Id);
        cookingPopup.Show();
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
