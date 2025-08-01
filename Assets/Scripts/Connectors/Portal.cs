using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class Portal : Connector
    {
        public override ConnectorType Type => ConnectorType.Swap;
        
        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            if (_loopB == null || _loopA == null)
                return;

            PlacedLoop toLoop = currentLoop == _loopA ? _loopB : _loopA;
            projectile.SwapBetweenLoops(currentLoop, toLoop);
        }
    }
}
