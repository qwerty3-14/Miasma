using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Miasma.Ships;

namespace Miasma.Upgrades
{
    class Confuse : Upgrade
    {
        public Confuse()
        {
            upgradeID = 13;
            name = "Confuse Shot";
            description = "Shots have a 50% chance to cancels enemy action.";
        }
        public override void OnPewPewHit(PewPew pewpew, Entity hitTarget)
        {
            if(hitTarget is Ship && !(hitTarget is BeamShip))
            {
                if(Miasma.random.Next(2) ==0)
                {
                    ((Ship)hitTarget).stopActing();
                }
                
            }
        }
    }
}
