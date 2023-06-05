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
using Microsoft.Xna.Framework.Audio;
using Slime.Managers;

namespace Slime.Controllers
{
    public class AbilityIconController
    {
        private Dictionary<string, Texture2D> textures;
        private float scale = 1.2f;
        private float depth = 0.8f;

        public List<AbilityIcon> icons = new List<AbilityIcon>();

        public AbilityIconController(Dictionary<string, Texture2D> abilityIcons)
        {
            textures = abilityIcons;
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            foreach (AbilityIcon icon in icons)
            {
                icon.Draw(spriteBatch, spriteFont);
            }
        }
        public void Update(GameTime gameTime, Camera camera, AbilityController abilityController,
            float attackTimer)
        {
            if (icons.Count == 0)
            {
                icons.Add(new AbilityIcon(textures["attack"], "attack"));
            }
            else if (icons.Count == 1 && abilityController.isTeleportUnlocked)
            {
                icons.Add(new AbilityIcon(textures["teleport"], "teleport"));
            }
            else if (icons.Count == 2 && abilityController.isTimeStopUnlocked)
            {
                icons.Add(new AbilityIcon(textures["timeStop"], "timeStop"));
            }
            for (int i = icons.Count - 1; i >= 0; i--)
            {
                icons[i].depth = depth;
                icons[i].scale = scale;
                icons[i].width = (int)(icons[i].texture.Width * scale);
                icons[i].height = (int)(icons[i].texture.Height * scale);
                if (i == icons.Count - 1)
                {
                    icons[i].position = new Vector2(camera.Position.X + camera.Width / 2 - icons[i].width - 20,
                        camera.Position.Y - camera.Height / 2 + 20);
                }
                else
                {
                    icons[i].position = new Vector2(icons[i + 1].position.X - icons[i].width - 10, icons[i + 1].position.Y);
                }
                if (icons[i].ID == "attack")
                {
                    icons[i].timer = attackTimer;
                }
                else if (icons[i].ID == "teleport")
                {
                    icons[i].timer = abilityController.teleportTimer;
                }
                else if (icons[i].ID == "timeStop")
                {
                    icons[i].timer = abilityController.timeStopTimer;
                }
                icons[i].Update(gameTime);
            }
        }
    }
}
