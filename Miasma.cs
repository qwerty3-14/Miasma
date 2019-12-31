using Miasma.Boss1;
using Miasma.Boss2;
using Miasma.Boss3;
using Miasma.Boss4;
using Miasma.UI;
using Miasma.Ships;
using Miasma.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Miasma.Boss5;

namespace Miasma
{

    public class Miasma : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Random random = new Random();
        public static Miasma instance;
        public static SpriteFont font;
        public static Vector2 screenSize = new Vector2(800, 800);
        public static Texture2D[] UISprites = new Texture2D[7];
        public static Texture2D[] EntitySprites = new Texture2D[41];
        public static Texture2D[] EntityExtras = new Texture2D[36];
        public static Texture2D[] UpgradeSprites = new Texture2D[20];
        public static Texture2D stars;
        public static Texture2D ParticleSprites;
        public static List<Entity> gameEntities = new List<Entity>();
        public static List<Particle> gameParticles = new List<Particle>();
        public static TheTransmission player;
        public static Deck<Ship> enemyFleet = new Deck<Ship>();
        public static List<Ship> BeamShips = new List<Ship>();
        public static List<Ship> InfectedBeamShips = new List<Ship>();
        public static List<Message> messages = new List<Message>();
        public static Deck<Upgrade> UpgradePool = new Deck<Upgrade>();
        public static int currentWaveIntensity = 2;
        public const int leftSide = 100;
        public const int rightSide = 500;
        public const int lowerBoundry = 540;
        public static Boss boss = null;
        public static bool hard = false;
        public static float sfxVolume = 1f;
        int deathTimer = 180;
        public static ColorOptions myColor = ColorOptions.Purple;
        public static GameScene gameState = GameScene.Intro;
        static Menu menu = new Menu();
        public static int totalProgress = 0;
        public static void Save()
        {
            if (!File.Exists("saveData.txt"))
            {
                File.Create("saveData.txt");
            }
            new Message("Saving", new Vector2(300, 750), 120, true);


            StreamWriter sw = new StreamWriter("saveData.txt");
            int stageProgress = 0;
            
            sw.WriteLine(stage*11 + wave);
            sw.WriteLine(hard);
            sw.WriteLine((byte)myColor);
            for(int i =0; i < 4; i++)
            {
                if(player != null && player.upgrades[i] != null)
                {
                    sw.WriteLine(player.upgrades[i].getId());
                }
                else if(player != null)
                {
                    
                    sw.WriteLine(-1);
                }
            }
            sw.WriteLine(Sounds.sfxVolume);
            sw.WriteLine(Sounds.musicVolume);
            sw.WriteLine(Controls.gamePadShoot);
            sw.WriteLine(Controls.gamePadMiasma);
            sw.WriteLine(Controls.keyboardShoot);
            sw.WriteLine(Controls.keyboardMiasma);
            sw.WriteLine(totalProgress);
            sw.Close();
        }

        public static void Load()
        {
            StreamReader sr = new StreamReader("saveData.txt");
            
            int progress = Int32.Parse(sr.ReadLine());
            stage = progress / 11;
            wave = progress % 11;
            hard = sr.ReadLine() == "True" ? true : false;
            myColor = (ColorOptions)Byte.Parse(sr.ReadLine());
            int[] loadedUpgradeIds = new int[4];
            for(int i =0; i < 4; i++)
            {
                loadedUpgradeIds[i] = Int32.Parse(sr.ReadLine());
            }
           
            if (player != null)
            {
                ResetUpgradePool();
                for (int i =0; i < 4; i++)
                {
                    switch(loadedUpgradeIds[i])
                    {
                        case 0:
                            player.upgrades[i] = new FirerateUp();
                            break;
                        case 1:
                            player.upgrades[i] = new Armor();
                            break;
                        case 2:
                            player.upgrades[i] = new TripleMiasma();
                            break;
                        case 3:
                            player.upgrades[i] = new MiasmaBlast();
                            break;
                        case 4:
                            player.upgrades[i] = new MiasmaGenerator();
                            break;
                        case 5:
                            player.upgrades[i] = new MiasmaRay();
                            break;
                        case 6:
                            player.upgrades[i] = new Speed();
                            break;
                        case 7:
                            player.upgrades[i] = new WaveGuns();
                            break;
                        case 8:
                            player.upgrades[i] = new Richoche();
                            break;
                        case 9:
                            player.upgrades[i] = new StableMiasma();
                            break;
                        case 10:
                            player.upgrades[i] = new Leech();
                            break;
                        case 11:
                            player.upgrades[i] = new Contagus();
                            break;
                        case 12:
                            player.upgrades[i] = new TurretUpgrade();
                            break;
                        case 13:
                            player.upgrades[i] = new Confuse();
                            break;
                    }
                }
                for(int i =0; i < 4; i++)
                {
                    if(player.upgrades[i] != null)
                    {
                        RemoveUpgradeFromPool(player.upgrades[i].getId());
                    }
                    
                }
            }
            Sounds.sfxVolume = byte.Parse(sr.ReadLine());
            Sounds.musicVolume = byte.Parse(sr.ReadLine());
            Controls.gamePadShoot = byte.Parse(sr.ReadLine());
            Controls.gamePadMiasma = byte.Parse(sr.ReadLine());
            Controls.keyboardShoot = int.Parse(sr.ReadLine());
            Controls.keyboardMiasma = int.Parse(sr.ReadLine());
            totalProgress = int.Parse(sr.ReadLine());
            sr.Close();
        }
        public static Color MiasmaColor()
        {
            switch (myColor)
            {
                case ColorOptions.Green:
                    return Color.Green;
                case ColorOptions.Orange:
                    return Color.Orange;
                case ColorOptions.Pink:
                    return Color.Pink;
                case ColorOptions.Dark:
                    return new Color(.3f, .3f, .3f);
                case ColorOptions.Spectre:
                    return Spectre();
                case ColorOptions.Rainbow:
                    return Rainbow();
            }
            return Color.Violet;
        }
        static int colorCounter = 0;
        public static Color Rainbow()
        {

            return Functions.ToRgb((float)(colorCounter % 120) / 120f, 1f, .5f);
        }
        public static Color Spectre()
        {
            float c = (float)(colorCounter % 60) / 30f;
            return Color.Lerp(new Color(.9f, .9f, .9f, .9f), new Color(.1f, .1f, .1f, .1f), c > 1f ? 2f - c : c);
        }
        public void NewGameReset()
        {


            deathTimer = 60;
            gameEntities.Clear();
            if (gameEntities.Contains(player))
            {
                gameEntities.Remove(player);
            }
            player = new TheTransmission(new Vector2(300, 700), team: 1);
        }
        public static void LeaveMenu()
        {
            menu.MenuReset();
            gameState = GameScene.Combat;
        }

        public Miasma()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        #region menuFunctions
        
        #endregion
        Capsule capsule;
        public static void ResetUpgradePool()
        {
            UpgradePool.Clear();
            UpgradePool.Add(new FirerateUp());
            UpgradePool.Add(new Armor());
            UpgradePool.Add(new TripleMiasma());
            UpgradePool.Add(new MiasmaBlast());
            UpgradePool.Add(new MiasmaGenerator());
            UpgradePool.Add(new MiasmaRay());
            UpgradePool.Add(new Speed());
            UpgradePool.Add(new WaveGuns());
            UpgradePool.Add(new Richoche());
            UpgradePool.Add(new StableMiasma());
            //UpgradePool.Add(new Leech());
            UpgradePool.Add(new Contagus());
            UpgradePool.Add(new TurretUpgrade());
            //UpgradePool.Add(new Confuse());
            if(player != null)
            {
                for(int i =0; i < player.upgrades.Length; i++)
                {
                    player.upgrades[i] = null;
                }
            }
            
        }
        static void RemoveUpgradeFromPool(int ID)
        {
            for(int i =0; i<UpgradePool.Count; i++)
            {
                if(UpgradePool[i].getId() == ID)
                {
                    UpgradePool.RemoveAt(i);
                }
            }
        }


        protected override void LoadContent()
        {
            instance = this;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Controls.Initialize();


            font = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"), 25, 1024, 1024, new[] { CharacterRange.BasicLatin, CharacterRange.Latin1Supplement, CharacterRange.LatinExtendedA, CharacterRange.Cyrillic }).CreateSpriteFont(GraphicsDevice);
            UISprites[0] = Content.Load<Texture2D>("LeftBorder");
            UISprites[1] = Content.Load<Texture2D>("RightBorder");
            UISprites[2] = Content.Load<Texture2D>("UIBar");
            UISprites[3] = Content.Load<Texture2D>("UIHealth");
            UISprites[4] = Content.Load<Texture2D>("UIMiasma");
            UISprites[5] = Content.Load<Texture2D>("SettingsBar");
            UISprites[6] = Content.Load<Texture2D>("SettingsSlider");
            EntitySprites[0] = Content.Load<Texture2D>("TheTransmission");
            EntitySprites[1] = Content.Load<Texture2D>("MiasmaPulse");
            EntitySprites[2] = Content.Load<Texture2D>("PewPew");
            EntitySprites[3] = Content.Load<Texture2D>("LightGunship");
            EntitySprites[4] = Content.Load<Texture2D>("LightCharger");
            EntitySprites[5] = Content.Load<Texture2D>("LightArtillary");
            EntitySprites[6] = Content.Load<Texture2D>("ArtillaryPulse");
            EntitySprites[7] = Content.Load<Texture2D>("BeamShip");
            EntitySprites[8] = Content.Load<Texture2D>("MediumGunship");
            EntitySprites[9] = Content.Load<Texture2D>("Boss1");
            EntitySprites[10] = Content.Load<Texture2D>("Shield");
            EntitySprites[11] = Content.Load<Texture2D>("TeleportingFighter");
            EntitySprites[12] = Content.Load<Texture2D>("Capsule");
            EntitySprites[13] = Content.Load<Texture2D>("MediumCharger");
            EntitySprites[14] = Content.Load<Texture2D>("Bomber");
            EntitySprites[15] = Content.Load<Texture2D>("Bomb");
            EntitySprites[16] = Content.Load<Texture2D>("Gemini");
            EntitySprites[17] = Content.Load<Texture2D>("HelixBuilder");
            EntitySprites[18] = Content.Load<Texture2D>("HelixSegment");
            EntitySprites[19] = Content.Load<Texture2D>("MediumArtillary");
            EntitySprites[20] = Content.Load<Texture2D>("BigArtillaryPulse");
            EntitySprites[21] = Content.Load<Texture2D>("Carrier");
            EntitySprites[22] = Content.Load<Texture2D>("Pulsar");
            EntitySprites[23] = Content.Load<Texture2D>("Block");
            EntitySprites[24] = Content.Load<Texture2D>("Missile");
            EntitySprites[25] = Content.Load<Texture2D>("Wave");
            EntitySprites[26] = Content.Load<Texture2D>("Spinner");
            EntitySprites[27] = Content.Load<Texture2D>("EliteGunship");
            EntitySprites[28] = Content.Load<Texture2D>("Spartan");
            EntitySprites[29] = Content.Load<Texture2D>("SpartanShield");
            EntitySprites[30] = Content.Load<Texture2D>("Jupiter");
            EntitySprites[31] = Content.Load<Texture2D>("ArmTip");
            EntitySprites[32] = Content.Load<Texture2D>("ArmTipBroken");
            EntitySprites[33] = Content.Load<Texture2D>("LightingBolt");
            EntitySprites[34] = Content.Load<Texture2D>("BallLightning");
            EntitySprites[35] = Content.Load<Texture2D>("RuthlessCharger");
            EntitySprites[36] = Content.Load<Texture2D>("Cruiser");
            EntitySprites[37] = Content.Load<Texture2D>("BombardmentArtillary");
            EntitySprites[38] = Content.Load<Texture2D>("AndromedaGunBase");
            EntitySprites[39] = Content.Load<Texture2D>("AndromedaDart");
            EntitySprites[40] = Content.Load<Texture2D>("AndromedaShield");

            EntityExtras[0] = Content.Load<Texture2D>("LightArtillaryGun");
            EntityExtras[1] = Content.Load<Texture2D>("DeathBeam");
            EntityExtras[2] = Content.Load<Texture2D>("MediumGunshipTurret");
            EntityExtras[3] = Content.Load<Texture2D>("Boss1Top");
            EntityExtras[4] = Content.Load<Texture2D>("GeminiTurret");
            EntityExtras[5] = Content.Load<Texture2D>("HelixBuilderTop");
            EntityExtras[6] = Content.Load<Texture2D>("HelixGun");
            EntityExtras[7] = Content.Load<Texture2D>("MediumArtillaryGun");
            EntityExtras[8] = Content.Load<Texture2D>("CarrierTop");
            EntityExtras[9] = Content.Load<Texture2D>("CarrierLeftGun");
            EntityExtras[10] = Content.Load<Texture2D>("CarrierRightGun");
            EntityExtras[11] = Content.Load<Texture2D>("PulsarBottom");
            EntityExtras[12] = Content.Load<Texture2D>("BlockGun");
            EntityExtras[13] = Content.Load<Texture2D>("MissilePrelaunch");
            EntityExtras[14] = Content.Load<Texture2D>("MissileFull");
            EntityExtras[15] = Content.Load<Texture2D>("TheTransmissionTurret");
            EntityExtras[16] = Content.Load<Texture2D>("SpinnerBeam");
            EntityExtras[17] = Content.Load<Texture2D>("SpartanClaw");
            Sounds.LoadSounds(Content);
            int width = 1000;
            int height = 1;
            Texture2D line = new Texture2D(instance.GraphicsDevice, width, height);
            var dataColors = new Color[width * height]; //Color array

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    dataColors[x + y * height] = Color.White;

                }
            }
            line.SetData(0, null, dataColors, 0, width * height);
            EntityExtras[18] = line;
            EntityExtras[19] = Content.Load<Texture2D>("SideDoor");
            EntityExtras[20] = Content.Load<Texture2D>("CenterDoor");
            EntityExtras[21] = Content.Load<Texture2D>("JupiterBottom");
            EntityExtras[22] = Content.Load<Texture2D>("ArmsCore");
            EntityExtras[23] = Content.Load<Texture2D>("ArmSegment");
            EntityExtras[24] = Content.Load<Texture2D>("LimbSegment");
            EntityExtras[25] = Content.Load<Texture2D>("Hand");
            EntityExtras[26] = Content.Load<Texture2D>("LightingBeam");
            EntityExtras[27] = Content.Load<Texture2D>("LightningAnnihilator");
            EntityExtras[29] = Content.Load<Texture2D>("BigLightningBeam");
            EntityExtras[30] = Content.Load<Texture2D>("TheAndromeda");
            EntityExtras[31] = Content.Load<Texture2D>("AndromedaMachineGun");
            EntityExtras[32] = Content.Load<Texture2D>("AndromedaStarGunCenter");
            EntityExtras[33] = Content.Load<Texture2D>("AndromedaStarGunPoint");
            EntityExtras[34] = Content.Load<Texture2D>("AndromedaBombLauncher");
            EntityExtras[35] = Content.Load<Texture2D>("AndromedaDartLauncher");

            UpgradeSprites[0] = Content.Load<Texture2D>("UFirerateUp");
            UpgradeSprites[1] = Content.Load<Texture2D>("UArmorUp");
            UpgradeSprites[2] = Content.Load<Texture2D>("UTripleMiasma");
            UpgradeSprites[3] = Content.Load<Texture2D>("UMiasmaBlast");
            UpgradeSprites[4] = Content.Load<Texture2D>("UMiasmaGenerator");
            UpgradeSprites[5] = Content.Load<Texture2D>("UMiasmaRay");
            UpgradeSprites[6] = Content.Load<Texture2D>("USpeed");
            UpgradeSprites[7] = Content.Load<Texture2D>("UWave");
            UpgradeSprites[8] = Content.Load<Texture2D>("URichoche");
            UpgradeSprites[9] = Content.Load<Texture2D>("UStableMiasma");
            UpgradeSprites[10] = Content.Load<Texture2D>("ULeech");
            UpgradeSprites[11] = Content.Load<Texture2D>("UContagus");
            UpgradeSprites[12] = Content.Load<Texture2D>("UTurret");
            UpgradeSprites[13] = Content.Load<Texture2D>("UConfuse");
            ParticleSprites = Content.Load<Texture2D>("Particle");

            width = 400;
            height = 800;
            stars = new Texture2D(instance.GraphicsDevice, width, height);
            dataColors = new Color[width * height]; //Color array
            for(int i =0; i < 700; i++)
            {
                dataColors[random.Next(width * height)] = Color.White;
            }
            stars.SetData(0, null, dataColors, 0, width * height);

            ResetUpgradePool();
            if (!File.Exists("saveData.txt"))
            {
                Save();
            }
            Load();
            Fleet.CreateFleets();
            capsule = new Capsule(new Vector2(300, 700));
            Sounds.UpdateMusicVolume();
        }


        protected override void UnloadContent()
        {

        }
        static Upgrade[] upgradeOptions = new Upgrade[4];
        static int upgradeSceneTimer = 0;
        static int upgradeSelected = 0;
        
        public static void LoadUpgrades()
        {
            gameState = GameScene.PickUpgrade;
            upgradeOptions = UpgradePool.Draw(4);
            upgradeSceneTimer = 0;
            upgradeSelected = 0;
            new Message("Select Upgrade", new Vector2(300, 300), -1);

        }
        public static void NextStage()
        {
            gameState = GameScene.Combat;
            wave = 0;
            stage++;
            Save();
            player.UpdateStats();
            player.health = player.maxHealth;
            messages.Clear();

        }
        public static bool BossIsActive()
        {
            return boss != null;
        }
        int actCooldown = 120;
        int betweenWaveTimer = 120;
        public static int stage = 0;
        public static int wave = 0;

        void ScrollCamera(float amt)
        {
            foreach(Entity entity in gameEntities)
            {
                entity.Position.Y += amt;
            }
        }
        void ScrollCameraTo(float loc)
        {
            float maxCameraSpeed = 4;
            if(player.Position.Y != loc)
            {
                float dist = loc - player.Position.Y;
                if(Math.Abs(dist) < maxCameraSpeed)
                {
                    ScrollCamera(dist);
                }
                else
                {
                    ScrollCamera((dist > 0 ? 1 : -1) * maxCameraSpeed);
                }
            }
        }


        
        
        public static int introTimer = 0;
        public static int introIncrimenter = 0;
        
        public static Message introDIalougeMessage;
        public static TimeSpan elapsedTime;
        public static String sideText = null;
        public static Entity guide = null;
        public static CutSceneAndromeda cutSceneAndromeda;
        Vector2 playerGoHere = new Vector2(300, 700);
        public static bool toCredits = false;
        protected override void Update(GameTime gameTime)
        {
            
            
            
            elapsedTime = gameTime.TotalGameTime;
            colorCounter++;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            
           
            switch (gameState)
            {
                case GameScene.MainMenu:
                    #region menus

                    Controls.ModifyControlSettings();
                    menu.Manage();
                    
                   
                    break;
                #endregion
                case GameScene.Combat:
                    #region combat
                    if(Controls.JustPushedPause())
                    {
                        gameState = GameScene.MainMenu;
                        menu.LoadPause();
                    }
                    if (gameEntities.Contains(player))
                    {
                        player.PlayerMovement();
                    }
                    if (!gameEntities.Contains(boss))
                    {
                        boss = null;
                    }
                    if (!gameEntities.Contains(player))
                    {
                        deathTimer--;
                        if (deathTimer <= 0)
                        {
                            gameState = GameScene.MainMenu;
                            menu.LoadGameOver();
                        }

                    }
                    for (int s = 0; s < BeamShips.Count; s++)
                    {
                        if (!gameEntities.Contains(BeamShips[s]) || BeamShips[s].team != 0)
                        {
                            BeamShips.RemoveAt(s);
                        }

                    }
                    for (int s = 0; s < InfectedBeamShips.Count; s++)
                    {
                        if (!gameEntities.Contains(InfectedBeamShips[s]))
                        {
                            InfectedBeamShips.RemoveAt(s);
                        }

                    }
                    //make enemy ships act as fleet
                    for (int s = 0; s < enemyFleet.Count; s++)
                    {
                        if (!gameEntities.Contains(enemyFleet[s]) || enemyFleet[s].team != 0)
                        {
                            enemyFleet.RemoveAt(s);
                        }

                    }

                    
                    bool noneActing = true;
                    foreach (Ship ship in enemyFleet)
                    {
                        if (ship.IsActing())
                        {
                            noneActing = false;

                        }
                    }
                    if (noneActing && enemyFleet.Count > 0)
                    {
                        actCooldown--;
                        if (actCooldown == 0)
                        {
                            Ship[] makeTheseAct = enemyFleet.Draw(Math.Min(currentWaveIntensity, enemyFleet.Count));
                            foreach (Ship ship in makeTheseAct)
                            {
                                ship.Act(random.Next(60));
                            }
                        }
                    }
                    else
                    {
                        actCooldown = hard ? 30 : 120;
                    }
                    if (enemyFleet.Count == 0 && !BossIsActive())
                    {
                        betweenWaveTimer--;
                        if (betweenWaveTimer <= 0)
                        {
                            betweenWaveTimer = 120;
                            player.health = player.maxHealth;
                            if (player.health > player.maxHealth)
                            {
                                player.health = player.maxHealth;
                            }
                            if(wave + stage*11 > totalProgress)
                            {
                                totalProgress = wave + stage * 11;
                            }
                            Save();
                            if (wave >= 10)
                            {
                                
                                switch (stage)
                                {
                                    case 0:
                                        boss = new ShieldedCarrier(new Vector2(300, -100));
                                        break;
                                    case 1:
                                        boss = new GeminiManager(new Vector2(0, 0));
                                        break;
                                    case 2:
                                        boss = new Pulsar(new Vector2(300, -100));
                                        break;
                                    case 3:
                                        boss = new Jupiter(new Vector2(300, 1094));
                                        break;
                                    case 4:
                                        boss = new TheAndromeda(new Vector2(0, -300));
                                        break;
                                }

                                new Message((stage == 4 ? "Final " : "" )+ "Boss Approaches!", new Vector2(300, 400));
                                
                            }
                            else
                            {
                                
                                Fleet.gameFleets[wave, stage].LoadFleet();
                                Ship.trigCounter = 0;
                                new Message("Wave " + (stage + 1) + "-" + (wave + 1), new Vector2(300, 400));
                                actCooldown = 180;
                                wave++;
                                

                            }
                            
                            
                        }
                    }

                    Ship.trigCounter += (float)Math.PI / 240;
                    for (int e = 0; e < gameEntities.Count; e++)
                    {
                        gameEntities[e].Physics();
                    }
                    for (int e = 0; e < gameEntities.Count; e++)
                    {
                        gameEntities[e].MainUpdate();
                        gameEntities[e].SpecialUpdate();
                    }
                    for (int e = 0; e < gameEntities.Count; e++)
                    {
                        gameEntities[e].UpdateHitbox();
                    }
                    if(BossIsActive() && boss is Jupiter)
                    {
                        ScrollCameraTo(400);
                    }
                    else
                    {
                        ScrollCameraTo(700);
                    }
                   
                    break;
                #endregion
                case GameScene.Intro:
                    #region intro
                    if(capsule != null && gameEntities.Contains(capsule))
                    {
                        capsule.rotation += (float)Math.PI / 120;
                        if (colorCounter % 15 == 0)
                        {
                            new Particle(capsule.Position, Functions.PolarVector(1, capsule.rotation - (float)Math.PI / 4), random.Next(2), 60);
                            new Particle(capsule.Position, Functions.PolarVector(1, capsule.rotation - (float)Math.PI / 4 + (float)Math.PI), random.Next(2), 60);
                        }
                    }
                    if(player != null)
                    {
                        if(player.Position.Y > 700)
                        {
                            player.Position.Y -= 2;
                        }
                        else if (capsule != null)
                        {
                            gameEntities.Remove(capsule);
                        }
                        
                    }
                    
                    if (Controls.JustPushedMenuSelect())
                    {
                        if (capsule != null)
                        {
                            gameEntities.Remove(capsule);
                        }
                        gameState = GameScene.MainMenu;
                        NewGameReset();
                        Sounds.PlayMusic(0);
                        menu.LoadMainMenu();
                        
                    }
                    if (introDIalougeMessage == null || introDIalougeMessage.Complete())
                    {
                        introTimer++;
                        if(introIncrimenter >=5)
                        {
                            if (introTimer > 60)
                            {
                                gameState = GameScene.MainMenu;
                                menu.LoadMainMenu();
                                NewGameReset();
                            }
                        }
                        else if (introTimer > 60)
                        {
                            introTimer = 0;
                            Vector2 msgAt = new Vector2(300, 100 + introIncrimenter * 50);
                            switch (introIncrimenter)
                            {
                                case 0:
                                    introDIalougeMessage = new Message("What is that strange Object?", msgAt, -1);
                                    break;
                                case 1:
                                    player = new TheTransmission(new Vector2(300, 820), team: 1);
                                    introDIalougeMessage = new Message("Let's investigate.", msgAt, -1);
                                    break;
                                case 2:
                                    introDIalougeMessage = new Message("It seems to be some sort of capsule", msgAt, -1);
                                    break;
                                case 3:
                                    introDIalougeMessage = new Message("Open it!", msgAt, -1);
                                    break;
                                case 4:
                                    for (int i = 0; i < 80; i++)
                                    {
                                        new Particle(player.Position, Functions.PolarVector((float)random.NextDouble() * 6f, Functions.RandomRotation()), Miasma.random.Next(2), 45);
                                    }
                                        
                                    introDIalougeMessage = new Message("You dare disturb my slumber!", msgAt, -1, true);
                                    break;
                            }
                            introIncrimenter++;
                        }
                    }
                    #endregion
                    break;
                case GameScene.PickUpgrade:
                    #region UpgradeMode
                    upgradeSceneTimer++;
                    if(upgradeSceneTimer > 60)
                    {
                        if(Controls.JustPushedRight())
                        {
                            upgradeSelected++;
                            if(upgradeSelected>3)
                            {
                                upgradeSelected = 0;
                            }
                            
                        }
                        if (Controls.JustPushedLeft())
                        {
                            upgradeSelected--;
                            if (upgradeSelected < 0)
                            {
                                upgradeSelected = 3;
                            }
                        }
                        if (Controls.JustPushedShoot() || Controls.JustPushedMenuSelect())
                        {
                            player.upgrades[stage] = upgradeOptions[upgradeSelected];
                            RemoveUpgradeFromPool(upgradeOptions[upgradeSelected].getId());
                            NextStage();
                        }
                    }
                    #endregion
                    break;
                case GameScene.Outro:
                    if (Controls.JustPushedMenuSelect())
                    {
                        gameState = GameScene.MainMenu;
                        NewGameReset();
                        menu.LoadMainMenu();
                    }
                    
                    if (introDIalougeMessage == null || introDIalougeMessage.Complete())
                    {
                        introTimer++;
                        
                        if (introIncrimenter >= 7)
                        {
                            if (introTimer > 60)
                            {
                                introIncrimenter = 0;
                                if(toCredits)
                                {
                                    gameState = GameScene.MainMenu;
                                    Sounds.PlayMusic(0);
                                    menu.LoadMainMenu();
                                    NewGameReset();
                                }
                                else
                                {
                                    messages.Clear();
                                    toCredits = true;
                                }
                            }
                        }
                        else if (introTimer > 60)
                        {
                            introTimer = 0;
                            Vector2 msgAt = new Vector2(300, 100 + introIncrimenter * 50);
                            if (toCredits)
                            {
                                switch (introIncrimenter)
                                {
                                    case 0:
                                        introDIalougeMessage = new Message("Miasma", msgAt, -1, true);
                                        break;
                                    case 2:
                                        introDIalougeMessage = new Message("Programming and Art by", msgAt, -1);
                                        introTimer = 50;
                                        break;
                                    case 3:
                                        introDIalougeMessage = new Message("Noah Linde", msgAt, -1);
                                        
                                        break;
                                    case 4:
                                        introDIalougeMessage = new Message("Sound Design by", msgAt, -1);
                                        introTimer = 50;
                                        break;
                                    case 5:
                                        introDIalougeMessage = new Message("Michel Linde", msgAt, -1);
                                        
                                        break;
                                    case 6:
                                        introDIalougeMessage = new Message("Thank you for playing.", msgAt, -1, true);
                                        break;
                                }
                            }
                            else
                            {
                                switch (introIncrimenter)
                                {
                                    case 0:
                                        introDIalougeMessage = new Message("... All of our weapons are offline", msgAt, -1);
                                        playerGoHere = new Vector2(300, 700);
                                        break;
                                    case 1:
                                        introDIalougeMessage = new Message("Your ship is mine!", msgAt, -1, true);
                                        playerGoHere = new Vector2(300, -100);
                                        introTimer = -120;
                                        break;
                                    case 2:
                                        introDIalougeMessage = new Message("And so in the end", msgAt, -1);
                                        cutSceneAndromeda.flyTo = new Vector2(0, 900);
                                        introTimer = 50;
                                        break;
                                    case 3:
                                        introDIalougeMessage = new Message("the Miasma flew to the edge of space.", msgAt, -1);
                                        break;
                                    case 4:
                                        introDIalougeMessage = new Message("With the largest fuel reserve in", msgAt, -1);
                                        introTimer = 50;
                                        break;
                                    case 5:
                                        introDIalougeMessage = new Message("the galaxy and no intention on stopping.", msgAt, -1);
                                        break;
                                    case 6:
                                        introDIalougeMessage = new Message("Finally none shall disturb my slumber!", msgAt, -1, true);
                                        break;
                                }
                            }
                            
                            
                            
                            introIncrimenter++;
                            
                        }
                        
                    }
                    if ((player.Position - playerGoHere).Length() < 8)
                    {
                        player.Position = playerGoHere;
                        player.Velocity = Vector2.Zero;
                        if (introIncrimenter == 2 && !toCredits)
                        {
                            for (int d = 0; d < 5; d++)
                            {
                                new Particle((leftSide + random.Next(400)) * Vector2.UnitX, Functions.PolarVector((float)random.NextDouble() * 10f, (float)Math.PI / 2 - (float)Math.PI / 8 + (float)random.NextDouble() * (float)Math.PI / 4), random.Next(2), 60);
                            }
                        }
                    }
                    else
                    {
                        player.Velocity = Functions.PolarVector(8, Functions.ToRotation(playerGoHere - player.Position));
                    }
                    for (int e = 0; e < gameEntities.Count; e++)
                    {
                        gameEntities[e].Physics();
                    }
                    for (int e = 0; e < gameEntities.Count; e++)
                    {
                        gameEntities[e].MainUpdate();
                        gameEntities[e].SpecialUpdate();
                    }
                    for (int e = 0; e < gameEntities.Count; e++)
                    {
                        gameEntities[e].UpdateHitbox();
                    }
                    break;


            }
            for (int p = 0; p < gameParticles.Count; p++)
            {
                gameParticles[p].ParticleUpdate();
            }
            for (int m = 0; m < messages.Count; m++)
            {
                messages[m].UpdateMessage();
            }
            base.Update(gameTime);
        }

        
        public static void StringWithSize(SpriteBatch spriteBatch, String text, Vector2 center, float width, Color? color = null)
        {
            Vector2 textEpicness = font.MeasureString(text);
            float scale = (width / textEpicness.X);
            if(color == null)
            {
                color = Color.Black;
            }
            spriteBatch.DrawString(font, text, center, (Color)color, 0f, textEpicness * .5f, scale, SpriteEffects.None, 0f);
        }
        const int barStartY = 650;
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(stars, Vector2.UnitX * leftSide);
            foreach (Entity drawMe in gameEntities)
            {
                
                drawMe.PreDraw(spriteBatch);
            }
            foreach (Entity drawMe in gameEntities)
            {
                drawMe.Draw(spriteBatch);
            }
            foreach (Entity drawMe in gameEntities)
            {
                drawMe.PostDraw(spriteBatch);
            }
            foreach (Particle drawMe in gameParticles)
            {
                spriteBatch.Draw(ParticleSprites, drawMe.position, new Rectangle(0, drawMe.type * 3, 3, 3), (drawMe.type < 2 ? MiasmaColor() : Color.White), 0f, new Vector2(3, 3) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(UISprites[0], Vector2.Zero);
            spriteBatch.Draw(UISprites[1], Vector2.UnitX * 500);
            switch (gameState)
            {
                case GameScene.MainMenu:
                    break;
                case GameScene.PickUpgrade:
                case GameScene.Combat:
                    if (player != null)
                    {

                        for (int b = -1; b < player.maxHealth + 1; b++)
                        {
                            spriteBatch.Draw(UISprites[2], new Vector2(rightSide + 80, barStartY - b));
                            if (b < player.health && b > 0)
                            {
                                spriteBatch.Draw(UISprites[3], new Vector2(rightSide + 80, barStartY - b));
                            }
                        }
                        StringWithSize(spriteBatch, "Health", new Vector2(rightSide + 80 + 5, barStartY + 10), 40f);
                        for (int b = -1; b < (player.MiasmaMaxCapacity / 40) + 1; b++)
                        {
                            spriteBatch.Draw(UISprites[2], new Vector2(rightSide + 130, barStartY - b));
                            if (b < (player.MisamaCapacity / 40) && b > 0)
                            {
                                spriteBatch.Draw(UISprites[4], new Vector2(rightSide + 130, barStartY - b), color: MiasmaColor());
                            }
                        }
                        StringWithSize(spriteBatch, "Miasma", new Vector2(rightSide + 130 + 5, barStartY + 10), 40f);
                        StringWithSize(spriteBatch, "Upgrades:", new Vector2(rightSide + 40, barStartY + 40), 70);
                        for (int u =0; u < 4; u++)
                        {
                            Vector2 here = new Vector2(rightSide + (u + 1) * (300f / 5f), 720);
                            if(player.upgrades[u] != null)
                            {
                                player.upgrades[u].Draw(spriteBatch, here);
                                StringWithSize(spriteBatch, player.upgrades[u].GetName(), here + Vector2.UnitY * 40, 50f);
                            }
                        }
                    }
                    if (BossIsActive())
                    {
                        StringWithSize(spriteBatch, boss.name, new Vector2(50, 750), 80f);
                        
                        int maxBarSize = 600;
                        int barRegionWIdth = 80;
                        int barCount = (boss.maxHealth / maxBarSize);
                        if((boss.maxHealth % maxBarSize) != 0)
                        {
                            barCount++;
                        }
                        for (int s = 0; s < barCount; s++)
                        {
                            for (int b = -1; b < ((s == barCount -1 && (boss.maxHealth % maxBarSize) !=0) ?  (boss.maxHealth % maxBarSize) : maxBarSize) + 1; b++)
                            {
                                Vector2 notchPosition = new Vector2(45 - barRegionWIdth / 2 + (((float)s + 1f) / ((float)barCount + 1f)) * barRegionWIdth, 700 - b);
                                spriteBatch.Draw(UISprites[2], notchPosition);
                                if (b + s * maxBarSize < boss.health && b > 0 && b < ((s == barCount - 1 && (boss.maxHealth % maxBarSize) != 0) ? (boss.maxHealth % maxBarSize) : maxBarSize))
                                {
                                    spriteBatch.Draw(UISprites[3], notchPosition);
                                }
                            }
                        }
                    }
                    if(gameState == GameScene.PickUpgrade)
                    {
                        float defaultScale = 2f;
                        for (int u =0; u < 4; u++)
                        {
                            Vector2 drawHere = new Vector2(100 + 80 * (u+1), 400);
                            
                            if(upgradeSceneTimer < 60)
                            {
                                upgradeOptions[u].Draw(spriteBatch, drawHere, defaultScale * ((float)upgradeSceneTimer / 60));
                            }
                            else
                            {
                                upgradeOptions[u].Draw(spriteBatch, drawHere, defaultScale, u == upgradeSelected);
                                if(u == upgradeSelected)
                                {
                                    StringWithSize(spriteBatch, upgradeOptions[u].GetName(), new Vector2(300, 480), 120f, Color.White);
                                    sideText = upgradeOptions[u].GetDescription();
                                }
                            }
                        }
                    }
                    
                    break;
            }

            foreach (Message message in messages)
            {
                message.Draw(spriteBatch);

            }
            if (guide != null)
            {
                guide.PreDraw(spriteBatch);
                guide.Draw(spriteBatch);
                guide.PostDraw(spriteBatch);
            }
            if (sideText != null)
            {
                float scale = .8f;
                Vector2 descriptionPosition = new Vector2(520, 300);
                string desc = sideText;
                string[] words = desc.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                int wordyIndex = 0;
                while (wordyIndex < words.Length)
                {
                    string line = "";
                    while (font.MeasureString(line).X * scale < 220 && wordyIndex < words.Length)
                    {
                        line += words[wordyIndex] + " ";
                        wordyIndex++;
                    }
                    spriteBatch.DrawString(font, line, descriptionPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                    descriptionPosition.Y += 50;
                }
            }
            
            spriteBatch.End();
            sideText = null;
            if (guide != null)
            {
                guide.health = 0;
                gameEntities.Remove(guide);
                guide = null;
            }
            base.Draw(gameTime);
        }
    }
    public enum ColorOptions : byte
    {
        Purple,
        Orange,
        Green,
        Pink,
        Dark,
        Spectre,
        Rainbow
    }
    public enum GameScene : byte
    {
        Combat,
        MainMenu,
        Intro,
        PickUpgrade,
        Outro

    }

}
