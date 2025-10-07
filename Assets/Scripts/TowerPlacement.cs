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

    void Update()
    {
        HandleTowerHotkeys();
        UpdatePlacementPreview();
    }

    private void HandleTowerHotkeys()
    {
        Button btn = null;
        if (Keyboard.current.digit1Key.wasPressedThisFrame) btn = button1;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) btn = button2;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) btn = button3;

        if (btn == null) return;

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

        button1.interactable = enable;
        button2.interactable = enable;
        button3.interactable = enable;

        button1.GetComponent<Image>().raycastTarget = enable;
        button2.GetComponent<Image>().raycastTarget = enable;
        button3.GetComponent<Image>().raycastTarget = enable;
    }

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
}