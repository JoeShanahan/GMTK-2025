using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gmtk2025
{
    public class PlacedLoop : MonoBehaviour
    {
        [Serializable]
        public class ConnectorInfo
        {
            public Connector Connector;
            
            [Tooltip("Value between 0 and 1, position in loop-space")]
            public float Offset;

            public PlacedLoop OtherLoop;
        }
        
        [SerializeField] private LineRenderer _line;

        [SerializeField, Range(0, 20)] private float _radius = 1;

        [SerializeField] private List<ConnectorInfo> _connectors;

        private CircleCollider2D _collider;
        
        private const int POINT_COUNT = 32;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _collider = GetComponent<CircleCollider2D>();
            Sync();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private const float TAU = Mathf.PI * 2;

        public float Circumference => _radius * TAU;

        // 0 is the right most point, 0.25 is the bottom, 0.5 is left most point
        public float PositionToLoopSpace(Vector3 position)
        {
            Vector2 offset = position - transform.position;
            float angle = Mathf.Atan2(offset.y, offset.x);
            return ((-angle / TAU) + 1) % 1;
        }

        public Vector3 LoopSpaceToPosition(float loopSpace)
        {
            float angle = -loopSpace * TAU;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;
            return transform.position + offset;
        }

        // Checks to see if there is a connector between [loopSpaceStart] and [loopSpaceEnd]
        // If so, return the remaining distance after it (in loop space)
        public bool WillPassConnector(float loopSpaceStart, float absoluteDistance, out float remainder, out Connector connector)
        {
            remainder = 0;
            connector = null;
            
            if (_connectors.Count == 0)
                return false;

            float loopSpaceEnd = loopSpaceStart + (absoluteDistance / Circumference);
            float min = Mathf.Min(loopSpaceStart, loopSpaceEnd);
            float max = Mathf.Max(loopSpaceStart, loopSpaceEnd);

            foreach (ConnectorInfo info in _connectors)
            {
                // Need to check all three because:
                //   0 should be between 0.9 and 1.1
                //   0.99 should be between 0.1 and -0.1
                bool a = info.Offset >= min && info.Offset <= max;
                bool b = info.Offset + 1 >= min && info.Offset + 1 <= max;
                bool c = info.Offset - 1 >= min && info.Offset - 1 <= max;

                for (int i = -1; i <= 1; i++)
                {
                    float offset = info.Offset + i;

                    if (offset > min && offset < max)
                    {
                        connector = info.Connector;
                        float distanceToConnector = Mathf.Abs(offset - loopSpaceStart) * Circumference;
                        remainder = absoluteDistance - distanceToConnector;
                        return true;
                    }
                    
                }
            }
            
            return false;
        }

        [ContextMenu("Sync!")]
        public void Sync()
        {
            Vector3[] positions = new Vector3[POINT_COUNT + 1];

            for (int i = 0; i <= POINT_COUNT; i++)
            {
                float angle = (TAU / POINT_COUNT) * i;

                float posX = Mathf.Cos(angle) * _radius;
                float posY = Mathf.Sin(angle) * _radius;

                positions[i] = new Vector3(posX, posY, 0);
            }

            _line.positionCount = POINT_COUNT + 1;
            _line.SetPositions(positions);

            GetComponent<CircleCollider2D>().radius = _radius;
        }
    }
}