using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public TowerData towerData;
    public TowerTargeting.TargetType targetType = TowerTargeting.TargetType.First;

    // Upgrade system
    public GameObject[] upgradePrefabs;
    public int upgradeLevel = 0;
    public int actualUpgradeLevel = 0; // The real upgrade level in the progression (0=base, 1=first upgrade, etc)
    public float[] upgradeDamages;
    public float currentDamage;
    [SerializeField] private Color resetColor;

    [Header("Economy")]
    public int totalInvestment = 0; // Track total money invested

    public bool isPlaced = false; // Set this to true when the tower is placed

    void Start()
    {
        if (currentDamage <= 0)
        {
            currentDamage = towerData.damage;
        }

        // Initialize total investment with base cost if this is the first level
        if (upgradeLevel == 0 && totalInvestment == 0)
        {
            totalInvestment = towerData.baseCost;
        }
    }

    public void UpgradeTower()
    {
        if (!isPlaced)
        {
            Debug.Log("Cannot upgrade: Tower is not placed.");
            return;
        }

        if (upgradeLevel < upgradePrefabs.Length)
        {
            // Check if player has enough currency for upgrade
            int upgradeCost = towerData.upgradeCosts.Length > actualUpgradeLevel ? towerData.upgradeCosts[actualUpgradeLevel] : 100;

            if (CurrencyManager.Instance == null || !CurrencyManager.Instance.SpendCurrency(upgradeCost))
            {
                Debug.Log("Not enough currency to upgrade!");
                return;
            }

            upgradeLevel++; // Increment first!

            // Instantiate the next prefab at the same position/rotation
            GameObject newTower = Instantiate(upgradePrefabs[upgradeLevel], transform.position, transform.rotation);
            TowerBehaviour newBehaviour = newTower.GetComponent<TowerBehaviour>();
            newBehaviour.isPlaced = true; // Mark the new tower as placed
            newBehaviour.SetHighlight();

            // Copy over targeting mode and investment (but reset upgrade level for new prefab)
            newBehaviour.targetType = this.targetType;
            newBehaviour.upgradeLevel = 0; // Reset to 0 because each prefab starts fresh
            newBehaviour.actualUpgradeLevel = this.actualUpgradeLevel + 1; // Track the real progression level
            newBehaviour.totalInvestment = this.totalInvestment + upgradeCost;

            // Set new damage if you want to override TowerData
            if (upgradeDamages.Length > upgradeLevel)
                newBehaviour.currentDamage = upgradeDamages[upgradeLevel];

            // Update the UI to show the new tower's info
            if (TowerDescriptionUI.Instance != null)
                TowerDescriptionUI.Instance.Show(newBehaviour);

            // Update the selected tower reference
            TowerSelector.selectedTower = newBehaviour;
            TowerSelector.lastHighlightedTower = newBehaviour;

            // Destroy the old tower
            Destroy(gameObject);
        }
    }

    public void SetHighlight()
    {
        Color highlight = new Color(1f, 1f, 0.5f, 0.3f);
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            var block = new MaterialPropertyBlock();
            rend.GetPropertyBlock(block);
            block.SetColor("_Color", highlight);
            rend.SetPropertyBlock(block);
        }
    }

    public void ClearHighlight()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            var block = new MaterialPropertyBlock();
            rend.GetPropertyBlock(block);
            block.SetColor("_Color", resetColor);
            rend.SetPropertyBlock(block);
        }
    }
}
