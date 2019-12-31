using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Projectiles
{
    public abstract class Projectile : Entity
    {
        public int timeLeft = 300;
        List<Entity> ignoreThese = new List<Entity>();
        public Projectile(Vector2 Position, Vector2 velocity, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            this.Velocity = velocity;
            maxHealth = -1; //this is what defines projectiles
            health = 10; //health is used to determine projectile damage instead
        }
        public override void MainUpdate()
        {
            timeLeft--;
            if (timeLeft == 0)
            {
                health = 0;
            }

            for (int i = 0; i < Miasma.gameEntities.Count; i++)
            {
                if (Miasma.gameEntities[i].maxHealth != -1 && Miasma.gameEntities[i].Hitbox.Intersects(Hitbox) && team != Miasma.gameEntities[i].team && !ignoreThese.Contains(Miasma.gameEntities[i]))
                {
                    HitEffects(Miasma.gameEntities[i]);
                    Miasma.gameEntities[i].GetHitEffects(this);
                    Miasma.gameEntities[i].health -= health;
                    if (Miasma.gameEntities[i].health <= 0)
                    {
                        Miasma.gameEntities[i].health = 0;
                        KillEffects(Miasma.gameEntities[i]);
                    }
                    if(Miasma.gameEntities[i].entityID != EntityID.SpartanShield)
                    {
                        health = 0;
                    }
                }
            }

        }
        public virtual void HitEffects(Entity target)
        {

        }
        public virtual void KillEffects(Entity target)
        {

        }
        public void addIgnore(Entity notMe)
        {
            ignoreThese.Add(notMe);
        }
    }
}
