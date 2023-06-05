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

namespace Slime
{
    public class Cheat
    {
        private KeyboardState kStateOld;
        private MouseState mStateOld;
        private bool isCheat = false;
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, ShopController shopManager, 
            CurrencyController currencyController, Player player, AbilityController abilityController) 
        {
            /*spriteBatch.DrawString(spriteFont, attack.width + "   " + attack.height,
                    new Vector2(currencyController.soulOrbPos.X + 100, currencyController.soulOrbPos.Y + 5), Color.White, 0, Vector2.Zero, 1f,
                    SpriteEffects.None, 0.95f);
            spriteBatch.DrawString(spriteFont, attack.position.X + "  " + attack.position.Y,
                    new Vector2(currencyController.soulOrbPos.X + 100, currencyController.soulOrbPos.Y + 50), Color.White, 0, Vector2.Zero, 1f,
                    SpriteEffects.None, 0.95f);*/
        }
        public void Update(GameTime gameTime, ShopController shopManager, CurrencyController currencyController, Player player, 
            AttackController attackController, AbilityController abilityController, MapController mapController)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                currencyController.souls += 10;
            }
            if (kState.IsKeyDown(Keys.D0))
            {
                mapController.newMap = Map.temple;
            }
            if (kState.IsKeyDown(Keys.T))
            {
                player.isGhost = true;
                attackController.damage = 1;
                player.speed = 180f;
                player.health = 10;
            }
            if (kState.IsKeyDown(Keys.C))
            {
                abilityController.isTeleportUnlocked = true;
                attackController.cooldown = 0.4f;
                abilityController.teleportCooldown = 0.1f;
                player.speed = 900f;
                attackController.damage = 100;
                player.maxHealth = 1000;
                player.health = 1000;
                player.isGhost = false;
                player.regenerationAmount = 100;
                abilityController.isTimeStopUnlocked = true;
            }
            if (kState.IsKeyDown(Keys.P))
            {
                shopManager.isShopOpen = true;
            }
            
            if (kState.IsKeyDown(Keys.H))
            {
                player.health = player.maxHealth;
            }
            if (kState.IsKeyDown(Keys.M))
            {
                player.hasMetWitch = true;
            }
            if (kState.IsKeyDown(Keys.J) && kStateOld.IsKeyUp(Keys.J))
            {
                if (attackController.currentAttack == "normal")
                {
                    attackController.currentAttack = "fireball";
                }
                else
                {
                    attackController.currentAttack = "normal";
                }
            }
            if (kState.IsKeyDown(Keys.O))
            {
                attackController.damage = 15000;
            }

            kStateOld = kState;
            mStateOld = mState;
        }
    }
}
