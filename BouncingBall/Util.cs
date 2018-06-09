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
        public static float CalcCircleArea(float radius)
        {
            return (float)Math.PI * (radius * radius);
        }


        public static float CalcMassCircle(float density, float radius)
        {
            return density * CalcCircleArea(radius);
        }


        public static float CalcPolygonArea(Line[] faces)
        {
            var area = 0.0f;

            foreach (var face in faces)
            {
                area += 0.5f * Math.Abs(Cross(face.Start, face.Stop));
            }


            return area;
        }


        public static float CalcPolygonMass(Polygon poly, float density)
        {
            return density * CalcPolygonArea(poly.Vertices.ToLines());
        }


        /// <summary>
        /// Calculates the slope of a given line.
        /// </summary>
        /// <param name="line">The line to calculate.</param>
        /// <returns></returns>
        public static float CalculateSlope(Line line)
        {
            return (line.Stop.Y - line.Start.Y) / (line.Stop.X - line.Start.X);
        }


        /// <summary>
        /// Calculates the length of the line created from the given <paramref name="startX"/>, <paramref name="startY"/>, 
        /// <paramref name="stopX"/>, and <paramref name="stopY"/>.
        /// </summary>
        /// <param name="startX">The X coordinate of the starting position of the line.</param>
        /// <param name="startY">The Y coordinate of the starting position of the line.</param>
        /// <param name="stopX">The X coordinate of the ending position of the line.</param>
        /// <param name="stopY">The Y coordinate of the ending position of the line.</param>
        /// <returns></returns>
        public static float CalculateLength(float startX, float startY, float stopX, float stopY)
        {
            var dx = stopX - startX;
            var dy = stopY - startX;


            return (float)Math.Sqrt((dx * dx) + (dy * dy));
        }


        /// <summary>
        /// Calculates the length of the line created from the given <paramref name="start"/> and <paramref name="stop"/> <see cref="Vector"/>s.
        /// </summary>
        /// <param name="start">The starting point of the line.</param>
        /// <param name="stop">The ending point of the line.</param>
        /// <returns></returns>
        public static float CalculateLength(Vector2 start, Vector2 stop)
        {
            return CalculateLength(start.X, start.Y, stop.X, stop.Y);
        }


        /// <summary>
        /// Calculates the length of the given <paramref name="line"/>.
        /// </summary>
        /// <param name="line">The line to get the length from.</param>
        /// <returns></returns>
        public static float CalculateLength(Line line)
        {
            return CalculateLength(line.Start, line.Stop);
        }


        /// <summary>
        /// Calculates the centroid of the given <see cref="Vector"/>s that make up a polygon.
        /// </summary>
        /// <param name="vertices">The list of <see cref="Vector"/>s of a polygon.</param>
        /// <returns></returns>
        public static Vector2 CalculateCentroid(Vector2[] vertices)
        {
            var sumX = 0f;
            var sumY = 0f;

            for (int i = 0; i < vertices.Length; i++)
            {
                sumX += vertices[i].X;
                sumY += vertices[i].Y;
            }


            return new Vector2(sumX / vertices.Length, sumY / vertices.Length);
        }


        /// <summary>
        /// Scales the length of the given <paramref name="line"/> by the given <paramref name="scale"/> amount.
        /// </summary>
        /// <param name="line">The line to to scale.</param>
        /// <param name="scale">The amount to scale the line as a percentage. 1 is 100% normal size.</param>
        /// <returns></returns>
        public static Line Scale(Line line, float scale)
        {
            line.Start *= scale;
            line.Stop *= scale;


            return line;
        }


        /// <summary>
        /// Scales the given <paramref name="lines"/> by the given <paramref name="scale"/> amount.
        /// </summary>
        /// <param name="lines">The <see cref="Line"/>s to scale by the given <paramref name="scale"/>.</param>
        /// <param name="scale">The amount to scale the line as a percentage. 1 is 100% normal size.</param>
        /// <returns></returns>
        public static Line[] Scale(Line[] lines, float scale)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Scale(lines[i], scale);
            }


            return lines;
        }


        /// <summary>
        /// Returns a value indicating if any of the given <paramref name="vector"/> is contained by the given <paramref name="poly"/>.
        /// </summary>
        /// <param name="poly">The <see cref="Polygon"/> that possibly contains the given point described by <paramref name="x"/> and <paramref name="y"/>.</param>
        /// <param name="x">The x component of the point that is possibly contained by the given <see cref="Polygon"/>.</param>
        /// <param name="y">The y component of the point that is possibly contained by the given <see cref="Polygon"/>.</param>
        /// <returns></returns>
        public static bool Contains(Polygon poly, float x, float y)
        {
            return Contains(poly.Vertices, x, y);
        }


        /// <summary>
        /// Returns a value indicating if any of the given <paramref name="vector"/> is contained by the given <paramref name="poly"/>.
        /// </summary>
        /// <param name="vertices">The vertices of a polygon that possibly contain the given point described by <paramref name="x"/> and <paramref name="y"/>.</param>
        /// <param name="x">The x component of the point that is possibly contained by the given <see cref="Polygon"/>.</param>
        /// <param name="y">The y component of the point that is possibly contained by the given <see cref="Polygon"/>.</param>
        /// <returns></returns>
        public static bool Contains(Vector2[] vertices, float x, float y)
        {
            int windingNumberCounter = 0;

            //Loop through all edgs of the polygon
            for (int i = 0; i < vertices.Length; i++)
            {
                int stopIndex = i < vertices.Length - 1 ? i + 1 : 0;

                if (vertices[i].Y <= y)
                {
                    if (vertices[stopIndex].Y > y)
                    {
                        if (DetermineWhichSide(vertices[i], vertices[stopIndex], x, y) > 0)
                            windingNumberCounter += 1;
                    }
                }
                else
                {
                    if (vertices[stopIndex].Y <= y)
                        if (DetermineWhichSide(vertices[i], vertices[stopIndex], x, y) < 0)
                            windingNumberCounter -= 1;
                }
            }


            return windingNumberCounter > 0;
        }


        /// <summary>
        /// Returns a number determining which side of the a line the given <paramref name="vector"/> is on constructed by the <paramref name="start"/>. and <paramref name="stop"/>.
        /// </summary>
        /// <param name="start">The start <see cref="Vector"/> of the line.</param>
        /// <param name="stop">The end <see cref="Vector"/> of theline.</param>
        /// <param name="x">The x component of the point that resides on either side of the <see cref="Line"/>.</param>
        /// <param name="y">The y component of the point that resides on either side of the <see cref="Line"/>.</param>
        /// <returns></returns>
        public static float DetermineWhichSide(Vector2 start, Vector2 stop, float x, float y)
        {
            return (stop.X - start.X) * (y - start.Y) - (stop.Y - start.Y) * (x - start.X);
        }


        /// <summary>
        /// Converts the given local <paramref name="localVertices"/> to world vertices based on the given <paramref name="origin"/>.
        /// </summary>
        /// <param name="localVertices">The local vertices to translate to world vertices.</param>
        /// <param name="origin">The origin to base the translation from.</param>
        /// <returns></returns>
        public static Vector2[] ConvertToWorldVertices(Vector2[] localVertices, Vector2 origin)
        {
            var worldVertices = new Vector2[localVertices.Length];

            for (int i = 0; i < localVertices.Length; i++)
            {
                worldVertices[i] = origin + localVertices[i];
            }


            return worldVertices;
        }


        /// <summary>
        /// Rotates the <paramref name="vectorToRotate"/> around the <paramref name="rotateOrigin"/> at the given <paramref name="angle"/>.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="origin">The vector to rotate the <paramref name="vectorToRotate"/> around.</param>
        /// <param name="angle">The angle in degrees to rotate <paramref name="vectorToRotate"/>.  Value must be positive.</param>
        /// <returns></returns>
        public static Vector2 RotateAround(Vector2 vector, Vector2 origin, float angle, bool clockWise = true)
        {
            if (angle < 0)
                throw new ArgumentOutOfRangeException(nameof(angle), "The angle must be a positive number.");

            angle = clockWise ? angle : angle * -1;

            var radians = Util.ToRadians(angle);

            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            var dx = vector.X - origin.X;//The delta x
            var dy = vector.Y - origin.Y;//The delta y

            var tempX = dx * cos - dy * sin;
            var tempY = dx * sin + dy * cos;

            var x = tempX + origin.X;
            var y = tempY + origin.Y;


            return new Vector2(x, y);
        }


        public static float CalcInverseOfMass(float mass)
        {
            return 1.0f / mass;
        }


        /// <summary>
        /// Converts the given degrees to radians.
        /// </summary>
        /// <param name="degrees">The degrees to convert.</param>
        /// <returns></returns>
        public static float ToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
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


        /// <summary>
        /// Returns the cross product scalar of the 2 given vectors pointing in the z direction.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns></returns>
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }


        /// <summary>
        /// Returns a vector crossed with a scalar(z-axis) that will return a vector on the 2D cartesion plane.
        /// </summary>
        /// <param name="a">The vector</param>
        /// <param name="scalar">The scalar</param>
        /// <returns></returns>
        public static Vector2 Cross(Vector2 a, float scalar)
        {
            return new Vector2(scalar * a.Y, -scalar * a.X);
        }


        /// <summary>
        /// Returns a vector crossed with a scalar(z-axis) that will return a vector on the 2D cartesion plane.
        /// </summary>
        /// <param name="scalar">The scalar</param>
        /// <param name="a">The vector</param>
        /// <returns></returns>
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
