using UnityEngine;

namespace Gmtk2025
{
    public class LevelCreator : MonoBehaviour
    {
        [SerializeField] 
        private PrefabFactory _prefabs;
        
        [SerializeField]
        private Placeable _currentGhost;
        
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
