using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private LayerMask PlacementCheckMask;
    [SerializeField] private LayerMask PlacementCollideMask;
    [SerializeField] private Camera PlayerCamera;

    private GameObject CurrentPlacingTower;
    private TowerBehaviour LatestClickedTower;
    private string currentTowerName;
    private bool isPlacingTower = false;

    [Header("Tower Buttons")]
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    [Header("Tower Costs")]
    [SerializeField] private int tower1Cost = 100;
    [SerializeField] private int tower2Cost = 400;
    [SerializeField] private int tower3Cost = 600;

    [SerializeField] private Color allowedTint = new Color(0.5f, 1f, 0.5f, 0.3f);
    [SerializeField] private Color blockedTint = new Color(1f, 0.5f, 0.5f, 0.3f);

    public event System.Action<bool> OnPlacingTowerChanged;

    public bool IsPlacingTower
    {
        get => isPlacingTower;
        set
        {
            if (isPlacingTower == value) return;
            isPlacingTower = value;
            OnPlacingTowerChanged?.Invoke(isPlacingTower);
        }
    }

    void Start()
    {
        // Initialize button states based on starting currency
        UpdateButtonAffordability();
    }

    void Update()
    {
        HandleTowerHotkeys();
        UpdatePlacementPreview();
        UpdateButtonAffordability();
    }

    private void HandleTowerHotkeys()
    {
        Button btn = null;
        int cost = 0;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) { btn = button1; cost = tower1Cost; }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) { btn = button2; cost = tower2Cost; }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) { btn = button3; cost = tower3Cost; }

        if (btn == null) return;

        // Check if player can afford the tower
        if (CurrencyManager.Instance == null || !CanAffordTower(cost))
        {
            Debug.Log($"Not enough currency! Need {cost}, have {CurrencyManager.Instance?.GetCurrency() ?? 0}");
            return;
        }

        SetTowerButtonsEnabled(false);
        DestroyCurrentPreviewIfAny();
        btn.onClick.Invoke();
    }

    private void UpdatePlacementPreview()
    {
        if (CurrentPlacingTower == null) return;
        if (CancelRequested()) return;
        if (!TryGetPlacementHit(out var hit)) return;

        PositionPreview(hit);
        var towerCollider = PreparePreviewCollider();
        bool canPlace = EvaluateCanPlace(towerCollider);
        ApplyPreviewTint(canPlace);
        TryFinalizePlacement(hit, towerCollider, canPlace);
    }

    private bool CancelRequested()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return false;
        SetTowerButtonsEnabled(true);
        DestroyCurrentPreviewIfAny();
        return true;
    }

    private bool TryGetPlacementHit(out RaycastHit hit)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        return Physics.Raycast(ray, out hit, 100f, PlacementCollideMask);
    }

    private void PositionPreview(RaycastHit hit)
    {
        float yOffset = currentTowerName.StartsWith("turret_2") ? 0.12f : 0f;
        CurrentPlacingTower.transform.position = hit.point + new Vector3(0f, yOffset, 0f);
    }

    private BoxCollider PreparePreviewCollider()
    {
        var col = CurrentPlacingTower.GetComponent<BoxCollider>();
        col.isTrigger = true;
        return col;
    }

    private bool EvaluateCanPlace(BoxCollider col)
    {
        Vector3 center = col.bounds.center;
        Vector3 halfExtents = col.bounds.extents;
        return !Physics.CheckBox(center, halfExtents, Quaternion.identity, PlacementCheckMask, QueryTriggerInteraction.Ignore);
    }

    private void ApplyPreviewTint(bool canPlace) => SetPreviewTint(canPlace ? allowedTint : blockedTint);

    private void TryFinalizePlacement(RaycastHit hit, BoxCollider towerCollider, bool canPlace)
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (hit.collider == null) return;
        if (hit.collider.CompareTag("PlaceNotAllowed")) return;
        if (!canPlace) return;

        var behaviour = CurrentPlacingTower.GetComponent<TowerBehaviour>();

        // Deduct cost when placing tower
        if (CurrencyManager.Instance != null && !CurrencyManager.Instance.SpendCurrency(behaviour.towerData.baseCost))
        {
            Debug.Log("Not enough currency to place tower!");
            return;
        }

        behaviour.isPlaced = true;
        towerCollider.isTrigger = false;

        TowerDescriptionUI.Instance.Show(behaviour);
        SetTowerButtonsEnabled(true);
        LatestClickedTower = behaviour;

        TowerSelector.NotifyTowerPlaced(behaviour);
        CurrentPlacingTower = null;
    }

    private void DestroyCurrentPreviewIfAny()
    {
        if (CurrentPlacingTower == null) return;
        Destroy(CurrentPlacingTower);
        CurrentPlacingTower = null;
    }

    private void SetTowerButtonsEnabled(bool enable)
    {
        if (!enable && LatestClickedTower != null)
        {
            TowerDescriptionUI.Instance.Hide();
            LatestClickedTower.ClearHighlight();
        }

        IsPlacingTower = !enable;

        if (enable)
        {
            // When enabling, check affordability for each button
            button1.interactable = CanAffordTower(tower1Cost);
            button2.interactable = CanAffordTower(tower2Cost);
            button3.interactable = CanAffordTower(tower3Cost);
        }
        else
        {
            // When disabling (placing mode), disable all buttons
            button1.interactable = false;
            button2.interactable = false;
            button3.interactable = false;
        }

        button1.GetComponent<Image>().raycastTarget = enable;
        button2.GetComponent<Image>().raycastTarget = enable;
        button3.GetComponent<Image>().raycastTarget = enable;
    }

    // Used by tower buttons onClick events
    public void SetTowerToPlace(GameObject towerPrefab)
    {
        float yOffset = towerPrefab.name == "turret_2" ? 0.12f : 0f;
        CurrentPlacingTower = Instantiate(
            towerPrefab,
            new Vector3(0f, yOffset, 0f),
            towerPrefab.transform.rotation);
        currentTowerName = towerPrefab.name;
        SetTowerButtonsEnabled(false);
    }

    private void SetPreviewTint(Color tint)
    {
        if (CurrentPlacingTower == null) return;
        var renderers = CurrentPlacingTower.GetComponentsInChildren<Renderer>();
        var block = new MaterialPropertyBlock();
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetColor("_Color", tint);
            r.SetPropertyBlock(block);
        }
    }

    private void UpdateButtonAffordability()
    {
        if (isPlacingTower) return; // Don't update when placing towers

        if (CurrencyManager.Instance != null)
        {
            int currentCurrency = CurrencyManager.Instance.GetCurrency();

            // Update button interactability based on affordability
            if (button1 != null) button1.interactable = CanAffordTower(tower1Cost);
            if (button2 != null) button2.interactable = CanAffordTower(tower2Cost);
            if (button3 != null) button3.interactable = CanAffordTower(tower3Cost);
        }
    }

    private bool CanAffordTower(int cost)
    {
        return CurrencyManager.Instance != null && CurrencyManager.Instance.GetCurrency() >= cost;
    }
}