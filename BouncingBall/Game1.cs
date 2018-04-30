using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BouncingBall
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _gravity;//The gravity in the world
        private Vector2 _wind;//The wind forces in the world
        private Rectangle _waterArea;
        private float _densityOfWater = 10f;
        private float _densityOfAir = 0.0f;
        private int _screenWidth = 800;
        private int _screenHeight = 480;
        private bool _isInWater = false;
        private PhysObj _ball;

        /*Calculations:
         * Legend:
         *      ||V|| = Vector Normal or Vector Length
         *      ^V = Unit vector
         *      ROW = Density
         *      
         * Force = Mass * Acceleration
         * Acceleration = Force / Mass
         * Friction = -1 * M * ||Friction|| * ^Velocity
         * Drag = -0.5 * Density * Velocity^2 * SurfaceArea * GragCoeffienct * ^Velocity
         */

        public Game1()
        {
            var monitorWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var monitorHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = _screenWidth,
                PreferredBackBufferHeight = _screenHeight
            };

            _graphics.ApplyChanges();

            Window.Position = new Point(monitorWidth / 2 - _graphics.PreferredBackBufferWidth / 2, monitorHeight / 2 - _graphics.PreferredBackBufferHeight / 2);

            //Create the area of the screen that is water
            _waterArea = new Rectangle(0, _screenHeight - (_screenHeight / 3), _screenWidth, _screenHeight / 3);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _ball = new PhysObj()
            {
                Location = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 3),
                Velocity = new Vector2(0, 0),
                Acceleration = new Vector2(0, 0),
                Mass = 2.00f,
                Radius = 50f,
                Friction = 0.01f,
                Drag = 0.01f,//Must be a positive number. If 0, then object does not move
                SurfaceArea = 1f
            };

            _gravity = new Vector2(0.0f, 0.1f);
            _wind = new Vector2(-0.01f, 0);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Calculate the friction vector
            var friction = Vector2.Normalize(_ball.Velocity);
            friction *= -1;

            //If the friction components are NaN, set to 0
            friction = friction.RemoveAnyNaN();

            friction *= _ball.Friction;

            var density = _isInWater ? _densityOfWater : _densityOfAir;

            var velocityUnitVector = Vector2.Normalize(_ball.Velocity);

            velocityUnitVector = velocityUnitVector.RemoveAnyNaN();

            float speed = _ball.Velocity.Length();
            var dragForce = _ball.Velocity;

            dragForce = -0.5f * density * (_ball.Velocity.Length() * _ball.Velocity.Length()) * _ball.SurfaceArea * _ball.Drag * velocityUnitVector;

            //Apply all of the forces
            ApplyForce(_ball, dragForce);
            ApplyForce(_ball, friction);
            ApplyForce(_ball, _gravity);
            ApplyForce(_ball, _wind);

            //Apply the acceleration to the velocity
            _ball.Velocity += _ball.Acceleration;

            //Update the location based on the velocity
            _ball.Location += _ball.Velocity;

            //Reset the acceleration.  If you do not do this, every frame the acceleration
            //will increase.  This only is needed in the current moment in time
            _ball.Acceleration *= 0;

            CheckEdges();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _ball.Render(_spriteBatch);

            //Render the water
            _spriteBatch.FillRectangle(_waterArea, new Color(0, 0, 255, 80));

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CheckEdges()
        {
            if (_ball.Location.X > _screenWidth - _ball.Radius)
            {
                _ball.SetVelocityX(_ball.Velocity.X * _ball.Restitution);
                _ball.SetLocationX(_screenWidth - _ball.Radius);
            }
            else if(_ball.Location.X < _ball.Radius)
            {
                _ball.SetVelocityX(_ball.Velocity.X * _ball.Restitution);
                _ball.SetLocationX(_ball.Radius);
            }

            //If the object is in the water
            _isInWater = _waterArea.Contains(new Point((int)_ball.Location.X, (int)(_ball.Location.Y + _ball.Radius)));

            //Check if touching bottom of screen
            if (_ball.Location.Y > _screenHeight - _ball.Radius)
            {
                _ball.SetVelocityY(_ball.Velocity.Y * _ball.Restitution);
                _ball.SetLocationY(_screenHeight - _ball.Radius);
            }

            //Check if touching top of screen
            if (_ball.Location.Y < _ball.Radius)
            {
                _ball.SetVelocityY(_ball.Velocity.Y * _ball.Restitution);
                _ball.SetLocationY(_ball.Radius);
            }
        }

        private void ApplyForce(PhysObj obj, Vector2 force)
        {
            if (obj.Mass <= 0)
                obj.Acceleration = Vector2.Zero;
            else
                obj.Acceleration += force / obj.Mass;
        }
    }
}
