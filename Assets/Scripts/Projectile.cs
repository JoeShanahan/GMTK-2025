using UnityEngine;

namespace Gmtk2025
{
    public class Projectile : MonoBehaviour
    {
        public bool IsOnLoop => _currentLoop != null;

        // Positive = clockwise
        private float _speed;
        
        private PlacedLoop _currentLoop;
        private Rigidbody2D _rb;
        private CircleCollider2D _collider;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        private void SwapToOnLoop(PlacedLoop loop)
        {
            // TODO use the dot product of the current velocity and the tangent
            _speed = 5;
            
            _rb.simulated = false;
            _currentLoop = loop;

            float loopSpace = loop.PositionToLoopSpace(transform.position);
            Vector3 newPos = loop.LoopSpaceToPosition(loopSpace);
            newPos.z = transform.position.z;
            transform.position = newPos;
        }

        private void SwapToFree()
        {
            _rb.simulated = true;
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsOnLoop)
                return;
            
            if (other is CircleCollider2D otherCircle)
            {
                // We need the center of our object to pass the edge of this collider
                float distance = Vector2.Distance(transform.position, other.transform.position);

                if (distance <= otherCircle.radius)
                {
                    if (other.TryGetComponent<PlacedLoop>(out var loop))
                    {
                        SwapToOnLoop(loop);
                        return;
                    }
                }
            }
        }

        void Update()
        {
            if (IsOnLoop == false)
                return;

            float currentLoopSpace = _currentLoop.PositionToLoopSpace(transform.position);
            float speedPerFrame = _speed * Time.deltaTime;

            float newLoopSpace = ((currentLoopSpace + (speedPerFrame / _currentLoop.Circumference)) + 100) % 1;
            Vector3 newPos = _currentLoop.LoopSpaceToPosition(newLoopSpace);
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
    }
}
