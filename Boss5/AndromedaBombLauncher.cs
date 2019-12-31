using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Ships;
using Microsoft.Xna.Framework;

namespace Miasma.Boss5
{
    public class AndromedaBombLauncher : AndromedaTurret
    {
        Bomb[] myBombs = new Bomb[3];
        Deck<int> bombOrder = new Deck<int>();
        public AndromedaBombLauncher(Entity owner, Vector2 relativePosition, float rotation = 0, float[] savedPositions = null) : base(owner, relativePosition, rotation, savedPositions)
        {
            turnSpeed = (float)Math.PI / 120;
            turretLength = 15.5f;
            texture = Miasma.EntityExtras[34];
            origin = new Vector2(40f, 26.5f);
            bombOrder.Add(0);
            bombOrder.Add(1);
            bombOrder.Add(2);
        }
        int counter = 0;
        public override void OnStart()
        {
            BombReset();
            
        }
        void BombReset()
        {
            for (int i = 0; i < 3; i++)
            {
                myBombs[i] = new Bomb(AbsolutePosition(), 0, owner.team);
            }
            bombOrder.Shuffle();
            counter = 0;
        }
        public override void UpdateRelativePosition(Vector2? move = null)
        {
            for(int i =0; i < 3; i++)
            {
                if(myBombs[i] != null)
                {
                    myBombs[i].Position = AbsolutePosition() + Functions.PolarVector(turretLength, AbsoluteRotation()) + Functions.PolarVector(-17 + 17 * i, AbsoluteRotation() + (float)Math.PI / 2);
                }
            }
            if (move != null)
            {
                nativePosition = (Vector2)move;
            }
            relativePosition = Functions.PolarVector(nativePosition.X, owner.rotation) + Functions.PolarVector(nativePosition.Y, owner.rotation + (float)Math.PI / 2);
        }
        public override void ActingUpdate()
        {
            counter++;
            int cooldown = 30;
            int minTime = 60;
            int maxTime = 120;
            if (counter % cooldown == 0 && counter >= minTime && counter <= maxTime)
            {
                myBombs[bombOrder[(counter- minTime) / cooldown]].Launch();
                myBombs[bombOrder[(counter - minTime) / cooldown]] = null;
            }
            /*
            if(counter > 150 & Miasma.hard)
            {
                if( ((AndromedaGunBase)owner).active && ((AndromedaGunBase)owner).actionTimer < ((AndromedaGunBase)owner).actionTime-10)
                {
                    ((AndromedaGunBase)owner).actionTimer = ((AndromedaGunBase)owner).actionTime - 10;
                }
            }*/
            
        }
        public override void InfectingUpdate()
        {
            owner.health = 0;
            for (int i = 0; i < 3; i++)
            {
                if (myBombs[i] != null)
                {
                    new Bomber(AbsolutePosition() + Functions.PolarVector(turretLength/2, AbsoluteRotation()) + Functions.PolarVector(-17 + 17 * i, AbsoluteRotation() + (float)Math.PI / 2), Vector2.Zero, 0, 1);
                }
            }
        }
    }
}
