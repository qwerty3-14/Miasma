using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma
{
    public static class Functions
    {
        public static Vector2 PolarVector(float radius, float theta)
        {
            return (new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius);
        }
        public static float ToRotation(Vector2 v)
        {
            return (float)Math.Atan2((double)v.Y, (double)v.X);
        }
        

        public static float SlowRotation(float currentRotation, float targetAngle, float speed)
        {
            int f = 1; //this is used to switch rotation direction
            float actDirection = ToRotation(new Vector2((float)Math.Cos(currentRotation), (float)Math.Sin(currentRotation)));
            targetAngle = ToRotation(new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)));

            //this makes f 1 or -1 to rotate the shorter distance
            if (Math.Abs(actDirection - targetAngle) > Math.PI)
            {
                f = -1;
            }
            else
            {
                f = 1;
            }

            if (actDirection <= targetAngle + speed * 2 && actDirection >= targetAngle - speed * 2)
            {
                actDirection = targetAngle;
            }
            else if (actDirection <= targetAngle)
            {
                actDirection += speed * f;
            }
            else if (actDirection >= targetAngle)
            {
                actDirection -= speed * f;
            }
            actDirection = ToRotation(new Vector2((float)Math.Cos(actDirection), (float)Math.Sin(actDirection)));

            return actDirection;
        }
        public static float RandomRotation()
        {
            return (float)(Math.PI * 2 * Miasma.random.NextDouble());
        }
        
        // based on code from https://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion
        public static Color ToRgb(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0f)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = hueToRgb(p, q, h + 1f / 3f);
                g = hueToRgb(p, q, h);
                b = hueToRgb(p, q, h - 1f / 3f);
            }
            

            return new Color(r, g, b);
        }
        static int to255(float v) { return (int)Math.Min(255, 256 * v); }

        /** Helper method that converts hue to rgb */
        static float hueToRgb(float p, float q, float t)
        {
            if (t < 0f)
                t += 1f;
            if (t > 1f)
                t -= 1f;
            if (t < 1f / 6f)
                return p + (q - p) * 6f * t;
            if (t < 1f / 2f)
                return q;
            if (t < 2f / 3f)
                return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }
        public static bool Between(this Vector2 vec, Vector2 minimum, Vector2 maximum)
        {
            return vec.X >= minimum.X && vec.X <= maximum.X && vec.Y >= minimum.Y && vec.Y <= maximum.Y;
        }
        public static bool RectangleLineCollision(Rectangle rectangle, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2? empty = null;
            return RectangleLineCollision(rectangle, lineStart, lineEnd, ref empty);
        }
        public static bool RectangleLineCollision(Rectangle rectangle, Vector2 lineStart, Vector2 lineEnd, ref Vector2? collisionPoint)
        {
            Vector2 rectTopLeft = rectangle.Location.ToVector2();
            Vector2 rectBottomRight = rectTopLeft + rectangle.Size.ToVector2();
            float length = (lineEnd - lineStart).Length();
            float jumpLength = 1;
            float jumpCounts = length / jumpLength;
            float direction = ToRotation(lineEnd - lineStart);
            for (int i =0; i < jumpCounts; i++)
            {
                //new Particle(lineStart + PolarVector(jumpLength * i, direction), Vector2.Zero, 6, 30);
                if ((lineStart +PolarVector(jumpLength *i, direction)).Between(rectTopLeft, rectBottomRight))
                {
                    collisionPoint = lineStart + PolarVector(jumpLength * i, direction);
                    return true;
                }
            }
            collisionPoint = null;
            return false;
        }
    }
}
