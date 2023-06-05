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
using System.Linq;
using System.Numerics;

namespace Slime.Controllers
{
    public class EnemyController
    {
        private GraphicsDeviceManager graphics;
        private Random rand = new Random();
        private List<Enemy> enemies = new List<Enemy>();
        private Dictionary<string, Texture2D> enemySprites;
        private Dictionary<string, List<Vector2>> enemySpawnPoints = new Dictionary<string, List<Vector2>>();
        private List<Vector2> currentEnemySpawnPoints = new List<Vector2>();
        private int enemyMaxHealth = 30;
        private int enemyDamage = 5;
        private float enemySpeed = 80f;
        private float spawnCliffEnemyTimer;

        public List<Enemy> currentEnemies = new List<Enemy>();
        public bool doSwitchEnemy = false;

        public EnemyController(Dictionary<string, Texture2D> sprites, 
            GraphicsDeviceManager newGraphics, List<Vector2> laskSpawnPoints)
        {
            graphics = newGraphics;
            enemySprites = sprites;
            enemySpawnPoints["laskSpawnPoints"] = laskSpawnPoints;
        }
        private void spawnCliffEnemy()
        {
            var enemyAnimations = new Dictionary<string, Animation>()
            {
                {"moveUp", new Animation(enemySprites["tempusUp"], 2) },
                {"moveDown", new Animation(enemySprites["tempusDown"], 2) },
                {"deathUp", new Animation(enemySprites["tempusDeathUp"], 6) },
                {"deathDown", new Animation(enemySprites["tempusDeathDown"], 6) },
                {"spawn", new Animation(enemySprites["laskSpawn"], 8) },
            };
            Vector2 enemySpawnPos = Vector2.Zero;
            switch (rand.Next(0, 3))
            {
                //Top
                case 0:
                    enemySpawnPos = new Vector2(rand.Next(-1300, 1300), -700);
                    break;
                //Left
                case 1:
                    enemySpawnPos = new Vector2(-1300, rand.Next(-700, 700));
                    break;
                //Right
                case 2:
                    enemySpawnPos = new Vector2(1300, rand.Next(-700, 700));
                    break;
            }
            Enemy enemy = new Enemy(enemyAnimations, graphics)
            {
                startPos = enemySpawnPos,
                position = enemySpawnPos,
                type = "tempus",
                range = 3000,
                addHealth = 620,
                addDamage = 75,
                projectileRange = rand.Next(300, 500),
                doUseProjectile = true,
                projectileSpeed = 100,
                addSoulAmount = 100,
            };
            enemy.maxHealth = enemyMaxHealth + enemy.addHealth;
            enemy.health = enemy.maxHealth;
            enemy.currentSoundKeys = new Dictionary<string, string>()
            {
                {"deathSound", "tempusDeath" },
                {"projectileLaunch", "tempusProjectileLaunch" },
            };
            currentEnemies.Add(enemy);
        }
        private void switchEnemy(MapController mapController)
        {
            if (doSwitchEnemy)
            {
                currentEnemies.Clear();
                currentEnemySpawnPoints.Clear();
                int spawnPointsIndex = 0;
                if (mapController.currentMap == Map.forest)
                {
                    foreach (Vector2 spawnPoint in enemySpawnPoints["laskSpawnPoints"])
                    {
                        var enemyAnimations = new Dictionary<string, Animation>()
                        {
                            {"moveUp", new Animation(enemySprites["laskUp"], 8) },
                            {"moveDown", new Animation(enemySprites["laskDown"], 8) },
                            {"deathUp", new Animation(enemySprites["laskDeathUp"], 7) },
                            {"deathDown", new Animation(enemySprites["laskDeathDown"], 7) },
                            {"spawn", new Animation(enemySprites["laskSpawn"], 8) },
                        };
                        Enemy enemy = new Enemy(enemyAnimations, graphics)
                        {
                            startPos = spawnPoint,
                            position = spawnPoint,
                            type = "lask",
                        };
                        enemy.currentSoundKeys = new Dictionary<string, string>()
                        {
                            {"deathSound", "laskDeath" },
                        };
                        if (spawnPointsIndex < 11)
                        {
                            enemy.addHealth = 0;
                            enemy.addDamage = 0;
                            enemy.addSpeed = 0;
                        }
                        else if (spawnPointsIndex < 19)
                        {
                            enemy.addHealth = 15;
                            enemy.addDamage = 5;
                            enemy.addSpeed = 10;
                        }
                        else if (spawnPointsIndex < 31)
                        {
                            enemy.addHealth = 30;
                            enemy.addDamage = 20;
                            enemy.addSpeed = 30;
                        }
                        else
                        {
                            enemy.addHealth = 50;
                            enemy.addDamage = 30;
                            enemy.addSpeed = 45;
                        }
                        enemy.maxHealth = enemyMaxHealth + enemy.addHealth;
                        enemy.health = enemy.maxHealth;
                        currentEnemies.Add(enemy);
                        spawnPointsIndex++;
                    }
                }
                else if (mapController.currentMap == Map.cave1)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        Vector2 spawnPoint = new Vector2();
                        spawnPoint.Y = rand.Next(25, 1400);
                        if (i < 7)
                        {
                            spawnPoint.X = rand.Next(55, 225);
                        }
                        else
                        {
                            spawnPoint.X = rand.Next(520, 700);
                        }
                        foreach (Vector2 point in currentEnemySpawnPoints)
                        {
                            while (Vector2.Distance(point, spawnPoint) < 50)
                            {
                                spawnPoint.Y = rand.Next(25, 1400);
                            }
                        }
                        currentEnemySpawnPoints.Add(spawnPoint);
                    }
                    foreach (var spawnPoint in currentEnemySpawnPoints)
                    {
                        bool isProjectileUser;
                        if (rand.Next(0, 4) == 0)
                        {
                            isProjectileUser = true;
                        }
                        else
                        {
                            isProjectileUser = false;
                        }
                        var enemyAnimations = new Dictionary<string, Animation>()
                        {
                            {"moveUp", new Animation(enemySprites["tarlitUp"], 4) },
                            {"moveDown", new Animation(enemySprites["tarlitDown"], 4) },
                            {"deathUp", new Animation(enemySprites["tarlitDeathUp"], 7) },
                            {"deathDown", new Animation(enemySprites["tarlitDeathDown"], 7) },
                            {"spawn", new Animation(enemySprites["tarlitSpawn"], 7) },
                        };
                        Enemy enemy = new Enemy(enemyAnimations, graphics)
                        {
                            startPos = spawnPoint,
                            position = spawnPoint,
                            addHealth = 90,
                            addDamage = 40,
                            addSpeed = -30,
                            type = "tarlit",
                            doUseProjectile = isProjectileUser,
                            addSoulAmount = 10,
                        };
                        enemy.currentSoundKeys = new Dictionary<string, string>()
                        {
                            {"deathSound", "tarlitDeath" },
                            {"projectileLaunch", "tarlitProjectileLaunch" },
                        };
                        enemy.maxHealth = enemyMaxHealth + enemy.addHealth;
                        enemy.health = enemy.maxHealth;
                        if (enemy.doUseProjectile)
                        {
                            enemy.projectileRange = 150;
                        }
                        currentEnemies.Add(enemy);
                    }
                }
                else if (mapController.currentMap == Map.cave2)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        Vector2 spawnPoint = new Vector2();
                        spawnPoint.Y = rand.Next(25, 2150);
                        if (i < 12)
                        {
                            spawnPoint.X = rand.Next(85, 720);
                        }
                        else
                        {
                            spawnPoint.X = rand.Next(1050, 1650);
                        }
                        foreach (Vector2 point in currentEnemySpawnPoints)
                        {
                            while (Vector2.Distance(point, spawnPoint) < 15)
                            {
                                spawnPoint.Y = rand.Next(25, 2150);
                            }
                        }
                        currentEnemySpawnPoints.Add(spawnPoint);
                    }
                    foreach (var spawnPoint in currentEnemySpawnPoints)
                    {
                        bool isProjectileUser;
                        if (rand.Next(0, 3) == 0)
                        {
                            isProjectileUser = true;
                        }
                        else
                        {
                            isProjectileUser = false;
                        }
                        var enemyAnimations = new Dictionary<string, Animation>()
                        {
                            {"moveUp", new Animation(enemySprites["tarlitUp"], 4) },
                            {"moveDown", new Animation(enemySprites["tarlitDown"], 4) },
                            {"deathUp", new Animation(enemySprites["tarlitDeathUp"], 7) },
                            {"deathDown", new Animation(enemySprites["tarlitDeathDown"], 7) },
                            {"spawn", new Animation(enemySprites["tarlitSpawn"], 7) },
                        };
                        Enemy enemy = new Enemy(enemyAnimations, graphics)
                        {
                            startPos = spawnPoint,
                            position = spawnPoint,
                            addHealth = 120,
                            addDamage = 55,
                            addSpeed = -30,
                            type = "tarlit",
                            doUseProjectile = isProjectileUser,
                            addSoulAmount = 15,
                        };
                        enemy.currentSoundKeys = new Dictionary<string, string>()
                        {
                            {"deathSound", "tarlitDeath" },
                            {"projectileLaunch", "tarlitProjectileLaunch" },
                        };
                        enemy.projectileSpeed += 6f;
                        enemy.attackCooldown -= 0.5f;
                        enemy.maxHealth = enemyMaxHealth + enemy.addHealth;
                        enemy.health = enemyMaxHealth + enemy.addHealth;
                        if (enemy.doUseProjectile)
                        {
                            enemy.projectileRange = 250;
                        }
                        currentEnemies.Add(enemy);
                    }
                }
                else if (mapController.currentMap == Map.cave3)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        if (i < 5)
                        {
                            currentEnemySpawnPoints.Add(new Vector2(rand.Next(140, 1000), rand.Next(2500, 2950)));
                        }
                        else if (i >= 5 && i < 10)
                        {
                            currentEnemySpawnPoints.Add(new Vector2(rand.Next(1400, 2300), rand.Next(2500, 2950)));
                        }
                        else
                        {
                            currentEnemySpawnPoints.Add(new Vector2(rand.Next(140, 2300), rand.Next(140, 2350)));
                        }
                    }
                    foreach (var spawnPoint in currentEnemySpawnPoints)
                    {
                        bool isProjectileUser;
                        if (rand.Next(0, 2) == 0)
                        {
                            isProjectileUser = true;
                        }
                        else
                        {
                            isProjectileUser = false;
                        }
                        var enemyAnimations = new Dictionary<string, Animation>()
                        {
                            {"moveUp", new Animation(enemySprites["tarlitUp"], 4) },
                            {"moveDown", new Animation(enemySprites["tarlitDown"], 4) },
                            {"deathUp", new Animation(enemySprites["tarlitDeathUp"], 7) },
                            {"deathDown", new Animation(enemySprites["tarlitDeathDown"], 7) },
                            {"spawn", new Animation(enemySprites["tarlitSpawn"], 7) },
                        };
                        Enemy enemy = new Enemy(enemyAnimations, graphics)
                        {
                            startPos = spawnPoint,
                            position = spawnPoint,
                            addHealth = 220,
                            addDamage = 80,
                            addSpeed = -20,
                            type = "tarlit",
                            doUseProjectile = isProjectileUser,
                            addSoulAmount = 20,
                        };
                        enemy.currentSoundKeys = new Dictionary<string, string>()
                        {
                            {"deathSound", "tarlitDeath" },
                            {"projectileLaunch", "tarlitProjectileLaunch" },
                        };
                        enemy.maxHealth = enemyMaxHealth + enemy.addHealth;
                        enemy.health = enemy.maxHealth;
                        enemy.projectileSpeed += 12f;
                        enemy.attackCooldown -= 0.8f;
                        if (enemy.doUseProjectile)
                        {
                            enemy.projectileRange = 400;
                        }
                        currentEnemies.Add(enemy);
                    }
                }
                doSwitchEnemy = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in currentEnemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
        public void Update(GameTime gameTime, Player player, MapController mapController, AbilityController abilityController, 
            SoundManager soundManager, TreeController treeController)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switchEnemy(mapController);
            if (!abilityController.isTimeStopped) 
            {
                if (mapController.currentMap == Map.cliff)
                {
                    if (spawnCliffEnemyTimer <= 0)
                    {
                        spawnCliffEnemy();
                        spawnCliffEnemyTimer = rand.Next(3, 7);
                    }
                    else
                    {
                        spawnCliffEnemyTimer -= dt;
                    }
                    foreach (Enemy enemy in currentEnemies.ToArray())
                    {
                        if (enemy.isRemoved)
                        {
                            currentEnemies.Remove(enemy);
                        }
                    }
                }
                foreach (Enemy enemy in currentEnemies)
                {
                    enemy.maxHealth = enemyMaxHealth + enemy.addHealth;
                    enemy.damage = enemyDamage + enemy.addDamage;
                    enemy.speed = enemySpeed + enemy.addSpeed;
                    enemy.Update(gameTime, player, currentEnemies, soundManager, mapController, treeController);
                }
            }
        }
    }
}
