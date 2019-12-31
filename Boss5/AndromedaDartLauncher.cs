using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss5
{
    public class AndromedaDartLauncher : AndromedaTurret
    {
        
        const int defaultOutAmount = -20;
        int[] outAmount = { defaultOutAmount, defaultOutAmount };
        int fire = 0;
        public AndromedaDartLauncher(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
        {
            turnSpeed = (float)Math.PI / 120;
            turretLength = 4f;
            texture = Miasma.EntityExtras[35];
            origin = new Vector2(40f, 26.5f);
        }
        public override void Shoot()
        {
            new AndromedaDart(AbsolutePosition() + Functions.PolarVector( 13 * (fire == 0 ? -1 : 1), AbsoluteRotation() + (float)Math.PI/2) + Functions.PolarVector(outAmount[fire], AbsoluteRotation()), AbsoluteRotation() - (float)Math.PI/2, owner.team);
            outAmount[fire] = defaultOutAmount;
            fire = fire == 0 ? 1 : 0;
        }
        int timer = 0;
        public override void ActingUpdate()
        {
            for(int i =0; i < 2; i++)
            {
                if(outAmount[i]< turretLength)
                {
                    outAmount[i]++;
                }
            }
            timer++;
            if(timer > 0)
            {
                if((timer % 60 ==0 && Miasma.hard) || (timer % 90 == 0 && !Miasma.hard))
                {
                    Shoot();
                }
            }
        }
        public override void InfectingUpdate()
        {
            ActingUpdate();
        }
        public override void OnStart()
        {
            timer = 0;
            for (int i = 0; i < 2; i++)
            {
                outAmount[i] = (int)turretLength;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            for(int i =0; i < 2; i++)
            {
                Texture2D dartTexture = Miasma.EntitySprites[39];
                spriteBatch.Draw(dartTexture, AbsolutePosition() + Functions.PolarVector(13 * (i == 0 ? -1 : 1), AbsoluteRotation() + (float)Math.PI/2) + Functions.PolarVector(outAmount[i], AbsoluteRotation()), null, Color.White, AbsoluteRotation() - (float)Math.PI/2, new Vector2(dartTexture.Width, dartTexture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, AbsolutePosition(), null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
        }
    }
    public class AndromedaDart : Entity
    {
        public AndromedaDart(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            entityID = 39;
            maxHealth = health = 10;
        }
        bool drawLine = false;
        int AimTimer = 0;
        float? AimAt = null;
        public override void MainUpdate()
        {
            if (Miasma.random.Next(2) == 0)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 2f, Functions.RandomRotation()), Miasma.random.Next(2) + (team == 0 ? 2: 0), 15);
            }
            drawLine = false;
            if (AimTimer < 90 && Position.Y > 1000)
            {
                Velocity = Vector2.Zero;
                if (AimAt != null)
                {
                    drawLine = true;
                    AimTimer++;
                    rotation = (float)AimAt;
                }
                else
                {
                    if (team == 1)
                    {
                        float closest = 1200;
                        Entity target = null;
                        foreach (Entity entity in Miasma.gameEntities)
                        {
                            if (entity.Position.Y > 80 && entity.team == 0 && (entity.Position - Position).Length() < closest && entity.maxHealth > 10)
                            {
                                closest = (entity.Position - Position).Length();
                                target = entity;
                            }
                        }
                        if (target != null)
                        {
                            AimAt = Functions.ToRotation((target.Position + Vector2.UnitX * Miasma.random.Next(-35, 36)) - Position) - (float)Math.PI / 2;
                        }
                    }
                    else
                    {
                        AimAt = Functions.ToRotation((Miasma.player.Position + Vector2.UnitX * Miasma.random.Next(-35, 36)) - Position) - (float)Math.PI / 2;
                    }
                }
                
                
                
            }
            else
            {
                Velocity = Functions.PolarVector(12, rotation + (float)Math.PI / 2);
            }
            if(Position.Y <0)
            {
                health = 0;
            }
            foreach (Entity entity in Miasma.gameEntities)
            {
                if (entity.Hitbox.Intersects(Hitbox) && entity.team != team && entity.maxHealth != -1)
                {
                    entity.Strike(10);
                    health = 0;
                    break;
                }
            }
        }
        
        int colorAlternater = 0;
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            if (drawLine)
            {
                colorAlternater++;
                spriteBatch.Draw(Miasma.EntityExtras[18], Position, null, colorAlternater % 20 < 10 ? Color.White : Color.Red, rotation + (float)Math.PI / 2, new Vector2(0, .5f), new Vector2(1, 1), SpriteEffects.None, 0);
            }
        }
        public override void DeathEffects()
        {
            for (int d = 0; d < 18; d++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2) + (team == 1 ? 0 : 2), 30);
            }
            Sounds.kaboom.Play();
        }

    }
}
