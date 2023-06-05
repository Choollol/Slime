using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slime.Models;
using Slime.Managers;
using Transform;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slime.Sprites
{
    public class Sprite
    {
        protected AnimationManager animationManager;
        protected Dictionary<string, Animation> animations;
        protected Vector2 pos;
        protected Vector2 velocity;

        public Input input;
        public Vector2 position
        {
            get { return pos; }
            set
            {
                pos = value;
                if (animationManager != null)
                {
                    animationManager.position = pos;
                }
            }
        }
        public Texture2D texture;
        public string text;
        public int width;
        public int height;
        public bool isRemoved = false;
        public Vector2 center 
        { 
            get { return new Vector2(position.X + width / 2, position.Y + height / 2); }
            set { }
        }
        public SpriteEffects spriteEffect = SpriteEffects.None;
        public bool doSpriteEffect = true;
        public float scale = 1f;
        public float textScale = 1f;
        public float depth = 0.5f;
        public float opacity = 1f;
        public Color color = Color.White;
        public Color textColor = Color.Black;
        public float rotation = 0f;
        public bool useSourceRect = false;
        public Rectangle sourceRect;
        public string ID;
        public int numID;
        public bool doRotate;
        public float rotationSpeed;
        public Vector2 origin = Vector2.Zero;
        public Sprite() { }
        public Sprite(Dictionary<string, Animation> anims)
        {
            animations = anims;
            animationManager = new AnimationManager(animations.First().Value);
        }
        public Sprite(Texture2D newTexture)
        {
            texture = newTexture;
            width = texture.Width;
            height = texture.Height;
        }
        public Sprite(Texture2D newTexture, string newID) : this(newTexture)
        {
            ID = newID;
        }
        public Sprite(string newText)
        {
            text = newText;
        }
        public Sprite(string newText, string newID) : this(newText)
        {
            ID = newID;
        }
        public Sprite(string newText, string newID, int newNumID) : this(newText, newID)
        {
            numID = newNumID;
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)(position.X - origin.X * scale), (int)(position.Y - origin.Y * scale), width, height); }
        }
        protected bool collideLeft(Rectangle rect, Rectangle spriteRect)
        {
            if (rect.Intersects(spriteRect) &&
                rect.Left <= spriteRect.Left &&
                ((rect.Top <= spriteRect.Top && rect.Bottom >= spriteRect.Bottom) ||
                (rect.Top >= spriteRect.Top && rect.Bottom <= spriteRect.Bottom) ||
                (rect.Top <= spriteRect.Top && rect.Bottom <= spriteRect.Bottom) ||
                (rect.Top >= spriteRect.Top && rect.Bottom >= spriteRect.Bottom)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected bool collideRight(Rectangle rect, Rectangle spriteRect)
        {
            if (rect.Intersects(spriteRect) &&
                rect.Right >= spriteRect.Right &&
                ((rect.Top <= spriteRect.Top && rect.Bottom >= spriteRect.Bottom) ||
                (rect.Top >= spriteRect.Top && rect.Bottom <= spriteRect.Bottom) ||
                (rect.Top <= spriteRect.Top && rect.Bottom <= spriteRect.Bottom) ||
                (rect.Top >= spriteRect.Top && rect.Bottom >= spriteRect.Bottom)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected bool collideTop(Rectangle rect, Rectangle spriteRect)
        {
            if (rect.Intersects(spriteRect) &&
                rect.Top <= spriteRect.Top &&
                ((rect.Left <= spriteRect.Left && rect.Right >= spriteRect.Right) ||
                (rect.Left >= spriteRect.Left && rect.Right <= spriteRect.Right) ||
                (rect.Left <= spriteRect.Left && rect.Right <= spriteRect.Right) ||
                (rect.Left >= spriteRect.Left && rect.Right >= spriteRect.Right)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected bool collideBottom(Rectangle rect, Rectangle spriteRect)
        {
            if (rect.Intersects(spriteRect) &&
                rect.Bottom >= spriteRect.Bottom &&
                ((rect.Left <= spriteRect.Left && rect.Right >= spriteRect.Right) ||
                (rect.Left >= spriteRect.Left && rect.Right <= spriteRect.Right) ||
                (rect.Left <= spriteRect.Left && rect.Right <= spriteRect.Right) ||
                (rect.Left >= spriteRect.Left && rect.Right >= spriteRect.Right)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, position, sourceRect, color * opacity, rotation, Vector2.Zero, scale, spriteEffect, depth);
                if (useSourceRect)
                {
                    spriteBatch.Draw(texture, position, sourceRect, color * opacity, rotation, Vector2.Zero, scale, spriteEffect, depth);
                }
                else
                {
                    spriteBatch.Draw(texture, position, null, color * opacity, rotation, Vector2.Zero, scale, spriteEffect, depth);
                }
            }
            else if (animationManager != null)
            {
                animationManager.Draw(spriteBatch);
            }
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            Draw(spriteBatch);
            if (text != null)
            {
                if (texture != null)
                {
                    spriteBatch.DrawString(spriteFont, text, new Vector2(position.X + width / 2 - spriteFont.MeasureString(text).X / 2
                         * textScale, position.Y + height / 2 - spriteFont.MeasureString(text).Y / 2 * textScale), textColor,
                         0f, Vector2.Zero, textScale, spriteEffect, depth + 0.01f);
                }
                else
                {
                    spriteBatch.DrawString(spriteFont, text, position, textColor, 0f, origin, textScale, spriteEffect, depth);
                }
            }
        }
        public virtual void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();

            if (texture != null)
            {
                width = (int)(texture.Width * scale);
                height = (int)(texture.Height * scale);
            }
            if (animationManager != null)
            {
                width = (int)(animationManager.FrameWidth * scale);
                height = (int)(animationManager.FrameHeight * scale);
            }

            if (animationManager != null)
            {
                animationManager.Scale = scale;
                animationManager.Depth = depth;
                animationManager.Opacity = opacity;
                animationManager.Update(gameTime);
            }
        }
        public void Update(GameTime gameTime, SpriteFont spriteFont)
        {
            Update(gameTime);
            if (texture == null && text != null)
            {
                width = (int)(spriteFont.MeasureString(text).X / 2 * textScale);
                height = (int)(spriteFont.MeasureString(text).Y / 2 * textScale);
            }
        }
        
    }
}
