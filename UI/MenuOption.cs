using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.UI
{
    public class MenuOption
    {
        Message msg;
        public Vector2 Position = Vector2.Zero;
        bool selected = true;
        public delegate void Action();
        public delegate void MenuHover();
        MenuHover menuHover;
        Action action;
        public MenuOption(string text, Action action, MenuHover menuHover = null, Message.DrawExtras drawExtras = null)
        {
            msg = new Message(text, Position, -1, false, drawExtras);
            this.action = action;
            this.menuHover = menuHover;
        }
        public void Click()
        {
            action();
        }
        int offFactor = 0;
        int maxOffset = 20;
        public void Select()
        {
            selected = true;
            menuHover?.Invoke();
        }
        public void Update()
        {
            /*
            if(selected && sliderPosition != -1)
            {
                if (Miasma.ControlLeft())
                {
                    if (sliderPosition > 0)
                    {
                        sliderPosition--;
                    }
                }
                if (Miasma.ControlRight())
                {
                    if (sliderPosition < 100)
                    {
                        sliderPosition++;
                    }
                }
            }
            msg.sliderPosition = sliderPosition;*/
            if (selected)
            {
                if(offFactor< maxOffset)
                {
                    offFactor++;
                }
                
            }
            else
            {
                if(offFactor>0)
                {
                    offFactor--;
                }
            }
            msg.setPosition(Position + Vector2.UnitX * (offFactor + msg.GetWidth()*.5f));
            msg.miasmic = selected;
            selected = false;
        }
        
    }
}
