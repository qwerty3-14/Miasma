using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Boss4
{
    public abstract class Limb
    {
        protected float rotation;
        protected Vector2 Position;
        protected Vector2 origin;
        protected Texture2D texture;
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public virtual void Action()
        {

        }
        public virtual bool IsOff()
        {
            return false;
        }
        public virtual void Rotate(float rotation)
        {
            this.rotation = rotation;
        }
        public float GetRotation()
        {
            return rotation;
        }
        public virtual void MoveTo(Vector2 here)
        {
            Position = here;
        }
        public Vector2 GetPosition()
        {
            return Position;
        }
        public virtual void Action2()
        {

        }
    }
    public class LimbSegment : Limb
    {
        Limb attached;
        float length = 46;
        public LimbSegment(Vector2 Position, Limb attached)
        {
            origin = new Vector2(4, 4);
            texture = Miasma.EntityExtras[24];
            this.Position = Position;
            this.attached = attached;
            attached.MoveTo(Position + Functions.PolarVector(length, rotation));
        }
        public override bool IsOff()
        {
            return attached.IsOff();
        }
        public override void Action()
        {
            attached.Action();
        }
        public override void Rotate(float rotation)
        {
            origin = new Vector2(4, 4);
            float dif = attached.GetRotation() - this.rotation;

            attached.Rotate(rotation + dif);
            attached.MoveTo(Position + Functions.PolarVector(length, rotation));
            base.Rotate(rotation);
        }
        public override void MoveTo(Vector2 here)
        {
            attached.MoveTo(here + Functions.PolarVector(length, rotation));
            base.MoveTo(here);
        }
        public void Straighten()
        {
            
            attached.Rotate(Functions.SlowRotation(attached.GetRotation(), rotation, (float)Math.PI / 60));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            
            base.Draw(spriteBatch);
            attached.Draw(spriteBatch);
        }
        public override void Action2()
        {
            attached.Action2();
        }
    }
    public class Hand : Limb
    {
        BallLightning ball;
        bool off = false;
        public Hand(BallLightning ball)
        {
            origin = new Vector2(6.5f, 9f);
            texture = Miasma.EntityExtras[25];
            this.ball = ball;
            ball.putInHand(this);
        }
        public override void Action()
        {
            if (!off)
            {
                float before = rotation;
                rotation = Functions.SlowRotation(rotation, Functions.ToRotation(Miasma.player.Position - Position), (float)Math.PI / 60);
                if (Math.Abs(before - rotation) < (float)Math.PI / 120)
                {
                    off = true;
                    ball.Launch(rotation);
                }
            }
        }
        public override bool IsOff()
        {
            return off;
        }
        public override void Action2()
        {
            ball = new BallLightning(Position);
            ball.putInHand(this);
            off = false;

        }

    }
    public class BallLightning : Entity
    {
        public bool launched = true;
        Hand beholder;
        float angularVelocity = 0;
        int beamLength = 0;
        int maxBeamLength = 50;
        Vector2[] beamTips = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public BallLightning( Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            if(Miasma.hard)
            {
                maxBeamLength = 100;
            }
            entityID = 34;
            maxHealth = health = 10;
            
        }
        
        public void putInHand(Hand beholder)
        {
            this.beholder = beholder;
            launched = false;
        }
        public override void MainUpdate()
        {
            if(!Miasma.BossIsActive())
            {
                health = 0;
            }
            rotation += angularVelocity;
            for(int b =0; b < beamTips.Length; b++)
            {
                beamTips[b] = Position + Functions.PolarVector(beamLength, rotation + b * (float)Math.PI / 2);
            }
            if(!launched)
            {
                Position = beholder.GetPosition() + Functions.PolarVector(10f, beholder.GetRotation());
                Velocity = Vector2.Zero;
            }
            else 
            {
                if (beamLength < maxBeamLength)
                {
                    beamLength+=maxBeamLength/50;
                }
                if(beamLength > 6)
                {
                    Sounds.beamLighting.PlayContinuous();
                    foreach (Vector2 tip in beamTips)
                    {
                        if (Miasma.random.Next(10) == 0)
                        {
                            new Particle(tip, Vector2.Zero, 7, 30);
                        }
                        Vector2? collisionAt = null;
                        if (Functions.RectangleLineCollision(Miasma.player.Hitbox, Position,tip, ref collisionAt))
                        {
                            Miasma.player.Strike(10);
                            beamLength = 0;
                            for (int i = 0; i < 20; i++)
                            {
                                new Particle((Vector2)collisionAt, Functions.PolarVector((float)Miasma.random.NextDouble() * 4f, Functions.RandomRotation()), 7, 15);
                            }
                        }
                    }
                }
            }
            if(Position.Y < -50)
            {
                health = 0;
            }
        }
        public void Launch(float direction)
        {
            launched = true;
            Velocity = Functions.PolarVector(4, direction);
            angularVelocity = (float)Math.PI / 60 * (Position.X > 300 ? -1 : 1);
        }
        int animateTimer = 0;
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            animateTimer++;
            foreach (Vector2 tip in beamTips)
            {
                float direction = Functions.ToRotation(tip - Position);
                for (int d = 0; d < beamLength; d++)
                {
                    spriteBatch.Draw(Miasma.EntityExtras[26], Position + Functions.PolarVector(d, direction), new Rectangle((d+animateTimer) % 7, 0, 1, 4), Color.White, direction, new Vector2(.5f, 2f), new Vector2(1f, 1f), SpriteEffects.None, 0);
                }
            }
        }
    }
}
