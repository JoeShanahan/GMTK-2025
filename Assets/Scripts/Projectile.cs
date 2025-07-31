using UnityEngine;

namespace Gmtk2025
{
    public class Projectile : MonoBehaviour
    {
        public bool IsOnLoop => _currentLoop != null;

        // Positive = clockwise
        [SerializeField]
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
            // TODO use the dot product of the current velocity and the tangent to calculate speed
            // TODO negative speed if going counter-clockwise
            _speed = 8;
            
            _rb.simulated = false;
            _currentLoop = loop;

            float loopSpace = loop.PositionToLoopSpace(transform.position);
            Vector3 newPos = loop.LoopSpaceToPosition(loopSpace);
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
        
        public void SwapBetweenLoops(PlacedLoop fromLoop, PlacedLoop toLoop)
        {
            _speed *= -1;
            
            _currentLoop = toLoop;

            float loopSpace = toLoop.PositionToLoopSpace(transform.position);
            Vector3 newPos = toLoop.LoopSpaceToPosition(loopSpace);
            newPos.z = transform.position.z;
            transform.position = newPos;
        }

        public void LeaveLoop()
        {
            _rb.simulated = true;
            _rb.linearVelocity = Vector3.up * 10; // TODO actually calculate
            
            _currentLoop = null;
        }

        private void SwapToFree()
        {
            _rb.simulated = true;
            _currentLoop = null;
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

        private void Update()
        {
            if (IsOnLoop == false)
                return;
            
            // TODO increase speed by using Physics2D.gravity, direction of travel, and Time.deltaTime
            
            Move(1.0f);

            
        }

        private void Move(float moveRemaining, int recursionDepth = 0, Connector toSkip = null)
        {
            if (recursionDepth > 99)
            {
                Debug.LogError("Hit recursion of 99 oops!");
                return;
            }
            
            float speedPerFrame = _speed * Time.deltaTime * moveRemaining;
            float currentLoopSpace = _currentLoop.PositionToLoopSpace(transform.position);
            
            float newLoopSpace = ((currentLoopSpace + (speedPerFrame / _currentLoop.Circumference)) + 100) % 1;
            Vector3 newPos = _currentLoop.LoopSpaceToPosition(newLoopSpace);
            newPos.z = transform.position.z;
            transform.position = newPos;

            bool didPassConnector = _currentLoop.WillPassConnector(currentLoopSpace, speedPerFrame, out float remainingDistance, out Connector connector);

            if (didPassConnector && connector != toSkip) // we don't want to do the same connector twice in a row
            {
                transform.position = connector.transform.position;
                float remainingPercent = remainingDistance / speedPerFrame;
                connector.OnProjectilePassed(this, _currentLoop);
                
                Move(remainingPercent, recursionDepth + 1, connector);
            }
        }
    }
}
