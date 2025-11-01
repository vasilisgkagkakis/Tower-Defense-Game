using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerDescriptionUI : MonoBehaviour
{
    public TMP_Text damage;
    public TMP_Text areaType;
    public TMP_Text sellValueText;
    public TMP_Text upgradeCostText;
    public TMP_Dropdown targetingDropdown;
    public GameObject DescriptionUI;
    public Button upgradeButton;
    public Button sellButton;

    private TowerBehaviour currentTower;

    public static TowerDescriptionUI Instance;

    void Awake()
    {
        Instance = this;
    }


    public void UpdateDropdown(TowerBehaviour tower)
    {
        if (currentTower == tower && targetingDropdown != null)
            targetingDropdown.value = (int)tower.targetType;
    }

    public void Show(TowerBehaviour tower)
    {
        currentTower = tower;
        var data = tower.towerData;
        if (damage != null) damage.text = $"Damage: {tower.currentDamage}";
        if (areaType != null) areaType.text = data.isAreaDamage ? "Target: Area" : "Type: Single";
        
        // Show sell value
        if (sellValueText != null)
        {
            int sellValue = tower.towerData.CalculateSellValue(tower.totalInvestment);
            sellValueText.text = $"Sell: ${sellValue}";
        }
        
        // Show upgrade cost
        if (upgradeCostText != null)
        {
            bool canUpgrade = tower.upgradeLevel < tower.upgradePrefabs.Length;
            if (canUpgrade && tower.towerData.upgradeCosts.Length > tower.actualUpgradeLevel)
            {
                int upgradeCost = tower.towerData.upgradeCosts[tower.actualUpgradeLevel];
                upgradeCostText.text = $"Upgrade: ${upgradeCost}";
            }
            else
            {
                upgradeCostText.text = "Max Level";
            }
        }

        // Setup dropdown options if not already set
        if (targetingDropdown != null && targetingDropdown.options.Count == 0)
        {
            targetingDropdown.options.Clear();
            foreach (var name in System.Enum.GetNames(typeof(TowerTargeting.TargetType)))
                targetingDropdown.options.Add(new TMP_Dropdown.OptionData(name));
        }

        if (targetingDropdown != null)
        {
            targetingDropdown.value = (int)tower.targetType;
            targetingDropdown.onValueChanged.RemoveAllListeners();
            targetingDropdown.onValueChanged.AddListener(OnTargetingChanged);
        }

        // Enable/disable upgrade button based on upgrade availability and currency
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeButton);
            
            bool canUpgrade = currentTower.upgradeLevel < currentTower.upgradePrefabs.Length;
            bool hasEnoughCurrency = true;
            
            if (canUpgrade && CurrencyManager.Instance != null)
            {
                int upgradeCost = currentTower.towerData.upgradeCosts.Length > currentTower.actualUpgradeLevel ? 
                    currentTower.towerData.upgradeCosts[currentTower.actualUpgradeLevel] : 100;
                hasEnoughCurrency = CurrencyManager.Instance.GetCurrency() >= upgradeCost;
            }
            
            upgradeButton.interactable = canUpgrade && hasEnoughCurrency;
        }

        // Setup sell button
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnSellButton);
            sellButton.interactable = true; // Can always sell a placed tower
        }

        DescriptionUI.SetActive(true);
    }

    public void Hide()
    {
        DescriptionUI.SetActive(false);
    }

    private void OnTargetingChanged(int value)
    {
        if (currentTower != null)
            currentTower.targetType = (TowerTargeting.TargetType)value;
    }

    public void OnUpgradeButton()
    {
        if (currentTower != null)
        {
            currentTower.UpgradeTower();
        }
    }

    public void OnSellButton()
    {
        if (currentTower != null)
        {
            SellTower(currentTower);
        }
    }

    private void SellTower(TowerBehaviour tower)
    {
        if (tower == null || !tower.isPlaced) return;

        // Calculate sell value using TowerData method
        int sellValue = tower.towerData.CalculateSellValue(tower.totalInvestment);
        
        // Add currency
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCurrency(sellValue);
            Debug.Log($"Sold tower for {sellValue} coins (75% of {tower.totalInvestment} invested)");
        }

        // Clear selection and hide UI
        if (TowerSelector.selectedTower == tower)
        {
            TowerSelector.selectedTower = null;
        }
        
        if (TowerSelector.lastHighlightedTower == tower)
        {
            TowerSelector.lastHighlightedTower = null;
        }

        Hide();

        // Destroy the tower
        Destroy(tower.gameObject);
    }
}