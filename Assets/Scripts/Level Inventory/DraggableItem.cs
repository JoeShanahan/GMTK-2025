using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableItem : MonoBehaviour {
    [SerializeField] private InputActionReference mouseClickAction;
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private GameObject[] inventoryAnchors;

    private Camera mainCamera;
    private bool isDragging;
    private Vector3 offset;
    private Collider2D itemCollider;

    private void Start() {
        mainCamera = Camera.main;
        itemCollider = GetComponent<Collider2D>();
    }

    private void OnEnable() {
        mouseClickAction.action.started += BeginDrag;
        mouseClickAction.action.canceled += EndDrag;
        mouseClickAction.action.Enable();
        mousePositionAction.action.Enable();
    }

    private void OnDisable() {
        mouseClickAction.action.started -= BeginDrag;
        mouseClickAction.action.canceled -= EndDrag;
        mouseClickAction.action.Disable();
        mousePositionAction.action.Disable();
    }

    private void Update() {
        if (isDragging) {
            Vector2 mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.WorldToScreenPoint(transform.position).z));
            Vector3 targetPosition = worldPosition + offset;

            Vector3 clampedPosition = ClampToCameraBounds(targetPosition);
            transform.position = clampedPosition;
        }
    }

    private Vector3 ClampToCameraBounds(Vector3 position) {
        Vector3 min = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.WorldToScreenPoint(transform.position).z));
        Vector3 max = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.WorldToScreenPoint(transform.position).z));

        float clampedX = Mathf.Clamp(position.x, min.x, max.x);
        float clampedY = Mathf.Clamp(position.y, min.y, max.y);
        return new Vector3(clampedX, clampedY, position.z);
    }

    private void BeginDrag(InputAction.CallbackContext context) {
        Vector2 mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.WorldToScreenPoint(transform.position).z));
        Vector2 worldPoint2D = new Vector2(worldPosition.x, worldPosition.y);

        RaycastHit2D hit = Physics2D.Raycast(worldPoint2D, Vector2.zero);
        if (hit.collider == null || hit.collider.gameObject != gameObject)
            return;

        transform.SetParent(null);
        offset = transform.position - worldPosition;
        isDragging = true;
    }

    private void EndDrag(InputAction.CallbackContext context) {
        isDragging = false;

        foreach (GameObject anchor in inventoryAnchors) {
            Collider2D anchorCollider = anchor.GetComponent<Collider2D>();
            if (anchorCollider != null && itemCollider.bounds.Intersects(anchorCollider.bounds)) {
                transform.SetParent(anchor.transform);
                transform.position = anchor.transform.position;
                return;
            }
        }
    }
}