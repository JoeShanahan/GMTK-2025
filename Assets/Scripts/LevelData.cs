using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace Gmtk2025
{
    [Flags]
    public enum LevelDataFlags
    {
        None = 0,
        StartWith = 1,
        WasPlacedByPlayer = 2
    }
    
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [Serializable]
        public class LoopData
        {
            public LevelDataFlags Flags;
            public float Radius;
            public Vector2 Pos;
        }

        [Serializable]
        public class ConnectorData
        {
            public ConnectorType Type;
            public LevelDataFlags Flags;
            public int Value;
            public Vector2 Pos;
        }

        public string Filename;
        public List<Vector2> Projectiles = new();
        public List<LoopData> Loops = new();
        public List<ConnectorData> Connectors = new();

        public string ToBase64()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(this));
            
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
            
            return Convert.ToBase64String(output.ToArray());
        }

        public void FromBase64Overwrite(string base64)
        {
            byte[] compressedBytes = Convert.FromBase64String(base64);
    
            using var input = new MemoryStream(compressedBytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
    
            gzip.CopyTo(output);
            string json = Encoding.UTF8.GetString(output.ToArray());
    
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}
