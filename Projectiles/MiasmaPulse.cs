using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Projectiles
{
    public class MiasmaPulse : Projectile
    {
        Upgrade[] upgrades = null;
        public MiasmaPulse(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0, Upgrade[] upgrades = null) : base(Position, velocity, rotation, team)
        {
            maxHealth = -1;
            health = 20;
            entityID = 1;
            this.upgrades = upgrades;
            Sounds.launchMisc.Play();
        }
        public override void SpecialUpdate()
        {
            if(team == 0)
            {
                new ArtillaryPulse(Position, Velocity, rotation, team);
                Miasma.gameEntities.Remove(this);
            }
            if (Miasma.random.Next(2)==0)
            {
                new Particle(Position + Vector2.UnitX * Miasma.random.Next(-2, 3), Vector2.Zero, Miasma.random.Next(2), 6);
            }
            foreach (Upgrade upgrade in upgrades)
            {
                if (upgrade != null)
                {
                    upgrade.MiasmaShotEffects(this);
                }
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Miasma.MiasmaColor(), rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public override void HitEffects(Entity target)
        {
            for(int i =0; i <8; i++)
            {
                new Particle(Position, Functions.PolarVector((float)Miasma.random.NextDouble() * 6f, Functions.RandomRotation()), Miasma.random.Next(2), 45);
            }
            foreach (Upgrade upgrade in upgrades)
            {
                if (upgrade != null)
                {
                    upgrade.OnMiasmaHit(this);
                }
            }
        }
        public override void KillEffects(Entity target)
        {
            target.team = 1;
            target.health = target.maxHealth;
            foreach (Upgrade upgrade in upgrades)
            {
                if (upgrade != null)
                {
                    upgrade.OnInfect(target);
                }
            }
            Sounds.infect.Play();
        }
    }
}
