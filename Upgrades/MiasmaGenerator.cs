using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Miasma.Upgrades
{
    public class MiasmaGenerator : Upgrade
    {
        public MiasmaGenerator()
        {
            upgradeID = 4;
            name = "Miasma Generator";
            description = "Max miasma incresed by 50%. Miasma regenerates 50% faster.";
        }
        public override void ModifyStats(TheTransmission player)
        {
            player.MiasmaRate += .5f;
            player.MiasmaMaxCapacity += 400;
        }
    }
}
