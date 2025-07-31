using TMPro;
using UnityEngine;

namespace Gmtk2025
{
    public class Projectile : MonoBehaviour
    {
        public bool IsOnLoop => _currentLoop != null;

        private float _angularSpeed;
        private float _loopAngle;
        
        private PlacedLoop _currentLoop;
        private Rigidbody2D _rb;
        private CircleCollider2D _collider;
        
        private const float TAU = Mathf.PI * 2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        private void SwapToOnLoop(PlacedLoop loop)
        {
            _loopAngle = loop.PositionToLoopSpace(transform.position);
            Vector3 loopTangent = loop.GetClockwiseTangentVector(_loopAngle);

            Vector3 speedOntoLoop = Vector3.Project(_rb.linearVelocity, loopTangent);
            float sign = Mathf.Sign(Vector3.Dot(_rb.linearVelocity, loopTangent));

            _angularSpeed = sign * speedOntoLoop.magnitude / (loop.Radius * TAU);

            Vector3 newPos = loop.LoopSpaceToPosition(_loopAngle);
            newPos.z = transform.position.z;
            transform.position = newPos;

            _rb.simulated = false;
            _currentLoop = loop;
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

        private void Update()
        {
            if (IsOnLoop == false)
                return;

            UpdateSpeed();
            Move();
        }

        private void Move()
        {
            _loopAngle += _angularSpeed * Time.deltaTime;
            
            Vector3 newPos = _currentLoop.LoopSpaceToPosition(_loopAngle);
            newPos.z = transform.position.z;
            transform.position = newPos;
        }

        private void UpdateSpeed()
        {
            Vector3 loopTangent = _currentLoop.GetClockwiseTangentVector(_loopAngle);
            Vector3 gravityAlongLoop = Vector3.Project(Physics.gravity, loopTangent);
            float sign = Mathf.Sign(Vector3.Dot(loopTangent, Physics.gravity));

            float angularAcceleration = sign * gravityAlongLoop.magnitude / (_currentLoop.Radius * TAU);

            _angularSpeed += angularAcceleration * Time.deltaTime;
        }
    }
}
