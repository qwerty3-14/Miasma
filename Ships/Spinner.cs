using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Ships
{
    public class Spinner : Ship
    {
        public Spinner(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 26;
            maxHealth = health = 30;
            speed = 4f;
        }
        public override void NormalMovement()
        {
            Velocity = (home - Position);
            if (Velocity.Length() > speed)
            {
                Velocity.Normalize();
                Velocity *= speed;
            }
            rotation += rotationSpeed * (Position.X > 300 ? 1 : -1);
        }
        int maxBeamLength = 60;
        int beamLength = 0;
        int actDirection = 1;
        float rotationSpeed = (float)Math.PI / 60;
        public override void SpecialUpdate()
        {
            if(!IsActing() && beamLength>0 && team != 1)
            {
                beamLength--;
            }
            if(beamLength > 6)
            {
                if(Miasma.random.Next(10)==0)
                {
                    new Particle(Position - Functions.PolarVector(beamLength, rotation), Vector2.Zero, 6, 30);
                }
                if (Miasma.random.Next(10) == 0)
                {
                    new Particle(Position + Functions.PolarVector(beamLength, rotation), Vector2.Zero, 6, 30);
                }
                Sounds.beam.PlayContinuous();

            }
        }
        bool startPlayerAttack = false;
        public override void ActionStart()
        {
            actDirection = (Position.X > 300) ? 1 : -1;
            startPlayerAttack = false;
        }
        public override void ActionUpdate()
        {
            Vector2? collisionAt = null;
            if (Functions.RectangleLineCollision(Miasma.player.Hitbox, Position - Functions.PolarVector(beamLength, rotation), Position + Functions.PolarVector(beamLength, rotation), ref collisionAt) )
            {
                Miasma.player.Strike(10);
                beamLength = 0;
                acting = -1;
                for (int i = 0; i < 20; i++)
                {
                    new Particle((Vector2)collisionAt, Functions.PolarVector((float)Miasma.random.NextDouble() * 4f, Functions.RandomRotation()), 6, 15);
                }
            }
            rotation += rotationSpeed * actDirection;
            if (beamLength < maxBeamLength)
            {
                beamLength++;
            }
            if (startPlayerAttack)
            {
                Velocity = Vector2.UnitX * speed * actDirection;
                if((Position.X < Miasma.leftSide- 50 && actDirection == -1) || ((Position.X > Miasma.rightSide + 50) && actDirection == 1))
                {
                    Position.X = home.X;
                    Position.Y = -50;
                    acting = -1;
                }
            }
            else
            {
                if (Position.Y < Miasma.player.Position.Y - 3 * (maxBeamLength / 4))
                {
                    if (Math.Abs(Position.X - 300) > 250)
                    {
                        Velocity = Vector2.UnitY * speed;
                    }
                    else
                    {
                        Velocity = Vector2.UnitX * speed * actDirection;
                    }
                }
                else
                {
                    actDirection *= -1;
                    startPlayerAttack = true;
                }
            }
        }
        int infectCounter = 0;
        Entity shootAt = null;
        public override void InfectedUpdate()
        {
            if(InfectedTargeting(ref shootAt))
            {
                if ((shootAt.Position - Position).Length() > beamLength / 2)
                {
                    Velocity = Functions.PolarVector(speed, Functions.ToRotation((shootAt.Position - Position)));
                }
                else
                {
                    Velocity = Vector2.Zero;
                }
                rotation += rotationSpeed * (Position.X > 300 ? 1 : -1);
            }
            else
            {
                NormalMovement();
            }
            if (beamLength < maxBeamLength)
            {
                beamLength++;
            }
            for(int e =0; e < Miasma.gameEntities.Count; e++)
            {
                Entity entity = Miasma.gameEntities[e];
                if (entity.maxHealth != -1 && entity.team != team)
                {
                    Vector2? collisionAt = null;
                    if (Functions.RectangleLineCollision(entity.Hitbox, Position - Functions.PolarVector(beamLength, rotation), Position + Functions.PolarVector(beamLength, rotation), ref collisionAt))
                    {
                        entity.Strike(5);
                        for (int i = 0; i < 2; i++)
                        {
                            new Particle((Vector2)collisionAt, Functions.PolarVector((float)Miasma.random.NextDouble() * 4f, Functions.RandomRotation()), 6, 15);
                        }
                    }
                }
            }
            infectCounter++;
            if(infectCounter % 6==0)
            {
                health--;
            }


        }
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            for(int d =1; d <= beamLength; d++)
            {
                spriteBatch.Draw(Miasma.EntityExtras[16], Position + Functions.PolarVector(d, rotation), new Rectangle((d == maxBeamLength ? 1 : 0), 0, 1, 4), Color.White, rotation, new Vector2(.5f, 2f), new Vector2(1f, 1f), SpriteEffects.None, 0);
                spriteBatch.Draw(Miasma.EntityExtras[16], Position + Functions.PolarVector(d, rotation + (float)Math.PI), new Rectangle((d == maxBeamLength ? 1 : 0), 0, 1, 4), Color.White, rotation + (float)Math.PI, new Vector2(.5f, 2f), new Vector2(1f, 1f), SpriteEffects.None, 0);
            }
        }
    }
}
