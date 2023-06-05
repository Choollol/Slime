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

namespace Slime.Controllers
{
    public class ProjectileController
    {
        private Dictionary<string, Texture2D> textures;

        public List<Projectile> currentProjectiles = new List<Projectile>();
        public ProjectileController(Dictionary<string, Texture2D> projectileTextures)
        {
            textures = projectileTextures;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Projectile projectile in currentProjectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
        public void Update(GameTime gameTime, EnemyController enemyController, Player player, 
            AbilityController abilityController, MapController mapController, SoundManager soundManager)
        {
            if (!abilityController.isTimeStopped || mapController.currentMap == Map.temple)
            {
                foreach (Enemy enemy in enemyController.currentEnemies)
                {
                    Vector2 enemyCenter = new Vector2(enemy.position.X + enemy.width / 2, enemy.position.Y + enemy.height / 2);
                    if (enemy.doUseProjectile && enemy.attackTimer <= 0 && 
                        Vector2.Distance(enemy.position, player.position) < enemy.projectileRange && enemy.health > 0)
                    {
                        soundManager.playSoundRandomPitch(enemy.currentSoundKeys["projectileLaunch"], -30, 30);
                        if (mapController.currentMap == Map.cave1 || mapController.currentMap == Map.cave2 ||
                            mapController.currentMap == Map.cave3)
                        {
                            currentProjectiles.Add(new Projectile(textures["tarlitProjectile"], enemyCenter, player.center, enemy.damage,
                                enemy.projectileSpeed, 5f, 5f));
                        }
                        else if (mapController.currentMap == Map.cliff)
                        {
                            currentProjectiles.Add(new Projectile(textures["tempusProjectile"], enemyCenter, player.center, enemy.damage,
                                enemy.projectileSpeed, 3f, 200f));
                        }
                        enemy.attackTimer = enemy.attackCooldown;
                    }
                }
                foreach (Projectile projectile in currentProjectiles.ToArray())
                {
                    projectile.Update(gameTime, player);
                    if (projectile.isRemoved)
                    {
                        currentProjectiles.Remove(projectile);
                    }
                }
            }
        }
    }
}
