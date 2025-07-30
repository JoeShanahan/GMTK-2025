using UnityEngine;

namespace Gmtk2025
{
    public class PlacedLoop : MonoBehaviour
    {
        [SerializeField] private LineRenderer _line;

        [SerializeField, Range(0, 20)] private float _radius = 1;

        private const int POINT_COUNT = 32;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Sync();
        }

        // Update is called once per frame
        void Update()
        {

        }

        [ContextMenu("Sync!")]
        public void Sync()
        {
            Vector3 origin = transform.position;
            Vector3[] positions = new Vector3[POINT_COUNT + 1];

            for (int i = 0; i <= POINT_COUNT; i++)
            {
                float angle = ((Mathf.PI * 2) / POINT_COUNT) * i;

                float posX = origin.x + (Mathf.Cos(angle) * _radius);
                float posY = origin.y + (Mathf.Sin(angle) * _radius);

                positions[i] = new Vector3(posX, posY, origin.z);
            }

            _line.positionCount = POINT_COUNT + 1;
            _line.SetPositions(positions);
        }
    }
}