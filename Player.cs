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
    internal class Player : Sprite
    {
        
        private KeyboardHandler inputKeyboard;  //Instance of class that handles keyboard input

        float horizontalSpeedMax = 8;

        float GravityAccel = 0.75f;             //Acceloration from gravity
        float Friction = 0.75f;                 //Friction to slow down when no input is set
        float Accel = 1.25f;               //Acceloration
        int jumpHeight = -15;          //Jump impulse

        float DashSpeed = 16;

        bool isOnGround = false;                //Hack to stop falling at a certian point

        float GroundHeight = 600; // technical debt to hard-code the floor in

        public Keys keyLeft = Keys.Left;
        public Keys keyRight = Keys.Right;
        public Keys keyJump = Keys.Up;

        public enum PlayerState
        {
            Netral,
            Dashing
        }

        public PlayerState currentState;

        private int lastMoveDir = 1;
        private bool lastWasNeutral = false;
        private float lastMoveTimer = 0;
        private float doubleTapTimerMax = 20;

        public Player(Vector2 _position) : base(_position)
        {
            textureName = "pacManSingle";

            currentState = PlayerState.Netral;

            inputKeyboard = KeyboardHandler.GetKeyboardHandler();
        }
        public Player(float x, float y) : this(new Vector2(x, y))
        {

        }



        public override void Update(GameTime gameTime)
        {
            //Elapsed time since last update
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float timeFactor = time / 1000 * 60; //convert miliseconds to seconds, and seconds to frames at 60 fps. since thats what i am used to.



            //handle move input
            int move = GetMoveDir();

            bool doubleTapped = WasDoubleTapped(move, timeFactor);

            bool firstFrameStateChange = false;

            if (currentState == PlayerState.Netral && doubleTapped)
            {
                currentState = PlayerState.Dashing;
                firstFrameStateChange = true;
            }

            //handle jump input
            bool jump = inputKeyboard.WasKeyPressed(keyJump);

            switch (currentState)
            {
                case PlayerState.Netral:
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
                    break;
                case PlayerState.Dashing:
                    // handle x velocity 
                    if (firstFrameStateChange)
                    {

                        velocity.X = DashSpeed * move;

                    }
                    else
                    {
                        
                        if (Math.Abs(velocity.X) > horizontalSpeedMax)
                        {
                            velocity.X += Friction * -Math.Sign(velocity.X) * timeFactor;
                        }
                        else
                        {
                            currentState = PlayerState.Netral;
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
                        velocity.Y = Math.Min(0, velocity.Y);
                    }
                    break;
            }

            //apply velocity
            position += velocity * timeFactor;

            // do screen collisions
            UpdateScreenCollisions();


            //OutputData = string.Format("PacDir:{0}\nPacLoc:{1}\nGravityDir:{2}\nGravityAccel:{3}\nTime:{4}\njumpHeight:{5}", PacManDir.ToString(),
            //  PacManLoc.ToString(), GravityDir.ToString(), GravityAccel.ToString(), time, jumpHeight);
        
        }

        private int GetMoveDir()
        {
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

            return move;
        }

        private bool WasDoubleTapped(int move, float timeFactor)
        {
            if (lastMoveTimer > 0) lastMoveTimer -= 1*timeFactor;
            


            if (lastMoveDir == move)
            {
                

                if (lastMoveTimer > 0 && lastWasNeutral)
                {
                    // this is a double tap!
                    lastWasNeutral = false;
                    lastMoveTimer = 0;
                    return true;
                }

                // too slow to be a double tap
                lastWasNeutral = false;
                lastMoveDir = move;
                lastMoveTimer = doubleTapTimerMax;
            }
            else
            {
                if (move == 0)
                {
                    // returned to neutral
                    lastWasNeutral = true;
                }
                else
                {
                    // swapped diretions.
                    lastMoveDir = move;
                    lastMoveTimer = doubleTapTimerMax;
                    lastWasNeutral = false;
                }
            }

            return false;
        }

        private void UpdateScreenCollisions()
        {
            //Keep PacMan On Screen
            int roomWidth = 1024; // TODO had to hard - code screen width, fix later
            if (position.X > roomWidth - origin.X)
            {
                //Negate X
                position.X = roomWidth - origin.X;
            }
            if (position.X < 0 + origin.X)
            {
                position.X = 0 + origin.X;
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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //flip horizontal
            if (velocity.X != 0)
            {
                if (Math.Sign(velocity.X) == 1)
                {
                    spriteEffects = SpriteEffects.None;
                }
                else
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }

            //squash/stretch
            
            scale.X = 1 + (0.4f * Math.Clamp(Math.Abs(velocity.Y) / jumpHeight, -1.5f, 1f));
            if (currentState == PlayerState.Dashing)
            {
                scale.Y = 1 - (0.4f * Math.Clamp(Math.Abs(velocity.X) / DashSpeed, 0.25f, 1f));
            }
            else
            {
                scale.Y = 1;
            }

            base.Draw(gameTime, spriteBatch);
        }

    }
}
