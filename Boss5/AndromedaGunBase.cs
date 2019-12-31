using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss5
{
    public class AndromedaGunBase : Entity
    {
        AndromedaTurret turret;
        public bool active = false;
        TheAndromeda owner;
        public AndromedaGunBase(TheAndromeda owner, Vector2 Position, int type, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            this.owner = owner;
            entityID = 38;
            health = maxHealth = 150;
            Vector2 relativePosition = new Vector2(0, 30);
            switch(type)
            {
                case 0:
                    turret = new AndromedaMachineGun(this, relativePosition, (float)Math.PI / 2);
                    break;
                case 1:
                    turret = new AndromedaStarGun(this, relativePosition, (float)Math.PI / 2);
                    break;
                case 2:
                    turret = new AndromedaBombLauncher(this, relativePosition, (float)Math.PI / 2);
                    heightAbove -= 20;
                    break;
                case 3:
                    turret = new AndromedaDartLauncher(this, relativePosition, (float)Math.PI / 2);
                    heightAbove -= 20;
                    break;
            }
        }
        int heightAbove = 100;
        public int actionTimer = -1;
        float deploySpeed = 2;
        public int actionTime = 300;
        float relativeX = 0;
        public void SetRelativeX(float x)
        {
            relativeX = x;
        }
        public override void MainUpdate()
        {
            Position.X = owner.Position.X + relativeX;
            if(team ==1)
            {
                actionTimer = 0;
                for (int d = 0; d < 1; d++)
                {
                    new Particle(turret.AbsolutePosition(), Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2), 30);
                }
                if(Miasma.random.Next(2)==0)
                {
                    Strike(1);
                }
            }
            if (active)
            {
                if (actionTimer == -1)
                {
                    turret.OnStart();
                    actionTimer = 0;
                }
                if (actionTimer > actionTime)
                {
                    if (Position.Y > owner.Position.Y )
                    {
                        Position.Y -= deploySpeed;
                    }
                    else
                    {
                        actionTimer = -1;
                        active = false;
                        Position.Y = owner.Position.Y;
                    }
                }
                else
                {
                    if (Position.Y < owner.Position.Y + heightAbove)
                    {
                        Position.Y += deploySpeed;
                    }
                    else
                    {
                        actionTimer++;
                        Position.Y = heightAbove;
                        if(team == 1)
                        {
                            turret.InfectingUpdate();
                        }
                        else
                        {
                            turret.ActingUpdate();
                        }
                        
                    }
                }
                
            }
            else
            {
                turret.AimToward((float)Math.PI / 2);
            }
            turret.UpdateRelativePosition();
        }
        public override void UpdateHitbox()
        {
            if(!active)
            {
                Hitbox = new Rectangle(0, 0, 0, 0);
                return;
            }
            base.UpdateHitbox();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            turret.Draw(spriteBatch);
        }
        public override void DeathEffects()
        {
            active = false;
            for (int d = 0; d < 36; d++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2) + (team == 1 ? 0 : 2), 30);
            }
        }
        public AndromedaGunBase OtherGun()
        {
            if(owner.gunSlots[0] == null || owner.gunSlots[1] == null || !Miasma.gameEntities.Contains(owner.gunSlots[0]) || !Miasma.gameEntities.Contains(owner.gunSlots[1]) || owner.gunSlots[0].Position.Y < owner.Position.Y + heightAbove || owner.gunSlots[1].Position.Y < owner.Position.Y + heightAbove)
            {
                return null;
            }
            return owner.gunSlots[0] == this ? owner.gunSlots[1] : owner.gunSlots[0];
        }
    }
    
}
