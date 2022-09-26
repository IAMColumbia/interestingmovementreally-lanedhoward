using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation.DirectX;
using SharpDX.X3DAudio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace SimpleMovementJump
{
    internal class Player : IGameObject
    {
        public Vector2 position;
        public Vector2 velocity;

        public string textureName = "pacManSingle";
        public Texture2D texture;

        private KeyboardHandler inputKeyboard;  //Instance of class that handles keyboard input

        float horizontalSpeedMax = 8;

        float GravityAccel = 0.75f;             //Acceloration from gravity
        float Friction = 0.75f;                 //Friction to slow down when no input is set
        float Accel = 1.25f;               //Acceloration
        int jumpHeight = -15;          //Jump impulse

        bool isOnGround = false;                //Hack to stop falling at a certian point

        float GroundHeight = 600; // technical debt to hard-code the floor in

        public Keys keyLeft = Keys.Left;
        public Keys keyRight = Keys.Right;
        public Keys keyJump = Keys.Up;

        public Player(Vector2 _position)
        {
            position = _position;
            velocity = new Vector2(0, 0);

            inputKeyboard = KeyboardHandler.GetKeyboardHandler();
        }
        public Player(float x, float y) : this(new Vector2(x, y))
        {

        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(textureName);
        }

        public void Update(GameTime gameTime)
        {
            //Elapsed time since last update
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float timeFactor = time / 1000 * 60; //convert miliseconds to seconds, and seconds to frames at 60 fps. since thats what i am used to.

            

            //handle move input
            int move = 0;
            bool left = inputKeyboard.IsKeyDown(keyLeft);
            bool right = inputKeyboard.IsKeyDown(keyRight);

            if ((left && right) || (!left && !right))
            {
                move = 0;
            }
            else
            {
                if (left)
                {
                    move = -1;
                }
                else
                {
                    move = 1;
                }
            }

            //handle jump input
            bool jump = inputKeyboard.WasKeyPressed(keyJump);

            // handle x velocity 
            if (move != 0)
            {

                velocity.X += Accel * move * timeFactor;

                velocity.X = Math.Clamp(velocity.X, -horizontalSpeedMax, horizontalSpeedMax);
            }
            else
            {
                //not currently moving, apply friction
                if (Math.Abs(velocity.X) >= Friction * timeFactor)
                {
                    velocity.X += Friction * -Math.Sign(velocity.X) * timeFactor;
                }
                else
                {
                    velocity.X = 0;
                }
            }

            // handle y velocity

            if (isOnGround)
            {
                if (jump)
                {
                    velocity.Y = jumpHeight;
                }
            }
            else
            {
                velocity.Y += GravityAccel * timeFactor;
            }


            //apply velocity
            position += velocity * timeFactor;

            // do screen collisions
            UpdateScreenCollisions();


            //OutputData = string.Format("PacDir:{0}\nPacLoc:{1}\nGravityDir:{2}\nGravityAccel:{3}\nTime:{4}\njumpHeight:{5}", PacManDir.ToString(),
            //  PacManLoc.ToString(), GravityDir.ToString(), GravityAccel.ToString(), time, jumpHeight);
        
        }

        

        private void UpdateScreenCollisions()
        {
            //Keep PacMan On Screen
            int roomWidth = 1024; // TODO had to hard - code screen width, fix later
            if (position.X > roomWidth - texture.Width)
            {
                //Negate X
                position.X = roomWidth - texture.Width;
            }
            if (position.X < 0)
            {
                position.X = 0;
            }

            //Y stop at 400
            //Hack Floor location is hard coded
            //TODO viloates single resposibilty principle should be moved to it's own method
            if (position.Y >= GroundHeight) //HACK
            {
                position.Y = GroundHeight;
                velocity.Y = 0;
                isOnGround = true;
            }
            else
            {
                isOnGround = false;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, position, Microsoft.Xna.Framework.Color.White);

        }

    }
}
