using UnityEngine;

namespace Gmtk2025
{
    public class Projectile : Placeable
    {
        public bool IsOnLoop => _currentLoop != null;

        // Positive = clockwise
        [SerializeField]
        private float _speed;
        
        [SerializeField] private SpriteRenderer _ghostSprite;
        [SerializeField] private Transform _realVisuals;
        
        [SerializeField] private Color _invalidColor = new Color(1,0, 0, 0.7f);
        [SerializeField] private Color _validColor = new Color(0,1, 0, 0.9f);

        
        private PlacedLoop _currentLoop;
        private Rigidbody2D _rb;
        private CircleCollider2D _collider;

        private bool _isGhost;
        
        public Vector3 Velocity
        {
            get
            {
                if (IsOnLoop)
                    return -Mathf.Sign(_speed) * _currentLoop.GetTangent(transform.position);
                else
                    return _rb.linearVelocity; 
            }
        }

        public override void SetAsGhost(float value)
        {
            _ghostSprite.gameObject.SetActive(true);
            _realVisuals.gameObject.SetActive(false);
            _rb ??= GetComponent<Rigidbody2D>();
            _rb.simulated = false;
            _isGhost = true;
        }
        
        public override void StopBeingAGhost()
        {
            _ghostSprite.gameObject.SetActive(false);
            _realVisuals.gameObject.SetActive(true);
            _rb.simulated = false;
            _isGhost = false;
        }

        public override void MoveTo(Vector3 worldPos)
        {
            worldPos.x = Mathf.RoundToInt(worldPos.x * 4) / 4f;
            worldPos.y = Mathf.RoundToInt(worldPos.y * 4) / 4f;
            
            if (true)
            {
                _ghostSprite.color = _validColor;
                CanPlace = true;
            }
            else
            {
                transform.position = worldPos;
                _ghostSprite.color = _invalidColor;
                CanPlace = false;
            }
            
            transform.position = worldPos;
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        public void Freeze()
        {
            _rb ??= GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        public void Unfreeze()
        {
            _rb ??= GetComponent<Rigidbody2D>();
            _rb.simulated = true;
        }

        private void SwapToOnLoop(PlacedLoop loop)
        {
            Vector3 previousVelocity = _rb.linearVelocity;
            
            _rb.simulated = false;
            _currentLoop = loop;

            float loopSpace = loop.PositionToLoopSpace(transform.position);
            Vector3 newPos = loop.LoopSpaceToPosition(loopSpace);
            newPos.z = transform.position.z;
            transform.position = newPos;

            Vector3 loopTangent = _currentLoop.GetTangent(newPos);
            Vector3 previousDirection = previousVelocity.magnitude > 0 ? previousVelocity.normalized : Vector3.down;

            float alignment = Vector3.Dot(loopTangent, previousDirection);
            _speed = -alignment * previousVelocity.magnitude;
        }

        public void WarpTo(Vector3 position, PlacedLoop otherLoop)
        {
            Vector3 previousVelocity = GetVelocity();
            
            _currentLoop = otherLoop;
            transform.position = position;

            Vector3 newTangent = _currentLoop.GetTangent(position);
            Vector3 previousDirection = previousVelocity.magnitude > 0 ? previousVelocity.normalized : Vector3.down;
            float alignment = Vector3.Dot(newTangent, previousDirection);
            _speed = -alignment * previousVelocity.magnitude;
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
            _rb.linearVelocity = GetVelocity();
            
            _currentLoop = null;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isGhost)
                return;
            
            if (IsOnLoop)
                return;
            
            if (other is CircleCollider2D otherCircle)
            {
                // We need the center of our object to pass the edge of this collider
                float distance = Vector2.Distance(transform.position, other.transform.position);

                if (distance <= otherCircle.radius + 0.01f)
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
            if (_isGhost)
                return;
            
            if (IsOnLoop == false)
                return;

            Move(1.0f);
        }

        private void FixedUpdate()
        {
            if (IsOnLoop == false)
                return;

            _speed = Mathf.Clamp(_speed, -50, 50);
            
            Vector3 currentNormal = _currentLoop.GetTangent(transform.position);
            
            if (_speed > 0)
            {
                currentNormal = -currentNormal;
            }

            float sign = _speed > 0 ? 1 : - 1;

            float percentAlignedDown = Vector2.Dot(currentNormal, Vector2.down);
            float speedChange = -Physics2D.gravity.y * Time.deltaTime * percentAlignedDown * sign;
            _speed += speedChange;

        }

        private Vector3 GetVelocity()
        {
            Vector3 currentNormal = _currentLoop.GetTangent(transform.position);
            
            if (_speed > 0)
            {
                currentNormal = -currentNormal;
            }
            
            float sign = _speed > 0 ? 1 : - 1;

            return currentNormal * _speed * sign;
        }

        private void Move(float moveRemaining, int recursionDepth = 0, Connector toSkip = null)
        {
            if (recursionDepth > 99)
            {
                Debug.LogError("Hit recursion of 99 oops!");
                return;
            }

            if (IsOnLoop == false)
                return;
            
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
