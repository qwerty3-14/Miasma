using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Miasma.Ships
{
    public class Bomber : Ship
    {
        Bomb myBomb;
        public Bomber(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 14;
            health = maxHealth = 20;
        }
        int countDown = 240;
        public override void ActionStart()
        {

            myBomb = new Bomb(Position + Vector2.UnitY * 6, 0f, team);

        }
        public override void SpecialUpdate()
        {
            if (myBomb != null)
            {
                myBomb.Position.X = Position.X;
                if(Velocity.Y != 0)
                {
                    myBomb.Launch();
                    myBomb = null;
                }
            }
        }
        public override void ActionUpdate()
        {
            NormalMovement();
            if(countDown>0)
            {
                countDown--;
            }
            else
            {
                countDown = 240;
                if(myBomb != null)
                {
                    myBomb.Launch();
                    myBomb = null;
                }
                acting = -1;
            }
        }
        bool starting = true;
        bool infectedCanBomb = false;
        void InfectedNormalMovement()
        {
            Velocity = (home - Position);
            if (Velocity.Length() > speed)
            {

                Velocity.Normalize();
                Velocity *= speed;
                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
                infectedCanBomb = false;
            }
            else
            {
                rotation = Functions.SlowRotation(rotation, (float)Math.PI, (float)Math.PI / 30);
                infectedCanBomb = true;
            }
        }
        public override void InfectedUpdate()
        {
            InfectedNormalMovement();
            if (starting)
            {
                if(myBomb != null)
                {
                    myBomb.Launch();
                    myBomb = null;
                }
                home = new Vector2(Miasma.random.Next(Miasma.leftSide + 50, Miasma.rightSide-50), 750);
                starting = false;
            }
            if(infectedCanBomb)
            {
                if(countDown == 60)
                {
                    myBomb = new Bomb(Position - Vector2.UnitY * 6, 0f, team);
                }
                if (countDown > 0)
                {
                    countDown--;
                }
                else
                {
                    countDown = 120;
                    if (myBomb != null)
                    {
                        myBomb.Launch();
                        health -= 5;
                        myBomb = null;
                    }
                }
            }
        }
        public override void ExtraDeathEffects()
        {
            if (myBomb != null)
            {
                myBomb.Explode();
                myBomb = null;
            }
        }
        

    }
}
