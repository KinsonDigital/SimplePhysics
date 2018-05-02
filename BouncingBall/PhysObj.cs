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
        private Texture2D _texture;


        #region Constructors
        public PhysObj(Texture2D texture)
        {
            _texture = texture;
        }
        #endregion


        #region Props
        public float Mass { get; set; } = 1f;

        public float HalfHeight { get; set; }

        /// <summary>
        /// The friction of gas/fluid on the object.  Keep positive to properly simulate.
        /// </summary>
        public float Friction { get; set; } = 0.02f;

        public Vector2 Location { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        public float Angle { get; set; }

        public float SurfaceArea { get; set; } = 1f;

        /// <summary>
        /// Get or sets the drag coefficient
        /// </summary>
        public float Drag { get; set; } = 0.01f;

        public float Restitution { get; set; } = -1f;
        #endregion


        #region Public Methods
        public void Render(SpriteBatch spriteBatch)
        {
            var width = _texture.Width;
            var height = _texture.Height;
            var halfWidth = width / 2;
            var halfHeight = height / 2;

            var location = new Vector2(Location.X - halfWidth, Location.Y - halfHeight);
            var origin = new Vector2(halfWidth, halfHeight);
            var srcRect = new Rectangle(0, 0, width, height);

            spriteBatch.Draw(_texture, Location, srcRect, Color.White, Angle, origin, 1f, SpriteEffects.None, 0f);

            //Draw the origin of the texture
            spriteBatch.FillCircle(Location, 5, 10, Color.Black);
            //spriteBatch.FillCircle(Location, Radius, 50, Color.DarkRed);
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
            Location = new Vector2(value, Location.Y);
        }


        public void SetLocationY(float value)
        {
            Location = new Vector2(Location.X, value);
        }
        #endregion
    }
}