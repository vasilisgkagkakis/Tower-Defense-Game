using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // Add this line to use the Dropdown class

public class TowerBehaviour : MonoBehaviour
{
    public TowerData towerData;
    public TowerTargeting.TargetType targetType = TowerTargeting.TargetType.First;
    public Dropdown targetingDropdown;

    // Upgrade system
    public GameObject[] upgradePrefabs;
    public int upgradeLevel = 0;
    public float[] upgradeDamages;
    public float currentDamage;
    [SerializeField] private Color resetColor;

    public bool isPlaced = false; // Set this to true when the tower is placed

    void Start()
    {
        if (currentDamage <= 0)
        {
            currentDamage = towerData.damage;
        }
    }

    public void UpgradeTower()
    {
        if (!isPlaced)
        {
            Debug.Log("Cannot upgrade: Tower is not placed.");
            return;
        }

        if (upgradeLevel + 1 < upgradePrefabs.Length)
        {
            upgradeLevel++; // Increment first!

            // Instantiate the next prefab at the same position/rotation
            GameObject newTower = Instantiate(upgradePrefabs[upgradeLevel], transform.position, transform.rotation);
            TowerBehaviour newBehaviour = newTower.GetComponent<TowerBehaviour>();
            newBehaviour.isPlaced = true; // Mark the new tower as placed
            newBehaviour.SetHighlight();

            // Copy over targeting mode, etc.
            newBehaviour.targetType = this.targetType;

            // Set new damage if you want to override TowerData
            if (upgradeDamages.Length > upgradeLevel)
                newBehaviour.currentDamage = upgradeDamages[upgradeLevel];

            // Update the UI to show the new tower's info
            if (TowerDescriptionUI.Instance != null)
                TowerDescriptionUI.Instance.Show(newBehaviour);

            // Update the selected tower reference
            TowerSelector.selectedTower = newBehaviour; // <-- Add this line
            TowerSelector.lastHighlightedTower = newBehaviour; // Add this line

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
