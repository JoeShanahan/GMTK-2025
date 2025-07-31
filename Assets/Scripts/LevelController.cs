using System.Collections.Generic;
using UnityEngine;

namespace Gmtk2025
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelData _currentLevel;
        [SerializeField] private PrefabFactory _prefabs;
        
        [Space(16)]
        [SerializeField] private List<PlacedLoop> _loops;
        [SerializeField] private List<Connector> _connectors;
        [SerializeField] private List<Projectile> _projectiles;

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

        private void SpawnConnections(PlacedLoop loop, List<LevelData.ConnectorData> connectors)
        {
            foreach (var connData in connectors)
            {
                GameObject newObj = Instantiate(_prefabs.GetConnector(connData.Type), transform);
                Connector conn = newObj.GetComponent<Connector>();
                _connectors.Add(conn);
                
                Vector3 connPos = loop.LoopSpaceToPosition(connData.LoopSpace);
                connPos.z = CONN_DISTANCE;
                conn.transform.position = connPos;

                if (connData.IsConnected)
                {
                    PlacedLoop newLoop = Instantiate(_prefabs.GetLoop(), transform).GetComponent<PlacedLoop>();
                    newLoop.Init(loop, conn, connData.AttachedLoop.Radius);
                    SpawnConnections(newLoop, connData.AttachedLoop.Connectors);
                    loop.AddConnection(conn, connData.LoopSpace, newLoop);
                    newLoop.AddConnection(conn, (connData.LoopSpace + 0.5f) % 1, loop);
                    conn.SetLoops(loop, newLoop);
                }
                else
                {
                    loop.AddConnection(conn, connData.LoopSpace, null);
                    conn.SetLoops(loop, null);
                }
            }
        }
    }
}