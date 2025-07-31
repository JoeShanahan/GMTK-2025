using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class BinarySwapper : Connector
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Color _goColor;
        [SerializeField] private Color _stopColor;
        [SerializeField] private bool _canGo = true;
        
        public override void SetParameter(int number)
        {
            _canGo = number > 0;
        }
        
        private void Start()
        {
            SetColor();
        }

        private void SetColor()
        {
            _sprite.color = _canGo ? _goColor : _stopColor;
        }
        
        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            if (_canGo)
            {
                PlacedLoop toLoop = currentLoop == _firstLoop ? _attachedLoop : _firstLoop;
                projectile.SwapBetweenLoops(currentLoop, toLoop);
            }

            _canGo = !_canGo;
            SetColor();
        }
    }
}
