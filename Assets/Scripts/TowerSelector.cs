using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelector : MonoBehaviour
{
    public static TowerBehaviour selectedTower;
    public static TowerBehaviour lastHighlightedTower;

    void Update()
    {
        // Select tower on click (your existing logic)
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TowerBehaviour tower = hit.collider.GetComponent<TowerBehaviour>();
                if (tower != null)
                {
                    // Remove highlight from previous
                    if (lastHighlightedTower != null)
                    {
                        Debug.Log("Clearing highlight from previous tower: " + lastHighlightedTower.name);
                        lastHighlightedTower.ClearHighlight();
                    }

                    selectedTower = tower;
                    Debug.Log("Selected tower: " + tower.name);
                    selectedTower.SetHighlight();
                    //if not trigger, it's not instatiated yet so don't show UI
                    if (!hit.collider.isTrigger)
                        TowerDescriptionUI.Instance.Show(tower);

                    lastHighlightedTower = tower;
                    return; // Prevent hiding UI if a tower was clicked
                }
            }
            // Debug.Log(lastHighlightedTower.name);
            // Remove highlight if clicking elsewhere
            if (lastHighlightedTower != null)
            {
                Debug.Log("Clicked outside tower, clearing highlight");
                lastHighlightedTower.ClearHighlight();
                lastHighlightedTower = null;
            }
            if (TowerDescriptionUI.Instance != null)
                TowerDescriptionUI.Instance.Hide();

            selectedTower = null;
        }

        // Only the selected tower responds to T and U
        if (selectedTower != null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                selectedTower.targetType = (TowerTargeting.TargetType)(((int)selectedTower.targetType + 1) % System.Enum.GetValues(typeof(TowerTargeting.TargetType)).Length);
                TowerDescriptionUI.Instance.UpdateDropdown(selectedTower);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                selectedTower.UpgradeTower();
            }
        }
    }

    
}
