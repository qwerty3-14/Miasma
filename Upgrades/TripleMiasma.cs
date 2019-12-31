using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Miasma.Upgrades
{
    public class TripleMiasma : Upgrade
    {
        public TripleMiasma()
        {
            upgradeID = 2;
            name = "Triple Miasma";
            description = "Fire a spread of 3 miasma capsules on firing. Reduces Miasma regeneration by 20%";
        }
        public override void ModifyStats(TheTransmission player)
        {
            player.MiasmaRate -= .2f;
        }
        public override void OnLaunchMiasma(TheTransmission player)
        {
            player.LaunchMiasma((float)Math.PI / 12);
            player.LaunchMiasma(-(float)Math.PI / 12);
        }
    }
}
