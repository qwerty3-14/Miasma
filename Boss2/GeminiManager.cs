using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Miasma.Boss2
{
    public class GeminiManager : Boss
    {
        Gemini[] gemini = new Gemini[2];
        public GeminiManager(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
            maxHealth = 600;
            health = 600;
            name = "The Gemini";
            gemini[0] = new Gemini(new Vector2(0, 50));
            gemini[1] = new Gemini(new Vector2(600, 50 + (2*Gemini.speed * Gemini.turnAroundSpeedModifier) /(float)Math.PI), (float)Math.PI);
        }
        public override void SpecialUpdate()
        {
            health = gemini[0].health + gemini[1].health;
            maxHealth = gemini[0].maxHealth + gemini[1].maxHealth;
            if(!Miasma.gameEntities.Contains(gemini[0]))
            {
                gemini[1].Enrage();
            }
            if (!Miasma.gameEntities.Contains(gemini[1]))
            {
                gemini[0].Enrage();
            }
        }
        public override void UpdateHitbox()
        {
            Hitbox = new Rectangle(0,0,0,0);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
    
}
