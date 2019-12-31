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
    public class MediumGunship : LightGunship
    {
        MediumGunshipTurret[] turrets = new MediumGunshipTurret[2];
        public MediumGunship(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 8;
            health = maxHealth = 40;
            turrets[0] = new MediumGunshipTurret(this, new Vector2(-7, -4), rotation + (float)Math.PI/2);
            turrets[1] = new MediumGunshipTurret(this, new Vector2(7, -4), rotation + (float)Math.PI / 2);
            foreach (MediumGunshipTurret turret in turrets)
            {
                turret.UpdateRelativePosition();
            }
            
        }

        public override void SpecialUpdate()
        {
            foreach(MediumGunshipTurret turret in turrets)
            {
                turret.UpdateRelativePosition();
            }
            if (acting == -1 && team == 0)
            {
                aimAt = rotation + (float)Math.PI / 2;
            }
            foreach (MediumGunshipTurret turret in turrets)
            {
                turret.AimTowardAbsolute(aimAt);
            }
            

        }

        public override void Shoot(float direction)
        {
            foreach (MediumGunshipTurret turret in turrets)
            {
                turret.Shoot();
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            
            foreach (MediumGunshipTurret turret in turrets)
            {
                turret.Draw(spriteBatch);
            }
        }
        private class MediumGunshipTurret : Turret
        {
            public MediumGunshipTurret(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI / 60;
                turretLength = 6;
                texture = Miasma.EntityExtras[2];
                origin = new Vector2(5, 5) * .5f;
            }
        }
    }
}
