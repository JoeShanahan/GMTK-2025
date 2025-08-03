using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace Gmtk2025
{
    [CreateAssetMenu]
    public class LevelDataHolder : ScriptableObject
    {
        public int SelectedLevelIdx;
        public LevelData[] AllLevels;

        public LevelData CurrentLevel
        {
            get
            {
                try
                {
                    return AllLevels [SelectedLevelIdx];
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}
