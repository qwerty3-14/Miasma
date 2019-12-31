using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Miasma.Upgrades
{
    public class FirerateUp : Upgrade
    {
        public FirerateUp()
        {
            upgradeID = 0;
            name = "Rapid Fire";
            description = "Shoot faster";
        }
        public override void ModifyStats(TheTransmission player)
        {
            player.shotCooldown -= 5;
        }
    }
}
