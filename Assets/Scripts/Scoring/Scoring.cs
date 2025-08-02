using UnityEngine;

namespace Gmtk2025
{
    public class Scoring : Placeable
    {
        [SerializeField] private SpriteRenderer _ghostSprite;
        [SerializeField] private Transform _realVisuals;

        private bool _isGhost;

        [SerializeField] private bool canPlace = true;

        public override void SetAsGhost(float value)
        {
            _ghostSprite.gameObject.SetActive(true);
            _realVisuals.gameObject.SetActive(false);
            _isGhost = true;
        }

        public override void StopBeingAGhost()
        {
            _ghostSprite.gameObject.SetActive(false);
            _realVisuals.gameObject.SetActive(true);
            _isGhost = false;
        }

        public override void MoveTo(Vector3 worldPos)
        {
            CanPlace = true;
            worldPos.x = Mathf.RoundToInt(worldPos.x * 4) / 4f;
            worldPos.y = Mathf.RoundToInt(worldPos.y * 4) / 4f;

            transform.position = worldPos;
        }
    }
}
