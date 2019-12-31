using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma
{
    public class Particle
    {
        int time;
        public Vector2 position;
        Vector2 velocity;
        public int type;
        public Particle(Vector2 position, Vector2 velocity, int type, int time = 60)
        {
            Miasma.gameParticles.Add(this);
            this.position = position;
            this.velocity = velocity;
            this.type = type;
            this.time = time;
        }
        public void ParticleUpdate()
        {
            position += velocity;
            time--;
            if(time<=0)
            {
                Miasma.gameParticles.Remove(this);
            }
        }
    }
}
