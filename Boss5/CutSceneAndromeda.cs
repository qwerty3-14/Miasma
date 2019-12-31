using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miasma.Boss5
{
    public class CutSceneAndromeda : Entity
    {
        public CutSceneAndromeda(Vector2 Position, float rotation = 0, int team = 0) : base(Position, rotation, team)
        {
        }
        float flyInSpeed = 8f;
        public Vector2 flyTo;
        public override void SpecialUpdate()
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
        }
        public override void UpdateHitbox()
        {
            Hitbox = new Rectangle(0, 0, 0, 0);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
        }
        public override void PostDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Miasma.EntityExtras[30], Position + Vector2.UnitX * 100 + new Vector2(-47, -78));
        }
    }
}
