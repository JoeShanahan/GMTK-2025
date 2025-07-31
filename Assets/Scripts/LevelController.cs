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
        
        private const float LOOP_DISTANCE = 0;
        private const float CONN_DISTANCE = -0.1f;
        private const float PROJ_DISTANCE = -0.2f;
        
        void Start()
        {
            SpawnLevel(_currentLevel);
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
                newLoop.Init(loop, conn, connData.AttachedLoop.Radius);
                SpawnConnections(newLoop, connData.AttachedLoop.Connectors);
                loop.AddConnection(conn, connData.LoopSpace, newLoop);
                newLoop.AddConnection(conn, (connData.LoopSpace + 0.5f) % 1, loop);
                conn.SetLoops(loop, newLoop);
            }
        }
    }
}