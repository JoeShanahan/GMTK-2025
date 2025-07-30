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

        [SerializeField] private List<Connector> _connectors;

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

        // 0 is the absolute top, 0.25 is the absolute right edge, 0.5 is the absolute bottom
        public float PositionToLoopSpace(Vector3 position)
        {
            Vector2 offset = position - transform.position;
            float angle = Mathf.Atan2(offset.y, offset.x);
            return -angle / TAU;
        }

        public Vector3 LoopSpaceToPosition(float loopSpace)
        {
            float angle = -loopSpace * TAU;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;
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