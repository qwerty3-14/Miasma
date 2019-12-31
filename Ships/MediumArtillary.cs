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
    public class MediumArtillary : Ship
    {
        Turret[] turrets = new Turret[3];
        public MediumArtillary(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 19;
            maxHealth = health = 160;
            turrets[0] = new SmallArtillaryTurret(this, new Vector2(-12, 7), rotation + (float)Math.PI / 2);
            turrets[1] = new SmallArtillaryTurret(this, new Vector2(12, 7), rotation + (float)Math.PI / 2);
            turrets[2] = new MediumArtillaryTurret(this, new Vector2(0, -10), rotation + (float)Math.PI/2);
            for (int i = 0; i < 3; i++)
            {
                turrets[i].UpdateRelativePosition();
            }
        }
        int salvoCount = 0;
        int salvoStep = 0;
        int salvoCounter = 0;
        public override void SpecialUpdate()
        {
            if(!IsActing() && team==0)
            {
                for(int i = 0; i < 3; i++)
                {
                    turrets[i].AimToward((float)Math.PI / 2);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                turrets[i].UpdateRelativePosition();
            }
        }
        public override void ActionStart()
        {
            salvoCount = 0;
            salvoStep = 0;
            salvoCounter = 0;
        }
        public override void ActionUpdate()
        {
            NormalMovement();
            salvoCounter++;
            if(salvoCounter % 45 ==0)
            {
                salvoStep++;
                fire();
            }
            if(salvoStep > 2)
            {
                salvoStep = 0;
                salvoCount++;
            }
            if (salvoCount>= 3)
            {
                acting = -1;
            }
            switch (salvoStep)
            {
                case 0:
                    turrets[0].AimTowardAbsolute(Functions.ToRotation(((-3 * (turrets[0].AbsolutePosition() - Miasma.player.Position).Length() / 9f) * Vector2.UnitX + Miasma.player.Position) - turrets[0].AbsolutePosition()));
                    turrets[1].AimTowardAbsolute(Functions.ToRotation(((3 * (turrets[1].AbsolutePosition() - Miasma.player.Position).Length() / 9f) * Vector2.UnitX + Miasma.player.Position) - turrets[1].AbsolutePosition()));
                    break;
                case 1:
                    turrets[0].AimTowardAbsolute(Functions.ToRotation(((-1.5f * (turrets[0].AbsolutePosition() - Miasma.player.Position).Length() / 9f) * Vector2.UnitX + Miasma.player.Position) - turrets[0].AbsolutePosition()));
                    turrets[1].AimTowardAbsolute(Functions.ToRotation(((1.5f * (turrets[1].AbsolutePosition() - Miasma.player.Position).Length() / 9f) * Vector2.UnitX + Miasma.player.Position) - turrets[1].AbsolutePosition()));
                    break;
                default:
                    turrets[0].AimToward((float)Math.PI / 2);
                    turrets[1].AimToward((float)Math.PI / 2);
                    break;
            }
            turrets[2].AimTowardAbsolute(Functions.ToRotation(Miasma.player.Position - turrets[2].AbsolutePosition()));
        }
        void fire()
        {
            switch(salvoStep)
            {
                case 1:
                case 2:
                    turrets[0].Shoot();
                    turrets[1].Shoot();
                    break;
                case 3:
                    turrets[2].Shoot();
                    break;

            }
        }
        Entity target;
        public override void InfectedUpdate()
        {
            NormalMovement();
           
            if (InfectedTargeting(ref target))
            {
                for (int i = 0; i < 3; i++)
                {
                    turrets[i].AimTowardAbsolute(Functions.ToRotation(target.Position - turrets[i].AbsolutePosition()));
                }
                salvoCounter++;
                if (salvoCounter % 30 == 0)
                {
                    salvoStep++;
                    fire();
                }
                if (salvoStep > 2)
                {
                    salvoStep = 0;
                    Strike(30);
                }
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            for (int i = 0; i < 3; i++)
            {
                turrets[i].Draw(spriteBatch);
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
