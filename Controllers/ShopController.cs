using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slime.Sprites;
using Comora;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Diagnostics;
using System.Collections;
using Slime.Managers;

namespace Slime.Controllers
{
    public class ShopController
    {
        private Dictionary<string, Texture2D> textures;
        private MouseState mStateOld;
        private SpriteFont spriteFont;
        private Vector2 shopBackgroundPos;
        private Vector2 exitButtonPos;
        private float exitButtonOpacity;
        private Vector2 iconOffset = new Vector2(16, 100);
        private bool doShowDescription = false;
        private string description;
        private Vector2 descriptionPos;
        private float smallTextScale = 1f;
        private int[] upgradeAmounts = { 5, 10, 50, 1, 2 };
        private int[] maxUpgradeAmounts = { 10, 26, 13, 81, 51, 2, 2, 2, 2 };
        private Dictionary<string, List<int>> upgradeCosts;
        private int unlockMultiplier = 4;

        public List<ShopItem> shopItems = new List<ShopItem>();
        public bool isShopOpen = false;
        public ShopController(Dictionary<string, Texture2D> newTextures, SpriteFont font)
        {
            textures = newTextures;
            spriteFont = font;

            upgradeCosts = new Dictionary<string, List<int>>()
            {
                {"attack", new List<int>() { 5, 15, 40, 70, 150, 240, 340, 460, 640, 900 } },
                {"health", new List<int> { 40 } },
                {"speed", new List<int> { 60 } },
                {"cooldown", new List<int> { 60 } },
                {"regeneration", new List<int> { 80 } },
                {"fireball", new List<int> { 500 / unlockMultiplier } },
                {"ghost", new List<int> { 800 / unlockMultiplier } },
                {"teleport", new List<int> { 1000 / unlockMultiplier } },
                {"timeStop", new List<int> { 3000 / unlockMultiplier } },
            };

            for (int i = 0; i < 9; i++)
            {
                ShopItem item;
                if (i == 0)
                {
                    //attack
                    item = new ShopItem(spriteFont, true, textures["attackUpgradeIcon"], "ATTACK", upgradeCosts["attack"],
                        maxUpgradeAmounts[i]);
                }
                else if (i == 1)
                {
                    //health
                    for (int j = 1; j < maxUpgradeAmounts[i]; j++)
                    {
                        upgradeCosts["health"].Add(upgradeCosts["health"][j - 1] + 15);
                    }
                    item = new ShopItem(spriteFont, true, textures["healthUpgradeIcon"], "HEALTH", upgradeCosts["health"],
                        maxUpgradeAmounts[i]);
                }
                else if (i == 2)
                {
                    //speed
                    for (int j = 1; j < maxUpgradeAmounts[i]; j++)
                    {
                        upgradeCosts["speed"].Add(upgradeCosts["speed"][j - 1] + 20);
                    }
                    item = new ShopItem(spriteFont, true, textures["speedUpgradeIcon"], "SPEED", upgradeCosts["speed"],
                        maxUpgradeAmounts[i]);
                }
                else if (i == 3)
                {
                    //cooldown
                    for (int j = 1; j < maxUpgradeAmounts[i]; j++)
                    {
                        upgradeCosts["cooldown"].Add(upgradeCosts["cooldown"][j - 1] + 10);
                    }
                    item = new ShopItem(spriteFont, false, textures["cooldownUpgradeIcon"], "COOLDOWN", upgradeCosts["cooldown"],
                        maxUpgradeAmounts[i]);
                }
                else if (i == 4)
                {
                    //regeneration
                    for (int j = 1; j < maxUpgradeAmounts[i]; j++)
                    {
                        upgradeCosts["regeneration"].Add(upgradeCosts["regeneration"][j - 1] + 5);
                    }
                    item = new ShopItem(spriteFont, false, textures["regenerationUpgradeIcon"], "REGENERATION", 
                        upgradeCosts["regeneration"], maxUpgradeAmounts[i]);
                }
                else if (i == 5)
                {
                    //fireball
                    item = new ShopItem(spriteFont, false, textures["fireballUpgradeIcon"], "FIREBALL", upgradeCosts["fireball"],
                        maxUpgradeAmounts[i]);
                    item.isAbility = true;
                }
                else if (i == 6)
                {
                    //ghost
                    item = new ShopItem(spriteFont, false, textures["ghostUpgradeIcon"], "GHOST", upgradeCosts["ghost"],
                        maxUpgradeAmounts[i]);
                    item.isAbility = true;
                }
                else if (i == 7)
                {
                    //teleport
                    item = new ShopItem(spriteFont, false, textures["teleportUpgradeIcon"], "TELEPORT", upgradeCosts["teleport"], 2);
                    item.isAbility = true;
                }
                else if (i == 8)
                {
                    //timeStop
                    item = new ShopItem(spriteFont, false, textures["timeStopUpgradeIcon"], "TIMESTOP", upgradeCosts["timeStop"], 2);
                    item.isAbility = true;
                }
                else
                {
                    item = new ShopItem(spriteFont, false, null, null, null, 10);
                }
                item.numID = i;
                item.width = textures["shopItemLight"].Width;
                item.height = textures["shopItemLight"].Height;
                item.background = textures["shopItemLight"];
                shopItems.Add(item);
            }
        }
        private void closeShop(Player player, KeyboardState kState, MouseState mState, Rectangle mRect, 
            AttackController attackController, SoundManager soundManager, AbilityController abilityController)
        {
            Rectangle exitButtonRect = new Rectangle((int)exitButtonPos.X, (int)exitButtonPos.Y, 64, 64);
            soundManager.addButtonRect(exitButtonRect);

            if (mRect.Intersects(exitButtonRect))
            {
                exitButtonOpacity = 1f;
            }
            else
            {
                exitButtonOpacity = 0.7f;
            }
            if (kState.IsKeyDown(Keys.E) || kState.IsKeyDown(Keys.Escape) ||
                (mRect.Intersects(exitButtonRect) && mState.LeftButton == ButtonState.Pressed &&
                mStateOld.LeftButton == ButtonState.Released))
            {
                if (mRect.Intersects(exitButtonRect) && mState.LeftButton == ButtonState.Pressed &&
                mStateOld.LeftButton == ButtonState.Released)
                {
                    soundManager.playSound("buttonClick");
                }
                isShopOpen = false;
                player.canMove = true;
                player.canAttack = true;
                attackController.timer = 0.1f;
                abilityController.canUseAbilities = true;
            }
        }
        private void placeShopItems(Vector2 shopBackgroundPos)
        {
            int shopItemWidth = shopItems[0].width;
            int shopItemHeight = shopItems[0].height;
            shopItems[0].position = new Vector2(shopBackgroundPos.X + 34, shopBackgroundPos.Y + 34);
            shopItems[1].position = new Vector2(shopBackgroundPos.X + 34, shopBackgroundPos.Y);
            for (int i = 1; i < shopItems.Count; i++)
            {
                if (i == 3 || i == 6)
                {
                    shopItems[i].position = new Vector2(shopItems[0].position.X,
                        shopItems[i - 1].position.Y + shopItemHeight + 8);

                }
                else
                {
                    shopItems[i].position = new Vector2(shopItems[i - 1].position.X + shopItemWidth + 8,
                        shopItems[i - 1].position.Y);
                }
            }

            for (int i = 0; i < shopItems.Count; i++)
            {
                shopItems[i].iconPos = new Vector2(shopItems[i].position.X + iconOffset.X,
                    shopItems[i].position.Y + iconOffset.Y);
            }

            descriptionPos = new Vector2(shopBackgroundPos.X + textures["shopBackground"].Width / 2 -
                textures["itemDescriptionBackground"].Width / 2,
                shopBackgroundPos.Y - textures["itemDescriptionBackground"].Height - 20);
        }
        private void itemUpdate(Rectangle mRect, Player player, AttackController attackController, SoundManager soundManager)
        {
            doShowDescription = false;

            foreach (ShopItem item in shopItems)
            {
                if (item.isUnlocked)
                {
                    soundManager.addButtonRect(item.Rectangle);
                }
                switch (item.typeID)
                {
                    case "attack":
                        item.currentUpgradeAmount = attackController.damage.ToString();
                        item.nextUpgradeAmount = (attackController.damage + upgradeAmounts[item.numID]).ToString();
                        break;
                    case "health":
                        item.currentUpgradeAmount = player.maxHealth.ToString();
                        item.nextUpgradeAmount = (player.maxHealth + upgradeAmounts[item.numID]).ToString();
                        break;
                    case "speed":
                        item.currentUpgradeAmount = player.speed.ToString();
                        item.nextUpgradeAmount = (player.speed + upgradeAmounts[item.numID]).ToString();
                        break;
                    case "cooldown":
                        item.currentUpgradeAmount = (100 - (item.upgradeCount - 1) * upgradeAmounts[item.numID] + "%").ToString();
                        item.nextUpgradeAmount = (100 - item.upgradeCount * upgradeAmounts[item.numID] + "%").ToString();
                        break;
                    case "regeneration":
                        item.currentUpgradeAmount = player.regenerationAmount.ToString();
                        item.nextUpgradeAmount = (player.regenerationAmount + upgradeAmounts[item.numID]).ToString();
                        break;
                }
                if (item.isMaxed)
                {
                    item.nextUpgradeAmount = item.currentUpgradeAmount;
                }

                if (mRect.Intersects(item.Rectangle) && item.isUnlocked)
                {
                    item.background = textures["shopItemDark"];
                }
                else if (item.isUnlocked)
                {
                    item.background = textures["shopItemLight"];
                }
                else
                {
                    item.background = textures["shopItemLocked"];
                }
                if (!item.isMaxed)
                {
                    item.cost = item.upgradeCostAmounts[item.upgradeCount - 1];
                }
                if (!item.isUnlocked && shopItems[item.numID - 1].isUnlocked)
                {
                    item.cost *= unlockMultiplier;
                }
                if (mRect.Intersects(item.Rectangle))
                {
                    doShowDescription = true;
                    if (item.isUnlocked)
                    {
                        description = "COST:" + item.cost;
                    }
                    else if (shopItems[item.numID - 1].isUnlocked)
                    {
                        description = "UNLOCK:" + item.cost;
                    }
                    else
                    {
                        description = "LOCKED";
                    }
                    if (item.isMaxed)
                    {
                        description = "MAX";
                    }
                }
            }
        }
        private void buyUpgrade(Rectangle mRect, CurrencyController currencyController, MouseState mState, Player player, 
            AttackController attackController, AbilityController abilityController, AbilityIconController abilityIconController, 
            SoundManager soundManager)
        {
            foreach (ShopItem item in shopItems)
            {
                if (mRect.Intersects(item.Rectangle) && mState.LeftButton == ButtonState.Pressed &&
                mStateOld.LeftButton == ButtonState.Released &&
                currencyController.souls >= item.cost)
                {
                    soundManager.playSound("buttonClick");
                    if (item.isUnlocked && !item.isMaxed)
                    {
                        currencyController.souls -= item.cost;
                        item.upgradeCount++;
                        if (item.typeID == "attack")
                        {
                            attackController.damage += upgradeAmounts[item.numID];
                        }
                        else if (item.typeID == "health")
                        {
                            player.maxHealth += upgradeAmounts[item.numID];
                        }
                        else if (item.typeID == "speed")
                        {
                            player.speed += upgradeAmounts[item.numID];
                        }
                        else if (item.typeID == "cooldown")
                        {
                            attackController.cooldown = 2f * (1f - item.upgradeCount / 100f);
                            abilityController.teleportCooldown = 5f * (1f - item.upgradeCount / 100f);
                            abilityController.timeStopCooldown = 30f * (1f - item.upgradeCount / 100f);
                        }
                        else if (item.typeID == "regeneration")
                        {
                            player.regenerationAmount += upgradeAmounts[item.numID];
                        }
                    }
                    else if (!item.isUnlocked && shopItems[item.numID - 1].isUnlocked)
                    {
                        currencyController.souls -= item.cost;
                        item.isUnlocked = true;
                        if (item.isAbility)
                        {
                            if (item.typeID == "fireball")
                            {
                                attackController.currentAttack = "fireball";
                                attackController.damage += 50;
                            }
                            else if (item.typeID == "ghost")
                            {
                                player.isGhost = true;
                            }
                            else if (item.typeID == "teleport")
                            {
                                abilityController.isTeleportUnlocked = true;
                            }
                            else if (item.typeID == "timestop")
                            {
                                abilityController.isTimeStopUnlocked = true;
                            }
                            item.upgradeCount++;
                        }
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (isShopOpen)
            {
                spriteBatch.Draw(textures["shopBackground"], shopBackgroundPos, null,
                    Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                spriteBatch.Draw(textures["exitButton"], exitButtonPos, null,
                    Color.White * exitButtonOpacity, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                spriteBatch.Draw(textures["blackScreen"], new Vector2(camera.Position.X - camera.Width / 2,
                        camera.Position.Y - camera.Height / 2), null, Color.White * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None,
                        0.7f);
                foreach (ShopItem item in shopItems)
                {
                    item.Draw(spriteBatch, smallTextScale);
                    if (item.isUnlocked)
                    {
                        if (!item.isAbility)
                        {
                            spriteBatch.DrawString(spriteFont, item.currentUpgradeAmount,
                                new Vector2(item.currentTextPos.X + spriteFont.MeasureString("CURRENT").X / 2 * smallTextScale -
                                spriteFont.MeasureString(item.currentUpgradeAmount).X / 2 * smallTextScale, item.currentTextPos.Y + 30),
                                Color.Black, 0f, Vector2.Zero, smallTextScale, SpriteEffects.None, 0.92f);
                            spriteBatch.DrawString(spriteFont, item.nextUpgradeAmount,
                                new Vector2(item.nextTextPos.X + spriteFont.MeasureString("NEXT").X / 2 * smallTextScale -
                                spriteFont.MeasureString(item.nextUpgradeAmount).X / 2 * smallTextScale,
                                item.nextTextPos.Y + 30), Color.Black, 0f, Vector2.Zero, smallTextScale, SpriteEffects.None, 0.92f);
                        }
                        else if (item.isAbility)
                        {
                            spriteBatch.DrawString(spriteFont, "UNLOCKED",
                                new Vector2(item.currentTextPos.X + 30, item.currentTextPos.Y + 15),
                                Color.Black, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0.92f);
                        }
                    }
                }

                if (doShowDescription)
                {
                    spriteBatch.Draw(textures["itemDescriptionBackground"], descriptionPos, null, Color.White, 0f, Vector2.Zero,
                        1f, SpriteEffects.None, 0.91f);
                    spriteBatch.DrawString(spriteFont, description,
                        new Vector2(descriptionPos.X + textures["itemDescriptionBackground"].Width / 2 -
                        spriteFont.MeasureString(description).X, descriptionPos.Y + 24),
                        Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.92f);
                }
            }
        }
        public void Update(GameTime gameTime, Player player, MapController mapController, DialogueController dialogueController,
            CurrencyController currencyController, AbilityController abilityController, Camera camera, AttackController attackController,
            AbilityIconController abilityIconController, SoundManager soundManager)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            Rectangle mRect = new Rectangle((int)(camera.Position.X - camera.Width / 2 + mState.X),
                (int)(camera.Position.Y - camera.Height / 2 + mState.Y), 1, 1);
            shopBackgroundPos = new Vector2(camera.Position.X - textures["shopBackground"].Width / 2,
                    camera.Position.Y - textures["shopBackground"].Height / 2 + 60);
            exitButtonPos = new Vector2(shopBackgroundPos.X + textures["shopBackground"].Width - 40,
                    shopBackgroundPos.Y - 24);

            foreach (ShopItem item in shopItems)
            {
                item.Update(gameTime);
            }
            if (dialogueController.openShop)
            {
                isShopOpen = true;
                dialogueController.openShop = false;
            }
            if (isShopOpen)
            {
                closeShop(player, kState, mState, mRect, attackController, soundManager, abilityController);
                placeShopItems(shopBackgroundPos);
                itemUpdate(mRect, player, attackController, soundManager);
                buyUpgrade(mRect, currencyController, mState, player, attackController, abilityController, 
                    abilityIconController, soundManager);
            }
            else
            {
                soundManager.clearButtonRects();
            }

            mStateOld = mState;
        }
    }
}
