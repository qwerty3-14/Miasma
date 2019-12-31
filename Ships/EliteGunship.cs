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
    public class EliteGunship : LightGunship
    {
        EliteGunshipTurret[] turrets = new EliteGunshipTurret[4];
        public EliteGunship(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 27;
            health = maxHealth = 100;
            turrets[0] = new EliteGunshipTurret(this, new Vector2(-11, 8), rotation + (float)Math.PI/2);
            turrets[1] = new EliteGunshipTurret(this, new Vector2(11, 8), rotation + (float)Math.PI / 2);
            turrets[2] = new EliteGunshipTurret(this, new Vector2(-17, -7), rotation + (float)Math.PI / 2);
            turrets[3] = new EliteGunshipTurret(this, new Vector2(17, -7), rotation + (float)Math.PI / 2);
            // turrets[4] = new EliteGunshipTurret(this, new Vector2(-10, -12));
            //turrets[5] = new EliteGunshipTurret(this, new Vector2(10, -12));
            foreach (EliteGunshipTurret turret in turrets)
            {
                turret.UpdateRelativePosition();
            }
        }
        public override void SpecialUpdate()
        {
            foreach(EliteGunshipTurret turret in turrets)
            {
                turret.UpdateRelativePosition();
            }
            if (acting == -1 && team == 0)
            {
                aimAt = rotation + (float)Math.PI / 2;
            }
            foreach (EliteGunshipTurret turret in turrets)
            {
                turret.AimTowardAbsolute(aimAt);
            }
            

        }

        public override void Shoot(float direction)
        {
            foreach (EliteGunshipTurret turret in turrets)
            {
                turret.Shoot();
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            foreach (EliteGunshipTurret turret in turrets)
            {
                turret.Draw(spriteBatch);
            }
        }
        private class EliteGunshipTurret : Turret
        {
            public EliteGunshipTurret(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI / 60;
                turretLength = 6;
                texture = Miasma.EntityExtras[2];
                origin = new Vector2(5, 5) * .5f;
            }
        }
    }
}
