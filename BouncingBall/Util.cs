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


        public static Vector2 ApplyImpulse(Vector2 impulse, float mass)
        {
            //https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331

            var linearVelocity = (CalcInverseOfMass(mass) * impulse);

            return linearVelocity;
        }

        public static Vector2 ApplyImpulse2(PhysObj obj, Vector2 impulse)
        {
            var result = CalcInverseOfMass(obj.Mass) * impulse;

            return result;
            //Calculate angular velocity result from impulse
            //angularVelicty += inverseInertia * Cross(contactVector, impulse);
        }


        public static Vector2 Max(Vector2 v, float max)
        {
            v.X = v.X > max ? max : v.X;
            v.Y = v.Y > max ? max : v.Y;

            return v;
        }


        public static float ToPounds(float mass, float gravity)
        {
            return (mass * gravity) * 0.2248f;
        }

        
        public static Vector2 CalculateForce(Vector2 accleration, float mass)
        {
            return accleration * mass;
        }
    }
}
