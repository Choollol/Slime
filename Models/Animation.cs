using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slime.Models
{
    public class Animation
    { 
        public int currentFrame { get; set; }
        public int frameCount { get; private set; }
        public int frameWidth { get { return texture.Width / frameCount; } }
        public int frameHeight { get { return texture.Height; } }
        public float frameSpeed { get; set; }
        public bool isLooping { get; set; }

        public Texture2D texture { get; private set; }

        public Animation(Texture2D newTexture, int newFrameCount)
        {
            texture = newTexture;
            frameCount = newFrameCount;
            isLooping = true;
        }


    }
}
