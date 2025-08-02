using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gmtk2025
{
    public class SaveMenu : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        
        private LevelEditorSaveData _saveData = new();

        public void Close()
        {
            gameObject.SetActive(false);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            _saveData.LoadFromPrefs();
            _inputField.text = LevelCreator.CurrentFilename;
        }
        
        public void ButtonPressSave()
        {
            var levelController = FindFirstObjectByType<LevelController>();
            
            if (string.IsNullOrEmpty(_inputField.text))
                return;
            
            LevelData level = levelController.ConvertScreenToLevelData();
            level.Filename = _inputField.text;
            _saveData.SaveLevel(level);
            Close();
        }
    }
}
