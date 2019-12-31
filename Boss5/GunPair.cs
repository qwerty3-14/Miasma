using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.Boss5
{
    public class GunPair
    {
        AndromedaGunBase[] guns = new AndromedaGunBase[2];
        public GunPair(AndromedaGunBase gun1, AndromedaGunBase gun2)
        {
            guns[0] = gun1;
            guns[1] = gun2;
        }
        public AndromedaGunBase[] GetGuns()
        {
            return guns;
        }
        public bool IsActive()
        {
            return Miasma.gameEntities.Contains(guns[0]) && Miasma.gameEntities.Contains(guns[1]);
        }
    }
}
