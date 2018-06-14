using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingBall
{
    public class Manifold
    {
        public int ContactCount { get; set; }

        public float Penetration { get; set; }

        public Vector2 Normal { get; set; }

        public List<Vector2> ContactVectors { get; set; } = new List<Vector2>();
    }
}
