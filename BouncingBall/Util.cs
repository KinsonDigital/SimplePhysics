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


        public static Vector2 ApplyImpulse(PhysObj obj, Vector2 impulse)
        {
            var result = CalcInverseOfMass(obj.Mass) * impulse;

            return result;
            //Calculate angular velocity result from impulse
            //angularVelicty += inverseInertia * Cross(contactVector, impulse);
        }


        public static void CalcAngularStuff(PhysObj obj)
        {
            //var centerOfMass = new Vector2(obj.Location.X + obj.HalfWidth, obj.Location.Y - obj.HalfHeight);
            //var applicationPoint = obj.Location; //Top left corner
            //var momentArm = applicationPoint - centerOfMass;

            //var myForce = new Vector2(2, 2);// Random chosen values
            //var parallelComponent = momentArm * (CalcDotProduct(myForce, momentArm) / CalcDotProduct(momentArm, momentArm));
            //var angularForce = myForce - parallelComponent;

            //var torque = angularForce * momentArm.Length();

            //return angularForce * pointOfApplication.Length();
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }


        public static Vector2 Cross(Vector2 a, float scalar)
        {
            return new Vector2(scalar * a.Y, -scalar * a.X);
        }

        public static Vector2 Cross(float scalar, Vector2 a)
        {
            return new Vector2(-scalar * a.Y, scalar * a.X);
        }


        public static float CalcDotProduct(Vector2 vector1, Vector2 vector2)
        {
            return (vector1.X * vector2.X) + (vector1.Y * vector2.Y);
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
