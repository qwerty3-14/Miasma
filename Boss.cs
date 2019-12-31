using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Ships;
using Microsoft.Xna.Framework;

namespace Miasma
{
    public abstract class Boss : Entity
    {
        public string name = "Boss";
        public Boss(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            for (int i = 0; i < Miasma.gameEntities.Count; i++)
            {
                if (Miasma.gameEntities[i].team == 1 && !(Miasma.gameEntities[i] is TheTransmission))
                {
                    Miasma.gameEntities[i].health = 0;
                }
            }
            if(Miasma.gameState == GameScene.Combat)
            {
                Sounds.PlayMusic(2);
            }
            
        }
        public override void MainUpdate()
        {
            if(team ==1)
            {
                health = 0;
            }
        }
        public override void DeathEffects()
        {
            Miasma.LoadUpgrades();
            for (int s = 0; s < Miasma.gameEntities.Count; s++)
            {
                if (Miasma.gameEntities[s] is Ship)
                {
                    ((Ship)Miasma.gameEntities[s]).Flee();
                }

            }
            Miasma.enemyFleet.Clear();
            for (int d = 0; d < 18; d++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2) + (team == 1 ? 0 : 2), 30);
            }
            Sounds.PlayMusic(1);
        }
    }
}
