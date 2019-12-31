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
    class BombardmentArtillary : Ship
    {
        Turret[] turrets = new Turret[9];
        public BombardmentArtillary(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 37;
            maxHealth = health = 300;
            turrets[0] = new SmallArtillaryTurret(this, new Vector2(-16, 16), rotation+(float)Math.PI/2);
            turrets[1] = new SmallArtillaryTurret(this, new Vector2(-20, 2), rotation + (float)Math.PI / 2);
            turrets[2] = new SmallArtillaryTurret(this, new Vector2(-17, -8), rotation + (float)Math.PI / 2);
            turrets[5] = new SmallArtillaryTurret(this, new Vector2(16, 16), rotation + (float)Math.PI / 2);
            turrets[4] = new SmallArtillaryTurret(this, new Vector2(20, 2), rotation + (float)Math.PI / 2);
            turrets[3] = new SmallArtillaryTurret(this, new Vector2(17, -8), rotation + (float)Math.PI / 2);
            turrets[6] = new MediumArtillaryTurret(this, new Vector2(-14, -17), rotation + (float)Math.PI / 2);
            turrets[7] = new MediumArtillaryTurret(this, new Vector2(0,  -21), rotation + (float)Math.PI / 2);
            turrets[8] = new MediumArtillaryTurret(this, new Vector2(14, -17), rotation + (float)Math.PI / 2);
            foreach (Turret turret in turrets)
            {
                turret.UpdateRelativePosition();
            }
        }
        public override void SpecialUpdate()
        {
            foreach (Turret turret in turrets)
            {
                turret.UpdateRelativePosition();
            }
            if(!IsActing() && team == 0)
            {
                foreach (Turret turret in turrets)
                {
                    turret.AimToward((float)Math.PI / 2);
                }
            }
        }
        int shootTimer = 0;
        public override void ActionStart()
        {
            shootTimer = 0;
        }
        void ShootBehavior()
        {
            shootTimer++;
            if (shootTimer % 45 == 0)
            {
                if(team ==1)
                {
                    health -= 30;
                }
                if (shootTimer % 90 == 0)
                {
                    for (int t = 6; t < 9; t++)
                    {
                        turrets[t].Shoot();
                    }
                }
                else
                {
                    for (int t = 0; t < 6; t++)
                    {
                        turrets[t].Shoot();
                    }
                }
            }
        }
        public override void ActionUpdate()
        {
            NormalMovement();
            float subLength = 320;
            for(int t =0; t < 6; t++)
            {
                turrets[t].AimTowardAbsolute(Miasma.player.Position + Vector2.UnitX * (-subLength + (subLength*2) * ((float)t / 5f)));
            }
            subLength = 50;
            for (int t =0; t < 3; t++)
            {
                turrets[t+6].AimTowardAbsolute(Miasma.player.Position + Vector2.UnitX * (-subLength + (subLength * 2) * ((float)t / 2f)));
            }
            ShootBehavior();
            if (shootTimer > 90 * 4)
            {
                acting = -1;
            }
        }
        Entity target;
        public override void InfectedUpdate()
        {
            if(InfectedTargeting(ref target))
            {
                foreach (Turret turret in turrets)
                {
                    turret.AimTowardAbsolute(target.Position);
                    
                }
                ShootBehavior();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach(Turret turret in turrets)
            {
                turret.Draw(spriteBatch);
            }
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
        private class MediumArtillaryTurret : Turret
        {
            public MediumArtillaryTurret(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI / 240;
                turretLength = 21;
                texture = Miasma.EntityExtras[7];
                origin = new Vector2(7, 10);
            }
            public override void Shoot()
            {
                new BigArtillaryPulse(AbsoluteShootPosition(), Functions.PolarVector(7, AbsoluteRotation()), AbsoluteRotation() + (float)Math.PI / 2, team: owner.team);
            }
        }
    }
}
