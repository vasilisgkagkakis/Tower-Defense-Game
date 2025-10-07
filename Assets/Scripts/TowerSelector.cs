using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerSelector : MonoBehaviour
{
    public static TowerBehaviour selectedTower;
    public static TowerBehaviour lastHighlightedTower;
    private static int justPlacedFrame = -1;

    private bool isPlacingTower = false;
    private TowerPlacement towerPlacement;

    void Start()
    {
        towerPlacement = GetComponent<TowerPlacement>();
        isPlacingTower = towerPlacement.IsPlacingTower;
        towerPlacement.OnPlacingTowerChanged += OnPlacingTowerChanged;
    }

    private void OnPlacingTowerChanged(bool newValue) => isPlacingTower = newValue;

    public static void NotifyTowerPlaced(TowerBehaviour tower)
    {
        justPlacedFrame = Time.frameCount;
        if (lastHighlightedTower != null && lastHighlightedTower != tower)
            lastHighlightedTower.ClearHighlight();

        selectedTower = tower;
        lastHighlightedTower = tower;
        tower.SetHighlight();

        if (TowerDescriptionUI.Instance != null)
            TowerDescriptionUI.Instance.Show(tower);
    }

    void Update() => HandleKeyboardShortcuts();

    void LateUpdate() => HandleClickSelection();

    private void HandleKeyboardShortcuts()
    {
        if (selectedTower == null) return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            selectedTower.targetType = (TowerTargeting.TargetType)
                (((int)selectedTower.targetType + 1) %
                 System.Enum.GetValues(typeof(TowerTargeting.TargetType)).Length);
            TowerDescriptionUI.Instance.UpdateDropdown(selectedTower);
        }

        if (Keyboard.current.uKey.wasPressedThisFrame)
            selectedTower.UpgradeTower();
    }

    private void HandleClickSelection()
    {
        if (!ShouldProcessClick()) return;

        if (TrySelectTowerUnderCursor()) return;

        ClearSelection();
    }

    private bool ShouldProcessClick()
    {
        if (Time.frameCount == justPlacedFrame) return false;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return false;
        if (isPlacingTower) return false;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return false;
        return true;
    }

    private bool TrySelectTowerUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return false;

        TowerBehaviour tower = hit.collider.GetComponent<TowerBehaviour>();
        if (tower == null) return false;

        SelectTower(tower, hit.collider.isTrigger);
        return true;
    }

    private void SelectTower(TowerBehaviour tower, bool isTrigger)
    {
        if (lastHighlightedTower != null && lastHighlightedTower != tower)
            lastHighlightedTower.ClearHighlight();

        selectedTower = tower;
        selectedTower.SetHighlight();

        if (!isTrigger && TowerDescriptionUI.Instance != null)
            TowerDescriptionUI.Instance.Show(tower);

        lastHighlightedTower = tower;
    }

    private void ClearSelection()
    {
        if (lastHighlightedTower != null)
        {
            lastHighlightedTower.ClearHighlight();
            lastHighlightedTower = null;
        }

        if (TowerDescriptionUI.Instance != null)
            TowerDescriptionUI.Instance.Hide();

        selectedTower = null;
    }
}