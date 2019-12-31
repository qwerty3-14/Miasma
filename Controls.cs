using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma
{
    public static class Controls
    {

        delegate bool GamePadButtonPress();
        delegate int KeyboardButtonPress();
        static List<GamePadButtonPress> gamePadButtonOptions = new List<GamePadButtonPress>();
        static List<KeyboardButtonPress> keyboardButtonOptions = new List<KeyboardButtonPress>();
        public static int keyboardShoot = (int)Keys.Space;
        public static int keyboardMiasma = (int)Keys.LeftShift;
        public static int gamePadShoot = 0;
        public static int gamePadMiasma = 1;
        public static bool changingShootButtonGamepad = false;
        public static bool changingMiasmaButtonGamepad = false;
        public static bool changingShootButtonKeyboard = false;
        public static bool changingMiasmaButtonKeyboard = false;
         static bool pushedUp = false;
         static bool pushedDown = false;
         static bool pushedSpace = false;
         static bool pushedPause = false;
         static bool pushedRight = false;
         static bool pushedLeft = false;
         static bool pushedEnter = false;
        public static bool nonMenuPushed()
        {
            return !pushedUp && !pushedDown && !pushedEnter;
        }
        public static void setAllMenuToPushed()
        {
            pushedUp = true;
            pushedDown = true;
            pushedEnter = true;
        }
        static bool ControlMenu(bool buttons, ref bool status)
        {
            if (buttons)
            {

                if (!status)
                {
                    status = true;
                    return true;
                }
                status = true;

            }
            else
            {
                status = false;
            }
            return false;
        }
        public static bool ControlMenuSelect()
        {
            return Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed;
        }
        public static bool ControlShoot()
        {
            return Keyboard.GetState().IsKeyDown((Keys)keyboardShoot) || gamePadButtonOptions[gamePadShoot]();
        }
        public static bool ControlMiasma()
        {
            return Keyboard.GetState().IsKeyDown((Keys)keyboardMiasma) || gamePadButtonOptions[gamePadMiasma]();
        }
        public static bool ControlUp()
        {
            return Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed;
        }
        public static bool ControlDown()
        {
            return Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed;
        }
        public static bool ControlLeft()
        {
            return Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed;
        }
        public static bool ControlRight()
        {
            return Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed;
        }
        public static bool ControlPause()
        {
            return Keyboard.GetState().IsKeyDown(Keys.P) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed;
        }
        public static bool JustPushedMenuSelect()
        {
            return ControlMenu(ControlMenuSelect(), ref pushedEnter);
        }
        
        public static bool JustPushedShoot()
        {
            return ControlMenu(ControlShoot(), ref pushedSpace);
        }
        public static bool JustPushedUp()
        {
            return ControlMenu(ControlUp(), ref pushedUp);
        }
       
        public static bool JustPushedDown()
        {
            return ControlMenu(ControlDown(), ref pushedDown);
        }
        public static bool JustPushedLeft()
        {
            return ControlMenu(ControlLeft(), ref pushedLeft);
        }
        public static bool JustPushedRight()
        {
            return ControlMenu(ControlRight(), ref pushedRight);
        }
        public static bool JustPushedPause()
        {
            return ControlMenu(ControlPause(), ref pushedPause);
        }

        public static void Initialize()
        {
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.BigButton == ButtonState.Pressed;
            });
            gamePadButtonOptions.Add(delegate ()
            {
                return GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed;
            });
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                keyboardButtonOptions.Add(delegate ()
                {
                    return Keyboard.GetState().IsKeyDown(key) ? (int)key : -1;
                });
            }
        }
        public static void ModifyControlSettings()
        {
            if (changingShootButtonKeyboard && nonMenuPushed())
            {
                for (int i = 0; i < keyboardButtonOptions.Count; i++)
                {
                    if (keyboardButtonOptions[i]() != -1)
                    {
                        keyboardShoot = keyboardButtonOptions[i]();
                        changingShootButtonKeyboard = false;
                        setAllMenuToPushed();
                        break;
                    }
                }
            }
            if (changingMiasmaButtonKeyboard && nonMenuPushed())
            {
                for (int i = 0; i < keyboardButtonOptions.Count; i++)
                {
                    if (keyboardButtonOptions[i]() != -1)
                    {
                        keyboardMiasma = keyboardButtonOptions[i]();
                        changingMiasmaButtonKeyboard = false;
                        setAllMenuToPushed();
                        break;
                    }
                }
            }
            if (changingShootButtonGamepad && nonMenuPushed())
            {
                for (int i = 0; i < gamePadButtonOptions.Count; i++)
                {
                    if (gamePadButtonOptions[i]())
                    {

                        gamePadShoot = i;
                        changingShootButtonGamepad = false;
                        setAllMenuToPushed();
                        break;
                    }
                }
            }
            if (changingMiasmaButtonGamepad && nonMenuPushed())
            {
                for (int i = 0; i < gamePadButtonOptions.Count; i++)
                {
                    if (gamePadButtonOptions[i]())
                    {
                        gamePadMiasma = i;
                        changingMiasmaButtonGamepad = false;
                        setAllMenuToPushed();
                        break;
                    }
                }
            }
        }
    }
}
