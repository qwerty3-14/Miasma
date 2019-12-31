using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Projectiles
{
    public class HelixSegment : Projectile
    {
        bool red = false;
        int layer = 0;
        Vector2? linkTo = null;
        public HelixSegment(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0, bool red = false, int layer = 0, Vector2? linkTo = null) : base(Position, velocity, rotation, team)
        {
            entityID = 18;
            maxHealth = -1;
            health = 2;
            this.red = red;
            this.layer = layer;
            timeLeft = 360;
            this.linkTo = linkTo;
        }
        void drawStep(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            int size = 3;


            if (linkTo == null)
            {
                spriteBatch.Draw(texture, Position, new Rectangle(0, size * (red ? 1 : 0), size, size), Color.White, rotation, new Vector2(size, size) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            }
            else
            {
                float distance = ((Vector2)linkTo - Position).Length();
                float direction = Functions.ToRotation(((Vector2)linkTo - Position));
                spriteBatch.Draw(texture, Position, new Rectangle(0, size * (red ? 1 : 0), size-1, size), Color.White, direction, new Vector2(1, size*.5f), new Vector2(1, 1), SpriteEffects.None, 0);
                for (int i = 0; i < distance; i++)
                {
                    spriteBatch.Draw(texture, Position + Functions.PolarVector(i, direction), new Rectangle(1, size * (red ? 1 : 0), size - 2, size), Color.White, direction, new Vector2(0, size * .5f) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
                }
                spriteBatch.Draw(texture, (Vector2)linkTo, new Rectangle(1, size * (red ? 1 : 0), size-1, size), Color.White, direction, new Vector2(0, size * .5f) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            }

        }
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            if(layer < 0)
            {
                drawStep(spriteBatch);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (layer == 0)
            {
                drawStep(spriteBatch);
            }

        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            if (layer > 0)
            {
                drawStep(spriteBatch);
            }
        }
    }
}
