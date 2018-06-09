using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingBall
{
    /// <summary>
    /// Represents a rectangle.
    /// </summary>
    public class Polygon
    {
        protected Vector2[] _localVertices;
        protected Vector2[] _worldVertices;
        protected Line[] _sides;
        protected Vector2 _position;
        private float _angle;
        private float _scale = 1f;
        private float _restitution = -1;
        private float _drag = 0.01f;
        private float _frictionCoefficient = 0.02f;

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="width">The width of the <see cref="Rectangle"/>.</param>
        /// <param name="height">The height of the <see cref="Rectangle"/>.</param>
        /// <param name="vertices">The position of the <see cref="Rectangle"/>.</param>
        public Polygon(Vector2[] vertices, Vector2 position)
        {
            _localVertices = vertices;
            _worldVertices = new Vector2[vertices.Length];
            _sides = new Line[vertices.Length];
            _position = position;

            UpdateVertices(Vector2.Zero);
        }
        #endregion

        #region Props
        public string Name { get; set; }

        /// <summary>
        /// The unique ID of the polygon.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The mass of the polygon.
        /// </summary>
        public float Mass { get; set; } = 1f;

        /// <summary>
        /// The friction coefficient of drag on the polygon.
        /// </summary>
        /// <remarks>Must be a positive value to properly simulate physics.  Any incoming negative values will be set to positive.</remarks>
        public float FrictionCoefficient
        {
            get => _frictionCoefficient;
            set => _frictionCoefficient = Math.Abs(value);
        }

        /// <summary>
        /// Coefficient of restitution ("bounciness"). Needs to be a negative number for flipping the direction of travel (velocity Y) to move the ball 
        /// in the opposition direction when it hits a surface.  This is what simulates the bouncing effect of an object hitting another object.
        /// </summary>
        /// <remarks>Must be a negative value to properly simulate physics.  Any incoming positive values will be set to negative.</remarks>
        public float Restitution
        {
            get
            {
                return _restitution;
            }
            set
            {
                _restitution = value > 0 ? value * -1 : value;
            }
        }

        /// <summary>
        /// Frontal area of the ball; divided by 10000 to compensate for the 1px = 1cm relation
        /// frontal area of the ball is the area of the ball as projected opposite of the direction of motion.
        /// In other words, this is the "silhouette" of the ball that is facing the "wind" (since this variable is used for air resistance calculation).
        ///   It is the total area of the ball that faces the wind. In short: this is the area that the air is pressing on.
        /// http://www.softschools.com/formulas/physics/air_resistance_formula/85/
        /// </summary>
        //TODO: Figure out how to calculate the surface area that is 
        //TODO: in contact with the air/liquid
        public float SurfaceArea { get; set; } = 1f;

        /// <summary>
        /// Coeffecient of drag for on a object.
        /// </summary>
        /// <remarks>Must be a positive value to properly simulate physics.  Any incoming negative values will be set to positive.</remarks>
        public float Drag
        {
            get => _drag;
            set => _drag = Math.Abs(value);
        }

        /// <summary>
        /// The position of the Rectangle in world coordinates.  This is also the center of the Rectangle.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                var positionDelta = value - _position;

                _position = value;

                UpdateVertices(positionDelta);
            }
        }

        /// <summary>
        /// Gets the X position of the <see cref="Rectangle"/>.
        /// </summary>
        public float X => Position.X;

        /// <summary>
        /// Gets the Y position of the <see cref="Rectangle"/>.
        /// </summary>
        public float Y => Position.Y;

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public bool IsColliding { get; set; }

        /// <summary>
        /// Gets or sets the angle of the rectangle in degrees.
        /// </summary>
        public virtual float Angle
        {
            get => _angle;
            set
            {
                if (IsColliding)
                    return;

                _angle = value > 360 ? 0 : value;
                _angle = value < 0 ? 360 : value;

                UpdateVertices(Vector2.Zero);
            }
        }

        /// <summary>
        /// Gets or sets the scale of the polygon.
        /// </summary>
        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;

                _scale = _scale < 0 ? 0 : _scale;
                UpdateVertices(Vector2.Zero);
            }
        }

        /// <summary>
        /// Gets all of the sides of the rectangle as an array.
        /// </summary>
        public Line[] Sides => _sides;

        /// <summary>
        /// Gets all of the vertices that make up the <see cref="Polygon"/>.
        /// </summary>
        public Vector2[] Vertices => _worldVertices;

        /// <summary>
        /// Gets or sets the local vertices.
        /// </summary>
        internal Vector2[] LocalVertices
        {
            get => _localVertices;
            set
            {
                _localVertices = value;
                UpdateVertices(Vector2.Zero);
            }
        }

        /// <summary>
        /// Calculates the centroid of the polygon.
        /// </summary>
        public Vector2 Centroid => Util.CalculateCentroid(_worldVertices);
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a value indicating if the given X and Y coordinates are contained by this <see cref="Polygon"/>.
        /// </summary>
        /// <param name="x">The X coordinate that might possibly be contained in <see cref="Polygon"/>.</param>
        /// /// <param name="y">The X coordinate that might possibly be contained in <see cref="Polygon"/>.</param>
        /// <returns></returns>
        public bool Contains(float x, float y)
        {
            return Util.Contains(this, x, y);
        }

        /// <summary>
        /// Returns a value indicating if the given vector is contained by this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="vector">The vector that might possibly be contained in <see cref="Rectangle"/>.</param>
        /// <returns></returns>
        public bool Contains(Vector2 vector)
        {
            return Contains(vector.X, vector.Y);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Updates the world vertices and there new X & Y values after rotation and convetion to world coordinates.
        /// </summary>
        /// <param name="positionDelta">The amount that the position has changed.</param>
        protected void UpdateVertices(Vector2 positionDelta)
        {
            //Update the position of all the vertices
            if (positionDelta.X != 0 && positionDelta.Y != 0)
            {
                for (int i = 0; i < _worldVertices.Length; i++)
                {
                    _worldVertices[i] += positionDelta;
                }
            }

            //Calculate the scale of the polygon
            var scaledLines = Util.Scale(_localVertices.ToLines(), _scale);

            var scaledVertices = scaledLines.ToVertices();

            //Calculate the world vertices
            var unrotatedWorldVertices = Util.ConvertToWorldVertices(scaledVertices, _position);

            for (int i = 0; i < _worldVertices.Length; i++)
            {
                _worldVertices[i] = Util.RotateAround(unrotatedWorldVertices[i], _position, _angle);
            }

            //Create the sides of the polygon
            for (int i = 0; i < _sides.Length; i++)
            {
                _sides[i].Start = _worldVertices[i];
                _sides[i].Stop = _worldVertices[i < _worldVertices.Length - 1 ? i + 1 : 0];
            }
        }
        #endregion
    }

}
