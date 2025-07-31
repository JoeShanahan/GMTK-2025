using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableItem : MonoBehaviour {
    [SerializeField] private InputActionReference mouseClickAction;
    [SerializeField] private InputActionReference mousePositionAction;

    private Camera mainCamera;
    private bool isDragging;
    private Vector3 offset;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void OnEnable() {
        mouseClickAction.action.started += OnMouseDown;
        mouseClickAction.action.canceled += OnMouseUp;
        mouseClickAction.action.Enable();
        mousePositionAction.action.Enable();
    }

    private void OnDisable() {
        mouseClickAction.action.started -= OnMouseDown;
        mouseClickAction.action.canceled -= OnMouseUp;
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

    private void OnMouseDown(InputAction.CallbackContext context) {
        Vector2 mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.WorldToScreenPoint(transform.position).z));
        offset = transform.position - worldPosition;
        isDragging = true;
    }

    private void OnMouseUp(InputAction.CallbackContext context) {
        isDragging = false;
    }
}