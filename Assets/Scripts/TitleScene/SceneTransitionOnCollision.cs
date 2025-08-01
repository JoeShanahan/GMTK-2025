using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnCollision : MonoBehaviour {
    [SerializeField] private GameObject triggeringObject;
    [SerializeField] private string sceneToLoad;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!hasTriggered && other.gameObject == triggeringObject) {
            hasTriggered = true;
            Debug.Log("Scene transition triggered.");
            // SceneManager.LoadScene(sceneToLoad);
        }
    }
}