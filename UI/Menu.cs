using Miasma.Ships;
using Miasma.Boss1;
using Miasma.Boss2;
using Miasma.Boss3;
using Miasma.Boss4;
using Miasma.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using Miasma.Boss5;

namespace Miasma.UI
{
    public class Menu : List<MenuOption>
    {
        Vector2 menuPosition = new Vector2(120, 100);
        Vector2 cornerShipPosition = new Vector2(650, 150);
        public Menu()
        {

        }
        int menuIndex = 0;
        public void MenuReset(string renameTitle = "")
        {
            Miasma.messages.Clear();
            new Message(renameTitle, new Vector2(300, 50), -1, true);
            Clear();
            menuIndex = 0;

        }
        void LoadSoundTest()
        {
            MenuReset("Sound Player");
            Add(new MenuOption("Back", delegate ()
            {
                LoadExtrasMenu();
            }));
            foreach (SoundPack sound in Sounds.allSounds)
            {
                Add(new MenuOption(sound.name, delegate ()
                {
                    sound.Play();
                }));
            }
        }
        void LoadMusicTest()
        {
            MenuReset("Music Player");
            Add(new MenuOption("Back", delegate ()
            {
                LoadExtrasMenu();
            }));
            Add(new MenuOption("Play: Menu", delegate ()
            {
                Sounds.PlayMusic(0);
            }));
            Add(new MenuOption("Play: Combat", delegate ()
            {
                Sounds.PlayMusic(1);
            }));
            Add(new MenuOption("Play: Boss Battle", delegate ()
            {
                Sounds.PlayMusic(2);
            }));
        }
        public void LoadGameOver()
        {
            MenuReset("Game Over!");
            Add(new MenuOption("To Main Menu", delegate ()
            {

                Miasma.instance.NewGameReset();
                Sounds.PlayMusic(0);
                LoadMainMenu();
            }));
        }
        public void LoadMainMenu()
        {

            Miasma.Load();
            MenuReset("Miasma");
            string prog = "0";
            if (Miasma.wave == 10)
            {
                prog = "Boss";
            }
            else
            {
                prog = "" + (Miasma.wave + 1);
            }
            if (Miasma.stage > 0 || Miasma.wave > 0)
            {

                Add(new MenuOption("Continue (stage: " + (Miasma.stage + 1) + "-" + prog + ", " + (Miasma.hard ? "hard" : "normal") + ")", delegate ()
                {
                    MenuReset();

                    Miasma.gameState = GameScene.Combat;
                    Sounds.PlayMusic(1);
                    Miasma.player.UpdateStats();
                }));
            }
            Add(new MenuOption("New Game", delegate ()
            {
                LoadDifficulty();
            }));
            Add(new MenuOption("Settings", delegate ()
            {
                LoadSettingsMenu();
            }));
            Add(new MenuOption("Extras", delegate ()
            {
                LoadExtrasMenu();
            }));
            Add(new MenuOption("Quit", delegate ()
            {
                Miasma.instance.Exit();
            }));
        }
        public void LoadPause()
        {
            MenuReset("Paused");
            Add(new MenuOption("Back to Game", delegate ()
            {
                Miasma.LeaveMenu();
            },
            delegate ()
            {
                if (Controls.JustPushedPause())
                {
                    Miasma.LeaveMenu();
                }
            }));
            Add(new MenuOption("To Main Menu", delegate ()
            {
                Miasma.instance.NewGameReset();
                LoadMainMenu();
            },
            delegate ()
            {
                if (Controls.JustPushedPause())
                {
                    Miasma.LeaveMenu();
                }
            }));
        }
        void LoadDifficulty()
        {
            MenuReset("Select Difficulty");
            Add(new MenuOption("Normal", delegate ()
            {
                Miasma.ResetUpgradePool();
                Miasma.stage = 0;
                Miasma.wave = 0;
                Miasma.hard = false;

                Miasma.LeaveMenu();
                Miasma.player.UpdateStats();
                Miasma.Save();
                Sounds.PlayMusic(1);
            }));
            Add(new MenuOption("Hard", delegate ()
            {
                Miasma.ResetUpgradePool();
                Miasma.stage = 0;
                Miasma.hard = true;
                Miasma.wave = 0;
                Miasma.LeaveMenu();
                Miasma.player.UpdateStats();
                Miasma.Save();
                Sounds.PlayMusic(1);
            }));
            Add(new MenuOption("Back", delegate ()
            {
                LoadMainMenu();
            }));
        }

        void SliderUI(SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic, int value)
        {
            Texture2D bar = Miasma.UISprites[5];
            spritebatch.Draw(bar, position + Vector2.UnitX * (textSize.X * .5f + 20), null, miasmic ? Miasma.MiasmaColor() : Color.White, 0, new Vector2(0, 1.5f), new Vector2(1, 1), SpriteEffects.None, 0);
            Texture2D slider = Miasma.UISprites[6];
            spritebatch.Draw(slider, position + Vector2.UnitX * (textSize.X * .5f + 20 + value), null, miasmic ? Miasma.MiasmaColor() : Color.White, 0, new Vector2(3.5f, 4.5f), new Vector2(1, 1), SpriteEffects.None, 0);
        }
        void ExtraText(SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic, String value)
        {
            Vector2 mySize = Miasma.font.MeasureString(value);
            spritebatch.DrawString(Miasma.font, value, (position - mySize * .5f) + (mySize.X * .5f + textSize.X * .5f) * Vector2.UnitX, miasmic ? Miasma.MiasmaColor() : Color.White);
        }
        void GamepadButtonText(SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic, int value, bool isSetting)
        {
            String showText = "???";
            if (!isSetting)
            {
                switch (value)
                {
                    case 0:
                        showText = "A";
                        break;
                    case 1:
                        showText = "B";
                        break;
                    case 2:
                        showText = "X";
                        break;
                    case 3:
                        showText = "Y";
                        break;
                    case 4:
                        showText = "LB";
                        break;
                    case 5:
                        showText = "LStick";
                        break;
                    case 6:
                        showText = "RB";
                        break;
                    case 7:
                        showText = "RStick";
                        break;
                    case 8:
                        showText = "Back";
                        break;
                    case 9:
                        showText = "Big";
                        break;
                    case 10:
                        showText = "Start";
                        break;
                }
            }
            ExtraText(spritebatch, position, textSize, miasmic, showText);
        }
        void LoadSettingsMenu()
        {
            MenuReset("Settings");
            Add(new MenuOption("Miasma Color", delegate ()
            {
                LoadColorMenu();
            }));
            Add(new MenuOption("Sound Effects volume", delegate ()
            {

            }, delegate ()
            {
                if (Controls.ControlLeft())
                {
                    if (Sounds.sfxVolume > 0)
                    {
                        Sounds.sfxVolume--;
                    }
                }
                if (Controls.ControlRight())
                {
                    if (Sounds.sfxVolume < 100)
                    {
                        Sounds.sfxVolume++;

                    }
                }
            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                SliderUI(spritebatch, position, textSize, miasmic, Sounds.sfxVolume);
            }));
            Add(new MenuOption("Music volume", delegate ()
            {

            }, delegate ()
            {
                if (Controls.ControlLeft())
                {
                    if (Sounds.musicVolume > 0)
                    {
                        Sounds.musicVolume--;
                        Sounds.UpdateMusicVolume();
                    }
                }
                if (Controls.ControlRight())
                {
                    if (Sounds.musicVolume < 100)
                    {
                        Sounds.musicVolume++;
                        Sounds.UpdateMusicVolume();
                    }
                }
            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                SliderUI(spritebatch, position, textSize, miasmic, Sounds.musicVolume);
            }));
            Add(new MenuOption("Keyboard Controls", delegate ()
            {
                LoadKeyboardConfig();
            }));
            Add(new MenuOption("Gamepad Controls", delegate ()
            {
                LoadGamepadConfig();
            }));


            Add(new MenuOption("Back", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            }));

        }
        void LoadGamepadConfig()
        {
            MenuReset("Change Controls");
            Add(new MenuOption("Back", delegate ()
            {
                LoadSettingsMenu();
            }));
            Add(new MenuOption("Shoot: ", delegate ()
            {
                Controls.changingShootButtonGamepad = true;
            }, delegate ()
            {


            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                GamepadButtonText(spritebatch, position, textSize, miasmic, Controls.gamePadShoot, Controls.changingShootButtonGamepad);
            }));
            Add(new MenuOption("Launch Miasma: ", delegate ()
            {
                Controls.changingMiasmaButtonGamepad = true;
            }, delegate ()
            {


            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                GamepadButtonText(spritebatch, position, textSize, miasmic, Controls.gamePadMiasma, Controls.changingMiasmaButtonGamepad);
            }));
        }
        void LoadKeyboardConfig()
        {
            MenuReset("Change Controls");
            Add(new MenuOption("Back", delegate ()
            {
                LoadSettingsMenu();
            }));
            Add(new MenuOption("Shoot: ", delegate ()
            {
                Controls.changingShootButtonKeyboard = true;
            }, delegate ()
            {


            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                ExtraText(spritebatch, position, textSize, miasmic, Controls.changingShootButtonKeyboard ? "???" : " " + (Keys)Controls.keyboardShoot);
            }));
            Add(new MenuOption("Launch Miasma: ", delegate ()
            {
                Controls.changingMiasmaButtonKeyboard = true;
            }, delegate ()
            {


            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                ExtraText(spritebatch, position, textSize, miasmic, Controls.changingMiasmaButtonKeyboard ? "???" : " " + (Keys)Controls.keyboardMiasma);
            }));
        }
        void LoadColorMenu()
        {
            MenuReset("Miasma Color");
            Add(new MenuOption("Classic", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            },
            delegate ()
            {
                Miasma.myColor = ColorOptions.Purple;

            }));
            Add(new MenuOption("Plague", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            },
            delegate ()
            {
                Miasma.myColor = ColorOptions.Orange;
            }));

            Add(new MenuOption("Alien", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            },
            delegate ()
            {
                Miasma.myColor = ColorOptions.Green;
            }));
            Add(new MenuOption("Flesh", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            },
            delegate ()
            {
                Miasma.myColor = ColorOptions.Pink;
            }));
            Add(new MenuOption("Dark", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            },
            delegate ()
            {
                Miasma.myColor = ColorOptions.Dark;
            }));
            Add(new MenuOption("Spectre", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            },
            delegate ()
            {
                Miasma.myColor = ColorOptions.Spectre;
            }));
            Add(new MenuOption("Rainbow", delegate ()
            {
                Miasma.Save();
                LoadMainMenu();
            },
            delegate ()
            {
                Miasma.myColor = ColorOptions.Rainbow;
            }));
        }
        void LoadExtrasMenu()
        {
            MenuReset("Extras");
            Add(new MenuOption("Sound Player", delegate ()
            {
                LoadSoundTest();
            }));
            Add(new MenuOption("Music Player", delegate ()
            {
                LoadMusicTest();
            }));
            Add(new MenuOption("Guide", delegate ()
            {
                LoadGuide();
            }));
            Add(new MenuOption("Back", delegate ()
            {
                LoadMainMenu();
            }));
        }

        void LoadGuide()
        {
            MenuReset("Guide");
            Add(new MenuOption("Mechanics", delegate ()
            {
                LoadMechanics();
            }));
            Add(new MenuOption("Ships", delegate ()
            {
                LoadShips();
            }));
            Add(new MenuOption("Bosses", delegate ()
            {
                LoadBosses();
            }));
            Add(new MenuOption("Upgrades", delegate ()
            {
                LoadUpgrades();
            }));
            Add(new MenuOption("Back", delegate ()
            {
                LoadMainMenu();
            }));
        }
        void LoadMechanics()
        {
            MenuReset("Mechanics");
            Add(new MenuOption("Fleets", delegate ()
            {
                LoadGuide();
            }, delegate ()
            {
                
            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                if(miasmic)
                {
                    Miasma.sideText = "Enemy ships act as a fleet. When no ships are 'acting' the game will pick a random selection of ships (amount based on wave/difficulty) to start acting. Different ships act in different ways.";
                }
            }));
            Add(new MenuOption("Infecting", delegate ()
            {
                LoadGuide();
            }, delegate ()
            {
                
            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                if (miasmic)
                {
                    Miasma.sideText = "When your Miasma capsule lands the killing blow on an enemy they will be infected instead of destroyed. Infected ships will attack your enemies and slowly lose health over time. Don't worry about friendly fire you and the infected ship can't hurt each other.";
                }
            }));
            Add(new MenuOption("Upgrades", delegate ()
            {
                LoadGuide();
            }, delegate ()
            {
                
            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                if (miasmic)
                {
                    Miasma.sideText = "Upon defeating a boss, 4 random upgrades will apear you will be asked to pick one. These upgrades will last for the rest of the game until you start a new game.";
                }
            }));
        }
        void LoadShipData(string name, int level, Entity ship, string description)
        {
            if(Miasma.totalProgress < level)
            {
                name = "???";
                description = "Play more of the game to see this.";
            }
            Add(new MenuOption(name, delegate ()
            {
                Miasma.gameEntities.Clear();
                Miasma.player = new TheTransmission(new Vector2(300, 700), team: 1);
                LoadGuide();
                Debug.WriteLine(Miasma.gameEntities.Count);
                
                Miasma.boss = null;
            }, delegate ()
            {

            }, delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
            {
                if (miasmic)
                {
                    Vector2 size = Miasma.font.MeasureString(name);
                    spritebatch.DrawString(Miasma.font, name, cornerShipPosition + new Vector2( -size.X/2, -80), Color.Black);
                    if (Miasma.totalProgress >= level)
                    {
                        Miasma.guide = ship;
                    }
                    Miasma.sideText = description;
                }
            }));
        }
        void LoadShips()
        {
            MenuReset("Ships");
            LoadShipData("Light Gunship", 0, new LightGunship(cornerShipPosition, cornerShipPosition), "The most common ship in the galaxy. This ship owes its popularity to being the cheapest ship approved by the intergalactic society.");
            LoadShipData("Light Charger", 1, new LightCharger(cornerShipPosition, cornerShipPosition), "Charges toward you at highs speeds. There is ravenous debate in the intergalactic society on whether this should be classified as a ship or a warhead.");
            LoadShipData("Light Artillary", 2, new LightArtillary(cornerShipPosition, cornerShipPosition), "Fires artillary shells. The smallest ship that can handle an artillary gun.");
            LoadShipData("Beam ship", 4, new BeamShip(cornerShipPosition, cornerShipPosition), "Works with another beams ship to vaporize you! Origonally built to break incoming asteriod, the military saw great applications for wrafare.");
            LoadShipData("Medium Gunship", 7, new MediumGunship(cornerShipPosition, cornerShipPosition), "A stronger gunship. Ships of this size and larger are usually banned for civilian use.");
            LoadShipData("Teleporting Fighter", 11, new TeleportingFighter(cornerShipPosition, cornerShipPosition), "Teleports to random locations and shoots. If they know how to make teleporters, then why don't they teleport a bomb on your ship? It's almost like this is a game and not a real universe.");
            LoadShipData("Medium Charger", 13, new MediumCharger(cornerShipPosition, cornerShipPosition), "Charges toward you at highs speeds... twice. Somehow the larger size allows the targetting computer to go into overdrive.");
            LoadShipData("Bomber", 17, new Bomber(cornerShipPosition, cornerShipPosition), "Drops a bomb, bombs must be shot down before the blow up next to you!");
            LoadShipData("Helix Builder", 22, new HelixBuilder(cornerShipPosition, cornerShipPosition), "Builds a helix to limit where enemies can go. For some reason the intergalactic society has banned helix builders and bombers appearing in the same fleet.");
            LoadShipData("Medium Artillary", 25, new MediumArtillary(cornerShipPosition, cornerShipPosition), "Fires more artillary shells. In space the best defense is evasion... so this ship fires in multiple locations to maximise the chance of hitting.");
            LoadShipData("Carrier", 26, new Carrier(cornerShipPosition, cornerShipPosition), "Launches new ships, fires artillary when ships are already built. These ships were made to supply the demand for light gunships, the artillary guns were added to fight pirates.");
            LoadShipData("Spinner", 33, new Spinner(cornerShipPosition, cornerShipPosition), "Spins around with beam. I'm getting dizzy...");
            LoadShipData("Elite Gunship", 36, new EliteGunship(cornerShipPosition, cornerShipPosition), "An even stronger gunship. Big and expensive, the intergalactic society reserves these for only the most imminent of galactic threats... good job");
            LoadShipData("Spartan", 38, new Spartan(cornerShipPosition, cornerShipPosition), "Blocks with a deflector shield. It seems these ships weren't prepared for the outbreak, their shields have no effect on your miasma!");
            LoadShipData("Ruthless Charger", 44, new RuthlessCharger(cornerShipPosition, cornerShipPosition), "Charges toward you at highs speeds... four times. If at first you don't succeed try try try again, then quit.");
            LoadShipData("Cruiser", 46, new Cruiser(cornerShipPosition, cornerShipPosition), "Fires a large volley. This ship delivers a lot of firepower for its size.");
            LoadShipData("Bombardment Artillary", 48, new BombardmentArtillary(cornerShipPosition, cornerShipPosition), "Fires even more artillary shells. The largest mass produced ship in the galaxy, this thing rivals some custom built flagships.");

        }
        void LoadBosses()
        {
            MenuReset("Bosses");
            LoadShipData("The Phalax", 10, new ShieldedCarrier(cornerShipPosition), "Shielded battle carrier. Capable of similating an army, this ship is one of the first responses to threats.");
            LoadShipData("The Gemini", 21, new Gemini(cornerShipPosition), "Siege twins. This par of ships was made to bombard planets. Their use was discontinued when it was decided that firing lots of projetiles wasn't very \"Economical for the federation\"");
            LoadShipData("The Pulsar", 32, new Pulsar(cornerShipPosition), "Compact destroyer. This ship was built to hunt down and take out targets in one on one combat.");
            LoadShipData("The Jupiter", 43, new Jupiter(cornerShipPosition + Vector2.UnitY * 50), "Specialty and espianage lightning battleship. One of the strangest ships in the galaxy invent by the mad scientist Alset Elocin");
            LoadShipData("The Andromeda", 54, new TheAndromeda(Vector2.Zero), "The largest ship in galaxy named after a galaxy itself. This thing has saved many worlds and is used to end only the most dire of situations.");
        }
        void LoadUpgrades()
        {
            MenuReset("Upgrades");
            foreach(Upgrade upgrade in Miasma.player.upgrades)
            {
                if(upgrade != null)
                {
                    Add(new MenuOption(upgrade.GetName(), delegate ()
                    {
                        LoadGuide();
                    },
                    delegate ()
                    {

                    }, 
                    delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
                    {
                        if (miasmic)
                        {
                            Vector2 size = Miasma.font.MeasureString(upgrade.GetName());
                            spritebatch.DrawString(Miasma.font, upgrade.GetName(), cornerShipPosition + new Vector2(-size.X / 2, -80), Color.Black);
                            Miasma.sideText = upgrade.GetDescription();
                            upgrade.Draw(spritebatch, cornerShipPosition, 1f, false);
                        }
                    }));
                }
            }
            foreach (Upgrade upgrade in Miasma.UpgradePool)
            {
                if (upgrade != null)
                {
                    Add(new MenuOption(upgrade.GetName(), delegate ()
                    {
                        LoadGuide();
                    },
                    delegate ()
                    {

                    },
                    delegate (SpriteBatch spritebatch, Vector2 position, Vector2 textSize, bool miasmic)
                    {
                        if (miasmic)
                        {
                            Vector2 size = Miasma.font.MeasureString(upgrade.GetName());
                            spritebatch.DrawString(Miasma.font, upgrade.GetName(), cornerShipPosition + new Vector2(-size.X / 2, -80), Color.Black);
                            Miasma.sideText = upgrade.GetDescription();
                            upgrade.Draw(spritebatch, cornerShipPosition, 1f, false);
                        }
                    }));
                }
            }
        }

        public void Manage()
        {
            if (Controls.JustPushedUp())
            {
                menuIndex--;
                Sounds.menuChange.Play();
            }

            if (Controls.JustPushedDown())
            {
                menuIndex++;
                Sounds.menuChange.Play();
            }

            if (Count > 0)
            {
                while (menuIndex >= Count)
                {
                    menuIndex -= Count;
                }
                while (menuIndex < 0)
                {
                    menuIndex += Count;
                }
            }

            this[menuIndex].Select();

            if (Controls.JustPushedMenuSelect())
            {
                this[menuIndex].Click();
            }
            Vector2 offPosition = menuPosition;
            foreach (MenuOption option in this)
            {
                option.Position = offPosition;
                option.Update();
                offPosition += Vector2.UnitY * (Count > 12 ? 35: 50);
            }
        }
    }
}
