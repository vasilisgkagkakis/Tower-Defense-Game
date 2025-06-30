using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // Add this line to use the Dropdown class

public class TowerBehaviour : MonoBehaviour
{
    public TowerData towerData;
    public TowerTargeting.TargetType targetType = TowerTargeting.TargetType.First;
    public Dropdown targetingDropdown; // Reference to the Dropdown UI element

    // Upgrade system
    public GameObject[] upgradePrefabs; // Assign your prefabs in order (level 1, 2, 3)
    public int upgradeLevel = 0;
    public float[] upgradeDamages; // e.g. [10, 20, 30]
    private Color previousColor = Color.white;

    public float currentDamage; // Instance-specific damage
    [SerializeField] private Color resetColor;

    void Start()
    {
        if (currentDamage <= 0)
        {
            currentDamage = towerData.damage;
        }
    }

    public void UpgradeTower()
    {
        if (upgradeLevel + 1 < upgradePrefabs.Length)
        {
            upgradeLevel++; // Increment first!

            // Instantiate the next prefab at the same position/rotation
            GameObject newTower = Instantiate(upgradePrefabs[upgradeLevel], transform.position, transform.rotation);
            TowerBehaviour newBehaviour = newTower.GetComponent<TowerBehaviour>();
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
            Debug.Log("Rend: " + rend.name + "Inside setHightlight - highlight: " + highlight);
            block.SetColor("_Color", highlight);
            rend.SetPropertyBlock(block);
        }
    }

    public void ClearHighlight()
    {
        Debug.Log("Inside ClearHighlight - resetColor: " + resetColor);
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
