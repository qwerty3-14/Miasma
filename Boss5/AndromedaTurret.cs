using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Boss5
{
    public abstract class AndromedaTurret : Turret
    {
        public AndromedaTurret(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
        {
           
        }
        public virtual void ActingUpdate()
        {

        }
        public virtual void InfectingUpdate()
        {

        }
        public virtual void OnStart()
        {

        }
    }
}
