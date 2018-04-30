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
        public float Mass { get; set; } = 1f;

        public float Radius { get; set; }

        /// <summary>
        /// The friction of gas/fluid on the object.  Keep positive to properly simulate.
        /// </summary>
        public float Friction { get; set; } = 0.02f;

        public Vector2 Location { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public float SurfaceArea { get; set; } = 1f;

        /// <summary>
        /// Get or sets the drag coefficient
        /// </summary>
        public float Drag { get; set; } = 0.01f;

        public float Restitution { get; set; } = -1f;

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
            Location = new Vector2(value, Location.Y);
        }

        public void SetLocationY(float value)
        {
            Location = new Vector2(Location.X, value);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.FillCircle(Location, Radius, 50, Color.DarkRed);
        }
    }
}