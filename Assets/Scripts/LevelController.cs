using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        [SerializeField] private LevelDataHolder _levelDataHolder;
        [SerializeField] private PrefabFactory _prefabs;
        
        [Space(16)]
        [SerializeField] private List<PlacedLoop> _loops;
        [SerializeField] private List<Connector> _connectors;
        [SerializeField] private List<Projectile> _projectiles;
        [SerializeField] private List<Scoring> _scoring;

        [Space(16)] 
        [SerializeField] private Text _buttonText;

        [SerializeField] private LevelEditorUI _levelEditUI;
        [SerializeField] private GameEditorUI _gameEditUI;
        
        private List<GameObject> _riderFlags = new();
        private int _neededScore;
        private int _currentScore;

        public IEnumerable<Projectile> AllProjectiles => _projectiles;
        public IEnumerable<Connector> AllConnectors => _connectors;
        public IEnumerable<PlacedLoop> AllLoops => _loops;
        
        private const float LOOP_DISTANCE = 5;
        private const float CONN_DISTANCE = -0.1f;
        private const float PROJ_DISTANCE = -0.2f;

        private bool _isPlayingSolution;
        [SerializeField]
        private LevelData _tempLevel;

        [SerializeField]
        private bool _isEditMode;
        
        private List<LevelData> _undoHistory = new();

        public Rect GetLevelBounds()
        {
            float minX = 999999;
            float minY = 999999;
            float maxX = -999999;
            float maxY = -999999;
    
            foreach (PlacedLoop loop in _loops)
            {
                Vector2 pos = loop.transform.position;
                float r = loop.Radius;

                minX = Mathf.Min(minX, pos.x - r);
                maxX = Mathf.Max(maxX, pos.x + r);
                minY = Mathf.Min(minY, pos.y - r);
                maxY = Mathf.Max(maxY, pos.y + r);
            }

            foreach (Connector conn in _connectors)
            {
                Vector2 pos = conn.transform.position;
                float r = 1f;

                minX = Mathf.Min(minX, pos.x - r);
                maxX = Mathf.Max(maxX, pos.x + r);
                minY = Mathf.Min(minY, pos.y - r);
                maxY = Mathf.Max(maxY, pos.y + r);
            }
            
            foreach (Projectile proj in _projectiles)
            {
                Vector2 pos = proj.transform.position;
                float r = 1f;

                minX = Mathf.Min(minX, pos.x - r);
                maxX = Mathf.Max(maxX, pos.x + r);
                minY = Mathf.Min(minY, pos.y - r);
                maxY = Mathf.Max(maxY, pos.y + r);
            }
            
            foreach (Scoring s in _scoring)
            {
                Vector2 pos = s.transform.position;
                float r = 1f;

                minX = Mathf.Min(minX, pos.x - r);
                maxX = Mathf.Max(maxX, pos.x + r);
                minY = Mathf.Min(minY, pos.y - r);
                maxY = Mathf.Max(maxY, pos.y + r);
            }

            if (minX > 99999)
            {
                return new Rect(-1, -1, 2, 2);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private LevelData _currentLevel;
        
        void Start()
        {
            Application.targetFrameRate = 60;
            if (_gameEditUI != null) // this is a dumb way to check we're in game mode rather than edit mode but oh well
            {
                _currentLevel = _levelDataHolder.CurrentLevel;
                SpawnLevel(_currentLevel, true);
                _gameEditUI.SetBackground(_currentLevel.Background);

                foreach (Projectile proj in _projectiles)
                {
                    proj.Freeze();
                }
                
                FindFirstObjectByType<CameraLevelFramer>().SnapFrameLevel();

                _gameEditUI?.UpdateScore(_currentScore, _neededScore);
                _gameEditUI?.SetInventory(_currentLevel.LoopInventory, _currentLevel.ConnectorInventory);
            }
            else
            {
                _currentLevel = ScriptableObject.CreateInstance<LevelData>();
            }
        }

        public bool IsPlaying => _isPlayingSolution;

        public void AddPlaceable(Placeable p)
        {
            AddToUndoHistory();

            Vector3 position = p.transform.position;
            
            if (p is Projectile proj)
            {
                position.z = PROJ_DISTANCE;
                _projectiles.Add(proj);
            }
            else if (p is PlacedLoop loop)
            {
                position.z = LOOP_DISTANCE;
                _loops.Add(loop);
                RefreshAllConnections();

                foreach (float f in _currentLevel.LoopInventory)
                {
                    if (Mathf.Approximately(f, loop.Radius))
                    {
                        _currentLevel.LoopInventory.Remove(f);
                        break;
                    }
                }
            }
            else if (p is Connector conn)
            {
                position.z = CONN_DISTANCE;
                _connectors.Add(conn);
                RefreshAllConnections();
                
                foreach (var ci in _currentLevel.ConnectorInventory)
                {
                    if (ci.Type == conn.Type && ci.Value == conn.IntValue)
                    {
                        _currentLevel.ConnectorInventory.Remove(ci);
                        break;
                    }
                }
            }
            else if (p is Scoring scor)
            {
                position.z = CONN_DISTANCE;
                _scoring.Add(scor);
                Debug.Log("Adding scoring item to scene");
            }

            p.transform.position = position;
            
            p.SetAsPlayerPlaced();
            _gameEditUI?.SetInventory(_currentLevel.LoopInventory, _currentLevel.ConnectorInventory);

            if (_gameEditUI != null)
                FindFirstObjectByType<CameraLevelFramer>().FramePressed();
        }

        public LevelData ConvertScreenToLevelData()
        {
            var tempLevel = ScriptableObject.CreateInstance<LevelData>();
            
            tempLevel.Projectiles = new List<LevelData.ProjectileData>();
            tempLevel.Scoring = new List<LevelData.ProjectileData>();

            foreach (Projectile proj in _projectiles)
            {
                tempLevel.Projectiles.Add(new LevelData.ProjectileData()
                {
                    Flags = proj.Flags,
                    Pos = proj.transform.localPosition
                });
            }
            
            foreach (Scoring sc in _scoring)
            {
                tempLevel.Scoring.Add(new LevelData.ProjectileData()
                {
                    Flags = sc.Flags,
                    Pos = sc.transform.localPosition
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

        public void PlayButtonPressed()
        {
            if (_isPlayingSolution)
            {
                _levelEditUI?.OnStopPlaying();
                SoftReset();
                _buttonText.text = "Play";
            }
            else
            {
                _levelEditUI?.OnStartPlaying();
                StartPlayerSolution();
                _buttonText.text = "Stop";
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

        public void IncreaseScore(int amount)
        {
            _currentScore += amount;
            _gameEditUI?.UpdateScore(_currentScore, _neededScore);

            if (_currentScore >= _neededScore)
            {
                _gameEditUI?.OnLevelComplete();
            }
        }

        private void ClearEverything()
        {
            _currentScore = 0;
            
            foreach (Projectile proj in _projectiles)
                Destroy(proj.gameObject);
            
            foreach (PlacedLoop loop in _loops)
                Destroy(loop.gameObject);
            
            foreach (Connector conn in _connectors)
                Destroy(conn.gameObject);
            
            foreach (Scoring sc in _scoring)
                Destroy(sc.gameObject);
            
            _projectiles.Clear();
            _loops.Clear();
            _connectors.Clear();
            _scoring.Clear();
        }

        public void RemovePlaceable(Placeable p)
        {
            AddToUndoHistory();
            
            if (p is Projectile proj)
            {
                _projectiles.Remove(proj);
            }
            else if (p is PlacedLoop loop)
            {
                _loops.Remove(loop);
                RefreshAllConnections();
            }
            else if (p is Connector conn)
            {
                _connectors.Remove(conn);
                RefreshAllConnections();
            }
            else if (p is Scoring scor)
            {
                _scoring.Remove(scor);
            }
            
            Destroy(p.gameObject);
            
            if (_gameEditUI != null)
                FindFirstObjectByType<CameraLevelFramer>().FramePressed();
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
            _currentLevel.LoopInventory = prev.LoopInventory;
            _currentLevel.ConnectorInventory = prev.ConnectorInventory;
            _gameEditUI?.SetInventory(_currentLevel.LoopInventory, _currentLevel.ConnectorInventory);

            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }
            
            if (_gameEditUI != null)
                FindFirstObjectByType<CameraLevelFramer>().FramePressed();
        }

        public void SoftReset()
        {
            if (_isPlayingSolution == false)
                return;

            if (_tempLevel == null)
                return;
            
            ClearEverything();
            
            SpawnLevel(_tempLevel);
            _gameEditUI?.UpdateScore(_currentScore, _neededScore);

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
            SpawnLevel(newLevel, true);
            _gameEditUI?.UpdateScore(_currentScore, _neededScore);
            _gameEditUI?.SetInventory(_currentLevel.LoopInventory, _currentLevel.ConnectorInventory);
            
            foreach (Projectile proj in _projectiles)
            {
                proj.Freeze();
            }

            _isPlayingSolution = false;
            _undoHistory.Clear();
        }

        public void ClearProjectileFlags()
        {
            // We will destroy all of extant flag GameObjects and spawn new ones
            // Object pooling would be more efficient but life goes on
            while (_riderFlags.Count > 0)
            {
                Destroy(_riderFlags[0]);
                _riderFlags.RemoveAt(0);
            }
        }
        
        public void GoProjectileFlags()
        {
            if (!_isPlayingSolution)
                return;

            ClearProjectileFlags();

            foreach (var projectile in _projectiles)
            {
                Quaternion velocityDirection = Quaternion.FromToRotation(new(1, 0, 0), projectile.Velocity);

                var flag = Instantiate(_prefabs.Flag, projectile.transform.position, velocityDirection, transform);
                _riderFlags.Add(flag);
            }
        }

        private void AddToUndoHistory()
        {
            LevelData dat = ConvertScreenToLevelData();
            dat.LoopInventory = new List<float>(_currentLevel.LoopInventory);
            dat.ConnectorInventory = new List<ConnectorItem>(_currentLevel.ConnectorInventory);
            _undoHistory.Add(dat);

            while (_undoHistory.Count > 10)
                _undoHistory.RemoveAt(0);
        }

        private void SpawnLevel(LevelData level, bool isHardReset=false)
        {
            _neededScore = 0;
            
            if (isHardReset)
            {
                _currentLevel.LoopInventory.Clear();
                _currentLevel.ConnectorInventory.Clear();
            }
            
            foreach (var projData in level.Projectiles)
            {
                Projectile newProjectile =
                    Instantiate(_prefabs.GetProjectile(), transform).GetComponent<Projectile>();
                newProjectile.transform.localPosition = new Vector3(projData.Pos.x, projData.Pos.y, PROJ_DISTANCE);
                newProjectile.Flags = projData.Flags;
                _projectiles.Add(newProjectile);

            }
            
            foreach (var projData in level.Scoring)
            {
                Scoring newScoring = Instantiate(_prefabs.GetScoring(), transform).GetComponent<Scoring>();
                newScoring.transform.localPosition = new Vector3(projData.Pos.x, projData.Pos.y, PROJ_DISTANCE);
                newScoring.Flags = projData.Flags;
                _scoring.Add(newScoring);
                _neededScore++;
            }
            
            foreach (var loopData in level.Loops)
            {
                if (!_isEditMode && isHardReset && !loopData.Flags.HasFlag(LevelDataFlags.StartWith))
                {
                    _currentLevel.LoopInventory.Add(loopData.Radius);
                }
                else
                {
                    PlacedLoop newLoop = Instantiate(_prefabs.GetLoop(), transform).GetComponent<PlacedLoop>();
                    newLoop.Init(loopData.Radius);
                    newLoop.transform.localPosition = new Vector3(loopData.Pos.x, loopData.Pos.y, LOOP_DISTANCE);
                    newLoop.Flags = loopData.Flags;
                    _loops.Add(newLoop);
                }
            }
            
            foreach (var connData in level.Connectors)
            {
                if (!_isEditMode && isHardReset && !connData.Flags.HasFlag(LevelDataFlags.StartWith))
                {
                    _currentLevel.ConnectorInventory.Add(new ConnectorItem
                    {
                        Type = connData.Type,
                        Value = connData.Value
                    });
                }
                else
                {
                    Connector newConnector = Instantiate(_prefabs.GetConnector(connData.Type), transform)
                        .GetComponent<Connector>();
                    newConnector.SetParameter(connData.Value);
                    newConnector.Flags = connData.Flags;
                    newConnector.transform.localPosition = new Vector3(connData.Pos.x, connData.Pos.y, CONN_DISTANCE);
                    _connectors.Add(newConnector);
                }
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