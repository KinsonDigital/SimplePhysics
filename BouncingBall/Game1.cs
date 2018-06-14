using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
        private float _densityOfWater = 0f;
        private float _densityOfAir = 30.0f;
        private int _screenWidth = 800;
        private int _screenHeight = 480;
        private bool _isInWater = false;
        private PhysObj _ballA;
        private PhysObj _ballB;
        private Stopwatch _timer = new Stopwatch();
        private bool _timerFinished = false;
        private Vector2 normal = Vector2.Zero;
        private float e; //Average restitution
        private float sf;//Static friction
        private float df;//Dynamic friction

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
            _gravity = new Vector2(0.0f, 0.2f);
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

            _ballA = new PhysObj(25)
            {
                Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 3),
                Velocity = new Vector2(0, 0),
                Acceleration = new Vector2(0, 0),
                /*A mass that is too low will make the object unstable and react to quick
                    This is due to the huge ratio between the small amount of mass being moved
                    by the velocity.
                */
                Mass = 10f,
                Friction = 0.05f,
                Drag = 0.01f,//Must be a positive number. If 0, then object does not move
                SurfaceArea = 1f,
                Restitution = -1,
                ObjectColor = Color.White
            };


            _ballB = new PhysObj(40)
            {
                Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, (_graphics.PreferredBackBufferHeight / 3) * 2),
                Velocity = new Vector2(0, 0),
                Acceleration = new Vector2(0, 0),
                /*A mass that is too low will make the object unstable and react to quick
                    This is due to the huge ratio between the small amount of mass being moved
                    by the velocity.
                */
                Mass = 0f,
                Friction = 0.05f,
                Drag = 0.01f,//Must be a positive number. If 0, then object does not move
                SurfaceArea = 1f,
                Restitution = -1,
                ObjectColor = Color.DarkSeaGreen
            };


            var resultInPounds = Util.ToPounds(_ballA.Mass, _gravity.Y);
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

            ProcessMovementKeys();

            ProcessImpulseKey((float)gameTime.ElapsedGameTime.TotalSeconds);

            ResolveCollisions();

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

            _ballB.Render(_spriteBatch);
            _ballA.Render(_spriteBatch);

            //Render the water
            //_spriteBatch.FillRectangle(_waterArea, new Color(0, 0, 255, 80));

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
                var newVelocity = Util.ApplyImpulse(_ballA, new Vector2(0, -40));

                _ballA.Velocity += newVelocity;
            }
        }


        private void UpdatePhysics()
        {
            if (!_timer.IsRunning && _timerFinished == false)
                _timer.Start();

            //Update time passed
            Window.Title = $"Touch Bottom Time: {_timer.Elapsed.TotalSeconds}";

            //Calculate the friction vector
            var friction = Vector2.Normalize(_ballA.Velocity);
            friction *= -1;

            //If the friction components are NaN, set to 0
            friction = friction.RemoveAnyNaN();

            friction *= _ballA.Friction;

            var density = _isInWater ? _densityOfWater : _densityOfAir;

            var velocityUnitVector = Vector2.Normalize(_ballA.Velocity);

            velocityUnitVector = velocityUnitVector.RemoveAnyNaN();

            float speed = _ballA.Velocity.Length();
            var dragForce = _ballA.Velocity;

            dragForce = -0.5f * density * (_ballA.Velocity.Length() * _ballA.Velocity.Length()) * _ballA.SurfaceArea * _ballA.Drag * velocityUnitVector;

            //Apply all of the forces
            ApplyForce(_ballA, dragForce);
            ApplyForce(_ballA, friction);
            ApplyForce(_ballA, _gravity);
            ApplyForce(_ballA, _wind);
            ApplyForce(_ballA, _movementForce);

            //Apply the acceleration to the velocity
            _ballA.Velocity += _ballA.Acceleration;

            //Apply a max to the components of the vector if any of the components are larger than the max
            _ballA.Velocity = Util.Max(_ballA.Velocity, 15);

            //Update the location based on the velocity
            _ballA.Position += _ballA.Velocity;

            //Reset the acceleration.  If you do not do this, every frame the acceleration
            //will increase.  This only is needed in the current moment in time
            _ballA.Acceleration *= 0;
        }


        private void ResolveCollisions()
        {
            var manifold = BuildManifold();

            for (int i = 0; i < manifold.ContactCount; i++)
            {
                // Calculate radii from COM to contact
                 Vector2 radiusA = manifold.ContactVectors[i] - _ballA.Position;
                 Vector2 radiusB = manifold.ContactVectors[i] - _ballB.Position;

                // Relative velocity
                Vector2 relativeVelocity = _ballB.Velocity + Util.Cross(_ballB.AngularVelocity, radiusB) - _ballA.Velocity - Util.Cross(_ballA.AngularVelocity, radiusA);

                // Relative velocity along the normal
                float contactVel = Dot(relativeVelocity, normal);// Normal is a unit vector

                // Do not resolve if velocities are separating
                if (contactVel > 0)
                {
                    return;
                }

                float crossProdRadiusAndNormA = Util.Cross(radiusA, normal);
                float crossProdRadiusAndNormB = Util.Cross(radiusB, normal);
                float invMassSum = _ballA.InvMass + _ballB.InvMass + (crossProdRadiusAndNormA * crossProdRadiusAndNormA) * _ballA.InvInertia + (crossProdRadiusAndNormB * crossProdRadiusAndNormB) * _ballB.InvInertia;

                // Calculate impulse scalar
                float j = -(1.0f + e) * contactVel;
                j /= invMassSum;
                j /= manifold.ContactCount;

                // Apply impulse
                 Vector2 impulse = normal * j;
                _ballA.ApplyImpulse(Neg(impulse), radiusA);//Negate impulse to force body A in opposite direction of body B
                _ballB.ApplyImpulse(impulse, radiusB);

                // Friction impulse
                relativeVelocity = _ballB.Velocity + Util.Cross(_ballB.AngularVelocity, radiusB) - _ballA.Velocity - Util.Cross(_ballA.AngularVelocity, radiusA);

                Vector2 tangent = new Vector2(relativeVelocity.X, relativeVelocity.Y);
                tangent = (tangent + normal) * -Dot(relativeVelocity, normal);
                tangent.Normalize();


                // j tangent magnitude
                float jt = -Dot(relativeVelocity, tangent);
                jt /= invMassSum;
                jt /= manifold.ContactCount;

                // Don't apply tiny friction impulses
                if ((jt - 0.0f) <= 0.0001f)
                {
                    return;
                }

                // Coulumb's law
                 Vector2 tangentImpulse;
                if (Math.Abs(jt) < j * sf)
                {
                    tangentImpulse = tangent * jt;
                }
                else
                {
                    tangentImpulse = (tangent * j) * -df;
                }

                // Apply friction impulse
                _ballA.ApplyImpulse(Neg(tangentImpulse), radiusA);
                _ballB.ApplyImpulse(tangentImpulse, radiusB);
            }
        }

        private Manifold BuildManifold()
        {
            var result = new Manifold();

            // Calculate translational vector, which is normal
            var normal = _ballB.Position - _ballA.Position;

            float distSqr = normal.X * normal.X + normal.Y * normal.Y;

            float radius = _ballA.Radius + _ballB.Radius;

            // Not in contact
            if (distSqr >= radius * radius)
            {
                result.ContactCount = 0;
                return result;
            }

            float distance = (float)Math.Sqrt(distSqr);

            result.ContactCount = 1;

            if (distance == 0.0f)
            {
                result.Penetration = _ballA.Radius;
                result.Normal = new Vector2(1, 0);
                result.ContactVectors.Add(_ballA.Position);
            }
            else
            {
                result.Penetration = radius - distance;
                result.Normal = normal / distance;
                result.ContactVectors.Add((normal / _ballA.Radius) + _ballA.Position);
            }


            return result;
        }


        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }


        public static Vector2 Neg(Vector2 v)
        {
            return new Vector2(v.X * -1, v.Y * -1);
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
