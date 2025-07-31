using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnCollision : MonoBehaviour {
    [SerializeField] private GameObject triggeringObject;
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == triggeringObject) {
            Debug.Log("Scene transition triggered.");
            // SceneManager.LoadScene(sceneToLoad);
        }
    }
}