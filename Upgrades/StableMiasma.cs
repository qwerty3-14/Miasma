using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Upgrades
{
    public class StableMiasma : Upgrade
    {
        public StableMiasma()
        {
            upgradeID = 9;
            name = "Stable Miasma";
            description = "Infected ships last longer.";
        }
        public override void OnInfect(Entity infected)
        {
            infected.maxHealth = (int)(infected.maxHealth * 1.5f);
            infected.health = infected.maxHealth;
        }
    }
}
