using DG.Tweening;
using UnityEngine;

namespace Gmtk2025
{
    public class InventoryPickerButton : MonoBehaviour
    {
        private enum PickType { Loop, Connector, Projectile, Scoring, Delete };

        [SerializeField] private RectTransform _moverRect;
        [SerializeField] private LevelCreator _levelCreator;
        [SerializeField] private InventoryBar _parentBar;
        [SerializeField] private PickType _buttonType;
        
        [Header("Loop")] 
        [SerializeField] private float _loopRadius;

        [Header("Connector")] 
        [SerializeField] private ConnectorType _connectorType;
        [SerializeField] private int _connectorValue;

        private bool _isSelected;
        
        public void ButtonPress()
        {
            if (_isSelected)
            {
                Deselect();
                return;
            }
            _parentBar.OnSelectButton(this);

            if (_buttonType == PickType.Loop)
            {
                _levelCreator.StartPlacingLoop(_loopRadius);
            }
            else if (_buttonType == PickType.Connector)
            {
                _levelCreator.StartPlacingConnector(_connectorType, _connectorValue);
            }
            else if (_buttonType == PickType.Projectile)
            {
                _levelCreator.StartPlacingProjectile();
            }
            else if (_buttonType == PickType.Delete)
            {
                _levelCreator.StartDeleting();
            }
            else if (_buttonType == PickType.Scoring)
            {
                _levelCreator.StartPlacingScoring();
            }
            
        }

        public void Deselect()
        {
            if (_isSelected == false)
                return;
            
            _isSelected = false;
            DOTween.Kill(_moverRect);
            _moverRect.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutExpo);
            
            if (_buttonType == PickType.Delete)
            {
                _levelCreator.StopDeleting();
            }
            else
            {
                _levelCreator.StopPlacing();
            }
        }

        public void Select()
        {
            if (_isSelected)
                return;

            _isSelected = true;
            DOTween.Kill(_moverRect);
            _moverRect.DOAnchorPosY(-20, 0.5f).SetEase(Ease.OutExpo);
        }
    }
}
