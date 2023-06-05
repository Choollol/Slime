using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slime.Sprites;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Slime.Controllers
{
    public class CurrencyController
    {
        private SpriteFont spriteFont;
        private Texture2D soulOrb;
        private float scale = 1.6f;

        public int souls = 0;
        public Vector2 soulOrbPos;
        public CurrencyController (SpriteFont font, Texture2D soulOrbTexture)
        {
            spriteFont = font;
            soulOrb = soulOrbTexture;
        }
        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            if (player.hasMetWitch)
            {
                spriteBatch.Draw(soulOrb, soulOrbPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.95f);
                spriteBatch.DrawString(spriteFont, souls.ToString(),
                    new Vector2(soulOrbPos.X + 64, soulOrbPos.Y + 8), Color.White, 0, Vector2.Zero, scale,
                    SpriteEffects.None, 0.95f);
            }
        }
        public void Update(GameTime gameTime, List<Enemy> enemies, Player player, Camera camera)
        {
            soulOrbPos = new Vector2(camera.Position.X - camera.Width / 2 + 20, camera.Position.Y - camera.Height / 2 + 20);
            foreach (Enemy enemy in enemies)
            {
                if (!player.hasMetWitch)
                {
                    enemy.preventSouls = true;
                }
                if (player.hasMetWitch && enemy.preventSouls)
                {
                    enemy.canGiveSouls = true;
                    enemy.preventSouls = false;
                    enemy.health = enemy.maxHealth;
                }
                if (enemy.health <= 0 && enemy.canGiveSouls && !enemy.preventSouls && player.hasMetWitch)
                {
                    int enemySoulCount = 0;

                    enemySoulCount = (int)(enemy.maxHealth / 10 + enemy.damage / 5 + enemy.speed / 20 + enemy.addSoulAmount) * 
                        (player.bossDefeatCount + 1);
                    if (enemy.doUseProjectile)
                    {
                        enemySoulCount = (int)(enemySoulCount * 1.4f);
                    }
                    souls += enemySoulCount;

                    enemy.canGiveSouls = false;
                }
            }

        }
    }
}
