using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma
{
    public abstract class Turret 
    {
        protected Entity owner;
        protected Vector2 relativePosition;
        protected Vector2 nativePosition;

        protected float rotation = 0f;

        protected float turnSpeed = (float)Math.PI / 60;
        protected float turretLength = 6;
        protected Texture2D texture = Miasma.EntityExtras[2];
        protected Vector2 origin = new Vector2(5, 5) * .5f;
        protected float[] savedPositions;
        public Turret(Entity owner, Vector2 relativePosition, float rotation = 0f, float[] savedPositions = null)
        {
            this.owner = owner;
            this.nativePosition = relativePosition;
            this.rotation = rotation;
            this.savedPositions = savedPositions;
        }
        public virtual void Shoot()
        {
            new PewPew(AbsoluteShootPosition(), Functions.PolarVector(4, AbsoluteRotation()), team: owner.team);
        }
        public bool AimToward(float here)
        {
            float old = rotation;
            rotation = Functions.SlowRotation(rotation, here, turnSpeed);
            return rotation == old;
        }
        public bool AimTowardSavedPosition(int index)
        {
            if(savedPositions != null && index < savedPositions.Length)
            {
                return AimToward(savedPositions[index]);
            }
            return false;
        }
        public void AimTowardAbsolute(float here)
        {
            rotation = Functions.SlowRotation(rotation , here - owner.rotation, turnSpeed);
        }
        public void AimTowardAbsolute(Vector2 here)
        {
            rotation = Functions.SlowRotation(rotation, Functions.ToRotation(here - AbsolutePosition())-owner.rotation, turnSpeed);
        }
        public virtual void UpdateRelativePosition(Vector2? move = null)
        {
            if(move != null)
            {
                nativePosition = (Vector2)move;
            }
            relativePosition = Functions.PolarVector(nativePosition.X, owner.rotation) + Functions.PolarVector(nativePosition.Y, owner.rotation + (float)Math.PI / 2);
        }
        
        public float AbsoluteRotation()
        {
            return rotation + owner.rotation;
        }
        public Vector2 AbsolutePosition()
        {
            return relativePosition + owner.Position;
        }
        public virtual Vector2 AbsoluteShootPosition()
        {
            return AbsolutePosition() + Functions.PolarVector(turretLength, AbsoluteRotation());
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, AbsolutePosition(), null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
        }

    }
}
