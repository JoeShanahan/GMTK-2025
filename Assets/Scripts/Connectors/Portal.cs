using UnityEngine;

namespace Gmtk2025.Connectors
{
    public class Portal : Connector
    {
        public override ConnectorType Type => ConnectorType.Portal;

        public int PortalId => _portalId;
        public override int IntValue => _portalId;
        public override bool CanConnect => false;

        
        private int _portalId;
        
        public override void OnProjectilePassed(Projectile projectile, PlacedLoop currentLoop)
        {
            var level = FindFirstObjectByType<LevelController>();

            foreach (Connector conn in level.AllConnectors)
            {
                if (conn == this || conn.Type != ConnectorType.Portal || conn.IntValue != IntValue)
                    continue;
                
                if (conn is Portal otherPortal)
                {
                    PlacedLoop destLoop = otherPortal._loopA;
                    projectile.WarpTo(conn.transform.position, destLoop);
                    return;
                }
            }
        }
        
        public override void SetParameter(int number)
        {
            _portalId = number;
        }
    }
}
