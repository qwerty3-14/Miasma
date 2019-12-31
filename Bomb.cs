using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Miasma.Ships;
using Microsoft.Xna.Framework;

namespace Miasma
{
    public class Bomb : Entity
    {
        bool launched = false;
        public Bomb(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            entityID = 15;
            health = maxHealth = 5;
        }
        public void Launch()
        {
            Sounds.launchMisc.Play();
            launched = true;
        }
        public void Explode()
        {
            
            health = 0;
        }
        public override void MainUpdate()
        {
            
            if(launched)
            {
                rotation += (float)Math.PI / 30;
                Velocity.Y = 1 * (team == 0 ? 1 : -1);
                if (team == 1)
                {
                    if (Miasma.random.Next(5) == 0)
                    {
                        new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 2f, Functions.RandomRotation()), Miasma.random.Next(2), 15);
                    }
                    Rectangle detection = new Rectangle(Miasma.leftSide, (int)Position.Y - 2, 400, 5);
                    foreach(Entity ship in Miasma.gameEntities)
                    {
                        if (ship.team == 0 && ship.maxHealth != -1 && ship.Velocity.Y == 0 && ship.Hitbox.Intersects(detection))
                        {
                            Explode();
                        }
                    }
                }
                else if( Position.Y >= Miasma.player.Position.Y)
                {
                    Explode();
                }
            }
            
        }
        public override void DeathEffects()
        {
            for (int i = 0; i < 8; i++)
            {
                new PewPew(Position, Functions.PolarVector(4, ((float)Math.PI * 2 * i) / 8f), team: team);
            }
        }
    }
}
