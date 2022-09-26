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
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace SimpleMovementJump
{
    public class Sprite : IGameObject
    {
        protected Vector2 origin;    //Orgin for Drawing
        public Vector2 position;
        public Vector2 velocity;
        protected Vector2 scale;
        public float rotate;     //degrees

        protected Rectangle drawRectangle;
        protected SpriteEffects spriteEffects;

        public string textureName = "pacManSingle";
        public Texture2D texture;

        public Sprite(Vector2 _position)
        {
            position = _position;
            velocity = new Vector2(0, 0);
            //drawRectangle = new Rectangle();
            
        }

        public virtual void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(textureName);
            origin = new Vector2(this.texture.Width / 2, this.texture.Height / 2);
            scale = new Vector2(1, 1);
            rotate = 0;
            spriteEffects = SpriteEffects.None;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            //spriteBatch.Draw(texture, position, Microsoft.Xna.Framework.Color.White);

            /*
            drawRectangle.X = (int)position.X;
            drawRectangle.Y = (int)position.Y;
            drawRectangle.Height = (int)(this.texture.Height);
            drawRectangle.Width = (int)(this.texture.Width);
            */

            spriteBatch.Draw(this.texture,  //texture2D
                position,
                null,   //no source rectangle
                Color.White,
                MathHelper.ToRadians(this.rotate), //rotation in radians
                this.origin,   //0,0 is top left
                this.scale,
                spriteEffects,
                0);

        }
    }
}
