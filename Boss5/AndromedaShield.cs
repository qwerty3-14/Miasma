using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Boss5
{
    public class AndromedaShield : Entity
    {
        TheAndromeda owner = null;
        Vector2 offset;
        public AndromedaShield(Vector2 Position, TheAndromeda owner, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            offset = Position;
            entityID = 40;
            maxHealth = health = 100;
            this.owner = owner;
        }
        byte col = 80;
        public override void MainUpdate()
        {
            if (col > 80)
            {
                col -= 6;
            }
            if (owner != null && Miasma.gameEntities.Contains(owner))
            {
                health = maxHealth;
                Position = owner.Position + Functions.PolarVector(offset.Y, owner.rotation + (float)Math.PI / 2) + Functions.PolarVector(offset.X, owner.rotation);
                rotation = owner.rotation;
            }
            else
            {
                health = 0;
            }

        }
        public override void GetHitEffects(Projectile hitBy)
        {
            if (255 - col < 60)
            {
                col = 255;
            }
            else
            {
                col += 60;
            }
            Sounds.shield.Play();

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, new Color(col, col, col, col), rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
    }
}
