using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Miasma.Projectiles;

namespace Miasma.Upgrades
{
    public class WaveGuns : Upgrade
    {
        public WaveGuns()
        {
            upgradeID = 7;
            name = "Wave Guns";
            description = "normal shots are replaced with waves. Waves are bigger and have greater velocity.";
        }
        
        public override void PewPewEffects(PewPew pewpew)
        {
            pewpew.entityID = 25;
            pewpew.Velocity.Normalize();
            pewpew.Velocity *= 10;
        }
    }
}
