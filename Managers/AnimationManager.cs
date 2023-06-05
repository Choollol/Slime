using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slime.Models;

namespace Slime.Managers
{
    public class AnimationManager
    {
        private Animation animation;
        private float timer;
        private bool isDraw = true;
        private float scale = 1f;
        private float depth = 0.5f;
        private SpriteEffects spriteEffect = SpriteEffects.None;
        private float rotation = 0;
        private Color color = Color.White;
        private float opacity = 1f;
        private Vector2 origin = Vector2.Zero;
        public Vector2 position { get; set; }
        public float FrameSpeed
        {
            get { return animation.frameSpeed; }
            set { animation.frameSpeed = value; }
        }
        public int CurrentFrame
        {
            get { return animation.currentFrame; }
            set { animation.currentFrame = value; }
        }
        public int FrameCount
        {
            get { return animation.frameCount; }
        }
        public bool IsLooping
        {
            get { return animation.isLooping; }
            set { animation.isLooping = value; }
        }
        public int FrameWidth
        {
            get { return animation.frameWidth; }
        }
        public int FrameHeight
        {
            get { return animation.frameHeight; }
        }
        public bool IsDraw
        {
            get { return isDraw; }
            set { isDraw = value; }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }
        public SpriteEffects SpriteEffect
        {
            get { return spriteEffect; }
            set { spriteEffect = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public float Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public AnimationManager(Animation anim)
        {
            animation = anim;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isDraw)
            {
                spriteBatch.Draw(animation.texture, position,
                    new Rectangle(animation.currentFrame * animation.frameWidth, 0, animation.frameWidth, animation.frameHeight),
                    color * opacity, rotation, origin, 
                    scale, spriteEffect, depth);
            }
        }
        public void Play(Animation anim)
        {
            if (animation == anim)
            {
                return;
            }
            animation = anim;
            animation.currentFrame = 0;
            timer = 0;
        }
        public void Stop()
        {
            timer = 0;
            animation.currentFrame = 0;
        }
        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += dt;
            if (timer > animation.frameSpeed)
            {
                timer = 0f;
                animation.currentFrame++;
                if (animation.currentFrame >= animation.frameCount)
                {
                    if (animation.isLooping)
                    {
                        isDraw = true;
                        animation.currentFrame = 0;
                    }
                    else
                    {
                        isDraw = false;
                    }
                }
            }
        }
    }
}
