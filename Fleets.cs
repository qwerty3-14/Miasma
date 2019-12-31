using Miasma.Ships;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma
{
    public class Fleet
    {
        List<Vector2> StartPositions = new List<Vector2>();
        List<Vector2> Homes = new List<Vector2>();
        List<int> ShipIDs = new List<int>();
        int intensity = 2;
        int carrierId = 0;
        List<int> carrierDistanceModifiers = new List<int>();
        List<int> childSpreads = new List<int>();
        public Fleet()
        {

        }
        public void LoadFleet()
        {
            carrierId = 0;
            for (int s = 0; s < ShipIDs.Count; s++)
            {
                switch (ShipIDs[s])
                {
                    case EntityID.LightGunship:
                        Miasma.enemyFleet.Add(new LightGunship(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.LightCharger:
                        Miasma.enemyFleet.Add(new LightCharger(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.LightArtillary:
                        Miasma.enemyFleet.Add(new LightArtillary(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.BeamShip:
                        Miasma.enemyFleet.Add(new BeamShip(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.MediumGunship:
                        Miasma.enemyFleet.Add(new MediumGunship(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.TeleportingFighter:
                        Miasma.enemyFleet.Add(new TeleportingFighter(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.MediumCharger:
                        Miasma.enemyFleet.Add(new MediumCharger(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.Bomber:
                        Miasma.enemyFleet.Add(new Bomber(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.HelixBuilder:
                        Miasma.enemyFleet.Add(new HelixBuilder(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.MediumArtillary:
                        Miasma.enemyFleet.Add(new MediumArtillary(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.Carrier:
                        Miasma.enemyFleet.Add(new Carrier(StartPositions[s], Homes[s], childrenHomeDistanceModifier: carrierDistanceModifiers[carrierId], childSpread: childSpreads[carrierId]));
                        carrierId++;
                        break;
                    case EntityID.Spinner:
                        Miasma.enemyFleet.Add(new Spinner(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.EliteGunship:
                        Miasma.enemyFleet.Add(new EliteGunship(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.Spartan:
                        Miasma.enemyFleet.Add(new Spartan(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.RuthlessCharger:
                        Miasma.enemyFleet.Add(new RuthlessCharger(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.Cruiser:
                        Miasma.enemyFleet.Add(new Cruiser(StartPositions[s], Homes[s]));
                        break;
                    case EntityID.BombardmentArtillary:
                        Miasma.enemyFleet.Add(new BombardmentArtillary(StartPositions[s], Homes[s]));
                        break;
                }
            }
            Miasma.currentWaveIntensity = intensity * (Miasma.hard ? 2 : 1);
        }
        public void SingleShip(Vector2 here, int id)
        {
            this.StartPositions.Add(here + Functions.PolarVector(400, (float)Miasma.random.NextDouble() * -(float)Math.PI));
            this.Homes.Add(here);
            AddToShipIds(id);
        }
        public void AddToShipIds(int ID, int carrierDistanceModifier = 0, int childSpread = 0)
        {
            if (ID == EntityID.Carrier)
            {
                carrierDistanceModifiers.Add(carrierDistanceModifier);
                childSpreads.Add(childSpread);
            }
            this.ShipIDs.Add(ID);
        }
        public void AddRow(float y, int count, int id, int carrierDistanceModifier = 0)
        {
            int[] ships = new int[count];
            for(int i = 0; i < count; i++)
            {
                ships[i] = id;
            }
            AddRowFromArray(y, ships, carrierDistanceModifier);

        }
        public void AddSmallRow(float y, float start, float width, int count, int id)
        {
            for (int x = 0; x < count; x++)
            {
                float HorizontalAlign = (x + 1) * (width / (float)(count + 1)) + start;
                float VerticalAlign = y;
                SingleShip(new Vector2(HorizontalAlign, VerticalAlign), id);
            }
        }
        public void AddRowFromArray(float y, int[] shipList, int carrierDistanceModifier = 0)
        {
            for (int x = 0; x < shipList.Length; x++)
            {

                float HorizontalAlign = (x + 1) * (400f / (float)(shipList.Length + 1)) + Miasma.leftSide;
                float VerticalAlign = y;
                this.StartPositions.Add(new Vector2(HorizontalAlign, VerticalAlign) + Functions.PolarVector(400, (float)Miasma.random.NextDouble() * -(float)Math.PI));
                this.Homes.Add(new Vector2(HorizontalAlign, VerticalAlign));
                AddToShipIds(shipList[x], carrierDistanceModifier, 400 / shipList.Length );


            }
        }
        public void AddCircle(Vector2 center, float radius, int count, int id)
        {
            for (int r = 0; r < count; r++)
            {
                Vector2 here = center + Functions.PolarVector(radius, r * (float)Math.PI * 2 / (float)count);
                this.StartPositions.Add(here +  Functions.PolarVector(400, (float)Miasma.random.NextDouble() * -(float)Math.PI));
                this.Homes.Add(here);
                AddToShipIds(id);

            }
        }
        public void AddLine(Vector2 start, Vector2 end, int count, int id, int endShips = -1)
        {
            Vector2 here;
            
            Vector2 incriment = (end - start) / (float)(count+1);
            for(int k = 0; k <= count+1; k++)
            {
                here = start + incriment * k;
                if((k == 0 || k == count + 1) && endShips !=-1)
                {
                    if (endShips != 0)
                    {
                        this.StartPositions.Add(here + Functions.PolarVector(400, (float)Miasma.random.NextDouble() * -(float)Math.PI));
                        this.Homes.Add(here);
                        AddToShipIds(endShips);
                    }
                }
                else
                {
                    this.StartPositions.Add(here + Functions.PolarVector(400, (float)Miasma.random.NextDouble() * -(float)Math.PI));
                    this.Homes.Add(here);
                    AddToShipIds(id);
                }
                
            }

        }
        public void Polygon(Vector2 center, int sides, float size,  int shipsPerSide, int id, float rotation =0f, int cornerShips = -1)
        {
            Vector2 here;
            if (cornerShips == -1)
            {
                cornerShips = id;
            }
            for(int s =0; s < sides; s++)
            {
                float angle = (float)Math.PI * 2 *((float)s / (float)sides) + rotation;
                here = center + Functions.PolarVector(size, angle);
                if (cornerShips != 0)
                {
                    
                    this.StartPositions.Add(here + Functions.PolarVector(400, (float)Miasma.random.NextDouble() * -(float)Math.PI));
                    this.Homes.Add(here);
                    AddToShipIds(cornerShips);
                }
                float nextAngle = (float)Math.PI * 2 * ((float)(s+1) / (float)sides) + rotation;
                this.AddLine(here, center +Functions.PolarVector(size, nextAngle), shipsPerSide, id, 0);



            }
        }
        public static Fleet[,] gameFleets = new Fleet[10, 5];
        public static void CreateFleets()
        {
            for (int i = 0; i < gameFleets.GetLength(0); i++)
            {
                for (int j = 0; j < gameFleets.GetLength(1); j++)
                {
                    gameFleets[i, j] = new Fleet();
                }
            }
            // wave 1-1

            for (int y = 0; y < 4; y++)
            {
                gameFleets[0, 0].AddRow(y * 40 + 100, 12, EntityID.LightGunship);

            }
            gameFleets[0, 0].intensity = 2;
            // wave 1-2
            for (int y = 0; y < 4; y++)
            {
                gameFleets[1, 0].AddRow(y * 40 + 100, 12, y < 2 ? EntityID.LightCharger : EntityID.LightGunship);

            }
            gameFleets[1, 0].intensity = 2;
            // wave 1-3
            for (int y = 0; y < 4; y++)
            {
                int[] row = { EntityID.LightCharger, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightCharger };
                gameFleets[2, 0].AddRowFromArray(y * 40 + 100, row);

            }
            gameFleets[2, 0].AddRow(60, 3, EntityID.LightArtillary);

            gameFleets[2, 0].intensity = 2;
            // wave 1-4

            Vector2 center;
            for (int c = 0; c < 3; c++)
            {
                center = new Vector2((c + 1) * 100 + 100, c % 2 == 0 ? 100 : 240);
                gameFleets[3, 0].StartPositions.Add(center - Vector2.UnitY * 400);
                gameFleets[3, 0].Homes.Add(center);
                gameFleets[3, 0].ShipIDs.Add(EntityID.LightArtillary);
                gameFleets[3, 0].AddCircle(center, 80, 10, EntityID.LightGunship);
                gameFleets[3, 0].AddCircle(center, 40, 5, EntityID.LightGunship);

            }
            gameFleets[3, 0].intensity = 2;
            // wave 1-5

            for (int y = 0; y < 4; y++)
            {
                gameFleets[4, 0].AddRow(y * 40 + 100, (y % 2 == 0 ? 10 : 12), EntityID.LightGunship);
            }
            gameFleets[4, 0].AddRow(60, 4, EntityID.BeamShip);

            gameFleets[4, 0].intensity = 2;

            // wave 1-6

            gameFleets[5, 0].Polygon(new Vector2(300, 200), 3, 200, 8, EntityID.LightCharger, (float)Math.PI / 2);
            gameFleets[5, 0].Polygon(new Vector2(300, 200), 3, 120, 4, EntityID.BeamShip, (float)Math.PI / 2);
            gameFleets[5, 0].Polygon(new Vector2(300, 200), 3, 40, 0, EntityID.LightArtillary, (float)Math.PI / 2);
            gameFleets[5, 0].intensity = 2;

            // wave 1-7
            gameFleets[6, 0].Polygon(new Vector2(300, 200), 4, 100, 1, EntityID.LightArtillary, (float)Math.PI / 4, 0);
            gameFleets[6, 0].AddRow(200, 1, EntityID.LightArtillary);
            gameFleets[6, 0].intensity = 2;

            // wave 1-8
            gameFleets[7, 0].AddRow(120, 6, EntityID.MediumGunship);
            for (int y = 0; y < 3; y++)
            {
                gameFleets[7, 0].AddRow(y * 40 + 140, 12, EntityID.LightGunship);

            }
            gameFleets[7, 0].intensity = 2;

            //wave 1-9
            for (int y = 0; y < 4; y++)
            {
                gameFleets[8, 0].AddRow(y * 40 + 100, (y % 2 == 0 ? 10 : 12), EntityID.BeamShip);
            }
            gameFleets[8, 0].intensity = 2;

            //wave 1-10
            int[] ships = { EntityID.BeamShip, EntityID.LightArtillary, EntityID.BeamShip, EntityID.LightArtillary, EntityID.BeamShip, EntityID.LightArtillary, EntityID.BeamShip, EntityID.LightArtillary, EntityID.BeamShip };
            gameFleets[9, 0].AddRowFromArray(60, ships);
            gameFleets[9, 0].AddRow(120, 12, EntityID.LightGunship);
            gameFleets[9, 0].AddRow(180, 8, EntityID.MediumGunship);
            gameFleets[9, 0].AddRow(240, 12, EntityID.LightGunship);
            gameFleets[9, 0].AddRow(300, 10, EntityID.LightCharger);
            gameFleets[9, 0].intensity = 2;

            //wave 2-1

            Vector2 cent = new Vector2(300, 240);
            gameFleets[0, 1].Polygon(cent, 3, 180, 6, EntityID.LightGunship, (float)Math.PI / 2);
            gameFleets[0, 1].Polygon(cent, 3, 90, 3, EntityID.TeleportingFighter, -(float)Math.PI / 2, 0);

            gameFleets[0, 1].intensity = 3;

            //wave 2-2
            for (int y = 0; y < 4; y++)
            {
                gameFleets[1, 1].AddRow(y * 40 + 100, (y % 2 == 0 ? 10 : 12), EntityID.TeleportingFighter);
            }
            gameFleets[1, 1].intensity = 3;

            //wave 2-3
            gameFleets[2, 1].Polygon(new Vector2(300, 240), 7, 160, 3, EntityID.TeleportingFighter, cornerShips: EntityID.MediumGunship);
            gameFleets[2, 1].AddCircle(new Vector2(300, 240), 50, 3, EntityID.LightArtillary);
            gameFleets[2, 1].intensity = 3;

            // wave 2-4
            gameFleets[3, 1].AddRow(120, 6, EntityID.MediumCharger);
            for (int y = 0; y < 3; y++)
            {
                gameFleets[3, 1].AddRow(y * 40 + 140, 12, EntityID.LightCharger);

            }
            gameFleets[3, 1].intensity = 3;
            // wave 2-5
            gameFleets[4, 1].AddRow(120, 6, EntityID.MediumCharger);
            gameFleets[4, 1].AddRow(160, 6, EntityID.MediumGunship);
            gameFleets[4, 1].AddRow(200, 12, EntityID.LightCharger);
            gameFleets[4, 1].AddRow(240, 12, EntityID.LightGunship);
            gameFleets[4, 1].intensity = 3;

            // wave 2-6
            gameFleets[5, 1].Polygon(new Vector2(200, 100), 4, 70, 2, EntityID.LightGunship);
            gameFleets[5, 1].Polygon(new Vector2(400, 100), 4, 70, 2, EntityID.LightCharger);
            gameFleets[5, 1].AddCircle(new Vector2(200, 100), 0, 1, EntityID.MediumGunship);
            gameFleets[5, 1].AddCircle(new Vector2(400, 100), 0, 1, EntityID.MediumCharger);
            gameFleets[5, 1].Polygon(new Vector2(300, 220), 4, 120, 3, EntityID.TeleportingFighter);
            gameFleets[5, 1].Polygon(new Vector2(300, 220), 4, 60, 1, EntityID.BeamShip);
            gameFleets[5, 1].Polygon(new Vector2(200, 340), 4, 70, 2, EntityID.LightCharger);
            gameFleets[5, 1].Polygon(new Vector2(400, 340), 4, 70, 2, EntityID.LightGunship);
            gameFleets[5, 1].AddCircle(new Vector2(200, 340), 0, 1, EntityID.MediumCharger);
            gameFleets[5, 1].AddCircle(new Vector2(400, 340), 0, 1, EntityID.MediumGunship);
            gameFleets[5, 1].intensity = 3;

            //wave 2-7
            gameFleets[6, 1].intensity = 3;
            gameFleets[6, 1].AddRow(150, 10, EntityID.Bomber);

            //wave 2-8
            gameFleets[7, 1].AddRow(120, 6, EntityID.MediumCharger);
            gameFleets[7, 1].AddRow(160, 8, EntityID.Bomber);
            gameFleets[7, 1].AddRow(200, 10, EntityID.TeleportingFighter);
            gameFleets[7, 1].intensity = 3;

            //wave 2-9
            gameFleets[8, 1].AddRow(120, 10, EntityID.BeamShip);
            gameFleets[8, 1].AddRow(160, 10, EntityID.Bomber);
            gameFleets[8, 1].AddRow(200, 10, EntityID.Bomber);
            gameFleets[8, 1].intensity = 3;

            //wave 2-10
            int[] ships2 = { EntityID.Bomber, EntityID.LightArtillary, EntityID.Bomber, EntityID.LightArtillary, EntityID.Bomber, EntityID.LightArtillary, EntityID.Bomber, EntityID.LightArtillary, EntityID.Bomber };
            gameFleets[9, 1].AddRowFromArray(60, ships2);
            int[] ships3 = { EntityID.MediumCharger, EntityID.MediumGunship, EntityID.MediumCharger, EntityID.MediumGunship, EntityID.MediumCharger, EntityID.MediumGunship, EntityID.MediumCharger, EntityID.MediumGunship, EntityID.MediumCharger };
            gameFleets[9, 1].AddRowFromArray(120, ships3);
            gameFleets[9, 1].AddRow(180, 6, EntityID.BeamShip);
            gameFleets[9, 1].AddRow(240, 12, EntityID.LightCharger);
            gameFleets[9, 1].AddRow(300, 12, EntityID.TeleportingFighter);
            gameFleets[9, 1].AddRow(340, 12, EntityID.LightGunship);
            gameFleets[9, 1].intensity = 3;

            //wave 3-1
            
            gameFleets[0, 2].intensity = 3;
            gameFleets[0, 2].AddRow(100, 6, EntityID.HelixBuilder);
            gameFleets[0, 2].AddRowFromArray(150, new int[] { EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.MediumGunship, EntityID.MediumGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship, EntityID.LightGunship });
            gameFleets[0, 2].AddRow(180, 8, EntityID.LightGunship);
            
           
            //wave 3-2

            gameFleets[1, 2].intensity = 3;
            gameFleets[1, 2].AddRowFromArray(80, new int[] { EntityID.HelixBuilder, EntityID.LightArtillary, EntityID.HelixBuilder, EntityID.LightArtillary, EntityID.HelixBuilder, EntityID.LightArtillary, EntityID.HelixBuilder, EntityID.LightArtillary, EntityID.HelixBuilder });
            gameFleets[1, 2].Polygon(new Vector2(200, 160), 5, 50, 1, EntityID.LightCharger, (float)Math.PI / 2);
            gameFleets[1, 2].Polygon(new Vector2(400, 160), 5, 50, 1, EntityID.LightCharger, (float)Math.PI / 2);
            gameFleets[1, 2].Polygon(new Vector2(300, 150), 3, 20, 0, EntityID.MediumCharger, (float)Math.PI / 2);

            //wave 3-3

            gameFleets[2, 2].intensity = 3;
            for (int y = 0; y < 4; y++)
            {
               
                gameFleets[2, 2].AddRowFromArray(y * 40 + 100, new int[] { 0, 0, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.TeleportingFighter, 0, 0 });

            }
            gameFleets[2, 2].AddRow(60, 5, EntityID.LightArtillary);
            gameFleets[2, 2].AddLine(new Vector2(140, 100), new Vector2(140, 220), 1, EntityID.HelixBuilder);
            gameFleets[2, 2].AddLine(new Vector2(460, 100), new Vector2(460, 220), 1, EntityID.HelixBuilder);
            //wave 3-4
            gameFleets[3, 2].AddRowFromArray(100, new int[] { EntityID.LightArtillary, EntityID.LightArtillary, EntityID.MediumArtillary, EntityID.LightArtillary, EntityID.LightArtillary });
            gameFleets[3, 2].AddRow(150, 3, EntityID.LightArtillary);
            gameFleets[3, 2].AddRow(200, 1, EntityID.LightArtillary);
            gameFleets[3, 2].intensity = 3;

            //wave 3-5
            gameFleets[4, 2].intensity = 3;
            gameFleets[4, 2].AddRow(100, 5, EntityID.Carrier);
            gameFleets[4, 2].AddRow(150, 5, EntityID.MediumGunship);

            //wave 3-6
            gameFleets[5, 2].intensity = 3;
            gameFleets[5, 2].AddRow(100, 6, EntityID.Carrier, 60);
            gameFleets[5, 2].Polygon(new Vector2(190, 200), 4, 30, 0, EntityID.Bomber, (float)Math.PI / 4);
            gameFleets[5, 2].Polygon(new Vector2(190, 200), 4, 60, 2, EntityID.LightCharger, (float)Math.PI / 4);
            gameFleets[5, 2].Polygon(new Vector2(300, 200), 4, 30, 0, EntityID.Bomber, (float)Math.PI / 4);
            gameFleets[5, 2].Polygon(new Vector2(300, 200), 4, 60, 2, EntityID.LightCharger, (float)Math.PI / 4);
            gameFleets[5, 2].Polygon(new Vector2(410, 200), 4, 30, 0, EntityID.Bomber, (float)Math.PI / 4);
            gameFleets[5, 2].Polygon(new Vector2(410, 200), 4, 60, 2, EntityID.LightCharger, (float)Math.PI / 4);

            //wave 3-7
            gameFleets[6, 2].AddRowFromArray(100, new int[] { EntityID.HelixBuilder, EntityID.HelixBuilder, EntityID.MediumArtillary, EntityID.HelixBuilder, EntityID.HelixBuilder });
            gameFleets[6, 2].AddRow(150, 3, EntityID.LightArtillary);
            gameFleets[6, 2].AddRow(200, 1, EntityID.LightArtillary);
            gameFleets[6, 2].intensity = 3;

            //wave 3-8
            gameFleets[7, 2].AddRowFromArray(100, new int[] { 0, 0, EntityID.Carrier, EntityID.Carrier, 0, 0 }, 50);
            gameFleets[7, 2].AddLine(new Vector2(240, 140), new Vector2(360, 140), 0, EntityID.HelixBuilder);
            gameFleets[7, 2].AddRow(150, 1, EntityID.MediumArtillary);
            gameFleets[7, 2].intensity = 3;

            // wave 3-9
            gameFleets[8, 2].AddRow(100, 3, EntityID.MediumArtillary);
            gameFleets[8, 2].intensity = 3;

            //wave 3-10
            gameFleets[9, 2].AddRow(100, 5, EntityID.Carrier, 200);
            gameFleets[9, 2].AddRow(125, 8, EntityID.BeamShip);
            gameFleets[9, 2].AddRow(200, 1, EntityID.MediumArtillary);
            gameFleets[9, 2].AddLine(new Vector2(240, 170), new Vector2(360, 170), 0, EntityID.HelixBuilder);
            gameFleets[9, 2].AddLine(new Vector2(180, 200), new Vector2(420, 200), 0, EntityID.LightArtillary);
            gameFleets[9, 2].AddLine(new Vector2(240, 230), new Vector2(360, 230), 0, EntityID.HelixBuilder);
            gameFleets[9, 2].AddRowFromArray(300, new int[] { EntityID.LightCharger, EntityID.MediumCharger, EntityID.LightCharger, EntityID.MediumCharger, EntityID.LightCharger, EntityID.MediumCharger, EntityID.LightCharger, EntityID.MediumCharger, EntityID.LightCharger });
            gameFleets[9, 2].AddRowFromArray(340, new int[] { EntityID.TeleportingFighter, EntityID.MediumGunship, EntityID.TeleportingFighter, EntityID.MediumGunship, EntityID.TeleportingFighter, EntityID.MediumGunship, EntityID.TeleportingFighter, EntityID.MediumGunship, EntityID.TeleportingFighter });
            gameFleets[8, 2].intensity = 3;


            
            //wave 4-1
            gameFleets[0, 3].intensity = 4;
            gameFleets[0, 3].AddRow(50, 2, EntityID.Spinner);
            for (int y = 0; y < 4; y++)
            {
                gameFleets[0, 3].AddRow(y * 40 + 100, 12, EntityID.LightGunship);

            }

            //wave 4-2
            gameFleets[1, 3].intensity = 4;
            gameFleets[1, 3].AddRow(50, 2, EntityID.Spinner);
            gameFleets[1, 3].AddRowFromArray(100, new int[] { EntityID.Bomber, EntityID.LightCharger, EntityID.Bomber, EntityID.LightCharger, EntityID.Bomber, EntityID.LightCharger, EntityID.Bomber, EntityID.LightCharger, EntityID.Bomber });
            gameFleets[1, 3].AddRow(140, 6, EntityID.MediumGunship);

            //wave 4-3
            gameFleets[2, 3].intensity = 4;
            gameFleets[2, 3].AddRow(50, 2, EntityID.Spinner);
            gameFleets[2, 3].AddRow(50, 3, EntityID.Carrier, 230);

            for (int j = 0; j < 6; j++)
            {
                float r = j * (float)Math.PI / 3 + (float)Math.PI/6;
                Vector2 c = new Vector2(300, 300) + Functions.PolarVector(120, r);
                gameFleets[2, 3].Polygon(c , 3, 100, 2, EntityID.TeleportingFighter, r + (float)Math.PI);
                gameFleets[2, 3].SingleShip(c, EntityID.LightArtillary);

            }

            // wave 4-4
            gameFleets[3, 3].AddRow(100, 4, EntityID.EliteGunship);
            for (int y = 0; y < 4; y++)
            {
                gameFleets[3, 3].AddRow(y * 40 + 140, y < 2 ?  8 : 16,  y < 2 ? EntityID.MediumGunship: EntityID.LightGunship);

            }
            gameFleets[3, 3].intensity = 4;

            // wave 4-5
            gameFleets[4, 3].intensity = 4;
            gameFleets[4, 3].AddRowFromArray(50, new int[] { EntityID.HelixBuilder, EntityID.Spinner, EntityID.Spinner, EntityID.HelixBuilder});
            gameFleets[4, 3].AddRowFromArray(100, new int[] { EntityID.EliteGunship, EntityID.EliteGunship, EntityID.MediumArtillary, EntityID.EliteGunship, EntityID.EliteGunship });

            
            // wave 4-6
            gameFleets[5, 3].intensity = 4;
            gameFleets[5, 3].AddRow(50, 2, EntityID.Spinner);
            gameFleets[5, 3].AddRow(100, 6, EntityID.Spartan);
            

            // wave 4-7
            gameFleets[6, 3].intensity = 4;
            gameFleets[6, 3].AddRow(50, 2, EntityID.Spinner);
            gameFleets[6, 3].AddRow(100, 4, EntityID.Carrier);
            gameFleets[6, 3].AddRow(140, 4, EntityID.Spartan);

            // wave 4-8
            gameFleets[7, 3].intensity = 4;
            gameFleets[7, 3].AddRow(50, 2, EntityID.Spinner);
            gameFleets[7, 3].AddRow(100, 4, EntityID.EliteGunship);
            gameFleets[7, 3].AddRow(160, 3, EntityID.Spartan);

            // wave 4-9
            gameFleets[8, 3].intensity = 4;
            gameFleets[8, 3].AddRow(50, 2, EntityID.Spinner);
            for(int i =0; i < 3; i++)
            {
                gameFleets[8, 3].AddRow(100 + i *40, 6,  i == 2 ? EntityID.Spartan : EntityID.Bomber);
            }
            for(int i =0; i < 5; i++)
            {
                float spacing = 400f / 7;
                gameFleets[8, 3].SingleShip(new Vector2(Miasma.leftSide + 1.5f * spacing + spacing * i, 220), EntityID.Spartan);
            }

            // wave 4-10
            gameFleets[9, 3].intensity = 4;
            gameFleets[9, 3].AddRowFromArray(50, new int[] { EntityID.HelixBuilder, EntityID.HelixBuilder, EntityID.Spinner, EntityID.Spinner, EntityID.HelixBuilder, EntityID.HelixBuilder });
            gameFleets[9, 3].AddRowFromArray(100, new int[] { EntityID.Carrier, EntityID.EliteGunship, EntityID.Carrier, EntityID.Carrier, EntityID.EliteGunship, EntityID.Carrier }, 250);
            gameFleets[9, 3].AddRow(150, 8, EntityID.BeamShip);
            for (int i = 0; i < 3; i++)
            {
                int gunship = i == 0 ? EntityID.MediumGunship : EntityID.LightGunship;
                int charger = i == 0 ? EntityID.MediumCharger : EntityID.LightCharger;
                int artillary = i == 0 ? EntityID.MediumArtillary : EntityID.LightArtillary;
                int[] row = (i == 0 ? new int[] { gunship, gunship, gunship, artillary, charger, charger, charger } : new int[] { gunship, gunship, gunship, gunship, gunship, gunship, artillary, artillary, charger, charger, charger, charger, charger, charger });
                gameFleets[9, 3].AddRowFromArray(200 + i * 50, row);
            }
            gameFleets[9, 3].AddRow(350, 7, EntityID.TeleportingFighter);
            gameFleets[9, 3].AddRow(400, 3, EntityID.Spartan);

            //wave 5-1
            gameFleets[0, 4].intensity = 4;
            gameFleets[0, 4].AddRow(80, 4, EntityID.RuthlessCharger);
            for (int y = 0; y < 2; y++)
            {
                gameFleets[0, 4].AddRow(120 + 40 * y, 8, EntityID.MediumCharger);
            }
            for (int y = 0; y < 2; y++)
            {
                gameFleets[0, 4].AddRow(200 + y * 20 , 16, EntityID.LightCharger);
            }


            //wave 5-2
            gameFleets[1, 4].intensity = 4;
            gameFleets[1, 4].AddRow(50, 8, EntityID.BeamShip);
            gameFleets[1, 4].AddRowFromArray(90, new int[] { EntityID.RuthlessCharger, EntityID.HelixBuilder, EntityID.RuthlessCharger, EntityID.HelixBuilder, EntityID.RuthlessCharger, EntityID.HelixBuilder, EntityID.RuthlessCharger });
            gameFleets[1, 4].AddRow(130, 6, EntityID.Carrier);
            gameFleets[1, 4].AddRow(170, 6, EntityID.MediumGunship);

            //wave 5-3
            gameFleets[2, 4].intensity = 4;

            gameFleets[2, 4].AddRowFromArray(50, new int[] { EntityID.HelixBuilder, EntityID.RuthlessCharger, EntityID.Cruiser, EntityID.Cruiser, EntityID.RuthlessCharger, EntityID.HelixBuilder});
            gameFleets[2, 4].AddRow(120, 12, EntityID.TeleportingFighter); 
            for (int y = 0; y < 3; y++)
            {
                gameFleets[2, 4].AddRow(y * 40 + 140, 12, EntityID.LightGunship);

            }

            //wave 5-4
            gameFleets[3, 4].intensity = 4;
            for (int y = 0; y < 4; y++)
            {
                gameFleets[3, 4].AddRow(120 + 40 * y, y % 2 == 0 ? 12 : 11,  y < 2 ? EntityID.LightGunship : EntityID.TeleportingFighter);
            }
            gameFleets[3, 4].AddRowFromArray(50, new int[] { EntityID.Cruiser, EntityID.Spinner, EntityID.LightArtillary, EntityID.Cruiser, EntityID.Cruiser, EntityID.LightArtillary, EntityID.Spinner, EntityID.Cruiser });

                //wave 5-5
                gameFleets[4, 4].intensity = 4;
            gameFleets[4, 4].AddRowFromArray(50, new int[] {EntityID.MediumArtillary, EntityID.BombardmentArtillary, EntityID.MediumArtillary });
            gameFleets[4, 4].AddRow(120, 4, EntityID.LightArtillary);



            

            
            // wave 5-6
            gameFleets[5, 4].intensity = 4;
            for (int y = 0; y < 4; y++)
            {

                gameFleets[5, 4].AddRowFromArray(y * 60 + 120, new int[] { EntityID.MediumCharger, EntityID.MediumGunship, EntityID.MediumGunship, EntityID.MediumGunship, EntityID.MediumGunship, EntityID.MediumGunship, EntityID.MediumGunship, EntityID.MediumGunship, EntityID.MediumCharger });

            }
            gameFleets[5, 4].AddRow(60, 2, EntityID.MediumArtillary);

            //wave 5-7
            gameFleets[6, 4].intensity = 4;
            gameFleets[6, 4].AddRowFromArray(50, new int[] { EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber });
            gameFleets[6, 4].AddRow(100, 4, EntityID.EliteGunship);
            gameFleets[6, 4].AddRow(140, 8, EntityID.Cruiser);
            gameFleets[6, 4].AddRow(170, 4, EntityID.Spartan);


            //wave 5-8
            gameFleets[7, 4].intensity = 4;
            gameFleets[7, 4].AddRowFromArray(50, new int[] { EntityID.Cruiser, EntityID.Spinner, EntityID.Cruiser, EntityID.Cruiser, EntityID.Spinner, EntityID.Cruiser });
            gameFleets[7, 4].AddRow(100, 6, EntityID.Carrier, -60);
            gameFleets[7, 4].Polygon(new Vector2(190, 200), 4, 30, 0, EntityID.Bomber, (float)Math.PI / 4);
            gameFleets[7, 4].Polygon(new Vector2(190, 200), 4, 60, 2, EntityID.LightCharger, (float)Math.PI / 4, EntityID.MediumCharger);
            gameFleets[7, 4].Polygon(new Vector2(300, 200), 4, 30, 0, EntityID.Bomber, (float)Math.PI / 4);
            gameFleets[7, 4].Polygon(new Vector2(300, 200), 4, 60, 2, EntityID.LightCharger, (float)Math.PI / 4, EntityID.MediumCharger);
            gameFleets[7, 4].Polygon(new Vector2(410, 200), 4, 30, 0, EntityID.Bomber, (float)Math.PI / 4);
            gameFleets[7, 4].Polygon(new Vector2(410, 200), 4, 60, 2, EntityID.LightCharger, (float)Math.PI / 4, EntityID.MediumCharger);
            gameFleets[7, 4].AddRow(280, 3, EntityID.Spartan);

            // wave 5-9
            gameFleets[8, 4].intensity = 4;
            for (int y = 0; y < 2; y++)
            {

                gameFleets[8, 4].AddRowFromArray(y * 60 + 120, new int[] { EntityID.RuthlessCharger,  EntityID.EliteGunship,  EntityID.EliteGunship, EntityID.EliteGunship, EntityID.EliteGunship, EntityID.EliteGunship, EntityID.RuthlessCharger });

            }
            gameFleets[8, 4].AddRow(60, 1, EntityID.BombardmentArtillary);


            //wave 5-10
            gameFleets[9, 4].intensity = 4;
            gameFleets[9, 4].AddRowFromArray(50, new int[] { EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Spinner, EntityID.Bomber, EntityID.Spinner, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber, EntityID.BeamShip, EntityID.Bomber });
            gameFleets[9, 4].AddRow(100, 8, EntityID.Carrier, 160);
            gameFleets[9, 4].AddRowFromArray(150, new int[] { EntityID.Cruiser, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.Cruiser, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.Cruiser, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.Cruiser, EntityID.TeleportingFighter, EntityID.TeleportingFighter, EntityID.Cruiser });
            float third = 400f / 3f;
            gameFleets[9, 4].SingleShip(new Vector2(300, 220), EntityID.BombardmentArtillary);
            gameFleets[9, 4].AddSmallRow(280, Miasma.leftSide + third, third, 2, EntityID.MediumArtillary);
            gameFleets[9, 4].AddSmallRow(320, Miasma.leftSide + third, third, 4, EntityID.LightArtillary);

            gameFleets[9, 4].AddSmallRow(220, Miasma.leftSide, third, 2, EntityID.EliteGunship);
            gameFleets[9, 4].AddSmallRow(280, Miasma.leftSide, third, 4, EntityID.MediumGunship);
            gameFleets[9, 4].AddSmallRow(320, Miasma.leftSide, third, 8, EntityID.LightGunship);

            gameFleets[9, 4].AddSmallRow(220, Miasma.leftSide + 2 * third, third, 2, EntityID.RuthlessCharger);
            gameFleets[9, 4].AddSmallRow(280, Miasma.leftSide + 2 * third, third, 4, EntityID.MediumCharger);
            gameFleets[9, 4].AddSmallRow(320, Miasma.leftSide + 2 * third, third, 8, EntityID.LightCharger);

            gameFleets[9, 4].AddRow(340, 6, EntityID.Spartan);

        }
        public int GetIntensity()
        {
            return intensity;
        }
    }
}
