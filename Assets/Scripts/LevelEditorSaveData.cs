using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gmtk2025
{
    [Serializable]
    public class LevelEditorSaveData
    {
        public List<string> Filenames = new();

        public void LoadFromPrefs()
        {
            if (PlayerPrefs.HasKey("SavedLevels") == false)
                return;
            
            string metaJson = PlayerPrefs.GetString("SavedLevels");
            PlayerPrefs.GetString("SavedLevels");
            
            JsonUtility.FromJsonOverwrite(metaJson, this);
            Debug.Log($"Loaded {Filenames.Count} levels from prefs!");
        }

        public LevelData GetLevel(string filename)
        {
            if (PlayerPrefs.HasKey($"SavedLevel.{filename}") == false)
                return null;

            var result = ScriptableObject.CreateInstance<LevelData>();

            string jdata = PlayerPrefs.GetString($"SavedLevel.{filename}");
            JsonUtility.FromJsonOverwrite(jdata, result);
            
            return result;
        }
        
        public void SaveLevel(LevelData level)
        {
            Filenames.Add(level.Filename);

            string prefsKey = $"SavedLevel.{level.Filename}";
            string levelJson = JsonUtility.ToJson(level);
            PlayerPrefs.SetString(prefsKey, levelJson);

            string metaJson = JsonUtility.ToJson(this);
            PlayerPrefs.SetString("SavedLevels", metaJson);
            Debug.Log(metaJson);

            PlayerPrefs.Save();
        }
    }
}
