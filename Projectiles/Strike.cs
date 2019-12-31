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
    public class Strike : Projectile
    {
        bool Miasmic = false;
        public Strike(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0, bool Miasmic = false) : base(Position, velocity, rotation, team)
        {
            Velocity = Vector2.Zero;
            timeLeft = 4;
            maxHealth = -1;
            health = 1;
            this.Miasmic = Miasmic;
        }
        public override void UpdateHitbox()
        {
            Point size = new Point(2, 2);
            Hitbox = new Rectangle((int)(Position.X - size.X / 2), (int)(Position.Y - size.Y / 2), size.X, size.Y);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
        public override void KillEffects(Entity target)
        {
            if (Miasmic)
            {
                target.team = 1;
                target.health = target.maxHealth;
                foreach (Upgrade upgrade in Miasma.player.upgrades)
                {
                    if (upgrade != null)
                    {
                        upgrade.OnInfect(target);
                    }
                }
            }
        }
    }
}
