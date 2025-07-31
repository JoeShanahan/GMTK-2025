using UnityEngine;

namespace Gmtk2025
{
    // TODO
    // Connectors can't be placed too close to each other
    public abstract class Connector : MonoBehaviour
    {
        public virtual void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            transform.localScale *= 1.5f;
        }
    }
}