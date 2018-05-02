using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingBall
{
    public static class Util
    {
        public static float CalcInverseOfMass(float mass)
        {
            return 1.0f / mass;
        }

        public static Vector2 RemoveNan(Vector2 value)
        {
            value.X = float.IsNaN(value.X) ? 0f : value.X;
            value.Y = float.IsNaN(value.Y) ? 0f : value.Y;

            return value;
        }

        public static Vector2 RemoveInfinity(Vector2 value)
        {
            value.X = float.IsInfinity(value.X) ? 0f : value.X;
            value.Y = float.IsInfinity(value.Y) ? 0f : value.Y;

            return value;
        }
    }
}
