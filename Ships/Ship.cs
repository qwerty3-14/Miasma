using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Upgrades;
using Microsoft.Xna.Framework;

namespace Miasma.Ships
{
    public abstract class Ship : Entity
    {
        protected int acting = -1;
        protected float speed = 3f;
        public Vector2 home;
        public static float trigCounter = 0f;
        public Ship(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            home = Home;
        }
        bool canDespawn = false;
        public void Flee()
        {
            if (team == 0)
            {
                acting = -1;
                home.X = Position.X > 300 ? 600 : 0;
                canDespawn = true;

                Miasma.enemyFleet.Remove(this);
            }
        }
        public virtual void NormalMovement()
        {
            Velocity = (home - Position);
            if (Velocity.Length() > speed)
            {

                Velocity.Normalize();
                Velocity *= speed;
                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
            }
            else
            {
                rotation = Functions.SlowRotation(rotation, Miasma.player.Position.Y > Position.Y ? 0 : (float)Math.PI, (float)Math.PI / 30);
            }
        }
        public bool InfectedTargeting(ref Entity shootAt)
        {
            Entity closest = null;
            float distance = 1000;
            if (Miasma.boss != null)
            {
                closest = Miasma.boss;
            }
            else
            {
                bool spartans = false;

                foreach (Ship ship in Miasma.enemyFleet)
                {
                    if(ship is Spartan && ship.Position.Y > Position.Y )
                    {
                        spartans = true;
                        break;
                    }
                    if ((ship.Position - Position).Length() < distance)
                    {
                        distance = (ship.Position - Position).Length();
                        closest = ship;
                    }
                }
                if(spartans)
                {
                    distance = 1000;
                    foreach (Ship ship in Miasma.enemyFleet)
                    {
                        
                        if (ship is Spartan && ship.Position.Y > Position.Y && (ship.Position - Position).Length() < distance)
                        {
                            distance = (ship.Position - Position).Length();
                            closest = ship;
                        }
                    }
                }
            }
            shootAt = closest;
            return shootAt != null;
        }
        public override void MainUpdate()
        {
            if(canDespawn)
            {
                NormalMovement();
                acting = -1;
                home.X = Position.X > 300 ? 600 : 0;
                if(Position.X < 20 || Position.X > 580)
                {
                    health = 0;
                }
            }
            else if(!Miasma.BossIsActive())
            {
                home.X += (float)Math.Cos(trigCounter) * .2f;
            }
            if (team ==1)
            {
                InfectedUpdate();
                if(Miasma.random.Next(5)==0)
                {
                    new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 2f, Functions.RandomRotation()), Miasma.random.Next(2), 15);
                }
                foreach (Upgrade upgrade in Miasma.player.upgrades)
                {
                    if (upgrade != null)
                    {
                        upgrade.InfectedShipUpdate(this);
                    }
                }

            }
            else
            {
                if (acting == 0)
                {
                    
                    ActionUpdate();
                }
                else
                {
                    
                    if (acting > 0)
                    {
                        if(acting == 1)
                        {
                            ActionStart();
                        }
                        acting--;
                    }
                    NormalMovement();



                }
            }
            
        }
        public void Act(int delay)
        {
            acting = delay;
        }
        public override void DeathEffects()
        {
            for (int d = 0; d < 18; d++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2) + (team == 1 ? 0 : 2), 30);
            }
            Sounds.kaboom.Play();
            foreach (Upgrade upgrade in Miasma.player.upgrades)
            {
                if (upgrade != null)
                {
                    upgrade.OnDeath(this);
                }
            }


            ExtraDeathEffects();
        }
        public virtual void ExtraDeathEffects()
        {

        }
        public virtual void ActionStart()
        {

        }
        public virtual void ActionUpdate()
        {

        }
        public virtual void InfectedUpdate()
        {

        }
        
        public bool IsActing()
        {
            return acting == 0;
        }
        public void stopActing()
        {
            acting = -1;
        }
    }
}
