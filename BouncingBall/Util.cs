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
    }
}
