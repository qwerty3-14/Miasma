using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;

namespace Miasma.Boss4
{
    public class ArmTip : Entity
    {
        bool deactivated = false;
        bool activated = false;
        Jupiter parent;
        Vector2 goTo;
        float speed = 4f;
        int shotTime = 120;
        public ArmTip(Jupiter parent, Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            if(Miasma.hard)
            {
                shotTime = 60;
            }
            entityID = 31;
            health = maxHealth = 10;
            this.parent = parent;
            pickSpot();
        }
        void pickSpot()
        {
            goTo = new Vector2(120 + Miasma.random.Next(361), 500 + Miasma.random.Next(241));
        }
        public void Activate()
        {
            activated = true;
            
        }
        public void Deactivate()
        {
            deactivated = true;
            health = 0;
        }
        int chargeTime = 0;
        
        public override void MainUpdate()
        {
            if (!Miasma.BossIsActive())
            {
                health = 0;
            }
            if (activated)
            {
                Velocity = (goTo - Position);
                if (Velocity.Length() > speed)
                {
                    Velocity.Normalize();
                    Velocity *= speed;
                    chargeTime = 0;
                }
                else if(Velocity.Length() == 0)
                {
                    chargeTime++;
                    if (chargeTime < shotTime)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            float r = Functions.RandomRotation() * -.5f;
                            new Particle(Position + Vector2.UnitY * -12f + Functions.PolarVector(15, r), Functions.PolarVector(-1, r), 7, 15);
                        }
                    }
                    else if (chargeTime == shotTime)
                    {
                        new LightningBolt(Position + Vector2.UnitY * -12f, Vector2.UnitY * -10, 0, team);
                    }
                    else if (chargeTime > shotTime + 60)
                    {
                        pickSpot();
                    }
                }
                
            }
            else
            {
                Velocity = Vector2.Zero;
            }
            if(team == 1)
            {
                health = 0;
            }
        }
        public override void DeathEffects()
        {
            float direction = Functions.ToRotation(Position - parent.CoreCenter());
            float vel = 8;
            
            
            if (deactivated)
            {
                new ArmTipDebris(Position, Functions.PolarVector(vel, direction), rotation, team);
            }
            else
            {
                parent.arms.Add(new ArmTipDebris(Position, Functions.PolarVector(-vel, direction), rotation, 1));
            }
        }
    }
    public class ArmTipDebris : Projectile
    {
        public ArmTipDebris(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0) : base(Position, velocity, rotation, team)
        {
            maxHealth = -1;
            health = 20;
            entityID = 32;
        }
        public override void DeathEffects()
        {
            for (int d = 0; d < 18; d++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2) +2, 30);
            }
        }
    }
}
