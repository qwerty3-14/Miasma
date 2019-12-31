using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Boss4
{
    public class LightningBolt : Projectile
    {
        public LightningBolt(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0) : base(Position, velocity, rotation, team)
        {
            entityID = 33;
            maxHealth = -1;
            health = 10;
            Sounds.zap.Play();
        }
        int frame = 0;
        int frameTimer = 0;
        public override void SpecialUpdate()
        {
            frameTimer++;
            if(frameTimer % 5 ==0)
            {
                frame++;
                if(frame >= 2)
                {
                    frame = 0;
                }
            }
            rotation = Functions.ToRotation(Velocity) + (float)Math.PI / 2;
            if (Miasma.random.Next(3) == 0)
            {
                new Particle(Position + Vector2.UnitX * Miasma.random.Next(-4, 5), Vector2.Zero, 7, 6);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, new Rectangle(0, frame * texture.Height/2, texture.Width, texture.Height / 2), Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public override void UpdateHitbox()
        {
            Point size = new Point(Miasma.EntitySprites[entityID].Width, Miasma.EntitySprites[entityID].Height/2);
            Hitbox = new Rectangle((int)(Position.X - size.X / 2), (int)(Position.Y - size.Y / 2), size.X, size.Y);


        }
    }
}
