using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Miasma.Upgrades
{
    class Armor : Upgrade
    {
        public Armor()
        {
            upgradeID = 1;
            name = "Armor";
            description = "50% increased max health";
        }
        public override void ModifyStats(TheTransmission player)
        {
            player.maxHealth += 10;
        }
    }
}
