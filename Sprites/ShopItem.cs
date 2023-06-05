using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slime.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Slime.Sprites
{
    public class ShopItem : Sprite
    {
        private Texture2D icon;
        private SpriteFont spriteFont;
        private string text;

        public bool isUnlocked = false;
        public int cost;
        public Texture2D background;
        public Vector2 iconPos;
        public Vector2 textPos;
        public int upgradeCount = 1;
        public Vector2 currentTextPos;
        public Vector2 nextTextPos;
        public string typeID;
        public int numID;
        public bool doMultiply = true;
        public int unlockCost;
        public List<int> upgradeCostAmounts;
        public int maxUpgradeAmount;
        public bool isMaxed = false;
        public string currentUpgradeAmount;
        public string nextUpgradeAmount;
        public bool isAbility = false;

        [JsonConstructor]
        public ShopItem() { }
        public ShopItem(SpriteFont font, bool unlocked, Texture2D itemIcon, string upgrade, List<int> upgradeCosts, int maxUpgrade)
        {
            isUnlocked = unlocked;
            icon = itemIcon;
            spriteFont = font;
            text = upgrade;
            if (upgrade != null)
            {
                typeID = upgrade.ToLower();
            }
            upgradeCostAmounts = upgradeCosts;
            maxUpgradeAmount = maxUpgrade;
            color = Color.White;
        }
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)position.X, (int)position.Y, width, height); }
        }
        public void Draw(SpriteBatch spriteBatch, float smallTextScale)
        {
            spriteBatch.Draw(background, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.91f);
            if (isUnlocked)
            {
                spriteBatch.Draw(background, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.91f);
                spriteBatch.Draw(icon, iconPos, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.92f);
                spriteBatch.DrawString(spriteFont, text, textPos, Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.92f);
                if (!isAbility)
                {
                    spriteBatch.DrawString(spriteFont, "CURRENT", currentTextPos, Color.Black, 0f,
                        Vector2.Zero, smallTextScale, SpriteEffects.None, 0.92f);
                    spriteBatch.DrawString(spriteFont, "NEXT", nextTextPos, Color.Black, 0f,
                        Vector2.Zero, smallTextScale, SpriteEffects.None, 0.92f);
                }
            }
        }
        public void Update(GameTime gameTime)
        {
            currentTextPos = new Vector2(iconPos.X + 150, iconPos.Y + 32);
            nextTextPos = new Vector2(currentTextPos.X + 180, currentTextPos.Y);

            if (text != null)
            {
                textPos = new Vector2(position.X + width / 2 - spriteFont.MeasureString(text).X,
                    position.Y + 40);
            }
            if (upgradeCount == maxUpgradeAmount)
            {
                isMaxed = true;
            }
            else
            {
                isMaxed = false;
            }
        }
    }
}
