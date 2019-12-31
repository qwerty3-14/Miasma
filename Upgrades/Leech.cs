using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Upgrades
{
    public class Leech : Upgrade
    {
        public Leech()
        {
            upgradeID = 10;
            name = "Leech";
            description = "When infected ships die recover health";
        }
        public override void OnDeath(Entity ship)
        {
            if(ship.team == 1)
            {
                Miasma.player.health += 2;
                if(Miasma.player.health > Miasma.player.maxHealth)
                {
                    Miasma.player.health = Miasma.player.maxHealth;
                }
            }
        }
    }
}
