using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Boss3
{
    public class Block : Entity
    {
        Pulsar parent;
        public Vector2 moveTo;
        public Block(Vector2 Position, Vector2 Velocity, Pulsar parent, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            this.Velocity = Velocity;
            this.parent = parent;
            health = maxHealth = 10;
            //parent.blocks.Add(this);
            moveTo = Position;
            entityID = 23;
        }
        public bool guarding = false;
        float speed = 5f;
        int infectTime = 0;
        public override void SpecialUpdate()
        {
            speed = parent.speed * 1.3f;
            if(!Miasma.BossIsActive())
            {
                health = 0;
            }
            if(team == 1)
            {
                infectTime++;
            }
            if (infectTime > 60 || team == 0)
            {
                for (int i = 0; i < Miasma.gameEntities.Count; i++)
                {
                    if (Miasma.gameEntities[i].maxHealth != -1 && Miasma.gameEntities[i].Hitbox.Intersects(Hitbox) && team != Miasma.gameEntities[i].team)
                    {
                        if(!(parent.blocks.Contains(Miasma.gameEntities[i]) && team == 0))
                        {
                            Miasma.gameEntities[i].Strike(10);
                            health = 0;
                            break;
                        }
                    }
                }
            }
            if (guarding)
            {
                rotation = 0;
                if ((moveTo - Position).Length() < speed)
                {
                    Position = moveTo;
                    Velocity = Vector2.Zero;
                    rotation = 0f;
                }
                else
                {
                    Velocity = Functions.PolarVector(speed, Functions.ToRotation(moveTo - Position));
                }

            }
            else
            {
                Velocity = Functions.PolarVector(Velocity.Length(), rotation + (float)Math.PI / 2);
                rotation = Functions.SlowRotation(rotation, 0, (float)Math.PI / 30);
                if(rotation == 0)
                {
                    guarding = true;
                }
            }
            if (team == 1)
            {
                if (Miasma.random.Next(5) == 0)
                {
                    new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 2f, Functions.RandomRotation()), Miasma.random.Next(2), 15);
                }

            }
        }
        public override void DeathEffects()
        {
            for (int d = 0; d < 18; d++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2) + (team == 1 ? 0 : 2), 30);
            }
        }
    }
}
