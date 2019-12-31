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
    public class AndromedaStarGun : AndromedaTurret
    {
        int tips = 5;
        public AndromedaStarGun(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
        {
            turnSpeed = (float)Math.PI / 120;
            turretLength = 14;
            texture = Miasma.EntityExtras[32];
            origin = new Vector2(8.5f, 8.5f);
        }
        public override void Shoot()
        {
            for(int i =0; i < tips; i++)
            {
                float d = AbsoluteRotation() + ((float)i / tips) * 2 * (float)Math.PI;
                new ArtillaryPulse(AbsolutePosition() + Functions.PolarVector(turretLength, d), Functions.PolarVector(7, d), team: owner.team);
            }
        }
        public override  void Draw(SpriteBatch spriteBatch)
        {
            for(int i =0; i < tips; i++)
            {
                spriteBatch.Draw(Miasma.EntityExtras[33], AbsolutePosition(), null, Color.White, AbsoluteRotation() + ((float)i/tips) * 2 * (float)Math.PI, new Vector2(0, 5.5f), new Vector2(1, 1), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, AbsolutePosition(), null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        int counter = 0;
        public override void ActingUpdate()
        {
            rotation += turnSpeed * ((AbsolutePosition().X > Miasma.boss.Position.X + 300) ? 1 : -1);
            counter++;
            if(counter% 20 ==0 || (Miasma.hard && counter % 20 == 5))
            {
                Shoot();
            }
        }
        public override void InfectingUpdate()
        {
            ActingUpdate();
        }
    }
}
