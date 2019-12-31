using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Projectiles
{
    public class BigArtillaryPulse : ArtillaryPulse
    {
        public BigArtillaryPulse(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0) : base(Position, velocity, rotation, team)
        {
            maxHealth = -1;
            health = 15;
            entityID = 20;
        }
    }
}
