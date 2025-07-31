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

        void Start()
        {
            SpawnLevel(_currentLevel);
        }

        private void SpawnLevel(LevelData level)
        {
            PlacedLoop startingLoop = Instantiate(_prefabs.GetLoop(), transform).GetComponent<PlacedLoop>();
            startingLoop.InitFirstLoop(level.StartingLoopPosition, level.StartingLoop.Radius);
        }
    }
}