using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Miasma.Projectiles;
using Microsoft.Xna.Framework;

namespace Miasma.Upgrades
{
    public class MiasmaBlast : Upgrade
    {
        public MiasmaBlast()
        {
            upgradeID = 3;
            name = "Miasma Blast";
            description = "Miasma capsules explode on hit, damaging and potentialy infecting nearby enemies.";
        }
        int blastRadius = 30;
        
        public override void OnMiasmaHit(MiasmaPulse pulse)
        {
            for (int i = 0; i < 60; i++)
            {
                new Particle(pulse.Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 4f, Functions.RandomRotation()), Miasma.random.Next(2), 15);
            }
            for (int i = 0; i < Miasma.gameEntities.Count; i++)
            {
                if (Miasma.gameEntities[i].maxHealth != -1 && Miasma.gameEntities[i].Hitbox.Intersects(new Rectangle((int)pulse.Position.X - blastRadius, (int)pulse.Position.Y - blastRadius, blastRadius * 2, blastRadius * 2)) && pulse.team != Miasma.gameEntities[i].team)
                {
                    Miasma.gameEntities[i].Strike(5, true);
                }
            }
        }
    }
}
