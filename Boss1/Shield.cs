using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss1
{
    public class Shield : Entity
    {
        ShieldedCarrier owner = null;
        public Shield(Vector2 Position, ShieldedCarrier owner, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            entityID = 10;
            maxHealth = health = 100;
            this.owner = owner;
        }
        byte col = 80;
        public override void MainUpdate()
        {
            if(col > 80)
            {
                col -= 6;
            }
            if(owner != null && Miasma.gameEntities.Contains(owner))
            {
                health = maxHealth;
                Position = owner.Position + Functions.PolarVector(35, owner.rotation + (float)Math.PI / 2);
                rotation = owner.rotation;
            }
            else
            {
                health = 0;
            }
            
        }
        public override void GetHitEffects(Projectile hitBy)
        {
            if( 255 - col < 60)
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
