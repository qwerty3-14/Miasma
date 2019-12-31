using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss3
{
    public class Pulsar : Boss
    {
        public List<Block> blocks = new List<Block>();
        BlockGun[] blockGuns = new BlockGun[4];
        MissileLauncher[] launchers = new MissileLauncher[4];
        Deck<int> launchOrder = new Deck<int>();
        void SetupLaunchOrder()
        {
            launchOrder.Clear(); // just in case
            for (int i =0; i < launchers.Length; i ++)
            {
                launchOrder.Add(i);
            }
            launchOrder.Shuffle();
        }
        public Pulsar(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            health = maxHealth = 450;
            entityID = 22;
            name = "The Pulsar";
            int outwardOffset = 10;
            blockGuns[0] = new BlockGun(this, new Vector2(-outwardOffset, -4), new float[] { (float)Math.PI / 2, 3 * (float)Math.PI / 4 });
            blockGuns[1] = new BlockGun(this, new Vector2(outwardOffset, -4), new float[] { (float)Math.PI / 2, 1 * (float)Math.PI / 4 });
            blockGuns[2] = new BlockGun(this, new Vector2(-outwardOffset, -8), new float[] { -(float)Math.PI / 2, -3 * (float)Math.PI / 4 });
            blockGuns[3] = new BlockGun(this, new Vector2(outwardOffset, -8), new float[] { -(float)Math.PI / 2, -1 * (float)Math.PI / 4 });
            launchers[0] = new MissileLauncher(this, new Vector2(-outwardOffset, 0), rotation + (float)Math.PI / 2);
            launchers[1] = new MissileLauncher(this, new Vector2(-outwardOffset+3, 1), rotation + (float)Math.PI / 2);
            launchers[2] = new MissileLauncher(this, new Vector2(outwardOffset, 0),  rotation + (float)Math.PI / 2);
            launchers[3] = new MissileLauncher(this, new Vector2(outwardOffset - 3, 1), rotation + (float)Math.PI / 2);
            SetupLaunchOrder();
        }
        int orbLimiter = 16;
        public float speed = 4f;
        int attackTimer = 0;
        Vector2 restSpot = new Vector2(300, 150);
        Vector2 flyTo = new Vector2(300, 150);
        int loopCounter = 0;
        bool resetHardMissile = true;
        void TeleportToTop()
        {
            resetHardMissile = true;
            loopCounter++;
            Vector2 teleTo = new Vector2(120 + Miasma.random.Next(360), -60);
            Vector2 teleOffset = (teleTo - Position);
            Position += teleOffset;
            foreach(Block block in blocks)
            {
                if(block.team ==0)
                {
                    block.Position += teleOffset;
                }
            }
            if (loopCounter >= 4)
            {
                flyTo = new Vector2(Position.X, 150);
                SetupLaunchOrder();
            }
            else
            {
                ChargePlayer();
            }
        }
        void ChargePlayer()
        {
            flyTo = Position + Functions.PolarVector(1600, Functions.ToRotation(Miasma.player.Position - Position));
        }
        public override void SpecialUpdate()
        {
            speed = 4f + (1f-(float)health / maxHealth) * 4f  + (Miasma.hard ? 1f : 0f);
            if( Miasma.hard && resetHardMissile && Position.Y > 310)
            {
                //new Missile(Position, Functions.PolarVector(4, rotation + (float)Math.PI), rotation + (float)Math.PI, team);
                //new Missile(Position, Functions.PolarVector(4, rotation), rotation, team);
                new Missile(Position, Functions.PolarVector(4, Functions.ToRotation(Miasma.player.Position-Position)), Functions.ToRotation(Miasma.player.Position - Position), team);
                resetHardMissile = false;
            }
            foreach (BlockGun turret in blockGuns)
            {
                turret.UpdateRelativePosition();
            }
            foreach (MissileLauncher turret in launchers)
            {
                turret.UpdateRelativePosition();
            }


            if ((flyTo - Position).Length() < speed)
            {
                Position = flyTo;
                Velocity = Vector2.Zero;
                rotation = 0f;
                attackTimer++;
               
            }
            else
            {
                if (blocks.Count <= 0)
                {
                    loopCounter = 4;
                }
                Velocity = Functions.PolarVector(speed, Functions.ToRotation(flyTo - Position));
                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
            }
            if (attackTimer > 360)
            {
                foreach (BlockGun turret in blockGuns)
                {
                    turret.AimTowardSavedPosition(1);
                }

                if (attackTimer > 480)
                {
                    attackTimer = 0;
                    loopCounter = 0;
                    ChargePlayer();
                }
                else if (attackTimer % 20 == 0 && blocks.Count < orbLimiter-2)
                {
                    foreach (BlockGun turret in blockGuns)
                    {
                        turret.Shoot();
                    }
                }

            }
            else if (attackTimer > 0)
            {

                if (attackTimer > 60)
                {
                    if (launchOrder.Count > 0 && attackTimer % 60 ==0)
                    {
                        
                        launchers[launchOrder[0]].Shoot();
                        launchOrder.RemoveAt(0);
                    }
                }
                else
                {
                    foreach (MissileLauncher turret in launchers)
                    {
                        turret.StickOut();
                    }
                }

            }
            else
            {

                foreach (BlockGun turret in blockGuns)
                {
                    turret.AimTowardSavedPosition(0);
                }
            }
            for (int b = 0; b < blocks.Count; b++)
            {
                if (!Miasma.gameEntities.Contains(blocks[b]))
                {
                    blocks.RemoveAt(b);
                }
            }
            float wallDistance = 50f;
            int myIndex = 0;
            int myInfectedIndex = 0;
            float playerDirection = Functions.ToRotation(Miasma.player.Position - Position);
            int guardBlockCount = 0;
            int infectedGuardCount = 0;
            foreach (Block block in blocks)
            {
                if (block.guarding && block.team == 1)
                {
                    infectedGuardCount++;
                }
                else if (block.guarding)
                {

                    guardBlockCount++;
                }
            }
            foreach (Block block in blocks)
            {
                if(block.guarding && block.team == 1)
                {
                    myInfectedIndex++;
                    block.moveTo = Miasma.player.Position + Functions.PolarVector(wallDistance, playerDirection -5 * (float)Math.PI / 4 + ((float)Math.PI / 2) * ((float)myInfectedIndex / (infectedGuardCount + 1)));
                }
                else if (block.guarding)
                {
                    myIndex++;
                    block.moveTo = Position + Functions.PolarVector(wallDistance, playerDirection - (float)Math.PI/4 + ((float)Math.PI/2)* ((float)myIndex / (guardBlockCount+1)));
                }
            }
            if(Position.Y > 850)
            {
                TeleportToTop();
            }
        }
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntityExtras[11];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            foreach (BlockGun turret in blockGuns)
            {
                turret.Draw(spriteBatch);
            }
            foreach (MissileLauncher turret in launchers)
            {
                turret.Draw(spriteBatch);
            }
            texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            
        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            
        }
        private class BlockGun : Turret
        {
            
            public BlockGun(Pulsar owner, Vector2 relativePosition, float[] savedPositions, float rotation = 0) : base(owner, relativePosition, rotation, savedPositions)
            {
                turnSpeed = (float)Math.PI / 60;
                turretLength = 11;
                texture = Miasma.EntityExtras[12];
                origin = new Vector2(3.5f, 3.5f);
                this.owner = owner;
            }
            public override void Shoot()
            {
                ((Pulsar)owner).blocks.Insert(nativePosition.X > 0 ? 0 : ((Pulsar)owner).blocks.Count, new Block(AbsoluteShootPosition(), Functions.PolarVector(5, AbsoluteRotation()), (Pulsar)owner, rotation - (float)Math.PI / 2, team: owner.team));
            }
        }
        private class MissileLauncher : Turret
        {
            float outAmount = 0;
            float outAmountMax = 6;
            public MissileLauncher(Entity owner, Vector2 relativePosition, float rotation = 0) : base(owner, relativePosition, rotation)
            {
                turnSpeed = (float)Math.PI / 60;
                turretLength = 0;
                texture = Miasma.EntityExtras[13];
                origin = new Vector2(4.5f, 1.5f);
            }
            public void StickOut()
            {
                if(outAmount < outAmountMax)
                {
                    outAmount += .2f;
                }
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, AbsolutePosition() + Functions.PolarVector(outAmount, rotation), null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
            }
            public override void Shoot()
            {
                new Missile(AbsoluteShootPosition() + Functions.PolarVector(outAmount, rotation), Functions.PolarVector(4, AbsoluteRotation()), AbsoluteRotation(),  owner.team);
                outAmount = 0;
            }
        }
    }
}
