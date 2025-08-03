using System.Collections.Generic;
using DG.Tweening;
using LudumDare55;
using UnityEngine;
using UnityEngine.UI;

namespace Gmtk2025
{
    public class GameEditorUI : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private LevelDataHolder _levels;
        [SerializeField] private CanvasGroup _levelCompleteScreen;
        [SerializeField] private InventoryBar _inventory;
        [SerializeField] private Image _bg;
        
        public void SetBackground(Sprite bg)
        {
            if (bg != null)
            {
                _bg.sprite = bg;
                _bg.gameObject.SetActive(true);
            }
        }
        
        public void SetInventory(List<float> loops, List<ConnectorItem> connectors)
        {
            _inventory.SetInventory(loops, connectors);
        }
        
        public void UpdateScore(int cur, int tot)
        {
            _scoreText.text = $"{cur}/{tot}";
        }

        private bool _hasPressedNext = false;

        public void OnLevelComplete()
        {
            _levelCompleteScreen.gameObject.SetActive(true);
            _levelCompleteScreen.DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        }
        
        public void LevelPressNext()
        {
            if (_hasPressedNext)
                return;

            _levels.SelectedLevelIdx++;
            _hasPressedNext = true;

            if (_levels.CurrentLevel == null)
            {
                TransitionManager.Instance.GoToMenu();
            }
            else
            {
                TransitionManager.Instance.GoToGame();
            }
            
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
