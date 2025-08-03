using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gmtk2025
{
    public class InventoryPickerButtonGame : InventoryPickerButtonBase
    {
        private enum PickType { Loop, Connector, Projectile, Scoring, Delete };

        [SerializeField] private Text _text;
        
        [SerializeField] private PickType _buttonType;
        
        [Header("Loop")] 
        [SerializeField] private float _loopRadius;

        [Header("Connector")] 
        [SerializeField] private ConnectorType _connectorType;
        [SerializeField] private int _connectorValue;

        
        public override void ButtonPress()
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

        public override void Deselect()
        {
            if (_isSelected == false)
                return;
            
            _isSelected = false;
            DOTween.Kill(_moverRect);
            _moverRect.DOAnchorPosX(-30, 0.5f).SetEase(Ease.OutExpo);

            if (_buttonType == PickType.Delete)
            {
                _levelCreator.StopDeleting();
            }
            else
            {
                _levelCreator.StopPlacing();
            }
        }

        public override void Select()
        {
            if (_isSelected)
                return;

            _isSelected = true;
            DOTween.Kill(_moverRect);
            _moverRect.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutExpo);
        }

        public void SetLoop(float radius)
        {
            _loopRadius = radius;
            _buttonType = PickType.Loop;

            int meters = Mathf.RoundToInt(radius);
            _text.text = $"Loop ({meters}m)";
        }
        
        public void SetConnector(ConnectorType type, int value)
        {
            _buttonType = PickType.Connector;
            _connectorType = type;
            _connectorValue = value;
            
            _text.text = $"{type}";
        }

    }
}
