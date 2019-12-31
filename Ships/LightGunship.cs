using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;

namespace Miasma.Ships
{
    public class LightGunship : Ship
    {
        public LightGunship(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 3;
            maxHealth = health = 10;
        }
        int infectedShotCooldown = 60;
        float flyDirection = 0;
        float infectedFlyGoal = 0;
        protected float aimAt;
        int birthTime = -1;
        public void CarrierLag(int amt)
        {
            acting = 0;
            birthTime = amt;
        }
        public virtual void Shoot(float direction)
        {
            
                new PewPew(Position, Functions.PolarVector(4, direction), team: team);
            
        }
        public override void InfectedUpdate()
        {
            if (birthTime >= 0)
            {
                Velocity = Functions.PolarVector(speed, rotation + (float)Math.PI / 2);
                birthTime--;
            }
            else
            {
                flyDirection = Functions.SlowRotation(flyDirection, infectedFlyGoal, (float)Math.PI / 30);
                Velocity = Functions.PolarVector(speed * .7f, flyDirection);
                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;


                Entity closest = null;

                if (InfectedTargeting(ref closest))
                {
                    aimAt = Functions.ToRotation(closest.Position - Position);

                }

                if (Position.X < Miasma.leftSide + 30)
                {
                    infectedFlyGoal = 0;
                }
                if (Position.X > Miasma.rightSide - 30)
                {
                    infectedFlyGoal = (float)Math.PI;
                }
                if (Position.Y > Miasma.lowerBoundry)
                {
                    infectedFlyGoal = -(float)Math.PI / 2;
                }
                if (Position.Y < 30)
                {
                    infectedFlyGoal = (float)Math.PI / 2;
                }
                if (infectedShotCooldown > 0)
                {
                    infectedShotCooldown--;

                }
                else
                {
                    if (closest != null)
                    {
                        Shoot(aimAt);
                    }
                    infectedFlyGoal = Functions.RandomRotation();
                    health--;
                    infectedShotCooldown = 60;



                }
            }
        }
        float reach = 0;
        Vector2[] FlyHere = new Vector2[5];
        
        int flightIndex = 0;
        public override void ActionStart()
        {
            flightIndex = 0;
            reach = Miasma.random.Next(200, Miasma.lowerBoundry) - home.Y;
            if(home.X > Miasma.leftSide+200)
            {
                FlyHere[0] = new Vector2(Miasma.rightSide - 40, home.Y + reach / 2);
                FlyHere[1] = new Vector2(Miasma.rightSide - 100, home.Y + reach);
                FlyHere[2] = new Vector2(Miasma.rightSide - 200, home.Y + reach);
                FlyHere[3] = new Vector2(Miasma.leftSide + 100, home.Y + reach);
                FlyHere[4] = new Vector2(Miasma.leftSide + 40, home.Y + reach / 2);
                flyDirection = 0;
            }
            else
            {
                FlyHere[4] = new Vector2(Miasma.rightSide - 40, home.Y + reach / 2);
                FlyHere[3] = new Vector2(Miasma.rightSide - 100, home.Y + reach);
                FlyHere[2] = new Vector2(Miasma.rightSide - 200, home.Y + reach);
                FlyHere[1] = new Vector2(Miasma.leftSide + 100, home.Y + reach);
                FlyHere[0] = new Vector2(Miasma.leftSide + 40, home.Y + reach / 2);
                flyDirection = (float)Math.PI;
            }
            
            

        }
        Vector2 flyTo;
        public override void ActionUpdate()
        {
            if (birthTime >= 0)
            {
                Velocity = Functions.PolarVector(speed, rotation + (float)Math.PI/2);
                birthTime--;
                if(birthTime == 0)
                {
                    acting = -1;
                    birthTime = -1;
                }
            }
            else
            {
                aimAt = Functions.ToRotation(Miasma.player.Position - Position);
                if (flightIndex < FlyHere.Length)
                {
                    flyTo = FlyHere[flightIndex];
                    if ((flyTo - Position).Length() < speed)
                    {
                        if (flightIndex != 0 && flightIndex != 4)
                        {
                            Shoot(aimAt);
                        }
                        flightIndex++;
                    }
                }
                else
                {
                    flyTo = home;
                    if ((flyTo - Position).Length() < speed)
                    {
                        acting = -1;
                    }
                }
                flyDirection = Functions.SlowRotation(flyDirection, Functions.ToRotation(flyTo - Position), ((flyTo - Position).Length() < (speed * 60) / (float)Math.PI / 2 ? 30 : 1) * (float)Math.PI / 30);
                Velocity = Functions.PolarVector(speed, flyDirection);
                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
            }
        }
        public override void SpecialUpdate()
        {
            if(team == 0)
            {
                infectedFlyGoal = rotation + (float)Math.PI / 2;
            }
        }
    }
}
