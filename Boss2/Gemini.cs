using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss2
{
    public class Gemini : Entity
    {
        GeminiTurret turret;
        public static float speed = 1f;
        public static float borderOffset = 50;
        public static float turnAroundSpeedModifier = 120;
        bool raged = false;
        public void Enrage()
        {
            raged = true;
        }
        public Gemini(Vector2 Position,  float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            entityID = 16;
            health = maxHealth = 300;
            Velocity = Functions.PolarVector(speed, rotation);
            turret = new GeminiTurret(this, new Vector2(-4, 0));
        }
        int counter = 20000;
        
        int bombModeTimer = -1;
        int shotDelay = 5;
        Vector2 aimCenter = Vector2.Zero;
        int offset = 50;
        bool startRight = true;
        int cooldown = 180;
        public override void MainUpdate()
        {
            if (team == 1)
            {
                health = 0;
            }
            counter++;

            turret.UpdateRelativePosition();



            if (Velocity.X > -speed && Position.X > Miasma.rightSide - borderOffset)
            {
                rotation += (float)Math.PI / turnAroundSpeedModifier;
            }
            if (Velocity.X < speed && Position.X < Miasma.leftSide + borderOffset)
            {
                rotation += (float)Math.PI / turnAroundSpeedModifier;
            }
            Velocity = Functions.PolarVector(speed, rotation);

            if (Position.X > 150 && Position.X < 450)
            {
                if (Position.Y > 50 + (Gemini.speed * Gemini.turnAroundSpeedModifier) / (float)Math.PI)
                {
                    
                    if(counter > 120 - (Miasma.hard ? 30 : 0))
                    {
                        counter = 0;
                    }
                    else if(counter > 60)
                    {
                        turret.AimTowardAbsolute(Miasma.player.Position + (Vector2.UnitX * 50));
                    }
                    else if (counter % shotDelay == 0)
                    {
                        turret.Shoot();
                    }
                    turret.LaunchBomb();
                }
                else
                {
                    turret.AimTowardAbsolute(-(float)Math.PI / 2);
                    turret.UpdateBombPosition();


                    if (counter % (Miasma.hard ? 60 : 120) == 0)
                    {
                        turret.MakeBomb();
                    }
                    if (counter % (Miasma.hard ? 60 : 120) == (Miasma.hard ? 30 : 6))
                    {
                        turret.LaunchBomb();
                    }

                }
            }

        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            turret.Draw(spriteBatch);
        }
        private class GeminiTurret : Turret
        {
            int frame = 0;
            Bomb myBomb = null;
            public GeminiTurret(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI / 60;
                turretLength = 29;
                texture = Miasma.EntityExtras[4];
                origin = new Vector2(15, 9);
            }
            public override void Shoot()
            {
                new PewPew(AbsolutePosition() + Functions.PolarVector(turretLength, AbsoluteRotation()), Functions.PolarVector(4, AbsoluteRotation()), team: owner.team);
                frame = (frame == 0 ? 1 : 0);
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, AbsolutePosition(), new Rectangle(0, frame* texture.Height/2, texture.Width, texture.Height / 2), Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
            }
            public void MakeBomb()
            {
                myBomb = new Bomb(AbsolutePosition());
            }
            public void UpdateBombPosition()
            {
                if (myBomb != null)
                {
                    myBomb.Position = AbsolutePosition() + Functions.PolarVector(-15, AbsoluteRotation());
                }
                    
            }
            public void LaunchBomb()
            {
                if(myBomb != null)
                {
                    myBomb.Launch();
                    myBomb = null;
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
