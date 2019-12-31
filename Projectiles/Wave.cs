using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Projectiles
{
    public class Wave : Projectile
    {
        public Wave(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0) : base(Position, velocity, rotation, team)
        {
            maxHealth = -1;
            health = 5;
            entityID = 25;
        }
        public override void SpecialUpdate()
        {
            rotation = Functions.ToRotation(Velocity) + (float)Math.PI / 2;
        }
    }
}
