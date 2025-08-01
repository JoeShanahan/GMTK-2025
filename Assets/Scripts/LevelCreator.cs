using UnityEngine;
using UnityEngine.InputSystem;

namespace Gmtk2025
{
    public class LevelCreator : MonoBehaviour
    {
        [SerializeField] 
        private PrefabFactory _prefabs;
        
        [SerializeField]
        private Placeable _currentGhost;
        
        [SerializeField] 
        private InputActionReference _mousePositionAction;

        private Camera _mainCamera;
        
        private void OnEnable() 
        {
            _mousePositionAction.action.Enable();
        }
        
        private void OnDisable() 
        {
            _mousePositionAction.action.Disable();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (_currentGhost != null)
            {
                Vector2 mpos = _mousePositionAction.action.ReadValue<Vector2>();
                Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mpos.x, mpos.y, _mainCamera.WorldToScreenPoint(transform.position).z));

                Vector3 clampedPosition = ClampToCameraBounds(worldPosition);
                _currentGhost.MoveTo(clampedPosition);
            }
        }
        
        private Vector3 ClampToCameraBounds(Vector3 position) 
        {
            Vector3 min = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.WorldToScreenPoint(transform.position).z));
            Vector3 max = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.WorldToScreenPoint(transform.position).z));

            float clampedX = Mathf.Clamp(position.x, min.x, max.x);
            float clampedY = Mathf.Clamp(position.y, min.y, max.y);
            return new Vector3(clampedX, clampedY, position.z);
        }
        
        public void StartPlacingLoop(float radius)
        {
            if (_currentGhost != null)
                Destroy(_currentGhost.gameObject);
            
            GameObject newObj = Instantiate(_prefabs.GetLoop());
            PlacedLoop newLoop = newObj.GetComponent<PlacedLoop>();
            newLoop.SetAsGhost(radius);
            _currentGhost = newLoop;
        }

        public void StartPlacingConnector(ConnectorType type, int value)
        {
            if (_currentGhost != null)
                Destroy(_currentGhost.gameObject);
            
            GameObject newObj = Instantiate(_prefabs.GetConnector(type));
            Connector newConn = newObj.GetComponent<Connector>();
            newConn.SetAsGhost(value);
            _currentGhost = newConn;
        }
    }
}
