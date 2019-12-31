using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Projectiles
{
    public class ArtillaryPulse : Projectile
    {
        public ArtillaryPulse(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0) : base(Position, velocity, rotation, team)
        {
            maxHealth = -1;
            health = 10;
            entityID = 6;
            Sounds.artillary.Play();
        }
        public override void SpecialUpdate()
        {
            rotation = Functions.ToRotation(Velocity) + (float)Math.PI / 2;
            if (Miasma.random.Next(2) == 0)
            {
                new Particle(Position + Vector2.UnitX * Miasma.random.Next(-2, 3), Vector2.Zero, Miasma.random.Next(2)+4, 6);
            }

        }

        public override void HitEffects(Entity target)
        {
            for (int i = 0; i < 8; i++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 6f, Functions.RandomRotation()), Miasma.random.Next(2)+4, 45);
            }
        }
    }

}
