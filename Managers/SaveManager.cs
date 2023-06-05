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

namespace Slime.Managers
{
    public class SaveManager
    {
        private float saveInterval = 180;
        private float saveTimer;
        public SaveManager()
        {
            shopItemUnlockStatus = new List<bool>();
            shopItemUpgradeCounts = new List<int>();
        }

        public static bool doWipeSave = false;
        public void wipeCurrentSave(AbilityIconController abilityIconController, ShopController shopController)
        {
            hasMetWitch = false;
            isGhost = false;
            speed = 180;
            maxHealth = 50;
            health = 50;
            regenerationAmount = 0;
            hasMetBossCurrentRun = false;
            hasDefeatedBossCurrentRun = false;
            damage = 5;
            cooldown = 2;
            currentAttack = "normal";
            isTeleportUnlocked = false;
            teleportCooldown = 5;
            isTimeStopUnlocked = false;
            timeStopCooldown = 30;
            souls = 0;
            bossDefeatCount++;

            abilityIconController.icons.Clear();
            for (int i = 0; i < shopItemUnlockStatus.Count; i++)
            {
                shopItemUpgradeCounts[i] = 1;
                if (i > 2)
                {
                    shopItemUnlockStatus[i] = false;
                }
            }
            Game1.doTransferSaveData = true;
            doWipeSave = false;
        }
        public void Update(GameTime gameTime, AbilityIconController abilityIconController, ShopController shopController)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (doWipeSave)
            {
                wipeCurrentSave(abilityIconController, shopController);
            }
            if (doAutoSave)
            {
                if (saveTimer <= 0)
                {
                    Game1.doSave = true;
                    saveTimer = saveInterval;
                }
                else if (saveTimer > 0)
                {
                    saveTimer -= dt;
                }
            }
        }

        public bool isFullScreen { get; set; }

        //Menu
        public bool doAutoSave { get; set; }

        //Camera
        public bool doShakeCamera { get; set; }

        //Player
        public bool hasMetWitch { get; set; }
        public bool isGhost { get; set; }
        public float speed { get; set; }
        public int maxHealth { get; set; }
        public int health { get; set; }
        public int regenerationAmount { get; set; }
        public bool hasMetBossCurrentRun { get; set; }
        public bool hasDefeatedBossCurrentRun { get; set; }
        public int bossDefeatCount { get; set; }
        public bool doMouseDirect { get; set; }

        //Attack
        public int damage { get; set; }
        public float cooldown { get; set; }
        public string currentAttack { get; set; }

        //Abilities
        public bool isTeleportUnlocked { get; set; }
        public float teleportCooldown { get; set; }
        public bool isTimeStopUnlocked { get; set; }
        public float timeStopCooldown { get; set; }

        //Currency
        public int souls { get; set; }
        
        //Shop
        public List<bool> shopItemUnlockStatus { get; set; }
        public List<int> shopItemUpgradeCounts { get; set; }

        //Sound
        public float masterVolume { get; set; }

        //Boss
        public bool isBossGone { get; set; }

    }
}
