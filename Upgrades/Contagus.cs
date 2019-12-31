using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Upgrades
{
    public class Contagus : Upgrade
    {
        public Contagus()
        {
            upgradeID = 11;
            name = "Contagous";
            description = "Infected ships occasionaly spread miasma";
        }
        int blastRadius = 30;
        public override void InfectedShipUpdate(Entity ship)
        {
            if(Miasma.random.Next(180) == 0)
            {
                for (int i = 0; i < 60; i++)
                {
                    new Particle(ship.Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 4f, Functions.RandomRotation()), Miasma.random.Next(2), 15);
                }
                for (int i = 0; i < Miasma.gameEntities.Count; i++)
                {
                    if (Miasma.gameEntities[i].maxHealth != -1 && Miasma.gameEntities[i].Hitbox.Intersects(new Rectangle((int)ship.Position.X - blastRadius, (int)ship.Position.Y - blastRadius, blastRadius * 2, blastRadius * 2)) && ship.team != Miasma.gameEntities[i].team)
                    {
                        Miasma.gameEntities[i].Strike(5, true);
                    }
                }
            }
        }
    }
}
