using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Comora;
using System.Collections.Generic;
using Slime.Sprites;
using Slime.Models;
using System.Diagnostics;
using System;
using Slime.Controllers;
using Microsoft.Xna.Framework.Audio;
using Slime.Managers;

namespace Slime.Sprites
{
    public class AbilityIcon : Sprite
    {
        private float textScale;
        private Color textColor = Color.White;
        private string shownTimer;

        public float timer;
        public bool doShowTimer = false;
        public string ID;

        public AbilityIcon() { }
        public AbilityIcon(Texture2D abilityIcon, string abilityID)
        {
            texture = abilityIcon;
            ID = abilityID;
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(texture, position, null, color * opacity, 0f, Vector2.Zero, scale, spriteEffect, depth);
            if (timer > 0)
            {
                spriteBatch.DrawString(spriteFont, shownTimer, 
                    new Vector2(position.X + width / 2 - spriteFont.MeasureString(shownTimer).X * textScale / 2, position.Y + 27), 
                    textColor * opacity, 0f, Vector2.Zero, textScale, spriteEffect, depth + 0.1f);
            }
        }
        public override void Update(GameTime gameTime)
        {
            width = (int)(texture.Width * scale);
            height = (int)(texture.Height * scale);
            if (timer > 0)
            {
                color = Color.Gray;
            }
            else
            {
                color = Color.White;
            }
            if (timer >= 10)
            {
                shownTimer = ((int)timer).ToString();
                textScale = 1.2f;
            }
            else
            {
                shownTimer = ((float)Decimal.Round((Decimal)timer, 1)).ToString();
                textScale = 1f;
            }
        }
    }
}
