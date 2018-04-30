using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingBall
{
    public static class ExtensionMethods
    {
        public static Vector2 RemoveAnyNaN(this Vector2 vector)
        {
            vector.X = float.IsNaN(vector.X) ? 0 : vector.X;
            vector.Y = float.IsNaN(vector.Y) ? 0 : vector.Y;

            return vector;
        }

        public static bool HasAnyNaN(this Vector2 v)
        {
            return float.IsNaN(v.X) || float.IsNaN(v.Y);
        }
    }
}
