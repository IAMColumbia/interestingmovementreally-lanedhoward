using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SimpleMovementJump
{
    /// <summary>
    /// Simple Movement For Jumping
    /// Uses a simple class called KeyboardHandler for input
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        string OutputData;

        List<IGameObject> gameObjects;

        KeyboardHandler keyboardHandler;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;

            gameObjects = new List<IGameObject>();

            keyboardHandler = KeyboardHandler.GetKeyboardHandler();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameObjects.Add(new Player(new Vector2(100, 100)));

            Player p2 = new Player(new Vector2(200, 100));
            p2.keyLeft = Keys.A;
            p2.keyRight = Keys.D;
            p2.keyJump = Keys.W;
            gameObjects.Add(p2);


            foreach (IGameObject o in gameObjects)
            {
                o.LoadContent(Content);
            }

            font = Content.Load<SpriteFont>("Arial");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardHandler.Update();

            foreach (IGameObject o in gameObjects)
            {
                o.Update(gameTime);
            }
            
            base.Update(gameTime);
        }
        

        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (IGameObject o in gameObjects)
            {
                o.Draw(gameTime, spriteBatch);
            }
            /*
             * Draw parameters on screen
             */

            //spriteBatch.DrawString(font, OutputData , new Vector2(10, 10), Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
