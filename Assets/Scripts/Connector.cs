using UnityEngine;

namespace Gmtk2025
{
    // TODO
    // Connectors can't be placed too close to each other
    public abstract class Connector : MonoBehaviour
    {
        [SerializeField] protected PlacedLoop _firstLoop;

        [SerializeField] protected PlacedLoop _attachedLoop;
        
        public virtual void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {

        }
    }
}