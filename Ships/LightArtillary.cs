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
    public class LightArtillary : Ship
    {
        
        SmallArtillaryTurret turret;
        public LightArtillary(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            
            entityID = 5;
            maxHealth = health = 40;
            
            turret = new SmallArtillaryTurret(this, new Vector2(0, -4), rotation - (float)Math.PI / 2);
            turret.UpdateRelativePosition();
        }
        
        bool AimAt(Vector2 target)
        {
            float before = turret.AbsoluteRotation();
            turret.AimTowardAbsolute(target);
            return before == turret.AbsoluteRotation();
        }
        
        public override void SpecialUpdate()
        {
            turret.UpdateRelativePosition();
            if (acting == -1 && team != 1)
            {
                turret.AimToward((float)Math.PI / 2);
            }
        }
        
        int timer = 0;
        public override void ActionUpdate()
        {

            NormalMovement();
            timer++;
            
            AimAt(Miasma.player.Position);
            if (timer % 90 == 0)
            {

                turret.Shoot();
            }
            if(timer > 360)
            {
                acting = -1;
                timer = 0;
            }
        }
        public override void InfectedUpdate()
        {
            
            NormalMovement();
            acting = -1;
            Entity shootAt = null;
            if (InfectedTargeting(ref shootAt))
            {

                
                timer++;
                if(timer > 60 && AimAt(shootAt.Position))
                {
                    health -= 5;
                    timer = 0;
                    turret.Shoot();
                }
               

            }
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            turret.Draw(spriteBatch);
        }
        private class SmallArtillaryTurret : Turret
        {
            public SmallArtillaryTurret(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI / 240;
                turretLength = 12;
                texture = Miasma.EntityExtras[0];
                origin = new Vector2(2, 5);
            }
            public override void Shoot()
            {
                new ArtillaryPulse(AbsoluteShootPosition(), Functions.PolarVector(7, AbsoluteRotation()), AbsoluteRotation() + (float)Math.PI / 2, team: owner.team);
            }
        }
    }
}
