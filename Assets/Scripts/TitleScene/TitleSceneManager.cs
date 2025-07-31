using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour {
    [SerializeField] private string mainGameSceneName = "MainGame";
    [SerializeField] private InputActionAsset inputActions;

    private InputAction startAction;

    private void OnEnable() {
        var actionMap = inputActions.FindActionMap("UI");
        startAction = actionMap.FindAction("Submit");
        startAction.Enable();
        startAction.performed += OnStartPressed;
    }

    private void OnDisable() {
        startAction.performed -= OnStartPressed;
        startAction.Disable();
    }

    private void OnStartPressed(InputAction.CallbackContext context) {
        SceneManager.LoadScene(mainGameSceneName);
    }
}