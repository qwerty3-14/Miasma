using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Ships
{
    public class MediumCharger : LightCharger
    {
        public MediumCharger(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 13;
            health = maxHealth = 40;
        }
        bool looped =false;
        public override void OffScreen()
        {
            if(looped)
            {
                looped = false;
                if (team == 0)
                {
                    Position.Y = 0;
                    
                    acting = -1;
                }
                if (team == 1)
                {
                    health = 0;
                }

            }
            else
            {
                looped = true;
                if (team == 0)
                {
                    Position.Y = 0;
                    rotation = Functions.ToRotation(Miasma.player.Position - Position) - (float)Math.PI / 2;
                    Velocity = Functions.PolarVector(12, rotation + (float)Math.PI / 2);
                }
                if (team == 1)
                {
                    ResetStrikes();
                    Position -= Velocity * 3;
                    float shootAt = (float)Math.PI / 2;
                    Entity closest = null;

                    if (InfectedTargeting(ref closest))
                    {
                        shootAt = Functions.ToRotation(closest.Position - Position);
                        rotation = shootAt - (float)Math.PI / 2;
                    }
                }
            }
            
        }
    }
}
