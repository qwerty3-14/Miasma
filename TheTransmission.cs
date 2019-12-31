using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Miasma.Ships;
using Miasma.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Miasma
{
    public class TheTransmission : Entity
    {
        public static int[] playerBoundries = new int[] { 100 + 15, 500 - 15 };
        public int playerHorizontalSpeed = 3;
        int shotTimer = 0;
        int shotTimer2 = 0;
        public int shotCooldown = 30;
        public float MisamaCapacity = 600;
        public float MiasmaRate = 1f;
        public int MiasmaMaxCapacity = 1200;
        public Upgrade[] upgrades= new Upgrade[4];
        public void UpdateStats()
        {
            MiasmaMaxCapacity = 800;
            maxHealth = 20;
            shotCooldown = 30;
            MiasmaRate = 1f;
            playerHorizontalSpeed = 3;
            foreach (Upgrade upgrade in upgrades)
            {
                if(upgrade != null)
                {
                    upgrade.ModifyStats(this);
                }
            }
            health = maxHealth;
        }
        public TheTransmission(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            entityID = 0;
            maxHealth = 30;
            health = 30;
            
        }

        public override void DeathEffects()
        {
            for (int d = 0; d < 32; d++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 3f, Functions.RandomRotation()), Miasma.random.Next(2), 30);
            }
            Sounds.kaboom.Play();
        }


        public void PlayerMovement()
        {
            foreach (Upgrade upgrade in upgrades)
            {
                if (upgrade != null)
                {
                    upgrade.UpdatePlayer(this);
                }
            }
            if (Controls.ControlLeft() && !Controls.ControlRight())
            {
                if (this.Position.X - playerBoundries[0] < playerHorizontalSpeed)
                {
                    this.Position.X = playerBoundries[0];
                    this.Velocity.X = 0;
                }
                else
                {
                    this.Velocity.X = -playerHorizontalSpeed;
                }

            }
            else if (Controls.ControlRight() && !Controls.ControlLeft())
            {
                if (playerBoundries[1] - this.Position.X < playerHorizontalSpeed)
                {
                    this.Position.X = playerBoundries[1];
                    this.Velocity.X = 0;
                }
                else
                {
                    this.Velocity.X = playerHorizontalSpeed;
                }
            }
            else
            {
                this.Velocity.X = 0;
            }
            if (Controls.ControlShoot() && shotTimer ==0)
            {
                shotTimer = shotCooldown;
                
                
                
                    new PewPew(this.Position + new Vector2(6, -4), Vector2.UnitY * -7, team: team, upgrades: upgrades);
                    new PewPew(this.Position + new Vector2(-6, -4), Vector2.UnitY * -7, team: team, upgrades: upgrades);
                
                
            }
            else if(shotTimer>0)
            {
                shotTimer--;
            }
            if(MisamaCapacity < MiasmaMaxCapacity)
            {
                MisamaCapacity += MiasmaRate * .666667f;
            }
            if (Controls.ControlMiasma() && shotTimer2 == 0 && MisamaCapacity > 400)
            {
                MisamaCapacity -= 400;
                shotTimer2 = shotCooldown;
                LaunchMiasma();
                foreach(Upgrade upgrade in upgrades)
                {
                    if(upgrade != null)
                    {
                        upgrade.OnLaunchMiasma(this);
                    }
                }
            }
            else if (shotTimer2 > 0)
            {
                shotTimer2--;
            }
        }
        public void LaunchMiasma(float angle = 0f)
        {
            
            new MiasmaPulse(this.Position + new Vector2(0, -3), Functions.PolarVector(5, -(float)Math.PI/2 + angle), team: team, upgrades: upgrades);
        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            foreach (Upgrade upgrade in upgrades)
            {
                if (upgrade != null)
                {
                    upgrade.TransmissionDrawTasks(spriteBatch);
                }
            }
        }
    }
}
