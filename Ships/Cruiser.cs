using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;

namespace Miasma.Ships
{
    public class Cruiser : Ship
    {
        public Cruiser(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 36;
            health = maxHealth = 60;
        }
        public override void SpecialUpdate()
        {
            if(Velocity.Length()>1f)
            {
                new Particle(Position + Functions.PolarVector(9, rotation) + Functions.PolarVector(-11.5f, rotation + (float)Math.PI/2), Vector2.Zero, 8, 30);
                new Particle(Position + Functions.PolarVector(-9, rotation) + Functions.PolarVector(-11.5f, rotation + (float)Math.PI / 2), Vector2.Zero, 8, 30);
            }
        }
        bool shooting = false;
        public override void ActionStart()
        {
            shooting = false;
            shootTimer = 0;
        }
        int shootTimer = 0;
        void Shoot()
        {
            shootTimer++;
            if (shootTimer % 4 == 0)
            {
                if(team ==1)
                {
                    health-=2;
                    if(health < 0)
                    {
                        health = 0;
                    }
                }
                if (shootTimer % 8 == 0)
                {
                    new PewPew(Position + Functions.PolarVector(9, rotation) + Functions.PolarVector(14f, rotation + (float)Math.PI / 2), Functions.PolarVector(5f, rotation + (float)Math.PI / 2), rotation, team);
                }
                else
                {
                    new PewPew(Position + Functions.PolarVector(-9, rotation) + Functions.PolarVector(14f, rotation + (float)Math.PI / 2), Functions.PolarVector(5f, rotation + (float)Math.PI / 2), rotation, team);
                }
            }
        }
        public override void ActionUpdate()
        {
            if(shooting)
            {
                if (shootTimer > 120)
                {
                    NormalMovement();
                    if(rotation == 0)
                    {
                        acting = -1;
                    }
                }
                else
                {
                    Velocity = Vector2.Zero;
                    Shoot();
                }
               
            }
            else
            {
                Velocity = Functions.PolarVector(speed, rotation + (float)Math.PI / 2);
                rotation = Functions.SlowRotation(rotation, Functions.ToRotation(Miasma.player.Position - Position) - (float)Math.PI/2, (float)Math.PI / 240);
                if((Miasma.player.Position - Position).Length() < 200)
                {
                    shooting = true;
                }
            }
        }
        Entity target;
        public override void InfectedUpdate()
        {

            if(InfectedTargeting(ref target))
            {
                
                Velocity = Functions.PolarVector(speed, rotation + (float)Math.PI / 2);
                rotation = Functions.SlowRotation(rotation, Functions.ToRotation(target.Position - Position) - (float)Math.PI / 2, (float)Math.PI / 240);
                if ((target.Position - Position).Length() < 200)
                {
                    rotation = Functions.SlowRotation(rotation, Functions.ToRotation(target.Position - Position) - (float)Math.PI / 2, (float)Math.PI / 60);
                    Velocity = Vector2.Zero;
                    Shoot();
                }
            }
           
        }
    }
}
