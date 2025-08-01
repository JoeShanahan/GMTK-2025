using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class Swapper : Connector
    {
        public override ConnectorType Type => ConnectorType.Swap;
        
        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            if (_attachedLoop == null || _firstLoop == null)
                return;

            PlacedLoop toLoop = currentLoop == _firstLoop ? _attachedLoop : _firstLoop;
            projectile.SwapBetweenLoops(currentLoop, toLoop);
        }
    }
}
