using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerDescriptionUI : MonoBehaviour
{
    public TMP_Text damage;
    public TMP_Text areaType;
    public TMP_Dropdown targetingDropdown;
    public GameObject DescriptionUI;
    public Button upgradeButton; // <-- Add this line

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

        // Enable/disable upgrade button based on upgrade availability
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeButton);
            upgradeButton.interactable = (currentTower.upgradeLevel + 1 < currentTower.upgradePrefabs.Length);
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
}