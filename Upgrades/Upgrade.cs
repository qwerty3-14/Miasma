
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Upgrades
{
    public abstract class Upgrade
    {
        protected int upgradeID = -1;
        protected string name = "Default";
        protected string description = "blah blah";
        public Upgrade()
        {

        }
        float trigCounter = 0f;
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 Position, float scale = 1f, bool selected = false)
        {
            if(selected)
            {
                trigCounter += (float)Math.PI / 60;
            }
            else
            {
                trigCounter = 0f;
            }
            scale *= 1f + (.1f * (float)Math.Sin(trigCounter));
            spriteBatch.Draw(Miasma.UpgradeSprites[upgradeID], Position, null, Color.White, 0f, new Vector2(16f, 16f), new Vector2(scale, scale), SpriteEffects.None, 0f);
        }
        public String GetName()
        {
            return name;
        }
        public String GetDescription()
        {
            return description;
        }
        public virtual void ModifyStats(TheTransmission player)
        {

        }
        public virtual void OnLaunchMiasma(TheTransmission player)
        {

        }
        public virtual void UpdatePlayer(TheTransmission player)
        {

        }
        public virtual void TransmissionDrawTasks(SpriteBatch spriteBatch)
        {

        }
        public virtual void MiasmaShotEffects(MiasmaPulse pulse)
        {

        }
        public virtual void OnMiasmaHit(MiasmaPulse pulse)
        {

        }
        public virtual void PewPewEffects(PewPew pewpew)
        {

        }
        public virtual void OnPewPewHit(PewPew pewpew, Entity hitTarget)
        {

        }
        public virtual void OnInfect(Entity infected)
        {

        }
        public virtual void OnDeath(Entity ship)
        {

        }
        public virtual void InfectedShipUpdate(Entity ship)
        {

        }
        
        public int getId()
        {
            return upgradeID;
        }
    }
}
