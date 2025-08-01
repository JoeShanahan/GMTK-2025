using UnityEngine;

namespace Gmtk2025
{
    public abstract class Placeable : MonoBehaviour
    {
        public bool CanPlace { get; protected set; }

        public LevelDataFlags Flags;

        public void SetAsPlayerPlaced()
        {
            Flags |= LevelDataFlags.WasPlacedByPlayer;
        }
        
        public virtual void SetAsGhost(float value)
        {
            
        }
        
        public virtual void StopBeingAGhost()
        {
            
        }

        public virtual void MoveTo(Vector3 worldPos)
        {
            transform.position = worldPos;
        }
    }
}