using DG.Tweening;
using UnityEngine;

namespace Gmtk2025
{
    public class LevelEditorUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _topBar;
        [SerializeField] private RectTransform _sideBar;
        [SerializeField] private RectTransform _bottomBar;

        private float _topY;
        private float _bottomY;
        private float _sideX;
        
        public void OnStartPlaying()
        {
            _topBar.DOAnchorPosY(120, 0.6f).SetEase(Ease.OutExpo);
            _bottomBar.DOAnchorPosY(-120, 0.6f).SetEase(Ease.OutExpo);
            _sideBar.DOAnchorPosX(-120, 0.6f).SetEase(Ease.OutExpo);
        }

        public void OnStopPlaying()
        {
            _topBar.DOAnchorPosY(_topY, 0.6f).SetEase(Ease.OutExpo);
            _bottomBar.DOAnchorPosY(_bottomY, 0.6f).SetEase(Ease.OutExpo);
            _sideBar.DOAnchorPosX(_sideX, 0.6f).SetEase(Ease.OutExpo);
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _topY = _topBar.anchoredPosition.y;
            _bottomY = _bottomBar.anchoredPosition.y;
            _sideX = _sideBar.anchoredPosition.x;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
