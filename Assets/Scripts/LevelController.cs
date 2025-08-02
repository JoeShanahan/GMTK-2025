using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<LevelData> _undoHistory = new();
        
        void Start()
        {
            Application.targetFrameRate = 60;
            SpawnLevel(_currentLevel);
            
            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }
        }

        public bool IsPlaying => _isPlayingSolution;

        public void AddPlaceable(Placeable p)
        {
            AddToUndoHistory();
            
            if (p is Projectile proj)
            {
                _projectiles.Add(proj);
            }
            else if (p is PlacedLoop loop)
            {
                _loops.Add(loop);
                RefreshAllConnections();
            }
            else if (p is Connector conn)
            {
                _connectors.Add(conn);
                RefreshAllConnections();
            }
            
            p.SetAsPlayerPlaced();
        }

        public LevelData ConvertScreenToLevelData()
        {
            var tempLevel = ScriptableObject.CreateInstance<LevelData>();
            
            tempLevel.Projectiles = new List<LevelData.ProjectileData>();

            foreach (Projectile proj in _projectiles)
            {
                tempLevel.Projectiles.Add(new LevelData.ProjectileData()
                {
                    Flags = proj.Flags,
                    Pos = proj.transform.localPosition
                });
            }

            foreach (PlacedLoop loop in _loops)
            {
                tempLevel.Loops.Add(new LevelData.LoopData()
                {
                    Flags = loop.Flags,
                    Pos = loop.transform.localPosition,
                    Radius = loop.Radius
                });
            }
            
            foreach (Connector conn in _connectors)
            {
                tempLevel.Connectors.Add(new LevelData.ConnectorData()
                {
                    Flags = conn.Flags,
                    Pos = conn.transform.localPosition,
                    Value = conn.IntValue,
                    Type = conn.Type
                });
            }
            
            return tempLevel;
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

        public void Undo()
        {
            if (_isPlayingSolution)
                return;
            
            if (_undoHistory.Count == 0)
                return;
            
            LevelData prev = _undoHistory.Last();
            _undoHistory.Remove(prev);
            
            ClearEverything();
            SpawnLevel(prev);

            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }
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

        public void HardReset(LevelData newLevel=null)
        {
            if (newLevel == null)
                newLevel = _currentLevel;
            
            ClearEverything();
            SpawnLevel(newLevel);
            
            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }

            _isPlayingSolution = false;
            _undoHistory.Clear();
        }

        private void AddToUndoHistory()
        {
            LevelData dat = ConvertScreenToLevelData();
            _undoHistory.Add(dat);

            while (_undoHistory.Count > 10)
                _undoHistory.RemoveAt(0);
        }

        private void SpawnLevel(LevelData level)
        {
            foreach (var projData in level.Projectiles)
            {
                Projectile newProjectile = Instantiate(_prefabs.GetProjectile(), transform).GetComponent<Projectile>();
                newProjectile.transform.localPosition = new Vector3(projData.Pos.x, projData.Pos.y, PROJ_DISTANCE);
                newProjectile.Flags = projData.Flags;
                _projectiles.Add(newProjectile);
            }
            
            foreach (var loopData in level.Loops)
            {
                PlacedLoop newLoop = Instantiate(_prefabs.GetLoop(), transform).GetComponent<PlacedLoop>();
                newLoop.Init(loopData.Radius);
                newLoop.transform.localPosition = new Vector3(loopData.Pos.x, loopData.Pos.y, LOOP_DISTANCE);
                newLoop.Flags = loopData.Flags;
                _loops.Add(newLoop);
            }
            
            foreach (var connData in level.Connectors)
            {
                Connector newConnector = Instantiate(_prefabs.GetConnector(connData.Type), transform).GetComponent<Connector>();
                newConnector.SetParameter(connData.Value);
                newConnector.Flags = connData.Flags;
                newConnector.transform.localPosition = new Vector3(connData.Pos.x, connData.Pos.y, CONN_DISTANCE);
                _connectors.Add(newConnector);
            }
            
            RefreshAllConnections();
        }

        private void RefreshAllConnections()
        {
            foreach (Connector conn in _connectors)
                conn.DetectConnections();
            
            foreach (PlacedLoop loop in _loops)
                loop.DetectConnections();
        }
    }
}