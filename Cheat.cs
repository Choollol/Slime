using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Comora;
using System.Collections.Generic;
using Slime.Sprites;
using Slime.Models;
using Slime.Managers;
using System.Diagnostics;
using System;

namespace Slime
{
    public class Cheat
    {
        private KeyboardState kStateOld;
        private bool isCheat = false;
        public void Update(GameTime gameTime, ShopManager shopManager, SoulManager soulManager, Player player, Attack attack)
        {
            KeyboardState kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.C) && kStateOld.IsKeyUp(Keys.C))
            {
                if (isCheat)
                {
                    isCheat = false;
                }
                else if (!isCheat)
                {
                    isCheat = true;
                }
            }
            if (isCheat)
            {
                player.speed = 900f;
                attack.Damage = 50;
                attack.Cooldown = 0.1f;
            }
            if (kState.IsKeyDown(Keys.P))
            {
                shopManager.isShopOpen = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                soulManager.Souls += 1000;
            }
            if (kState.IsKeyDown(Keys.H))
            {
                player.health = player.maxHealth;
            }
            if (kState.IsKeyDown(Keys.M))
            {
                player.hasMetWitch = true;
            }
            
            kStateOld = kState;
        }
    }
}
