using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private LayerMask PlacementCheckMask;
    [SerializeField] private LayerMask PlacementCollideMask;
    [SerializeField] private Camera PlayerCamera;
    private GameObject CurrentPlacingTower;
    private string currentTowerName;

    [Header("Tower Buttons")]
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    [SerializeField] private Color allowedTint = new Color(0.5f, 1f, 0.5f, 0.3f); // light green, low alpha
    [SerializeField] private Color blockedTint = new Color(1f, 0.5f, 0.5f, 0.3f); // light red, low alpha

    void Update()
    {
        HandleTowerButton();

        if (CurrentPlacingTower != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray camray = PlayerCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(camray, out hit, 100f, PlacementCollideMask))
            {
                float yOffset = (currentTowerName == "turret_2") ? 0.12f : 0f;
                CurrentPlacingTower.transform.position = hit.point + new Vector3(0, yOffset, 0);

                BoxCollider TowerCollider = CurrentPlacingTower.GetComponent<BoxCollider>();
                TowerCollider.isTrigger = true;
                Vector3 BoxCenter = TowerCollider.bounds.center;
                Vector3 HalfExtents = TowerCollider.bounds.extents;

                bool canPlace = !Physics.CheckBox(BoxCenter, HalfExtents, Quaternion.identity, PlacementCheckMask, QueryTriggerInteraction.Ignore);
                SetPreviewTint(canPlace ? allowedTint : blockedTint);

                // --- Only handle placement on click ---
                if (Mouse.current.leftButton.wasPressedThisFrame && hit.collider.gameObject != null)
                {
                    if (!hit.collider.gameObject.CompareTag("PlaceNotAllowed"))
                    {
                        if (canPlace)
                        {   TowerBehaviour towerBehaviour = CurrentPlacingTower.GetComponent<TowerBehaviour>();
                            towerBehaviour.isPlaced = true; // <-- Mark as placed
                            TowerCollider.isTrigger = false;
                            SetPreviewTint(Color.white);
                            TowerDescriptionUI.Instance.Show(towerBehaviour);

                            if (CurrentPlacingTower != null)
                            {
                                CurrentPlacingTower.gameObject.GetComponentInParent<TowerBehaviour>().SetHighlight();
                            }

                            CurrentPlacingTower = null;
                        }
                        else
                        {
                            Collider[] hits = Physics.OverlapBox(BoxCenter, HalfExtents, Quaternion.identity, PlacementCheckMask, QueryTriggerInteraction.Ignore);
                            foreach (var col in hits)
                            {
                                //Debug.Log("Blocked by: " + col.gameObject.name + " on layer " + LayerMask.LayerToName(col.gameObject.layer));
                            }
                        }
                    }
                    else if (Mouse.current.rightButton.wasPressedThisFrame)
                    {
                        Destroy(CurrentPlacingTower);
                        CurrentPlacingTower = null;
                    }
                }
            }
        }
    }

    private void HandleTowerButton()
    {
        Button button = null;
        if (Keyboard.current.digit1Key.wasPressedThisFrame) button = button1;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) button = button2;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) button = button3;

        if (button != null)
        {
            if (CurrentPlacingTower != null)
            {
                Destroy(CurrentPlacingTower);
                CurrentPlacingTower = null;
            }
            button.onClick.Invoke();
        }
    }

    public void SetTowerToPlace(GameObject towerPrefab)
    {
        float yOffset = (towerPrefab.name == "turret_2") ? 0.12f : 0f;
        Vector3 spawnPos = new Vector3(0, yOffset, 0);
        CurrentPlacingTower = Instantiate(towerPrefab, spawnPos, towerPrefab.transform.rotation);
        currentTowerName = towerPrefab.name;
    }

    private void SetPreviewTint(Color tint)
    {
        var renderers = CurrentPlacingTower.GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            var block = new MaterialPropertyBlock();
            rend.GetPropertyBlock(block);
            block.SetColor("_Color", tint);

            rend.SetPropertyBlock(block);
        }
    }
}
