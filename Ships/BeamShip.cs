using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Ships
{
    public class BeamShip : Ship
    {
        public BeamShip(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0) : base(Position, Home, rotation, team)
        {
            entityID = 7;
            maxHealth = health = 20;
            Miasma.BeamShips.Add(this);
        }
        int spinTime = 60;
        protected BeamShip partner = null;
        Vector2 startAt = new Vector2(-100, -100);
        bool readyToBeam = false;
        bool canStrike = false;
        public override void ActionStart()
        {
            canStrike = true;
            startAt = new Vector2(-100, -100);
            spinTime = 60;
            readyToBeam = false;
        }
        public override void SpecialUpdate()
        {
            if (partner != null && partner.team != team)
            {
                partner = null;
            }
        }
        void laserWarmup()
        {
            if (Miasma.random.Next(3) == 0)
            {
                new Particle(Position, Functions.PolarVector(3, rotation + (float)Math.PI / 2 + Functions.RandomRotation() / 36f), 4 + Miasma.random.Next(2), 10);
            }
        }
        bool Align()
        {

            rotation = Functions.SlowRotation(rotation, Functions.ToRotation(partner.Position - Position) - (float)Math.PI / 2, (float)Math.PI / 60);
            return rotation == Functions.SlowRotation(rotation, Functions.ToRotation(partner.Position - Position) - (float)Math.PI / 2, (float)Math.PI / 60);
        }
        public override void ActionUpdate()
        {
            if (spinTime > 0)
            {
                Velocity = Vector2.Zero;
                spinTime--;
                if (partner == null)
                {
                    foreach (BeamShip ship in Miasma.BeamShips)
                    {
                        if ((!ship.IsActing() || Miasma.BeamShips.Count < 3) && ship.partner == null && ship != this)
                        {
                            partner = ship;
                            ship.partner = this;
                            ship.Act(30);
                            break;
                        }
                    }
                }
                rotation += (float)Math.PI / 15;
            }
            else
            {
                if (!Miasma.BeamShips.Contains(partner))
                {
                    partner = null;
                }
                if (partner == null)
                {
                    acting = -1;
                }
                else if(readyToBeam && partner.readyToBeam)
                {
                    Velocity = Vector2.UnitY*4;
                    if (canStrike && partner != null)
                    {
                        Rectangle BeamBox = new Rectangle((int)Position.X, (int)Position.Y - 1, (int)partner.Position.X - (int)Position.X, 3);
                        new Particle(Position + Vector2.UnitX * Miasma.random.Next(BeamBox.Width), Vector2.Zero, 4 + Miasma.random.Next(2), 30);
                        if (BeamBox.Intersects(Miasma.player.Hitbox))
                        {
                            Miasma.player.Strike(10);
                            canStrike = false;
                        }
                        Sounds.beam.PlayContinuous();
                    }
                    if (Position.Y > 900)
                    {
                        Position.Y = -100;
                        acting = -1;
                        partner = null;
                        
                        
                    }
                }
                else
                {
                    laserWarmup();
                    if (Miasma.BeamShips.IndexOf(this) > Miasma.BeamShips.IndexOf(partner))
                    {
                        canStrike = false;
                        if(startAt == new Vector2(-100, -100))
                        {
                            startAt = new Vector2(340 + Miasma.random.Next(120), 30);
                            partner.startAt = startAt - Vector2.UnitX * 200 + Vector2.UnitY;
                        }
                    }
                    if(startAt != new Vector2(-100, -100))
                    {
                        if(Position != startAt)
                        {
                            if((Position -startAt).Length() < speed)
                            {
                                Position = startAt;
                                Velocity = Vector2.Zero;
                            }
                            else
                            {
                                Velocity = Functions.PolarVector(3, Functions.ToRotation(startAt - Position));
                                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
                            }
                            
                        }
                        else
                        {
                            Velocity = Vector2.Zero;
                            if(Align())
                            {
                                
                                readyToBeam = true;
                            }
                        }
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(partner != null && Miasma.gameEntities.IndexOf(this) < Miasma.gameEntities.IndexOf(partner) && (readyToBeam || infectedReadyToBeam) && (partner.readyToBeam || partner.infectedReadyToBeam) && Position.Y < 820)
            {
                for(int x =0; x < (partner.Position - Position).Length(); x++)
                {
                    spriteBatch.Draw(Miasma.EntityExtras[1], Position + Functions.PolarVector(x, Functions.ToRotation(partner.Position - Position)), new Rectangle(0, 0, 1, 3), Color.White, Functions.ToRotation(partner.Position - Position) , new Vector2(1, 3) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
                }
            }
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        int infectedSpinTime = 180;
        //protected BeamShip infectedPartner = null;
        Vector2 infectedStartAt = new Vector2(-100, -100);
        bool infectedReadyToBeam = false;
        List<Ship> strikedThese = new List<Ship>();
        public override void InfectedUpdate()
        {
            acting = -1;
            
            if(!Miasma.InfectedBeamShips.Contains(this))
            {
                Miasma.InfectedBeamShips.Add(this);
            }
            if (infectedSpinTime > 0)
            {
                Velocity = Vector2.Zero;
                infectedSpinTime--;
                if (partner == null)
                {
                    foreach (BeamShip ship in Miasma.InfectedBeamShips)
                    {
                        if ((!ship.IsActing() || Miasma.InfectedBeamShips.Count < 3) && ship.partner == null && ship != this)
                        {
                            partner = ship;
                            ship.partner = this;
                            break;
                        }
                    }
                }
                rotation += (float)Math.PI / 15;
            }
            else
            {
                if (!Miasma.InfectedBeamShips.Contains(partner))
                {
                    partner = null;
                }
                if (partner == null)
                {
                    health = 0;
                }
                else if (infectedReadyToBeam && partner.infectedReadyToBeam)
                {
                    /////////////////////////
                    Velocity = Vector2.UnitX * 4;
                    if (partner != null && Miasma.InfectedBeamShips.IndexOf(this) > Miasma.InfectedBeamShips.IndexOf(partner))
                    {
                        Rectangle BeamBox = new Rectangle((int)Position.X-1, (int)Position.Y, 3, (int)partner.Position.Y - (int)Position.Y);
                        new Particle(Position + Vector2.UnitY * Miasma.random.Next(BeamBox.Height), Vector2.Zero, 4 + Miasma.random.Next(2), 30);
                        Sounds.beam.PlayContinuous();
                        foreach (Ship ship in Miasma.enemyFleet)
                        {
                            if (!strikedThese.Contains(ship) &&BeamBox.Intersects(ship.Hitbox))
                            {
                                strikedThese.Add(ship);
                                ship.Strike(20);

                            }
                        }
                        
                    }
                    if (Position.X > Miasma.rightSide+100)
                    {
                        health = 0;
                        partner = null;
                    }
                }
                else
                {
                    laserWarmup();
                    if (Miasma.InfectedBeamShips.IndexOf(this) > Miasma.InfectedBeamShips.IndexOf(partner))
                    {
                        
                        if (infectedStartAt == new Vector2(-100, -100))
                        {
                            infectedStartAt = new Vector2(Miasma.leftSide + 30, 30);
                            partner.infectedStartAt = new Vector2(infectedStartAt.X, Miasma.lowerBoundry);
                        }
                    }
                    if (infectedStartAt != new Vector2(-100, -100))
                    {
                        if (Position != infectedStartAt)
                        {
                            if ((Position - infectedStartAt).Length() < speed)
                            {
                                Position = infectedStartAt;
                                Velocity = Vector2.Zero;
                            }
                            else
                            {
                                Velocity = Functions.PolarVector(3, Functions.ToRotation(infectedStartAt - Position));
                                rotation = Functions.ToRotation(Velocity) - (float)Math.PI / 2;
                            }

                        }
                        else
                        {
                            Velocity = Vector2.Zero;
                            if (Align())
                            {
                                
                                
                                infectedReadyToBeam = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
