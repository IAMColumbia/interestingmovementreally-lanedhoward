using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SimpleMovementJump
{
    class KeyboardHandler
    {

        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        private KeyboardHandler()
        {
            prevKeyboardState = Keyboard.GetState();
        }

        private static KeyboardHandler singleton;
        public static KeyboardHandler GetKeyboardHandler()
        {
            if (singleton == null)
            {
                singleton = new KeyboardHandler();
            }
            return singleton;
        }


        public bool IsKeyDown(Keys key)
        {
            return (keyboardState.IsKeyDown(key));
        }

        public bool IsHoldingKey(Keys key)
        {
            return(keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyDown(key));
        }

        public bool WasKeyPressed(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyUp(key));
        }

        public bool WasKeyReleased(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && prevKeyboardState.IsKeyDown(key));
        }

        public void Update()
        {
            //set our previous state to our new state
            prevKeyboardState = keyboardState;

            //get our new state
            keyboardState = Keyboard.GetState();
        }

        public bool WasAnyKeyPressed()
        {
            Keys[] keysPressed = keyboardState.GetPressedKeys();

            if (keysPressed.Length > 0)
            {
                foreach (Keys k in keysPressed)
                {
                    if (prevKeyboardState.IsKeyUp(k))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool WasAnyKeyReleased()
        {
            Keys[] prevKeysPressed = prevKeyboardState.GetPressedKeys();
            Keys[] nowKeysPressed = keyboardState.GetPressedKeys();

            if (prevKeysPressed.Length > 0) //if there was a key pressed before
            {
                foreach (Keys k in prevKeysPressed) // go through each key that was pressed
                {
                    if (keyboardState.IsKeyUp(k)) // check if it is up now
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
