using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gmtk2025
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [Serializable]
        public class LoopData
        {
            public float Radius;
            public List<ConnectorData> Connectors;
        }

        [Serializable]
        public class ConnectorData
        {
            public ConnectorType Type;
            public float LoopSpace;
            public bool IsConnected;
            public LoopData AttachedLoop;
        }

        public List<Vector2> Projectiles;
        public Vector2 StartingLoopPosition;
        public LoopData StartingLoop;
    }
}
