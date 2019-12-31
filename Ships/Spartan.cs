using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Boss4;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Ships
{
    public class Spartan : Ship
    {
        SpartanClaw[] claws = new SpartanClaw[2];
        SpartanShield shield;
        public Spartan(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 28;
            maxHealth = health = 20;
            claws[0] = new SpartanClaw(this, new Vector2(4, -1.5f), new float[] { (float)Math.PI / 4, (float)Math.PI / 2}, rotation + (float)Math.PI/4, false);
            claws[1] = new SpartanClaw(this, new Vector2(-4, -1.5f), new float[] { 3f * (float)Math.PI / 4, (float)Math.PI / 2 }, rotation + 3f * (float)Math.PI / 4, true);
            foreach(SpartanClaw claw in claws)
            {
                claw.UpdateRelativePosition();
            }

        }
        public override void SpecialUpdate()
        {
            foreach (SpartanClaw claw in claws)
            {
                claw.UpdateRelativePosition();
                if (!IsActing() && team == 0)
                {
                    claw.AimTowardSavedPosition(0);
                }
            }
            
        }
        void ShieldOff()
        {
            if (shield != null)
            {
                Miasma.gameEntities.Remove(shield);
                shield = null;
            }
        }
        void ShieldOn()
        {
            if (shield == null)
            {
                shield = new SpartanShield(Position, this, team: team);
            }
        }
        public override void NormalMovement()
        {
            Velocity = (home - Position);
            if (Velocity.Length() > speed)
            {
                Velocity.Normalize();
                Velocity *= speed;
                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
                ShieldOff();
            }
            else
            {
                ShieldOn();
                rotation = Functions.SlowRotation(rotation, 0, (float)Math.PI / 30);
            }
        }
        const int preShootTime = 180;
        const int postShootTime = 120;
        public void shootPattern(int localTimer)
        {
            foreach (SpartanClaw claw in claws)
            {
                if (localTimer > preShootTime)
                {
                    claw.AimTowardSavedPosition(0);
                }
                else if (localTimer == preShootTime)
                {
                    claw.AimTowardSavedPosition(0);
                    claw.Shoot();
                    //new ArtillaryPulse(Position + Functions.PolarVector(11, (float)Math.PI / 2), Functions.PolarVector(7, (float)Math.PI / 2), team: team);
                }
                else
                {
                    claw.AimTowardSavedPosition(1);
                    for (int i = 0; i < 1; i++)
                    {
                        float r = Functions.RandomRotation();
                        new Particle(claw.AbsoluteShootPosition() + Functions.PolarVector(15, r), Functions.PolarVector(-1, r), Miasma.random.Next(2) + 4, 15);
                    }
                }
            }
        }
        int infectTimer =0;
        int infectTimer2 = 0;
        public override void InfectedUpdate()
        {
            Vector2 flyTo = Miasma.player.Position + (Vector2.UnitY * -20f);
            float goalRotation = (float)Math.PI;
            if(Miasma.BossIsActive() && Miasma.boss is Jupiter)
            {
                flyTo = Miasma.player.Position + (Vector2.UnitY * 20f);
                goalRotation = 0f;
            }
            infectTimer++;
            if (health > 10)
            {
               
               
                Velocity = (flyTo - Position);
                if (Velocity.Length() > speed)
                {
                    ShieldOff();
                    Velocity.Normalize();
                    Velocity *= speed;
                    rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
                }
                else
                {
                    ShieldOn();
                    rotation = Functions.SlowRotation(rotation, goalRotation, (float)Math.PI / 30);
                }
                if(infectTimer % 60 == 0)
                {
                    health--;
                }
                foreach (SpartanClaw claw in claws)
                {
                    claw.AimTowardSavedPosition(0);

                }
            }
            else
            {
                
                ShieldOff();
                Velocity = Vector2.Zero;
                rotation = Functions.SlowRotation(rotation, goalRotation, (float)Math.PI / 30);
                infectTimer2++;
                shootPattern(infectTimer2);
                if (infectTimer2 > preShootTime + postShootTime + 60)
                {
                    health = 0;
                }
            }
        }
        int actTimer = 0;
        public override void ActionStart()
        {
            actTimer = 0;
        }
        public override void ActionUpdate()
        {
            NormalMovement();
            ShieldOff();
            actTimer++;
            shootPattern(actTimer);
            if (actTimer > preShootTime + postShootTime)
            {
                acting = -1;
                shield = new SpartanShield(Position, this);
            }



        }
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            foreach (SpartanClaw claw in claws)
            {
                claw.Draw(spriteBatch);
            }
        }
        private class SpartanShield : Entity
        {
            Entity owner;
            public SpartanShield(Vector2 Position, Entity owner, float rotation = 0, int team = 0) : base(Position, rotation, team)
            {
                entityID = 29;
                maxHealth = health = 100;
                this.owner = owner;
            }
            byte col = 80;
            public override void MainUpdate()
            {
                if(!Miasma.gameEntities.Contains(owner))
                {
                    health = 0;
                }
                if (col > 80)
                {
                    col -= 6;
                }
                if (owner != null && Miasma.gameEntities.Contains(owner))
                {
                    health = maxHealth;
                    Position = owner.Position + Functions.PolarVector(15, owner.rotation + (float)Math.PI / 2);
                    rotation = owner.rotation;
                }
                else
                {
                    health = 0;
                }

            }
            public override void GetHitEffects(Projectile hitBy)
            {
                if (255 - col < 60)
                {
                    col = 255;

                }
                else
                {
                    col += 60;
                }
                
                if (!(hitBy is MiasmaPulse))
                {
                    hitBy.Velocity *= -1;
                    hitBy.team = hitBy.team == 0 ? 1 : 0;
                    hitBy.rotation += (float)Math.PI;
                    Sounds.shield.Play();
                }

            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                Texture2D texture = Miasma.EntitySprites[entityID];
                spriteBatch.Draw(texture, Position, null, new Color(col, col, col, col), rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            }
        }
        private class SpartanClaw : Turret
        {
            bool inverted = false;
            public SpartanClaw(Entity owner, Vector2 relativePosition, float[] savedPositions, float rotation = 0, bool inverted = false) : base(owner, relativePosition, rotation, savedPositions)
            {
                this.inverted = inverted;
                turnSpeed = (float)Math.PI / 60;
                turretLength = 10.5f;
                texture = Miasma.EntityExtras[17];
                origin = new Vector2(2, 2);
                if(inverted)
                {
                    origin += Vector2.UnitY * 3;
                }
            }
            public override Vector2 AbsoluteShootPosition()
            {
                return AbsolutePosition() + Functions.PolarVector(turretLength, AbsoluteRotation()) + Functions.PolarVector(AbsoluteRotation() + (float)Math.PI/2, inverted ? 0 : 3.5f);
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, AbsolutePosition(), null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), inverted ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            }
            public override void Shoot()
            {
                for(int i =1; i <2; i++)
                {
                    float r = (savedPositions[0] + owner.rotation) + ((float)i / 3) * (savedPositions[1] - savedPositions[0]);
                    new ArtillaryPulse(AbsoluteShootPosition(), Functions.PolarVector(7, r), team: owner.team);
                }
            }

        }
        
    }
   

}
