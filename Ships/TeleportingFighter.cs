using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Ships
{
    class TeleportingFighter : Ship
    {
        public TeleportingFighter(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 11;
            maxHealth = health = 10;
        }
        float scale = 1f;
        int actMode = 0;
        int teleports = 0;
        public virtual void Shoot(float direction)
        {

            new PewPew(Position, Functions.PolarVector(4, direction), team: team);
            
        }
        public override void ActionStart()
        {
            teleports = 0;
            actMode = 0;
        }
        void Teleport()
        {
            if(teleports>=2 && team ==0)
            {
                Position = home;
                rotation = 0;
            }
            else
            {
                Position = new Vector2(Miasma.random.Next(Miasma.leftSide + 50, Miasma.rightSide - 50), Miasma.random.Next(200, Miasma.lowerBoundry));
                Vector2 pointToward = new Vector2(Miasma.random.Next(Miasma.leftSide + 80, Miasma.rightSide - 30), Miasma.random.Next(230, Miasma.lowerBoundry - 30));
                rotation = Functions.ToRotation(pointToward - Position) - (float)Math.PI / 2;
            }
            
            
        }
        int dashCounter = 0;
        public override void ActionUpdate()
        {
            
            switch(actMode)
            {
                case 0:
                    Velocity = Vector2.Zero;
                    if (scale > 0f)
                    {
                        scale -= 1f / 15f;
                    }
                    else
                    {
                        scale = 0;
                        Teleport();
                        actMode = 1;
                    }
                    break;
                case 1:
                    Velocity = Vector2.Zero;
                    if (scale < 1f)
                    {
                        scale += 1f / 15f;
                    }
                    else
                    {
                        scale = 1f;
                        if (teleports >= 2 && team ==0)
                        {
                            acting = -1;
                            actMode = 0;
                        }
                        else
                        {
                            actMode = 2;
                        }
                         
                    }
                    break;
                case 2:
                    Velocity = Functions.PolarVector(speed, rotation + (float)Math.PI / 2);
                    dashCounter++;
                    if(dashCounter % 28 ==0)
                    {
                        Vector2 aimHere = Miasma.player.Position;
                        Entity ShootAt = null;
                        if(team ==1)
                        {
                            InfectedTargeting(ref ShootAt);
                            
                        }
                        
                        if(ShootAt != null)
                        {
                            aimHere = ShootAt.Position;
                        }
                        Shoot(Functions.ToRotation(aimHere - Position));
                    }
                    if(dashCounter>60)
                    {
                        dashCounter = 0;
                        actMode = 0;
                        teleports++;
                        if (team == 1)
                        {
                            health-=2;
                        }
                    }
                    break;

            }
            
        }
        public override void InfectedUpdate()
        {
            ActionUpdate();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(scale, scale), SpriteEffects.None, 0);
        }

    }
}
