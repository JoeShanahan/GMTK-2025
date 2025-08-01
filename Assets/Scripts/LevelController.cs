using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gmtk2025
{
    [Serializable]
    public class ConnectorItem
    {
        public ConnectorType Type;
        public int Value;
    }
    
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelData _currentLevel;
        [SerializeField] private PrefabFactory _prefabs;
        
        [Space(16)]
        [SerializeField] private List<PlacedLoop> _loops;
        [SerializeField] private List<Connector> _connectors;
        [SerializeField] private List<Projectile> _projectiles;

        [Space(16)] 
        [SerializeField] private List<float> _loopInventory;
        [SerializeField] private List<ConnectorItem> _connectorInventory;

        public IEnumerable<Connector> AllConnectors => _connectors;
        public IEnumerable<PlacedLoop> AllLoops => _loops;
        
        private const float LOOP_DISTANCE = 0;
        private const float CONN_DISTANCE = -0.1f;
        private const float PROJ_DISTANCE = -0.2f;

        private bool _isPlayingSolution;
        private LevelData _tempLevel;
        
        void Start()
        {
            SpawnLevel(_currentLevel);
            
            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }
        }

        public bool IsPlaying => _isPlayingSolution;

        public void AddPlaceable(Placeable p)
        {
            if (p is PlacedLoop loop)
            {
                Connector closestConnector = null;
                float smallestDistance = 1;
                
                foreach (Connector conn in _connectors)
                {
                    if (conn.LoopB != null)
                        continue;

                    // Something has gone wrong if this is the case
                    if (conn.LoopA == null)
                        continue;

                    float distToConnector = Vector2.Distance(conn.transform.position, p.transform.position);
                    float distFromRadius = Mathf.Abs(distToConnector - loop.Radius);

                    if (distFromRadius < smallestDistance)
                    {
                        closestConnector = conn;
                        smallestDistance = distFromRadius;
                    }
                }

                if (closestConnector == null)
                {
                    Debug.LogError("AHHHH SOMETHING WENT WRONG CANT FIND THE CONNECTOR!");
                    Destroy(loop.gameObject);
                    return;
                }
                
                closestConnector.AttachLoop(loop);
                _loops.Add(loop);
            }
            else if (p is Connector conn)
            {
                PlacedLoop closestLoop = null;
                float smallestDistance = 0.1f;
                
                foreach (PlacedLoop pLoop in _loops)
                {
                    float distToConnector = Vector2.Distance(conn.transform.position, pLoop.transform.position);
                    float distFromRadius = Mathf.Abs(distToConnector - pLoop.Radius);

                    if (distFromRadius < smallestDistance)
                    {
                        closestLoop = pLoop;
                        smallestDistance = distFromRadius;
                    }
                }

                if (closestLoop == null)
                {
                    Debug.LogError("AHHHH SOMETHING WENT WRONG CANT FIND THE LOOP!");
                    Destroy(conn.gameObject);
                    return;
                }
                
                closestLoop.Connect(conn, null);
                _connectors.Add(conn);
            }
        }

        private LevelData ConvertScreenToLevelData()
        {
            var tempLevel = ScriptableObject.CreateInstance<LevelData>();
            var done = new List<PlacedLoop>();
            
            tempLevel.Projectiles = new List<Vector2>();

            foreach (Projectile proj in _projectiles)
            {
                tempLevel.Projectiles.Add(proj.transform.localPosition);
            }
            
            if (_loops.Count == 0)
                return tempLevel;
            
            PlacedLoop startingLoop = _loops[0];

            tempLevel.StartingLoopPosition = startingLoop.transform.localPosition;

            tempLevel.StartingLoop = new LevelData.LoopData()
            {
                DoStartWith = true,
                Radius = startingLoop.Radius,
                Connectors = new List<LevelData.ConnectorData>()
            };
            
            AddConnectionsToLevelData(startingLoop, tempLevel.StartingLoop.Connectors, done);
            
            return tempLevel;
        }

        private void AddConnectionsToLevelData(PlacedLoop loop, List<LevelData.ConnectorData> destList, List<PlacedLoop> done)
        {
            if (done.Contains(loop))
                return;
            
            done.Add(loop);
            
            foreach (PlacedLoop.ConnectorInfo placedConnector in loop.Connectors)
            {
                if (placedConnector.OtherLoop != null && done.Contains(placedConnector.OtherLoop))
                    continue;
                
                var connData = new LevelData.ConnectorData()
                {
                    Type = placedConnector.Connector.Type,
                    Value = placedConnector.Connector.IntValue,
                    LoopSpace =  placedConnector.Offset,
                    DoStartWith = true
                };

                if (placedConnector.OtherLoop != null)
                {
                    connData.IsConnected = true;
                    connData.AttachedLoop = new LevelData.LoopData()
                    {
                        DoStartWith = true,
                        Radius = placedConnector.OtherLoop.Radius,
                        Connectors = new List<LevelData.ConnectorData>()
                    };
                    
                    AddConnectionsToLevelData(placedConnector.OtherLoop, connData.AttachedLoop.Connectors, done);
                }
                
                destList.Add(connData);
            }
        }

        public void StartPlayerSolution()
        {
            if (_isPlayingSolution)
                return;
            
            _tempLevel = ConvertScreenToLevelData();
            
            foreach (Projectile proj in _projectiles)
            {
                proj.Unfreeze();
            }

            _isPlayingSolution = true;
        }

        private void ClearEverything()
        {
            foreach (Projectile proj in _projectiles)
                Destroy(proj.gameObject);
            
            foreach (PlacedLoop loop in _loops)
                Destroy(loop.gameObject);
            
            foreach (Connector conn in _connectors)
                Destroy(conn.gameObject);
            
            _projectiles.Clear();
            _loops.Clear();
            _connectors.Clear();
            _connectorInventory.Clear();
            _loopInventory.Clear();
        }

        public void SoftReset()
        {
            if (_isPlayingSolution == false)
                return;

            if (_tempLevel == null)
                return;
            
            ClearEverything();
            
            SpawnLevel(_tempLevel);
            
            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }

            _isPlayingSolution = false;
        }

        public void HardReset()
        {
            ClearEverything();
            SpawnLevel(_currentLevel);
            
            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }

            _isPlayingSolution = false;
        }

        private void SpawnLevel(LevelData level)
        {
            foreach (Vector2 projPos in level.Projectiles)
            {
                Projectile newProjectile = Instantiate(_prefabs.GetProjectile(), transform).GetComponent<Projectile>();
                newProjectile.transform.localPosition = new Vector3(projPos.x, projPos.y, PROJ_DISTANCE);
                _projectiles.Add(newProjectile);
            }
            
            PlacedLoop startingLoop = Instantiate(_prefabs.GetLoop(), transform).GetComponent<PlacedLoop>();
            _loops.Add(startingLoop);

            Vector3 pos = new Vector3(level.StartingLoopPosition.x, level.StartingLoopPosition.y, LOOP_DISTANCE);
            startingLoop.InitFirstLoop(pos, level.StartingLoop.Radius);
            
            SpawnConnections(startingLoop, level.StartingLoop.Connectors);
        }

        private void AddToInventory(LevelData.ConnectorData connData)
        {
            _connectorInventory.Add(new ConnectorItem()
            {
                Type = connData.Type,
                Value = connData.Value
            });

            if (connData.IsConnected)
            {
                AddToInventory(connData.AttachedLoop);    
            }
        }

        private void AddToInventory(LevelData.LoopData loopData)
        {
            _loopInventory.Add(loopData.Radius);

            foreach (LevelData.ConnectorData cData in loopData.Connectors)
            {
                AddToInventory(cData);
            }
        }

        private void SpawnConnections(PlacedLoop loop, List<LevelData.ConnectorData> connectors)
        {
            if (connectors == null)
                return;
            
            foreach (LevelData.ConnectorData connData in connectors)
            {
                if (connData.DoStartWith == false)
                {
                    AddToInventory(connData);
                    continue;
                }
                
                GameObject newObj = Instantiate(_prefabs.GetConnector(connData.Type), transform);
                Connector conn = newObj.GetComponent<Connector>();
                conn.SetParameter(connData.Value);
                _connectors.Add(conn);
                
                Vector3 connPos = loop.LoopSpaceToPosition(connData.LoopSpace);
                connPos.z = CONN_DISTANCE;
                conn.transform.position = connPos;

                if (connData.IsConnected == false)
                {
                    loop.AddConnection(conn, connData.LoopSpace, null);
                    conn.SetLoops(loop, null);
                    continue;
                }
                
                if (connData.AttachedLoop.DoStartWith == false)
                {
                    loop.AddConnection(conn, connData.LoopSpace, null);
                    conn.SetLoops(loop, null);
                    
                    AddToInventory(connData.AttachedLoop);
                    continue;
                }
                
                PlacedLoop newLoop = Instantiate(_prefabs.GetLoop(), transform).GetComponent<PlacedLoop>();
                _loops.Add(newLoop);
                newLoop.Init(loop, conn, connData.AttachedLoop.Radius);
                SpawnConnections(newLoop, connData.AttachedLoop.Connectors);
                loop.AddConnection(conn, connData.LoopSpace, newLoop);
                newLoop.AddConnection(conn, (connData.LoopSpace + 0.5f) % 1, loop);
                conn.SetLoops(loop, newLoop);
            }
        }
    }
}