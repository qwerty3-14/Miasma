using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Upgrades
{
    class TurretUpgrade : Upgrade
    {
        public TurretUpgrade()
        {
            upgradeID = 12;
            name = "Auto Turret";
            description = "Gain a turret. Turret shots at nearby enemies.";
        }
        PlayerTurret turret;
        public override void ModifyStats(TheTransmission player)
        {
            turret = new PlayerTurret(player, new Vector2(0, -3));
        }
        int cooldown = 0;
        public override void UpdatePlayer(TheTransmission player)
        {
            if (turret != null)
            {
                float aimDistance = 300;
                Entity target = null;
                cooldown--;
                foreach (Entity entity in Miasma.gameEntities)
                {
                    if(entity.team == 0 && entity.maxHealth != -1)
                    {
                        if((entity.Position - turret.AbsolutePosition()).Length() < aimDistance)
                        {
                            aimDistance = (entity.Position - turret.AbsolutePosition()).Length();
                            target = entity;
                        }
                    }
                }
                if(target == null)
                {
                    turret.AimToward(-(float)Math.PI / 2);
                }
                else
                {
                    turret.AimTowardAbsolute(Functions.ToRotation((target.Position + target.Velocity* aimDistance/8f) - turret.AbsolutePosition()));
                    if(cooldown <= 0)
                    {
                        cooldown = 30;
                        turret.Shoot();
                    }
                }
                
                turret.UpdateRelativePosition();
            }
        }
        public override void TransmissionDrawTasks(SpriteBatch spriteBatch)
        {
            if (turret != null)
            {
                turret.Draw(spriteBatch);
            }
        }
        private class PlayerTurret : Turret
        {
            public PlayerTurret(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI / 30;
                turretLength = 4.5f;
                texture = Miasma.EntityExtras[15];
                origin = new Vector2(1.5f, 2.5f);
            }
            public override void Shoot()
            {
                new PewPew(AbsoluteShootPosition(), Functions.PolarVector(8, AbsoluteRotation()), team: owner.team);
            }
        }
    }
}
