using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Gmtk2025
{
    public class LoadMenu : MonoBehaviour
    {
        [SerializeField] private InputField _codeOutput;
        [SerializeField] private InputField _codeInput;
        [SerializeField] private RectTransform _listParent;
        [SerializeField] private GameObject _listItemTemplate;
        [SerializeField] private string _selectedFilename;
        
        private List<LoadMenuItem> _items = new();

        private LevelEditorSaveData _saveData = new();

        public void Close()
        {
            gameObject.SetActive(false);
            _codeOutput.gameObject.SetActive(false);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);

            _saveData.LoadFromPrefs();
            _items.Clear();

            foreach (Transform t in _listParent)
            {
                Destroy(t.gameObject);
            }

            foreach (string levelName in _saveData.Filenames)
            {
                LoadMenuItem itm = Instantiate(_listItemTemplate, _listParent).GetComponent<LoadMenuItem>();
                itm.Init(levelName);
                itm.OnPress += OnButtonPress;
                _items.Add(itm);
            }

            if (_items.Count > 0)
            {
                _items[0].SelectMe();
                _selectedFilename = _items[0].Filename;
            }
        }


        private void OnButtonPress(LoadMenuItem itm)
        {
            _selectedFilename = itm.Filename;
            
            foreach (LoadMenuItem otherItm in _items)
            {
                if (otherItm == itm)
                {
                    otherItm.SelectMe();
                }
                else
                {
                    otherItm.UnselectMe();
                }
            }
        }

        public void ButtonPressLoad()
        {
            if (string.IsNullOrEmpty(_selectedFilename) == false)
            {
                LevelData level = _saveData.GetLevel(_selectedFilename);

                if (level != null)
                {
                    LevelCreator.CurrentFilename = _selectedFilename;
                    FindFirstObjectByType<LevelController>().HardReset(level);
                }
            }
            
            Close();
        }

        public void ButtonPressImportCustomLevel()
        {
            try
            {
                
                string b64Level = _codeInput.text;

                var lvl = ScriptableObject.CreateInstance<LevelData>();
                lvl.FromBase64Overwrite(b64Level);
                FindFirstObjectByType<LevelController>().HardReset(lvl);
                Close();

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
        }

        public void ButtonPressExport()
        {
            if (string.IsNullOrEmpty(_selectedFilename) == false)
            {
                LevelData level = _saveData.GetLevel(_selectedFilename);

                if (level != null)
                {
#if UNITY_EDITOR

                    string path = $"Assets/level_{_selectedFilename}.asset";
                    path = AssetDatabase.GenerateUniqueAssetPath(path);

                    AssetDatabase.CreateAsset(level, path);
                    AssetDatabase.SaveAssets();
#endif
                    _codeOutput.gameObject.SetActive(true);
                    _codeOutput.text = level.ToBase64();

                }
            }
        }
    }
}
