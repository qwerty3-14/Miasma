using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma
{
    public class Capsule : Entity
    {
        public Capsule(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            entityID = 12;
            health = maxHealth = 1;
        }
        public override void MainUpdate()
        {
            health = 0;
        }
    }
}
