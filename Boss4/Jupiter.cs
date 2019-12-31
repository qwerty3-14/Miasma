using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Miasma.Ships;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss4
{
    public class Jupiter : Boss
    {
        CenterDoor[] centerDoors = new CenterDoor[2];
        SideDoor[] sideDoors = new SideDoor[4];
        public const int openTime = 120;
        int preOpenTime = 120;
        public List<Entity> arms = new List<Entity>();
        int maxArms = 4;
        ArmsCore core;
        Spartan[] spartans = new Spartan[3];
        
        LimbSegment[] limbs = new LimbSegment[2];
        LightningAnnihilator annihilator;
        int maxBeamTime = 60;
        public Vector2 CoreCenter()
        {
            return core.AbsolutePosition();
        }
        public Jupiter(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            health = maxHealth = 500;
            entityID = 30;
            Miasma.currentWaveIntensity = 4;
            if(Miasma.hard)
            {
                maxArms += 2;
                maxBeamTime = 10;
            }
            name = "Jupiter";
            int x = 34;
            int y = 8;
            sideDoors[0] = new SideDoor(this, new Vector2(-x, -y), 0, new float[] { 0f, -(float)Math.PI / 2 }, 0);
            sideDoors[1] = new SideDoor(this, new Vector2(x, -y), 0, new float[] { 0f, (float)Math.PI / 2 }, 1);
            sideDoors[2] = new SideDoor(this, new Vector2(-x, y), 0, new float[] { 0f, (float)Math.PI / 2 }, 10);
            sideDoors[3] = new SideDoor(this, new Vector2(x, y), 0, new float[] { 0f, -(float)Math.PI / 2 }, 11);
            centerDoors[0] = new CenterDoor(this, new Vector2(0, -y), 0);
            centerDoors[1] = new CenterDoor(this, new Vector2(0, y), (float)Math.PI);
            core = new ArmsCore(this, new Vector2(0, -29), 0);

            limbs[0] = new LimbSegment(Position, new Hand(new BallLightning(Position)));
            limbs[1] = new LimbSegment(Position, new Hand(new BallLightning(Position)));
            limbs[1].Rotate((float)Math.PI);
            annihilator = new LightningAnnihilator(this, new Vector2(0, 8.5f), (float)Math.PI / 2);
            annihilator.UpdateRelativePosition();
            foreach (CenterDoor door in centerDoors)
            {
                door.UpdateRelativePosition();
            }
            foreach (SideDoor door in sideDoors)
            {
                door.UpdateRelativePosition();
            }
            core.UpdateRelativePosition();
        }
        int openTimer = 0;
        int spartanSpawnCooldown = 600;
        int spartanTimer = 0;
        bool beamAttack = false;
        int phase = 0;
        bool beamOn = false;
        int beamTimer = 0;
        int beamRunCounter = 0;
        public override void SpecialUpdate()
        {
            rotation = Functions.SlowRotation(rotation, beamAttack ? (float)Math.PI : 0, (float)Math.PI / 240);
            ArmAction();
            if (spartanTimer % spartanSpawnCooldown ==0 && !beamAttack && openTimer > preOpenTime)
            {
                for (int s =0; s < spartans.Length; s++)
                {
                    if (spartans[s] == null || !Miasma.gameEntities.Contains(spartans[s]))
                    {
                        float horizontal = 100 + 400 * ((float)(s + 1) / (spartans.Length + 1));
                        spartans[s] = new Spartan(new Vector2(horizontal, -50), new Vector2(horizontal, 50), 0, team);
                        Miasma.enemyFleet.Add(spartans[s]);
                    }
                }
                
            }
            annihilator.UpdateRelativePosition();
            foreach(CenterDoor door in centerDoors)
            {
                door.UpdateRelativePosition();
            }
            foreach(SideDoor door in sideDoors)
            {
                door.UpdateRelativePosition();
            }
            core.UpdateRelativePosition();
            
                openTimer++;
            
            for (int a = 0; a < arms.Count; a++)
            {
                if (!Miasma.gameEntities.Contains(arms[a]))
                {
                    arms.RemoveAt(a);
                }
            }
            if(openTimer > preOpenTime + openTime)
            {
                bool endIt = false;
                if(beamAttack)
                {
                    if (beamRunCounter > 4 && (Position.X >= 295 && Position.X <= 305))
                    {
                        Position.X = 300;
                        Velocity = Vector2.Zero;
                    }
                    if(Position.X<=100)
                    {
                        Position.X = 100;
                        Velocity = Vector2.Zero;
                    }
                    if (Position.X >= 500)
                    {
                        Position.X = 500;
                        Velocity = Vector2.Zero;
                    }
                    if ((Position.X <= 150  && Velocity.X <0)|| (Position.X >= 450 && Velocity.X >0))
                    {
                        beamTimer = 0;
                    }
                    if (beamTimer<maxBeamTime)
                    {
                        beamTimer++;
                        new Particle(annihilator.AbsoluteShootPosition() + Functions.PolarVector(4 - Miasma.random.Next(9), (float)Math.PI / 2 + annihilator.AbsoluteRotation()), Functions.PolarVector(3, annihilator.AbsoluteRotation() + Functions.RandomRotation()*.25f - (float)Math.PI/4), 7, 16);
                        beamOn = false;
                    }
                    else
                    {
                        beamOn = true;
                        if (Velocity == Vector2.Zero)
                        {
                            beamRunCounter++;
                            switch (Position.X)
                            {
                                case 300:
                                    if (beamRunCounter > 4)
                                    {
                                        beamAttack = false;
                                        Velocity = Vector2.Zero;
                                        endIt = true;
                                        beamRunCounter = 0;
                                        beamTimer = 0;
                                    }
                                    else
                                    {
                                        Velocity = Miasma.player.Position.X > Position.X ? Vector2.UnitX * 3 : Vector2.UnitX * -3;
                                    }
                                    
                                    break;
                                case 100:
                                    Velocity = Vector2.UnitX * 3;
                                    break;
                                case 500:
                                    Velocity =  Vector2.UnitX * -3;
                                    break;

                            }
                        }
                    }
                }
                else
                {
                    beamOn = false;
                }
                if(arms.Count <= 0 && !beamAttack)
                {
                    endIt = true;
                }
                if(endIt)
                {
                    foreach (CenterDoor door in centerDoors)
                    {
                        door.ToScale(1f);
                    }
                    bool ready = false;
                    foreach (SideDoor door in sideDoors)
                    {
                        if (door.AimTowardSavedPosition(0))
                        {
                            ready = true;
                        }
                    }
                    if (ready)
                    {
                        openTimer = 0;
                        if ((phase == 0 && health < 2 * (float)maxHealth / 3) || (phase == 1 && health < (float)maxHealth / 3))
                        {
                            beamAttack = true;
                            phase++;
                            for (int s = 0; s < Miasma.gameEntities.Count; s++)
                            {
                                if (Miasma.gameEntities[s] is Ship)
                                {
                                    ((Ship)Miasma.gameEntities[s]).Flee();
                                }

                            }
                        }
                    }
                }
            }
            else if (openTimer == preOpenTime + openTime)
            {
                if (beamAttack)
                {

                }
                else
                {
                    foreach (Entity arm in arms)
                    {
                        if (arm is ArmTip)
                        {
                            ((ArmTip)arm).Activate();
                        }
                    }
                }
            }
            else if(openTimer > preOpenTime)
            {
                foreach (CenterDoor door in centerDoors)
                {
                    door.ToScale(0f);
                }
                foreach (SideDoor door in sideDoors)
                {
                    door.AimTowardSavedPosition(1);
                }
            }
            else if (openTimer == preOpenTime)
            {
                if (beamAttack)
                {

                }
                else
                {
                    for (int i = 0; i < maxArms+phase; i++)
                    {
                        arms.Add(new ArmTip(this, CoreCenter() + new Vector2(Miasma.random.Next(-20, 20), Miasma.random.Next(-20, 20)), 0, team));
                    }
                }
            }
            else
            {
                foreach (CenterDoor door in centerDoors)
                {
                    door.ToScale(1f);
                }
                foreach (SideDoor door in sideDoors)
                {
                    door.AimTowardSavedPosition(0);
                }
            }

            if(beamOn)
            {
                Sounds.beamLighting.PlayContinuous();
                if(Functions.RectangleLineCollision(Miasma.player.Hitbox, annihilator.AbsoluteShootPosition() + Functions.PolarVector(4, annihilator.AbsoluteRotation() + (float)Math.PI/2), annihilator.AbsoluteShootPosition() + Functions.PolarVector(4, annihilator.AbsoluteRotation() + (float)Math.PI / 2) + Functions.PolarVector(800, annihilator.AbsoluteRotation()))||
                    Functions.RectangleLineCollision(Miasma.player.Hitbox, annihilator.AbsoluteShootPosition() + Functions.PolarVector(-4, annihilator.AbsoluteRotation() + (float)Math.PI / 2), annihilator.AbsoluteShootPosition() + Functions.PolarVector(-4, annihilator.AbsoluteRotation() + (float)Math.PI / 2) + Functions.PolarVector(800, annihilator.AbsoluteRotation())))
                {
                    Miasma.player.Strike(2);
                }
            }
        }
        int limbOut =-40;
        bool startThrow = false;
        bool returnLimbs = true;
        void ArmAction()
        {
            if (returnLimbs)
            {
                if (limbOut > -40)
                {
                    limbOut -= 2;
                }
                else
                {
                    foreach (LimbSegment limb in limbs)
                    {
                        if (limb.IsOff())
                        {
                            limb.Action2();
                        }
                    }
                }
                limbs[0].Rotate(Functions.SlowRotation(limbs[0].GetRotation(), rotation, (float)Math.PI / 60));
                limbs[1].Rotate(Functions.SlowRotation(limbs[1].GetRotation(), rotation + (float)Math.PI, (float)Math.PI / 60));
                foreach (LimbSegment limb in limbs)
                {
                    limb.Straighten();
                }
            }
            else
            {
                if (limbOut < 140)
                {
                    limbOut += 2;
                }
                else if (!startThrow)
                {
                    
                    foreach (LimbSegment limb in limbs)
                    {
                        limb.Rotate(Functions.SlowRotation(limb.GetRotation(), (limb.GetPosition().X > 300) ? -(float)Math.PI / 4 : -3 * (float)Math.PI / 4, (float)Math.PI / 60));
                    }
                    if((limbs[0].GetRotation() == -(float)Math.PI / 4 || limbs[0].GetRotation() == -3 * (float)Math.PI / 4) && (limbs[1].GetRotation() == -(float)Math.PI / 4 || limbs[1].GetRotation() == -3 * (float)Math.PI / 4))
                    {
                        startThrow = true;
                    }
                }
                else
                {
                    if (limbs[0].IsOff())
                    {
                        if (limbs[1].IsOff())
                        {
                            returnLimbs = true;
                            startThrow = false;
                        }
                        else
                        {
                            limbs[1].Action();
                        }
                    }
                    else
                    {
                        limbs[0].Action();
                    }

                }

            }
               
            
            for (int l =0; l<2; l++)
            {
                if (limbs[l] != null)
                {
                    limbs[l].MoveTo(Position + Functions.PolarVector(limbOut * (l == 0 ? 1 : -1), rotation));
                }

            }
        }
        public override void GetHitEffects(Projectile hitBy)
        {
            if(!limbs[0].IsOff()&& !limbs[1].IsOff())
            {
                returnLimbs = false;
            }
        }
        int beamDrawCounter = 0;
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D backTexture = Miasma.EntityExtras[21];
            spriteBatch.Draw(backTexture, Position, null, Color.White, rotation, new Vector2(backTexture.Width, backTexture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            Texture2D link = Miasma.EntityExtras[23];
            foreach(Entity arm in arms)
            {
                Vector2 dif = arm.Position - CoreCenter();
                float length = dif.Length();
                float direction = Functions.ToRotation(dif);
                for(int i =0; i < (float)length/  link.Width; i++)
                {
                    spriteBatch.Draw(link, CoreCenter() + Functions.PolarVector(i * link.Width, direction), null, Color.White,  direction, new Vector2(0, 2), new Vector2(1, 1),  SpriteEffects.None, 0);
                }
            }
            core.Draw(spriteBatch);
            annihilator.Draw(spriteBatch);
            if (beamOn)
            {
                int beamWidth = 8;
                beamDrawCounter++;
                for (int g = 0; g < 2; g++)
                {
                    for (int l = 0; l < 800; l++)
                    {
                        spriteBatch.Draw(Miasma.EntityExtras[29], annihilator.AbsoluteShootPosition() + Functions.PolarVector(l, annihilator.AbsoluteRotation()), new Rectangle((l + beamDrawCounter + g*9) % 18, 0, 1, 10), Color.White, annihilator.AbsoluteRotation(), new Vector2(.5f, 5f), new Vector2(1f, 1f), SpriteEffects.None, 0);
                    }
                }

            }
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
           

        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            Texture2D link = Miasma.EntityExtras[23];
            foreach (Limb limb in limbs)
            {
                if (limb != null)
                {
                    Vector2 dif = limb.GetPosition() - Position;
                    float length = dif.Length();
                    float direction = Functions.ToRotation(dif);
                    for (int i = 0; i < (float)length / link.Width; i++)
                    {
                        spriteBatch.Draw(link, Position + Functions.PolarVector(i * link.Width, direction), null, Color.White, direction, new Vector2(0, 2), new Vector2(1, 1), SpriteEffects.None, 0);
                    }
                    limb.Draw(spriteBatch);
                }

            }
            foreach (CenterDoor door in centerDoors)
            {
                door.Draw(spriteBatch);
            }
            foreach (SideDoor door in sideDoors)
            {
                door.Draw(spriteBatch);
            }
            
            base.Draw(spriteBatch);
        }
        private class SideDoor : Turret
        {
            int flipMode = 0;
            public SideDoor(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null, int flipMode= 0) : base(owner, relativePosition, rotation, savedPositions)
            {
                this.flipMode = flipMode;
                turnSpeed = (float)Math.PI / 60;
                texture = Miasma.EntityExtras[19];
                origin = new Vector2(15.5f, 102.5f);
                if(flipMode % 10 == 1)
                {
                    origin.X = 50 - origin.X;
                }
                if (flipMode / 10 == 1)
                {
                    origin.Y = 103 - origin.Y;
                }
                if(savedPositions.Length == 2)
                {
                    turnSpeed = Math.Abs(savedPositions[0] - savedPositions[1]) / openTime;
                }
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                int width = texture.Width / 2;
                int height = texture.Height / 2;
                spriteBatch.Draw(texture, AbsolutePosition(), new Rectangle(width * (flipMode % 10), height * (flipMode / 10), width, height), Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
            }
        }
        private class CenterDoor : Turret
        {
            float scale = 1f;
            public CenterDoor(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
            {
                texture = Miasma.EntityExtras[20];
                origin = new Vector2(34.5f, 69.5f);
            }
            public void ToScale(float goalScale)
            {
                float scaleSpeed = 1f / (float)openTime;
                float dist = goalScale - scale;
                if(Math.Abs(dist) < scaleSpeed)
                {
                    scale = goalScale;
                }
                else
                {
                    scale += (dist > 0 ? 1 : -1) * scaleSpeed;
                }
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, AbsolutePosition(), null, Color.White, AbsoluteRotation(), origin, new Vector2(1, scale), SpriteEffects.None, 0);
            }
        }
        private class ArmsCore : Turret
        {
            public ArmsCore(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
            {
                texture = Miasma.EntityExtras[22];
                origin = new Vector2(9.5f, 9.5f);
            }
        }
        private class LightningAnnihilator : Turret
        {
            
            public LightningAnnihilator(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
            {
                texture = Miasma.EntityExtras[27];
                origin = new Vector2(0, 19);
                turretLength = 46;
            }
        }
    }
}
