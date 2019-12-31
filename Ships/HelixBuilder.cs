using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Ships
{
    public class HelixBuilder : Ship
    {
        HelixGun[] guns = new HelixGun[2];
        bool moveGuns = false;
        public HelixBuilder(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            guns[0] = new HelixGun(this, new Vector2(7, -8), rotation - (float)Math.PI / 2);
            guns[1] = new HelixGun(this, new Vector2(-7, -8), rotation - (float)Math.PI / 2);

            entityID = 17;
            health = maxHealth = 40;
            foreach(Turret turret in guns)
            {
                turret.UpdateRelativePosition();
            }
        }
        int limiter = 0;
        float gunTrigCounter = 0;
        public override void SpecialUpdate()
        {
            if (moveGuns)
            {
                gunTrigCounter += (float)Math.PI / 10;
            }
            limiter++;
            for (int t = 0; t < guns.Length; t++)
            {
                HelixGun turret = guns[t];
                turret.UpdateRelativePosition(new Vector2((float)Math.Cos(gunTrigCounter) * 7 * (t == 0 ? 1: -1), -8));
                turret.AimToward(-(float)Math.PI / 2);
                if (moveGuns)
                {
                    
                        if (limiter % 2 == 0)
                        {
                            turret.Shoot(t == 0, (float)-Math.Sin(gunTrigCounter) * 7 * (t == 0 ? 1 : -1) > 0 ? 1 : -1);
                        }
                    
                    
                }
            }
        }
        float actRotation = (float)Math.PI/2;
        public override void ActionStart()
        {
            float horizontalPos = 100 + Miasma.random.Next(400); 
            while(Math.Abs(horizontalPos - Miasma.player.Position.X) < 70)
            {
                horizontalPos = 100 + Miasma.random.Next(400);
            }
            actRotation = Functions.ToRotation(new Vector2(horizontalPos, 700) - Position);
        }
        public override void ActionUpdate()
        {
            moveGuns = Position.Y > 600;
            Velocity = Functions.PolarVector(5f, actRotation);
            rotation = Functions.ToRotation(Velocity) - (float)Math.PI/2;
            if(Position.Y > 820)
            {
                Position.Y -= 840;
                acting = -1;
                foreach(HelixGun turret in guns)
                {
                    turret.reset();
                }
                moveGuns = false;
            }
        }
        Entity shootAt;
        public override void InfectedUpdate()
        {
            acting = -1;
            moveGuns = true;
            if (InfectedTargeting(ref shootAt))
            {
                rotation = Functions.SlowRotation(rotation, Functions.ToRotation(shootAt.Position - Position) - (float)Math.PI / 2,  (float)Math.PI/30);
            }
            Velocity = Functions.PolarVector(3f, rotation + (float)Math.PI / 2);
            if (limiter % 8 == 0)
            {
                health--;
            }
               
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //bottom
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);

            //turrets
            if(-1* Math.Sin(gunTrigCounter) > 0)
            {
                guns[0].Draw(spriteBatch);
                guns[1].Draw(spriteBatch);
            }
            else
            {
                guns[1].Draw(spriteBatch);
                guns[0].Draw(spriteBatch);
            }

            //top
            texture = Miasma.EntityExtras[5];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        
        private class HelixGun : Turret
        {
            Vector2? previusPosition = null;
            public HelixGun(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI;
                turretLength = 7;
                texture = Miasma.EntityExtras[6];
                origin = new Vector2(3, 3);
            }
            public void Shoot(bool red, int layer)
            {
                new HelixSegment(AbsoluteShootPosition(), Vector2.Zero, team: owner.team, red: red, layer: layer, linkTo: previusPosition);
                previusPosition = AbsoluteShootPosition();
            }
            public void reset()
            {
                previusPosition = null;
            }
        }
    }
}
