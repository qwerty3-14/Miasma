using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Boss3
{
    public class Missile : Entity
    {
        public Missile(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            health = maxHealth = 10;
            entityID = 24;
            this.Velocity = velocity;
            Sounds.launchMisc.Play();
        }
        int counter = 0;
        const int preWingDeployTime = 30;
        const int wingDeployTime = 30;
        int blastRadius = 30;
        bool justInfected = true;
        public override void SpecialUpdate()
        {
            if(team == 1 && justInfected)
            {
                rotation += (float)Math.PI;
                justInfected = false;
            }
            counter++;
            Velocity = Functions.PolarVector(4, rotation);
            if (counter > preWingDeployTime + wingDeployTime)
            {
                Vector2 aimAt = new Vector2(0, 0);
                if (team == 1)
                {
                    if (Miasma.BossIsActive())
                    {
                        aimAt = Miasma.boss.Position;
                    }
                }
                else
                {
                    aimAt = Miasma.player.Position;
                }
                rotation = Functions.SlowRotation(rotation, Functions.ToRotation(aimAt - Position), (float)Math.PI / 240);
            }
            if (team == 1)
            {
                if (Miasma.random.Next(5) == 0)
                {
                    new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 2f, Functions.RandomRotation()), Miasma.random.Next(2), 15);
                }

            }
            bool explode = false;
            for (int i = 0; i < Miasma.gameEntities.Count; i++)
            {
                if (Miasma.gameEntities[i].maxHealth != -1 && Miasma.gameEntities[i].Hitbox.Intersects(Hitbox) && team != Miasma.gameEntities[i].team)
                {
                    explode = true;
                }
            }
            if (explode)
            {
                for (int i = 0; i < Miasma.gameEntities.Count; i++)
                {
                    if (Miasma.gameEntities[i].maxHealth != -1 && Miasma.gameEntities[i].Hitbox.Intersects(new Rectangle((int)Position.X - blastRadius, (int)Position.Y - blastRadius, blastRadius*2, blastRadius*2)) && team != Miasma.gameEntities[i].team)
                    {
                        Miasma.gameEntities[i].Strike(10);
                    }
                }
                health = 0;
            }
            if(Position.Y > 850)
            {
                health = 0;
            }
        }
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntityExtras[14];
            if (counter > preWingDeployTime)
            {
                float factor = Math.Min((float)(counter - preWingDeployTime) / wingDeployTime, 1f);
                spriteBatch.Draw(texture, Position + Functions.PolarVector(3 * factor, rotation - (float)Math.PI / 2), new Rectangle(0, 0, 9, 3), Color.White, rotation, new Vector2(9, 3) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
                spriteBatch.Draw(texture, Position + Functions.PolarVector(3 * factor, rotation + (float)Math.PI / 2), new Rectangle(0, 6, 9, 3), Color.White, rotation, new Vector2(9, 3) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
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
