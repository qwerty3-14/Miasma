using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Miasma.Ships;
using Microsoft.Xna.Framework;

namespace Miasma.Upgrades
{
    public class Richoche : Upgrade
    {
        public Richoche()
        {
            upgradeID = 8;
            name = "Richoche";
            description = "You normal shot have a 50% chance to Richoche";
        }
        Entity target;
        public override void OnPewPewHit(PewPew pewpew, Entity hitTarget)
        {
            if (Miasma.random.Next(2) == 0)
            {
                if (Targeting(ref target, pewpew.Position, hitTarget))
                {
                    PewPew rich = new PewPew(pewpew.Position, Functions.PolarVector(5, Functions.ToRotation(target.Position - pewpew.Position)), team: 1);
                    rich.addIgnore(hitTarget);
                }
            }
        }
        bool Targeting(ref Entity shootAt, Vector2 Position, Entity hitTarget)
        {
            Entity closest = null;
            float distance = 300;
           

                foreach (Ship ship in Miasma.enemyFleet)
                {
                    if ((ship.Position - Position).Length() < distance && ship != hitTarget)
                    {
                        distance = (ship.Position - Position).Length();
                        closest = ship;
                    }
                }
            
            shootAt = closest;
            return shootAt != null;
        }
    }
}
