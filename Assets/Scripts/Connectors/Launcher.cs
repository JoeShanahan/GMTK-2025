using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class Launcher : Connector
    {
        public override ConnectorType Type => ConnectorType.Launch;
        public override bool CanConnect => false;
        
        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            projectile.LeaveLoop();
        }
    }
}