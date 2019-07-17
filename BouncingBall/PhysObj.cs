using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingBall
{
    public class PhysObj
    {
        #region Constructors
        public PhysObj(float radius)
        {
            Radius = radius;
        }
        #endregion


        #region Props
        public float Radius { get; set; }

        public float Mass { get; set; } = 1f;

        public float InvMass => Mass != 0.0f ? 1.0f / Mass : 0.0f;

        public float Inertia { get; set; }

        public float InvInertia => Inertia != 0.0f ? 1.0f / Inertia : 0.0f;

        public float StaticFriction { get; set; } = 0.5f;

        public float DynamicFriction { get; set; } = 0.3f;

        /// <summary>
        /// The friction of gas/fluid on the object.  Keep positive to properly simulate.
        /// </summary>
        public float Friction { get; set; } = 0.02f;

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public float AngularVelocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public float Angle { get; set; }

        public float SurfaceArea { get; set; } = 1f;

        /// <summary>
        /// Get or sets the drag coefficient
        /// </summary>
        public float Drag { get; set; } = 0.01f;

        public float Restitution { get; set; } = -1f;

        public Color ObjectColor { get; set; } = Color.White;
        #endregion


        #region Public Methods
        public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {
            Velocity = Velocity + (impulse * InvMass);
            AngularVelocity += InvInertia * Util.Cross(contactVector, impulse);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.FillCircle(Position, Radius, 100, ObjectColor);

            //Draw the origin of the texture
            spriteBatch.FillCircle(Position, 5, 10, Color.Black);
        }

        public void SetVelocityX(float value)
        {
            Velocity = new Vector2(value, Velocity.Y);
        }


        public void SetVelocityY(float value)
        {
            Velocity = new Vector2(Velocity.X, value);
        }


        public void SetLocationX(float value)
        {
            Position = new Vector2(value, Position.Y);
        }


        public void SetLocationY(float value)
        {
            Position = new Vector2(Position.X, value);
        }
        #endregion
    }
}