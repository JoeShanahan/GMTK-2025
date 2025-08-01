using UnityEngine;

namespace Gmtk2025
{
    public enum ConnectorType
    {
        Unknown,
        Swap,
        Launch,
        Binary,
        Countdown
    }
    
    // TODO
    // Connectors can't be placed too close to each other
    public abstract class Connector : Placeable
    {
        public virtual ConnectorType Type => ConnectorType.Unknown;
        public virtual int IntValue => 0;

        public PlacedLoop LoopA => _firstLoop;
        public PlacedLoop LoopB => _attachedLoop;
        
        [SerializeField] protected PlacedLoop _firstLoop;

        [SerializeField] protected PlacedLoop _attachedLoop;

        public void SetLoops(PlacedLoop parent, PlacedLoop child)
        {
            _firstLoop = parent;
            _attachedLoop = child;
        }
        
        public virtual void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {

        }

        public virtual void SetParameter(int number)
        {
            
        }
    }
}