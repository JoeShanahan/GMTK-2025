using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class BinarySwapper : Connector
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Color _goColor;
        [SerializeField] private Color _stopColor;
        [SerializeField] private bool _startsOn = true;
        private bool _canGo = true;

        // Sprite Stuff
        public Sprite sprite1;
        public Sprite sprite2;

        [SerializeField] private SpriteRenderer spriteRenderer;
        private bool isSprite1Active = true;
        
        public override ConnectorType Type => ConnectorType.Binary;
        public override int IntValue => _startsOn ? 1 : 0;
        
        public override void SetParameter(int number)
        {
            _canGo = number > 0;
            _startsOn = number > 0;
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
                PlacedLoop toLoop = currentLoop == _loopA ? _loopB : _loopA;
                projectile.SwapBetweenLoops(currentLoop, toLoop);

                // Swap Sprites
                if (isSprite1Active)
                {
                    spriteRenderer.sprite = sprite2;
                    isSprite1Active = false;
                } else
                {
                    spriteRenderer.sprite = sprite1;
                    isSprite1Active = true;
                }
            }

            _canGo = !_canGo;
            SetColor();
        }
    }
}
