using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Miasma.Upgrades
{
    public class Speed : Upgrade
    {
        public Speed()
        {
            upgradeID = 6;
            name = "Thrusters";
            description = "Move faster";
        }
        public override void ModifyStats(TheTransmission player)
        {
            player.playerHorizontalSpeed += 1;
        }
    }
}
