using UnityEngine;
using UnityEngine.InputSystem;

namespace Gmtk2025
{
    public class LevelCreator : MonoBehaviour
    {
        public static string CurrentFilename;

        public enum CreateMode
        {
            Creating,
            Playing
        };

        [SerializeField] 
        private PrefabFactory _prefabs;
        
        [SerializeField]
        private Placeable _currentGhost;

        [SerializeField] 
        private LevelController _levelController;

        [SerializeField] 
        private InventoryBar _inventoryBar;
        
        [SerializeField] 
        private InputActionReference _mousePositionAction;
        
        [SerializeField] 
        private InputActionReference _mousePressAction;

        [SerializeField] 
        private CreateMode _mode;

        [SerializeField] 
        private Transform _deleteCursor;

        private bool _isDeleting;
        
        private Camera _mainCamera;
        private LevelEditorSaveData _saveData = new();

        public void SetFreeBuild()
        {
            _mode = CreateMode.Creating;
        }

        public void SetPlaying()
        {
            _mode = CreateMode.Playing;
        }
        
        private void OnEnable() 
        {
            _mousePositionAction.action.Enable();
            _mousePressAction.action.Enable();
            _mousePressAction.action.performed += OnMousePress;
        }

        public void SavePressed()
        {
            
        }

        public void LoadPressed()
        {
            _saveData.LoadFromPrefs();
        }
        
        private void OnMousePress(InputAction.CallbackContext context) 
        {
            if (_currentGhost != null && _currentGhost.CanPlace)
            {
                _currentGhost.StopBeingAGhost();
                _levelController.AddPlaceable(_currentGhost);
                _currentGhost = null;
                _inventoryBar.OnSelectButton(null);
            }
        }
        
        private void OnDisable() 
        {
            _mousePositionAction.action.Disable();
            _mousePressAction.action.Disable();
            _mousePressAction.action.performed += OnMousePress;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (_currentGhost != null)
            {
                if (_levelController.IsPlaying)
                {
                    Destroy(_currentGhost.gameObject);
                    _currentGhost = null;
                    _inventoryBar.OnSelectButton(null);
                    return;
                }
                
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

        public void StartDeleting()
        {
            _isDeleting = true;
            _deleteCursor?.gameObject?.SetActive(true);
        }

        public void StopDeleting()
        {
            _isDeleting = false;
            _deleteCursor?.gameObject?.SetActive(true);
        }

        public void StartPlacingProjectile()
        {
            if (_currentGhost != null)
                Destroy(_currentGhost.gameObject);
            
            GameObject newObj = Instantiate(_prefabs.GetProjectile());
            Projectile newProjectile = newObj.GetComponent<Projectile>();
            newProjectile.SetAsGhost(0);
            newProjectile.SetCreateMode(_mode);
            _currentGhost = newProjectile;
        }
        
        public void StartPlacingLoop(float radius)
        {
            if (_currentGhost != null)
                Destroy(_currentGhost.gameObject);
            
            GameObject newObj = Instantiate(_prefabs.GetLoop());
            PlacedLoop newLoop = newObj.GetComponent<PlacedLoop>();

            newLoop.SetAsGhost(radius);
            newLoop.SetCreateMode(_mode);
            _currentGhost = newLoop;
        }

        public void StartPlacingConnector(ConnectorType type, int value)
        {
            if (_currentGhost != null)
                Destroy(_currentGhost.gameObject);
            
            GameObject newObj = Instantiate(_prefabs.GetConnector(type));
            Connector newConn = newObj.GetComponent<Connector>();

            newConn.SetAsGhost(value);
            newConn.SetCreateMode(_mode);
            _currentGhost = newConn;
        }
    }
}
