using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace BouncingBall
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardState _currentKeyState;
        private KeyboardState _prevKeyState;
        private Vector2 _gravity;//The gravity in the world
        private Vector2 _wind;//The wind forces in the world
        private Vector2 _movementForce = Vector2.Zero;//The force of movement to control and object
        private Rectangle _waterArea;
        private float _densityOfWater = 100f;
        private float _densityOfAir = 0.0f;
        private int _screenWidth = 800;
        private int _screenHeight = 480;
        private bool _isInWater = false;
        private PhysObj _box;
        private Stopwatch _timer = new Stopwatch();
        private bool _timerFinished = false;

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
            IsMouseVisible = true;
            _gravity = new Vector2(0.0f, 4.0f);
            _wind = new Vector2(0.0f, 0);

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

            _box = new PhysObj(Content.Load<Texture2D>(@"Graphics\WhiteBox"))
            {
                Location = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 3),
                Velocity = new Vector2(0, 0),
                Acceleration = new Vector2(0, 0),
                /*A mass that is too low will make the object unstable and react to quick
                    This is due to the huge ratio between the small amount of mass being moved
                    by the velocity.
                */
                Mass = 10f,
                HalfHeight = 25f,
                Friction = 0.05f,
                Drag = 0.01f,//Must be a positive number. If 0, then object does not move
                SurfaceArea = 1f,
                Restitution = 0
            };

            var resultInPounds = Util.ToPounds(_box.Mass, _gravity.Y);
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
            char letter = 'R';
            
            char result = letter.ToString().ToLower().ToCharArray()[0];
            _currentKeyState = Keyboard.GetState();

            UpdatePhysics();

            //ProcessMovementKeys();

            ProcessImpulseKey((float)gameTime.ElapsedGameTime.TotalSeconds);

            CheckEdges();

            _prevKeyState = _currentKeyState;
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

            _box.Render(_spriteBatch);

            //Render the water
            _spriteBatch.FillRectangle(_waterArea, new Color(0, 0, 255, 80));

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        private void ProcessMovementKeys()
        {
            if (_currentKeyState.IsKeyDown(Keys.Left))
            {
                _movementForce.X = MathHelper.Clamp(_movementForce.X - 5f, -1f, 0f);
                return;
            }
            else
            {
                _movementForce.X = 0;
            }

            if (_currentKeyState.IsKeyDown(Keys.Right))
            {
                _movementForce.X = MathHelper.Clamp(_movementForce.X + 5f, 0f, 1f);
                return;
            }
            else
            {
                _movementForce.X = 0;
            }

            if (_currentKeyState.IsKeyDown(Keys.Up))
            {
                _movementForce.Y = MathHelper.Clamp(_movementForce.Y - 5f, -4.5f, 0f);
                return;
            }
            else
            {
                _movementForce.Y = 0;
            }

            if (_currentKeyState.IsKeyDown(Keys.Down))
            {
                _movementForce.Y = MathHelper.Clamp(_movementForce.Y + 5f, 0f, 1f);
                return;
            }
            else
            {
                _movementForce.Y = 0;
            }
        }


        private void ProcessImpulseKey(float time)
        {
            //Apply an impulse below the object
            if (_currentKeyState.IsKeyDown(Keys.Space) && _prevKeyState.IsKeyUp(Keys.Space))
            {
                var newVelocity = Util.ApplyImpulse(new Vector2(0, 4), _box.Mass);

                _box.Velocity += newVelocity;
            }
        }


        private void UpdatePhysics()
        {
            if (!_timer.IsRunning && _timerFinished == false)
                _timer.Start();

            //Update time passed
            Window.Title = $"Touch Bottom Time: {_timer.Elapsed.TotalSeconds}";

            //Calculate the friction vector
            var friction = Vector2.Normalize(_box.Velocity);
            friction *= -1;

            //If the friction components are NaN, set to 0
            friction = friction.RemoveAnyNaN();

            friction *= _box.Friction;

            var density = _isInWater ? _densityOfWater : _densityOfAir;

            var velocityUnitVector = Vector2.Normalize(_box.Velocity);

            velocityUnitVector = velocityUnitVector.RemoveAnyNaN();

            float speed = _box.Velocity.Length();
            var dragForce = _box.Velocity;

            dragForce = -0.5f * density * (_box.Velocity.Length() * _box.Velocity.Length()) * _box.SurfaceArea * _box.Drag * velocityUnitVector;

            //Apply all of the forces
            ApplyForce(_box, dragForce);
            ApplyForce(_box, friction);
            ApplyForce(_box, _gravity);
            ApplyForce(_box, _wind);
            ApplyForce(_box, _movementForce);

            //Apply the acceleration to the velocity
            _box.Velocity += _box.Acceleration;

            //Apply a max to the components of the vector if any of the components are larger than the max
            _box.Velocity = Util.Max(_box.Velocity, 15);

            //Update the location based on the velocity
            _box.Location += _box.Velocity;

            //Reset the acceleration.  If you do not do this, every frame the acceleration
            //will increase.  This only is needed in the current moment in time
            _box.Acceleration *= 0;
        }


        private void CheckEdges()
        {
            if (_box.Location.X > _screenWidth - _box.HalfHeight)
            {
                _box.SetVelocityX(_box.Velocity.X * _box.Restitution);
                _box.SetLocationX(_screenWidth - _box.HalfHeight);
            }
            else if(_box.Location.X < _box.HalfHeight)
            {
                _box.SetVelocityX(_box.Velocity.X * _box.Restitution);
                _box.SetLocationX(_box.HalfHeight);
            }

            //If the object is in the water
            _isInWater = _waterArea.Contains(new Point((int)_box.Location.X, (int)(_box.Location.Y + _box.HalfHeight)));

            //Check if touching bottom of screen
            if (_box.Location.Y > _screenHeight - _box.HalfHeight)
            {
                _box.SetVelocityY(_box.Velocity.Y * _box.Restitution);
                _box.SetLocationY(_screenHeight - _box.HalfHeight);

                if (_timer.IsRunning && _timerFinished == false)
                {
                    _timer.Stop();
                    _timerFinished = true;
                }
            }

            //Check if touching top of screen
            if (_box.Location.Y < _box.HalfHeight)
            {
                _box.SetVelocityY(_box.Velocity.Y * _box.Restitution);
                _box.SetLocationY(_box.HalfHeight);
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
