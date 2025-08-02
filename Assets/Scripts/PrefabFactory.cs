using System;
using UnityEngine;

namespace Gmtk2025
{
    [CreateAssetMenu]
    public class PrefabFactory : ScriptableObject
    {
        [SerializeField] private GameObject _loopPrefab;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _swapPrefab;
        [SerializeField] private GameObject _launchPrefab;
        [SerializeField] private GameObject _binaryPrefab;
        [SerializeField] private GameObject _countdownPrefab;
        [SerializeField] private GameObject _splitterPrefab;
        [SerializeField] private GameObject _portalPrefab;
        [SerializeField] private GameObject _scoringPrefab;
        [SerializeField] private GameObject _flagPrefab;

        public GameObject Flag => _flagPrefab;

        public GameObject GetLoop() => _loopPrefab;
        
        public GameObject GetProjectile() => _projectilePrefab;

        public GameObject GetScoring() => _scoringPrefab;

        public GameObject GetConnector(ConnectorType type)
        {
            return type switch
            {
                ConnectorType.Unknown => null,
                ConnectorType.Swap => _swapPrefab,
                ConnectorType.Launch => _launchPrefab,
                ConnectorType.Binary => _binaryPrefab,
                ConnectorType.Countdown => _countdownPrefab,
                ConnectorType.Splitter => _splitterPrefab,
                ConnectorType.Portal => _portalPrefab,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
