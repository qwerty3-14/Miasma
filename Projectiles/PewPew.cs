using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Upgrades;
using Microsoft.Xna.Framework;

namespace Miasma.Projectiles
{
    public class PewPew : Projectile
    {
        Upgrade[] upgrades;
        public PewPew(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0, Upgrade[] upgrades = null) : base(Position, velocity, rotation, team)
        {
            maxHealth = -1;
            health = 5;
            entityID = 2;
            this.upgrades = upgrades;
            Sounds.pewpew.Play(); 
        }

        public override void SpecialUpdate()
        {
            rotation = Functions.ToRotation(Velocity) + (float)Math.PI / 2;
            if (upgrades != null)
            {
                foreach (Upgrade upgrade in upgrades)
                {
                    if (upgrade != null)
                    {
                        upgrade.PewPewEffects(this);
                    }
                }

            }
        }

        public override void HitEffects(Entity target)
        {
            if (upgrades != null)
            {


                foreach (Upgrade upgrade in upgrades)
                {
                    if (upgrade != null)
                    {
                        upgrade.OnPewPewHit(this, target);
                    }
                }
            }
        }
    }
}
