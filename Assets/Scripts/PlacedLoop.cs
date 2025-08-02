using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gmtk2025
{ 
    public class PlacedLoop : Placeable
    {
        [Serializable]
        public class ConnectorInfo
        {
            public Connector Connector;
            
            [Tooltip("Value between 0 and 1, position in loop-space")]
            public float Offset;
        }

        public float Radius => _radius;

        public IEnumerable<ConnectorInfo> Connectors => _connectors;
        
        [SerializeField] private LineRenderer _line;

        [SerializeField, Range(0, 20)] private float _radius = 1;

        [SerializeField] private List<ConnectorInfo> _connectors;

        [SerializeField] private Gradient _ghostInvalidColor;
        [SerializeField] private Gradient _ghostValidColor;
        [SerializeField] private Gradient _normalColor;
        
        private CircleCollider2D _collider;

        private List<SnapPoint> _availableSnapPoints = new();
        
        private const int POINT_COUNT = 48;

        public class SnapPoint
        {
            public Vector2 WorldPos;
            public Connector Connector;
        }

        public void DetectConnections()
        {
            var level = FindFirstObjectByType<LevelController>();

            _connectors = new List<ConnectorInfo>();


            foreach (Connector conn in level.AllConnectors)
            {
                float dist = Vector2.Distance(conn.transform.position, transform.position);
                float distFromRadius = Mathf.Abs(Radius - dist);
                
                if (distFromRadius < 0.0001f)
                {
                    float offset = PositionToLoopSpace(conn.transform.position);
                    
                    _connectors.Add(new ConnectorInfo()
                    {
                        Connector = conn,
                        Offset = offset
                    });
                }
            }
        }
        
        public override void MoveTo(Vector3 worldPos)
        {
            SnapPoint closestSnap = null;
            float minDist = 1.25f;

            foreach (SnapPoint snap in _availableSnapPoints)
            {
                float dist = Vector2.Distance(snap.WorldPos, worldPos);

                if (dist < minDist)
                {
                    closestSnap = snap;
                    minDist = dist;
                }
            }

            if (closestSnap != null)
            {
                // TODO check if in range of the level
                transform.position = closestSnap.WorldPos;
                _line.colorGradient = _ghostValidColor;
                CanPlace = true;
            }
            else if (_createMode == LevelCreator.CreateMode.Creating)
            {
                worldPos.x = Mathf.RoundToInt(worldPos.x * 2) / 2f;
                worldPos.y = Mathf.RoundToInt(worldPos.y * 2) / 2f;
                transform.position = worldPos;
                
                _line.colorGradient = _ghostValidColor;
                CanPlace = true;
            }
            else
            {
                transform.position = worldPos;
                _line.colorGradient = _ghostInvalidColor;
                CanPlace = false;
            }
        }
        
        private void DetectAllSnapPoints()
        {
            _availableSnapPoints.Clear();
            var level = FindFirstObjectByType<LevelController>();

            foreach (Connector conn in level.AllConnectors)
            {
                if (conn.LoopB != null)
                    continue;

                // Something has gone wrong if this is the case
                if (conn.LoopA == null)
                    continue;

                Vector3 dirToConnector = conn.transform.position - conn.LoopA.transform.position;
                dirToConnector.z = 0;
                dirToConnector.Normalize();
                
                _availableSnapPoints.Add(new SnapPoint()
                {
                    WorldPos = conn.transform.position + (dirToConnector * _radius),
                    Connector =  conn
                });
            }
        }
        
        public override void SetAsGhost(float value)
        {
            _line.colorGradient = _ghostInvalidColor;
            _radius = value;
            SyncVisuals();
            DetectAllSnapPoints();
        }
        
        public override void StopBeingAGhost()
        {
            _line.colorGradient = _normalColor;
        }

        public void Init(float radius)
        {
            _radius = radius;
            SyncVisuals();
        }
        
        // Returns a tangent, but is only accurate for clockwise (positive) movement
        public Vector2 GetTangent(Vector2 worldPosition)
        {
            Vector2 offset = worldPosition - new Vector2(transform.position.x, transform.position.y);
            float angleRad = Mathf.Atan2(offset.y, offset.x);
            
            float x = -Mathf.Sin(angleRad);
            float y =  Mathf.Cos(angleRad);
            return new Vector2(x, y).normalized;
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _collider = GetComponent<CircleCollider2D>();
            SyncVisuals();
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

        public void SyncVisuals()
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