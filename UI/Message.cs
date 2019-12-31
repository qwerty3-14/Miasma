using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma.UI
{
    public class Message 
    {
        string text;
        string partialText = "";
        Vector2 position;
        int lingerTime;
        public bool miasmic = false;
        public delegate void DrawExtras(SpriteBatch spriteBatch, Vector2 position, Vector2 textSize, bool miasmic);
        DrawExtras drawExtras;
        
        public Message(string text, Vector2 position, int lingerTime = 120, bool miasmic = false, DrawExtras drawExtras = null)
        {
            this.text = text;
            this.position = position;
            this.lingerTime = lingerTime;
            this.miasmic = miasmic;
            this.drawExtras = drawExtras;
            Miasma.messages.Add(this);
        }
        int counter = 0;
        int i = 0;
        public void Kill()
        {
            lingerTime = 0;
        }
        public void UpdateMessage()
        {
            if (partialText != text)
            {
                counter++;
                if (counter % 5 == 0)
                {
                    i++;
                    Sounds.click.Play();
                }
                partialText = text.Substring(0, i);
            }
            else
            {
                lingerTime--;
                if(lingerTime ==0)
                {
                    Miasma.messages.Remove(this);
                }
            }
        }
        public float GetWidth()
        {
            return Miasma.font.MeasureString(text).X; 
        }
        public void setPosition(Vector2 position)
        {
            this.position = position;
        }
        public Vector2 getPosition()
        {
            return position;
        }
        public bool Complete()
        {
            return text == partialText;
        }
        public void Draw(SpriteBatch spritebatch)
        {
            Vector2 textSize = Miasma.font.MeasureString(text);
            spritebatch.DrawString(Miasma.font, partialText, position - textSize * .5f, miasmic ? Miasma.MiasmaColor(): Color.White);
            if (drawExtras != null)
            {
                drawExtras(spritebatch, position, textSize, miasmic);
            }
            /*
            if (sliderPosition != -1)
            {
                Texture2D bar = Miasma.UISprites[5];
                spritebatch.Draw(bar, position + Vector2.UnitX * (textSize.X * .5f + 20), null, miasmic ? Miasma.MiasmaColor() : Color.White, 0, new Vector2(0, 1.5f), new Vector2(1, 1), SpriteEffects.None, 0);
                Texture2D slider = Miasma.UISprites[6];
                spritebatch.Draw(slider, position + Vector2.UnitX * (textSize.X * .5f + 20 + sliderPosition), null, miasmic ? Miasma.MiasmaColor() : Color.White, 0, new Vector2(3.5f, 4.5f), new Vector2(1, 1), SpriteEffects.None, 0);
            }*/
        }
        
    }
}
