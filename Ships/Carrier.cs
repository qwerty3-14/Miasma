using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miasma.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Ships
{
    public class Carrier : Ship
    {
        List<LightGunship> Children = new List<LightGunship>();
        int childrenHomeDistance = 100;
        int childSpread = 100;
        public Carrier(Vector2 Position, Vector2 Home, float rotation = 0, int team = 0, int childrenHomeDistanceModifier = 0, int childSpread = 100) : base(Position, Home, rotation, team)
        {
            entityID = 21;
            maxHealth = health = 60;
            childrenHomeDistance += childrenHomeDistanceModifier;
            this.childSpread = childSpread;
        }
        bool spawn = false;
        public override void ActionStart()
        {
            spawn = Children.Count <= 0;
            actCounter = 0;
        }
        int actCounter = 0;
        public override void ActionUpdate()
        {
            NormalMovement();
            actCounter++;
            if (spawn)
            {
                if(actCounter % 30 ==0)
                {
                    Sounds.carierLaunch.Play();
                    LightGunship gunner = new LightGunship(Position, new Vector2(home.X - childSpread / 2 + (Children.Count + 1) * childSpread / 4, home.Y + childrenHomeDistance), rotation, team);
                    gunner.CarrierLag((childrenHomeDistance-20)/3);
                    Children.Add(gunner);
                    Miasma.enemyFleet.Add(gunner);
                }
                if(Children.Count >=3)
                {
                    acting = -1;
                }
            }
            else
            {
                if(actCounter < 30)
                {
                    gunOutAmount = gunOutAmountMax * ((float)actCounter / 30f);
                }
                if(actCounter == 60 || actCounter == 90)
                {
                    Shoot();
                }
                if(actCounter > 90)
                {
                    gunOutAmount = gunOutAmountMax * (1f-((float)(actCounter - 90) / 30f));
                    
                }
                if(actCounter>= 120)
                {
                    acting = -1;
                }
            }
        }
        public override void InfectedUpdate()
        {
            NormalMovement();
            actCounter++;
            if (actCounter % 30 == 0)
            {
                health -= 10;
                if (health > 0)
                {
                    LightGunship gunner = new LightGunship(Position, Vector2.Zero, rotation, team);
                    gunner.CarrierLag(30);
                }
            }
        }
        public override void SpecialUpdate()
        {
            if (!IsActing())
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (!Miasma.gameEntities.Contains(Children[i]))
                    {
                        Children.RemoveAt(i);
                    }
                }
            }
        }
        void Shoot()
        {
            new ArtillaryPulse(Position + new Vector2(-15.5f, 11), Vector2.UnitY * 9, (float)Math.PI, team);
            new ArtillaryPulse(Position + new Vector2(15.5f, 11), Vector2.UnitY * 9, (float)Math.PI, team);
        }
        float gunOutAmount = 0;
        int gunOutAmountMax = 8;
        public override void PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntitySprites[entityID];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
            texture = Miasma.EntityExtras[9];
            spriteBatch.Draw(texture, Position + Functions.PolarVector(-11.5f + (gunOutAmountMax- gunOutAmount), rotation), null, Color.White, rotation, new Vector2(texture.Width, texture.Height * .5f), new Vector2(1, 1), SpriteEffects.None, 0);
            texture = Miasma.EntityExtras[10];
            spriteBatch.Draw(texture, Position + Functions.PolarVector(11.5f - (gunOutAmountMax - gunOutAmount), rotation), null, Color.White, rotation, new Vector2(0, texture.Height * .5f), new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Miasma.EntityExtras[8];
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, new Vector2(texture.Width, texture.Height) * .5f, new Vector2(1, 1), SpriteEffects.None, 0);
        }
    }
}
