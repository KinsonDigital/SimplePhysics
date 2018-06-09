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


        /// <summary>
        /// Converts the given list of <paramref name="vertices"/> to an array of <see cref="Vector"/>s.
        /// </summary>
        /// <param name="vertices">The list of <see cref="Vector"/>s to convert.</param>
        /// <returns></returns>
        public static Line[] ToLines(this Vector2[] vertices)
        {
            var result = new Line[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                var nextVerticesIndex = i < vertices.Length - 1 ? i + 1 : 0;

                result[i] = new Line(vertices[i], vertices[nextVerticesIndex]);
            }

            return result;
        }


        /// <summary>
        /// Converts the given list of <paramref name="lines"/> to an array of <see cref="Vector"/>s.
        /// </summary>
        /// <param name="lines">The list of <see cref="Line"/>s to convert.</param>
        /// <returns></returns>
        public static Vector2[] ToVertices(this Line[] lines)
        {
            var result = new Vector2[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                result[i] = lines[i].Start;
            }

            return result;
        }
    }
}
