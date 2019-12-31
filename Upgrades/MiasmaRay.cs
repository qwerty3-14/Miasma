using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;

namespace Miasma.Upgrades
{
    public class MiasmaRay : Upgrade
    {
        public MiasmaRay()
        {
            upgradeID = 5;
            name = "Miasma Ray";
            description = "Miasma capsules are replaced with a laser.";
        }
        public override void MiasmaShotEffects(MiasmaPulse pulse)
        {
            if (pulse.timeLeft == 299)
            {
                bool escape = false;
                while (pulse.Position.Y > 0 && !escape)
                {
                    pulse.Position += pulse.getVelocity();
                    pulse.UpdateHitbox();
                    new Particle(pulse.Position, Vector2.Zero, Miasma.random.Next(2), 10);
                    for (int i = 0; i < Miasma.gameEntities.Count; i++)
                    {
                        if (Miasma.gameEntities[i].maxHealth != -1 && Miasma.gameEntities[i].Hitbox.Intersects(pulse.Hitbox) && pulse.team != Miasma.gameEntities[i].team)
                        {
                            escape = true;
                            break;
                        }
                    }
                }
            }

        }
    }
}
