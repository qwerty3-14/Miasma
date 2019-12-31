using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Miasma.Ships;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss1
{
    public class ShieldedCarrier : Boss
    {
        public ShieldedCarrier(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            health = maxHealth = 300;
            entityID = 9;
            new Shield(Position, this);
            name = "The Phalax";
        }
        void Shoot()
        {
            new PewPew(Position + new Vector2(24, 12), Vector2.UnitY * 5);
            new PewPew(Position + new Vector2(-24, 12), Vector2.UnitY * 5);
        }
        float[] horizontalPoints = { 130, 215, 300, 385, 470};
        int flightIndex = 2;
        float front = 300;
        float back = 200;
        Vector2 flyTo = new Vector2(300, 300);
        int launchCooldown = 120;
        int switchCooldown = 120;
        float speed = 1.5f;
        int fireRate = 0;
        public override void SpecialUpdate()
        {
            Miasma.currentWaveIntensity = (Miasma.hard ? 4 : 1);
            flyTo.X = horizontalPoints[flightIndex];
            if (Miasma.enemyFleet.Count <= 0)
            {
                flyTo.Y = back;
            }
            else
            {
                if(Position.X == flyTo.X)
                {
                    switchCooldown--;
                    if (switchCooldown <= 0)
                    {

                        switchCooldown = 120;
                        if (flightIndex == 0)
                        {
                            flightIndex = 1;
                        }
                        else if (flightIndex == 4)
                        {
                            flightIndex = 3;
                        }
                        else
                        {
                            flightIndex = flightIndex + (Miasma.player.Position.X < Position.X ? -1 : 1);
                        }
                    }
                }
                else
                {
                    fireRate++;
                    if(fireRate % 10 ==0)
                    {
                        Shoot();
                    }
                    
                }
            }
            
            Vector2 difference = flyTo - Position;
            
            if (difference.Length() < speed)
            {
                Position = flyTo;
                Velocity = Vector2.Zero;
                if (Position.Y == back)
                {
                    launchCooldown--;
                    if (launchCooldown == 0)
                    {


                        foreach (float f in horizontalPoints)
                        {
                            Sounds.carierLaunch.Play();
                            if (f > Position.X)
                            {
                                Miasma.enemyFleet.Add(new LightArtillary(Position, new Vector2(f, Position.Y)));
                            }
                            else if (f < Position.X)
                            {
                                Miasma.enemyFleet.Add(new LightArtillary(Position, new Vector2(f, Position.Y), (float)Math.PI));
                            }

                        }
                    }
                    else if(launchCooldown < -60)
                    {
                        launchCooldown = 120;
                        flyTo.Y = front;
                    }
                    
                }
            }
            else
            {
                Velocity = Functions.PolarVector(speed, Functions.ToRotation(difference));
            }

        }
        
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntityExtras[3];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
    }
}
