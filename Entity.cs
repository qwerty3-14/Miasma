using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma
{
    public abstract class Entity
    {
        public float rotation = 0f;
        public Rectangle Hitbox;
        public Vector2 Position = Vector2.Zero;
        public Vector2 Velocity = Vector2.Zero;
        public int team;
        public int maxHealth = 10;
        public int health = 10;
        public int entityID = -1;
        public Entity(Vector2 Position, float rotation = 0f, int team = 0)
        {
            this.Position = Position;
            this.rotation = rotation;
            this.team = team;
            Miasma.gameEntities.Add(this);
        }
        public virtual void Physics()
        {

            Position += Velocity;
            if(health ==0)
            {
                DeathEffects();
                Miasma.gameEntities.Remove(this);
            }
            
        }
        public virtual void UpdateHitbox()
        {
            Point size = new Point(Miasma.EntitySprites[entityID].Width, Miasma.EntitySprites[entityID].Height);
            Hitbox = new Rectangle((int)(Position.X - size.X/2), (int)(Position.Y - size.Y/2), size.X, size.Y);
            

        }
       
        public virtual void MainUpdate()
        {

        }
        public virtual void SpecialUpdate()
        {

        }
        public void Strike(int damage, bool Miasamic = false)
        {
            for (int c = 0; c < damage; c++)
            {
                new Strike(Position, Vector2.Zero, rotation, team == 0 ? 1 : 0, Miasamic);
            }
        }
        public virtual void DeathEffects()
        {

        }
        public virtual void GetHitEffects( Projectile hitBy)
        {

        }
        public virtual void PreDraw(SpriteBatch spriteBatch)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public virtual void PostDraw(SpriteBatch spriteBatch)
        {

        }
        public Vector2 getVelocity()
        {
            return Velocity;
        }
        public override string ToString()
        {
            return "[Position: " + Position + "] [Velocity: "+ Velocity +"] [rotation: ]";
        }
    }
}
