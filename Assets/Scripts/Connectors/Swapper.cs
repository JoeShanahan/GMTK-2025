using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class Swapper : Connector
    {
        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            PlacedLoop toLoop = currentLoop == _firstLoop ? _attachedLoop : _firstLoop;
            projectile.SwapBetweenLoops(currentLoop, toLoop);
        }
    }
}
