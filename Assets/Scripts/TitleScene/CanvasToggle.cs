using UnityEngine;

public class CanvasToggle : MonoBehaviour {
    [SerializeField] private GameObject canvasObject;

    public void ToggleCanvas() {
        canvasObject.SetActive(!canvasObject.activeSelf);
    }
}