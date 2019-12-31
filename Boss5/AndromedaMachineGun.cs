using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss5
{
    public class AndromedaMachineGun : AndromedaTurret
    {
        int frame = 0;
        public AndromedaMachineGun(AndromedaGunBase owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
        {
            turnSpeed = (float)Math.PI / 60;
            turretLength = 59-16;
            texture = Miasma.EntityExtras[31];
            origin = new Vector2(16, 16);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, AbsolutePosition(), new Rectangle(0, frame * texture.Height / 2, texture.Width, texture.Height / 2), Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public override void Shoot()
        {
            frame = frame == 0 ? 1 : 0;
            new PewPew(AbsoluteShootPosition() + Functions.PolarVector(Miasma.random.Next(-3, 4), AbsoluteRotation()+(float)Math.PI/2), Functions.PolarVector(4, AbsoluteRotation()), team: owner.team);
        }
        int counter = 0;
        Vector2 aimAt;
        public override void ActingUpdate()
        {
            
            if(counter <= 0)
            {
                aimAt = Miasma.player.Position + Vector2.UnitX * Miasma.random.Next(-50, 51);
                counter = 60;
            }
            if(counter>0)
            {
                counter--;
                if(Miasma.hard  && counter <= 55 && counter > 5 && counter % 5 == 0)
                {
                    Shoot();
                }
                else if(counter <= 45 && counter > 15 && counter % 5 ==0)
                {
                    Shoot();
                }
            }
            AimTowardAbsolute(aimAt);

        }
        AndromedaGunBase other;
        public override void InfectingUpdate()
        {
            other = ((AndromedaGunBase)owner).OtherGun();
            if(other == null)
            {
                AimToward((float)Math.PI / 2);
            }
            else
            {
                if (counter <= 0)
                {
                    
                    counter = 60;
                }
                if (counter > 0)
                {
                    counter--;
                    if (counter <= 45 && counter > 15 && counter % 5 == 0)
                    {
                        Shoot();
                    }
                }
                AimTowardAbsolute(other.Position);
            }
        }
    }
}
