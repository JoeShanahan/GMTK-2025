using UnityEngine;

public class FlyRight : MonoBehaviour {
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;

    private void Start() {
        rb.linearVelocity = new Vector2(initialSpeed, 0f);
    }
}