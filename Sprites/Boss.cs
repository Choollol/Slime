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

namespace Slime.Sprites
{
    public class Boss : Enemy
    {
        private Dictionary<string, Texture2D> textures;
        private int rectWidth = 248;
        private float barrageCooldown = 0.5f;
        private float barrageTimer = 0;
        private Vector2 barrageStartPos;
        private bool doWingFire = false;
        private float wingFireCooldown = 5f;
        private float wingFireTimer = 1;
        private List<Vector2> wingPositions = new List<Vector2>() { new Vector2(125, 200), new Vector2(420, 200),
            new Vector2(125, 335), new Vector2(420, 335) };
        private bool doSummonHealingCrystals = false;
        private float healingCrystalCooldown = 60;
        private float healingCrystalTimer = 0;
        private List<Vector2> healingCrystalPositions = new List<Vector2>();
        private int healingCrystalOffset = 500;
        private bool doSetHealingCrystalPositions = true;
        private bool doActivateSun = true;
        private bool doSummonSun = false;
        private bool doPlayDeathSequence = true;

        public bool doAttack = false;
        public List<HealingCrystal> healingCrystals = new List<HealingCrystal>();
        public bool doFade;
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)(position.X + width / 2 - rectWidth / 2), 
                (int)position.Y, rectWidth, height); }
        }
        public Boss(Dictionary<string, Animation> anims, Dictionary<string, Texture2D> bossTextures) : base(anims)
        {
            animationManager = new AnimationManager(anims.First().Value);
            animations = anims;
            textures = bossTextures;
            depth = 0.6f;
            maxHealth = 20000;
            health = maxHealth;
            width = animationManager.FrameWidth;
            height = animationManager.FrameHeight;
        }
        private void activatePhases(MapController mapController)
        {
            if (doAttack)
            {
                if (health < maxHealth - 4000 && !doWingFire)
                {
                    doWingFire = true;
                }
                else if (health < maxHealth - 10000 && !doSummonHealingCrystals)
                {
                    doSummonHealingCrystals = true;
                }
                else if (health < maxHealth - 14000 && !doSummonSun)
                {
                    doSummonSun = true;
                }
            }
            if (mapController.currentMap != Map.temple)
            {
                deactivateAbilities();
                health = maxHealth;
            }
        }
        private void barrage(float dt, ProjectileController projectileController, Player player, 
            SoundManager soundManager)
        {
            if (barrageTimer <= 0)
            {
                Projectile barrageProjectile = new Projectile(textures["energyBall"], barrageStartPos,
                    player.center, 50, 20f, 10f, 250f)
                {
                    depth = 0.63f,
                };
                projectileController.currentProjectiles.Add(barrageProjectile);
                soundManager.playSoundRandomPitch("energyBlast", -30, 30);
                barrageTimer = barrageCooldown;
            }
            else if (barrageTimer > 0)
            {
                barrageTimer -= dt;
            }
        }
        private void wingFire(float dt, ProjectileController projectileController, Player player, 
            SoundManager soundManager)
        {
            if (wingFireTimer <= 0.5f)
            {
                soundManager.playSoundInstance("largeEnergyBlast");
            }
            if (wingFireTimer <= 0)
            {
                foreach (Vector2 wingPos in wingPositions)
                {
                    Projectile wingFireProjectile = new Projectile(textures["energyBall"], position + wingPos,
                        player.center, 70, 100f, 2f, 250f, 4f, 8f)
                    {
                        color = Color.Red,
                        depth = 0.64f,
                    };
                    projectileController.currentProjectiles.Add(wingFireProjectile);
                    CameraController.cameraShakeTimer = 0.8f;
                    CameraController.cameraShakeAmount = 6f;
                }
                wingFireTimer = wingFireCooldown;
            }
            else if (wingFireTimer > 0)
            {
                wingFireTimer -= dt;
            }
            
        }
        private void summonHealingCrystals(GameTime gameTime, float dt, ProjectileController projectileController, 
            SoundManager soundManager)
        {
            if (healingCrystalTimer <= 0)
            {
                if (doSetHealingCrystalPositions)
                {
                    healingCrystalPositions.Add(new Vector2(center.X, center.Y - healingCrystalOffset));
                    healingCrystalPositions.Add(new Vector2(center.X + healingCrystalOffset, center.Y));
                    healingCrystalPositions.Add(new Vector2(center.X, center.Y + healingCrystalOffset));
                    healingCrystalPositions.Add(new Vector2(center.X - healingCrystalOffset, center.Y));
                    foreach (Vector2 healingCrystalPos in healingCrystalPositions)
                    {
                        HealingCrystal healingCrystal = new HealingCrystal(textures["healingCrystal"], animations["healingCrystalDestroyed"], 
                            100, 600, 60, 1)
                        {
                            position = new Vector2(healingCrystalPos.X - textures["healingCrystal"].Width / 2, healingCrystalPos.Y),
                            sourceRect = new Rectangle(0, 0, textures["healingCrystal"].Width, 0),
                            depth = 0.62f,
                        };
                        healingCrystals.Add(healingCrystal);
                    }
                    doSetHealingCrystalPositions = false;
                }
                if (healingCrystals.Count == 0)
                {
                    healingCrystalTimer = healingCrystalCooldown;
                    doSetHealingCrystalPositions = true;
                    healingCrystalPositions.Clear();
                }
            }
            else if (healingCrystalTimer > 0)
            {
                healingCrystalTimer -= dt;
            }

        }
        private void summonSun(EnemyController enemyController, SoundManager soundManager)
        {
            if (doActivateSun)
            {
                Dictionary<string, Animation> sunAnimations = new Dictionary<string, Animation>()
                {
                    {"moveUp", animations["sunAnimation"] },
                    {"moveDown", animations["sunAnimation"] },
                    {"moveLeft", animations["sunAnimation"] },
                    {"moveRight", animations["sunAnimation"] },
                };
                Enemy sun = new Enemy(sunAnimations)
                {
                    doRotate = true,
                    rotationSpeed = 100f,
                    origin = new Vector2(animations["sunAnimation"].frameWidth / 2, animations["sunAnimation"].frameHeight / 2),
                    speed = 60f,
                    doShowHealth = false,
                    addDamage = 95,
                    scale = 5,
                    range = 100000,
                    attackCooldown = 0.5f,
                    position = center,
                    depth = 0.61f,
                };
                sun.maxHealth = 10000000;
                sun.health = sun.maxHealth;
                enemyController.currentEnemies.Add(sun);
                doActivateSun = false;
            }
        }
        private void deactivateAbilities()
        {
            doAttack = false;
            doWingFire = false;
            doSummonHealingCrystals = false;
            doSummonSun = false;
        }
        private void death(Player player, SoundManager soundManager, EnemyController enemyController, 
            ProjectileController projectileController, CameraController cameraController, MapController mapController, float dt)
        {
            if (doPlayDeathSequence)
            {
                doAttack = false;
                deactivateAbilities();
                foreach (HealingCrystal healingCrystal in healingCrystals)
                {
                    healingCrystal.health = 0;
                }
                enemyController.currentEnemies.Clear();
                projectileController.currentProjectiles.Clear();
                soundManager.stopSoundInstance("bossFightBackgroundMusic");

                cameraController.doPlayCutscene = true;
                cameraController.cutsceneStartPos = cameraController.position;
                cameraController.cutsceneTargetPos = center;
                cameraController.doMoveCutsceneCamera = true;

                player.canMove = false;
                player.canAttack = false;

                player.hasDefeatedBossCurrentRun = true;
                doPlayDeathSequence = false;
            }
            if (mapController.whiteScreenOpacity >= 2)
            {
                mapController.restartPlayer();
                SaveManager.doWipeSave = true;
                health = maxHealth;
                player.hasDefeatedBossCurrentRun = false;
                doPlayDeathSequence = true;
            }
            else
            {
                player.canMove = false;
                player.canAttack = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!base.isRemoved)
            {
                animationManager.Draw(spriteBatch);
                spriteBatch.Draw(textures["grey"], position + new Vector2(163, 99),
                    new Rectangle(0, 0, textures["grey"].Width, textures["grey"].Height * (maxHealth - health) / maxHealth),
                    Color.White * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth + 0.01f);
                foreach (HealingCrystal healingCrystal in healingCrystals)
                {
                    healingCrystal.Draw(spriteBatch);
                }
            }
        }
        public void Update(GameTime gameTime, ProjectileController projectileController, MapController mapController, 
            Player player, SoundManager soundManager, EnemyController enemyController, CameraController cameraController)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            animationManager.FrameSpeed = 0.1f;
            position = mapController.bossPos;
            animationManager.position = position;
            center = new Vector2(Rectangle.Left + Rectangle.Width / 2, Rectangle.Top + Rectangle.Height / 2);
            barrageStartPos = position + new Vector2(286, 50);

            if (!isRemoved)
            {
                activatePhases(mapController);
                if (doAttack)
                {
                    barrage(dt, projectileController, player, soundManager);
                    if (doWingFire)
                    {
                        wingFire(dt, projectileController, player, soundManager);
                    }
                    if (doSummonHealingCrystals)
                    {
                        summonHealingCrystals(gameTime, dt, projectileController, soundManager);
                    }
                    if (doSummonSun)
                    {
                        summonSun(enemyController, soundManager);
                    }
                }
                foreach (HealingCrystal healingCrystal in healingCrystals.ToArray())
                {
                    healingCrystal.Update(gameTime, dt, this, soundManager);
                    if (healingCrystal.isRemoved)
                    {
                        healingCrystals.Remove(healingCrystal);
                    }
                }
            }
            if (health <= 0)
            {
                health = 0;
                death(player, soundManager, enemyController, projectileController, cameraController, mapController, dt);
            }
            else if (player.hasMetBossCurrentRun)
            {
                doAttack = true;
            }
            if (doFade)
            {
                opacity -= 0.05f;
                animationManager.Opacity = opacity;
            }
            if (opacity <= 0)
            {
                player.canMove = true;
                player.canAttack = true;
                isRemoved = true;
            }

            animationManager.Update(gameTime);
        }
    }
}
