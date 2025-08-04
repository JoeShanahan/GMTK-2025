using UnityEngine;

namespace Gmtk2025
{
    public class Scoring : Placeable
    {
        [SerializeField] private SpriteRenderer _ghostSprite;
        [SerializeField] private Transform _realVisuals;

        //private bool _isGhost;

        public override void SetAsGhost(float value)
        {
            _ghostSprite.gameObject.SetActive(true);
            _realVisuals.gameObject.SetActive(false);
            //_isGhost = true;
        }

        public override void StopBeingAGhost()
        {
            _ghostSprite.gameObject.SetActive(false);
            _realVisuals.gameObject.SetActive(true);
            //_isGhost = false;
        }

        public override void MoveTo(Vector3 worldPos)
        {
            CanPlace = true;
            worldPos.x = Mathf.RoundToInt(worldPos.x * 4) / 4f;
            worldPos.y = Mathf.RoundToInt(worldPos.y * 4) / 4f;

            transform.position = worldPos;
        }

        // scoring methods
        // check if object with Projectile tag passes through this gameobject
        // remove gameobject if so
        // add 1 to score

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Projectile"))
            {
                FindFirstObjectByType<LevelController>().IncreaseScore(1); // can be changed then to accomodate different scores per pick-up but for now each one is just 1

                gameObject.SetActive(false);
            }
        }

    }
}
