using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss5
{
    public class TheAndromeda : Boss
    {
        Deck<AndromedaGunBase> guns = new Deck<AndromedaGunBase>();
        Deck<GunPair> gunPairs= new Deck<GunPair>();
        public AndromedaGunBase[] gunSlots = new AndromedaGunBase[2];
        public TheAndromeda(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            maxHealth = 0;
            health = 0;
            for (int i =0; i < 4; i++)
            {
                guns.Add(new AndromedaGunBase(this, Position, i, 0, team));
                guns.Add(new AndromedaGunBase(this, Position, i, 0, team));
                gunPairs.Add(new GunPair(guns[((i+1) * 2) - 2], guns[((i + 1) * 2)-1]));

            }
            
            foreach(AndromedaGunBase gun in guns)
            {
                maxHealth += gun.maxHealth;
                health += gun.health;
            }
            
            name = "The Andromeda";
            if (Miasma.gameState == GameScene.Combat)
            {
                float shieldOutAmount = 80;
                float shieldSideAmount = 200;
                new AndromedaShield(new Vector2(300, shieldOutAmount), this);
                //new AndromedaShield(new Vector2(300 + shieldSideAmount, shieldOutAmount), this);
                //new AndromedaShield(new Vector2(300 - shieldSideAmount, shieldOutAmount), this);
            }

        }
        public override void UpdateHitbox()
        {
            Hitbox = new Rectangle(0, 0, 0, 0);
        }
        float flyInSpeed = 3f;
        bool lockIn = false;
        Vector2 flyTo;
        int flyTimer =0;
        public override void SpecialUpdate()
        {
            health = 0;
            for (int i = 0; i < guns.Count; i++)
            {
                if (!Miasma.gameEntities.Contains(guns[i]))
                {
                    guns.RemoveAt(i);
                }
            }
            foreach (AndromedaGunBase gun in guns)
            {
                health += gun.health;
            }
            for (int e = 0; e < gunPairs.Count; e++)
            {
                if (!gunPairs[e].IsActive())
                {
                    gunPairs.RemoveAt(e);
                }
            }
            if(Miasma.hard)
            {
                flyTimer++;
                if(flyTimer > 120)
                {
                    flyTo = Vector2.UnitX * Miasma.random.Next(-80, 81);
                    flyTimer = 0;
                }
            }
            if ((Vector2.Zero - Position).Length() < flyInSpeed || lockIn)
            {
                if ((flyTo - Position).Length() < flyInSpeed)
                {
                    Velocity = Vector2.Zero;
                    Position = flyTo;
                }
                else
                {
                    Velocity = Functions.PolarVector(flyInSpeed, Functions.ToRotation(flyTo - Position));
                }
                lockIn = true;
                
                
                if (guns.Count > 1)
                {
                    if (gunPairs.Count == 0 || (gunSlots[0] != null && gunSlots[0].team == 1) || (gunSlots[1] != null && gunSlots[1].team == 1))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (gunSlots[i] == null || !gunSlots[i].active)
                            {
                                guns.Shuffle();
                                gunSlots[i] = (gunSlots[i == 0 ? 1 : 0] != null && gunSlots[i == 0 ? 1 : 0] == guns[0]) ? guns[1] : guns[0];
                                gunSlots[i].active = true;
                                switch (i)
                                {
                                    case 0:
                                        gunSlots[i].SetRelativeX(Miasma.leftSide + 104);
                                        break;
                                    case 1:
                                        gunSlots[i].SetRelativeX(Miasma.rightSide - 104);
                                        break;
                                }
                                gunSlots[i].Position.Y = Position.Y;
                            }


                        }
                    }
                    else
                    {
                        if ((gunSlots[0] == null || !gunSlots[0].active) && (gunSlots[1] == null || !gunSlots[1].active))
                        {
                            gunSlots = gunPairs.Draw(1)[0].GetGuns();
                            for (int i = 0; i < 2; i++)
                            {
                                gunSlots[i].active = true;
                                switch (i)
                                {
                                    case 0:
                                        gunSlots[i].SetRelativeX(Miasma.leftSide + 104);
                                        break;
                                    case 1:
                                        gunSlots[i].SetRelativeX(Miasma.rightSide - 104);
                                        break;
                                }
                                gunSlots[i].Position.Y = Position.Y;
                            }
                        }
                    }
                }
                else if (guns.Count > 0)
                {
                    if (guns[0] == null || !guns[0].active)
                    {
                        guns[0].active = true;
                        guns[0].Position.Y = Position.Y;
                        switch (Miasma.random.Next(2))
                        {
                            case 0:
                                guns[0].SetRelativeX(Miasma.leftSide + 104);
                                break;
                            case 1:
                                guns[0].SetRelativeX(Miasma.rightSide - 104);
                                break;
                        }

                    }
                }

            }
            else
            {
                Velocity = Functions.PolarVector(flyInSpeed, Functions.ToRotation(Vector2.Zero - Position));
            }
        }
        public override void PreDraw(SpriteBatch spriteBatch)
        {

        }
        public override void Draw(SpriteBatch spriteBatch)
        {

        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Miasma.EntityExtras[30], Position + Vector2.UnitX*100 + new Vector2(-47, -78));
        }
        public override void DeathEffects()
        {
            Miasma.cutSceneAndromeda =  new CutSceneAndromeda(Position);
            for (int i = 0; i < Miasma.gameEntities.Count; i++)
            {
                if (!(Miasma.gameEntities[i] is CutSceneAndromeda) && !(Miasma.gameEntities[i] is TheTransmission))
                {
                    Miasma.gameEntities[i].health = 0;
                }
            }
            Miasma.gameState = GameScene.Outro;
            Miasma.player.Velocity = Vector2.Zero;
            Miasma.introIncrimenter = 0;
            Miasma.introDIalougeMessage = null;
            Miasma.toCredits = false;

        }
    }
}
