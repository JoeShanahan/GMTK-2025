using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class Swapper : Connector
    {
        [SerializeField] private PlacedLoop _firstLoop;

        [SerializeField] private PlacedLoop _attachedLoop;

        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            // TODO swap from one loop to the other
            transform.localScale *= 1.3f;
        }
    }
}
