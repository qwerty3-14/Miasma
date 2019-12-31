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
    public class LightCharger : Ship
    {
        public LightCharger(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            speed = 5f;
            entityID = 4;
            maxHealth = health = 10;
        }
        bool drawLine = false;
        int countdown = 120;
        int infectedCountdown = 120;
        bool strike = true;
        List<Ship> infectedStrikes = new List<Ship>();
        public override void SpecialUpdate()
        {
            if(!IsActing())
            {
                drawLine = false;
            }
        }
        protected void ResetStrikes()
        {
            infectedStrikes.Clear();
        }
        public override void InfectedUpdate()
        {
            infectedCountdown--;
            if(Position.X > Miasma.rightSide+100 || Position.X < 0 || Position.Y > 800 || Position.Y < 0)
            {
                OffScreen();
            }
            else if(infectedCountdown <= 0)
            {
                Velocity = Functions.PolarVector(12, rotation + (float)Math.PI / 2);
                foreach(Ship ship in Miasma.enemyFleet)
                {
                    if(ship.Hitbox.Intersects(Hitbox) && !infectedStrikes.Contains(ship))
                    {
                        infectedStrikes.Add(ship);
                        ship.Strike(10);

                    }
                }
            }
            else
            {
                Velocity = Vector2.Zero;
                float shootAt = (float)Math.PI / 2;
                Entity closest = null;
                
                if (InfectedTargeting(ref closest))
                {
                    shootAt = Functions.ToRotation(closest.Position - Position);
                    rotation = shootAt - (float)Math.PI / 2;
                }
            }
            
        }
        public override void ActionStart()
        {
            countdown = 120;
            strike = true;
        }
        public override void ActionUpdate()
        { 
            drawLine = false;
            countdown--;
            if (countdown <= 0)
            {
                Velocity = Functions.PolarVector(12, rotation + (float)Math.PI / 2);
                
            }
            if(countdown>60)
            {
                Velocity = Vector2.Zero;
                rotation += (float)Math.PI / 30;
            }
            else if (countdown>0)
            {
                drawLine = true;
                rotation = Functions.ToRotation(Miasma.player.Position - Position) - (float)Math.PI / 2;
            }
            if(Miasma.player.Hitbox.Intersects(Hitbox) && strike)
            {
                strike = false;
                Miasma.player.Strike(10);


            }
            if (Position.Y > 800)
            {
                OffScreen();
                
            }
        }
        public virtual void OffScreen()
        {
            if(team ==0)
            {
                Position.Y = 0;
                acting = -1;
            }
            if(team ==1)
            {
                health = 0;
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
    }
}
