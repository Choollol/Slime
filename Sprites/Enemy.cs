using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slime.Models;
using Slime.Managers;
using Transform;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Diagnostics.SymbolStore;
using Slime.Controllers;
using System.Text.Json.Serialization;

namespace Slime.Sprites
{
    public class Enemy : Sprite
    {
        private Dictionary<string, Animation> animations;
        private Vector2 pos;
        private Texture2D texture;
        private KeyboardState kState;
        private KeyboardState kStateOld;
        private bool inRange = false;
        private bool isSameNest = false;
        private float respawnCooldown = 30f;
        private float respawnTimer = 0f;
        private float showDamagedDuration = 0.2f;
        private float showDamagedTimer = 0f;
        private int shownHealth = 50;
        private int shownMaxHealth = 50;
        private bool doPlayDeathSound = true;
        private Direction dir;
        private bool doPlayDeathAnimation = true;
        private bool canMove = true;

        public Input input;
        public Vector2 startPos;
        public bool isDead = false;
        public float speed { get; set; }
        public int range = 500;
        public int maxHealth;
        public int health;
        public int addHealth;
        public int addDamage;
        public int addSpeed;
        public Color[] healthData;
        public int healthBarHeight = 7;
        public Texture2D healthBar;
        public int damage = 5;
        public bool canGiveSouls = false;
        public bool preventSouls = true;
        public bool canBeHit = true;
        public Dictionary<string, string> currentSoundKeys = new Dictionary<string, string>();
        public string type = "lask";
        public int projectileRange = 5;
        public bool doUseProjectile = false;
        public float attackCooldown = 2f;
        public float attackTimer = 1f;
        public float projectileSpeed = 5f;
        public bool doShowHealth = true;
        public int addSoulAmount = 0;
        public Rectangle enemyCollisionRectangle
        {
            get { return new Rectangle((int)position.X + 5, (int)position.Y + 5, width - 10, height - 10); }
        }
        public Enemy(Dictionary<string, Animation> anims)
        {
            animations = anims;
            animationManager = new AnimationManager(animations.First().Value);
        }
        public Enemy(Dictionary<string, Animation> animations, GraphicsDeviceManager graphics)
        {
            this.animations = animations;
            animationManager = new AnimationManager(animations["moveDown"]);
            currentSoundKeys = new Dictionary<string, string>()
            {
                {"deathSound", "laskDeath" },
            };
            healthData = new Color[shownMaxHealth * healthBarHeight];
            for (int i = 0; i < shownMaxHealth * healthBarHeight; i++)
            {
                healthData[i] = Color.Red;
            }
            healthBar = new Texture2D(graphics.GraphicsDevice, shownMaxHealth, healthBarHeight);
            healthBar.SetData(healthData);
            animationManager.Depth = 0.55f;
        }
        private void move(Player player)
        {
            if ((!doUseProjectile && Vector2.Distance(center, player.center) > 10) ||
                (doUseProjectile && Vector2.Distance(center, player.center) > projectileRange))
            {
                velocity = player.position - position;
            }
            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
            }
        }
        private void collideWithEnemies(List<Enemy> enemies, float dt)
        {
            Rectangle movingXRect = new Rectangle((int)(position.X + velocity.X), (int)position.Y, width, height);
            Rectangle movingYRect = new Rectangle((int)position.X, (int)(position.Y + velocity.Y), width, height);
            foreach (Enemy enemy in enemies)
            {
                Rectangle enemyCenterRect = new Rectangle((int)enemy.position.X + width / 2 - 4,
                    (int)enemy.position.Y + height / 2 - 4, 8, 8);

                if (position != enemy.position && !enemy.isRemoved)
                {
                    if (collideLeft(movingXRect, enemy.enemyCollisionRectangle) && velocity.X > 0)
                    {
                        velocity.X -= 0.5f;
                    }
                    else if (collideRight(movingXRect, enemy.enemyCollisionRectangle) && velocity.X < 0)
                    {
                        velocity.X += 0.5f;
                    }
                    if (collideTop(movingYRect, enemy.enemyCollisionRectangle) && velocity.Y > 0)
                    {
                        velocity.Y -= 0.5f;
                    }
                    else if (collideBottom(movingYRect, enemy.enemyCollisionRectangle) && velocity.Y < 0)
                    {
                        velocity.Y += 0.5f;
                    }
                }
            }
        }
        private void setAnimation(Vector2 center, Player player)
        {
            if (health > 0)
            {
                if (velocity == Vector2.Zero)
                {
                    animationManager.Play(animations["moveDown"]);
                    if (type == "tarlit")
                    {
                        animationManager.Stop();
                    }
                }
                else if (center.Y > player.center.Y)
                {
                    animationManager.Play(animations["moveUp"]);
                    dir = Direction.Up;
                }
                else if (center.Y <= player.center.Y)
                {
                    animationManager.Play(animations["moveDown"]);
                    dir = Direction.Down;
                }
            }
        }
        private void attackPlayer(Player player)
        {
            if (Rectangle.Intersects(new Rectangle((int)player.position.X + 10, (int)player.position.Y + 10, 
                player.width - 20, player.height - 20)) && player.canBeDamaged && !doUseProjectile)
            {
                player.health -= damage;
                attackTimer = attackCooldown;
            }
        }
        private int getNest(Vector2 pos, MapController mapController)
        {
            if (mapController.currentMap == Map.forest)
            {
                //Bottom right nest
                if (pos.X > 2360 && pos.X < 3650 && pos.Y > 2410 && pos.Y < 3380)
                {
                    return 1;
                }
                //Bottom left nest
                else if (pos.X > 300 && pos.X < 1050 && pos.Y > 2330 && pos.Y < 3090)
                {
                    return 2;
                }
                //Top left nest
                else if (pos.X > 370 && pos.X < 1050 && pos.Y > 160 && pos.Y < 710)
                {
                    return 3;
                }
                //Top right nest
                else if (pos.X > 2900 && pos.X < 3935 && pos.Y > 15 && pos.Y < 1085)
                {
                    return 4;
                }
            }
            return 0;
        }
        private void death(MapController mapController, SoundManager soundManager, float dt)
        {
            if (health <= 0)
            {
                isDead = true;
                animationManager.FrameSpeed = 0.07f;
                canMove = false;
                if (doPlayDeathAnimation)
                {
                    if (dir == Direction.Up)
                    {
                        animationManager.Play(animations["deathUp"]);
                    }
                    else if (dir == Direction.Down)
                    {
                        animationManager.Play(animations["deathDown"]);
                    }
                    position = new Vector2(position.X + width / 2 - animations["deathDown"].frameWidth / 2,
                        position.Y + height / 2 - animations["deathDown"].frameHeight / 2);
                    doPlayDeathAnimation = false;
                }
                if (animationManager.CurrentFrame >= animationManager.FrameCount - 1)
                {
                    isRemoved = true;
                }
                if (doPlayDeathSound)
                {
                    soundManager.playSoundRandomPitch(currentSoundKeys["deathSound"], -20, 20);
                    doPlayDeathSound = false;
                }
            }
        }
        private void respawn(float dt)
        {
            if (respawnTimer < respawnCooldown)
            {
                respawnTimer += dt;
            }
            if (respawnTimer >= respawnCooldown - (animations["spawn"].frameCount - 1) * animationManager.FrameSpeed && 
                respawnTimer < respawnCooldown)
            {
                if (type == "lask")
                {
                    position = new Vector2(startPos.X - 9, startPos.Y - 11);
                }
                else if (type == "tarlit")
                {
                    position = new Vector2(startPos.X - 25, startPos.Y - 3);
                }
                isRemoved = false;
                animationManager.Play(animations["spawn"]);
            }
            if (respawnTimer >= respawnCooldown)
            {
                respawnTimer = 0;
                position = startPos;
                health = maxHealth;
                canMove = true;
                isDead = false;
                isRemoved = false;
                canGiveSouls = true;
                doPlayDeathSound = true;
                doPlayDeathAnimation = true;
                canBeHit = true;
            }
        }
        private void showDamage(float dt)
        {
            if (color != Color.White)
            {
                showDamagedTimer += dt;
                if (showDamagedTimer >= showDamagedDuration)
                {
                    color = Color.White;
                    showDamagedTimer = 0f;
                }
            }
        }
        private void collideObjects(MapController mapController, List<Sprite> trees, Rectangle movingXRect, Rectangle movingYRect, 
            Player player, float dt)
        {
            Vector2 center = new Vector2(position.X + width / 2, position.Y + height / 2);

            foreach (Rectangle objectRect in mapController.objectRects)
            {
                if (collideLeft(movingXRect, objectRect) || collideRight(movingXRect, objectRect))
                {
                    velocity.X = 0;
                }
                if (collideTop(movingYRect, objectRect) || collideBottom(movingYRect, objectRect))
                {
                    velocity.Y = 0;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isRemoved)
            {
                if (texture != null)
                {
                    spriteBatch.Draw(texture, position, Color.White);
                }
                else if (animationManager != null)
                {
                    animationManager.Draw(spriteBatch);
                }
                else throw new Exception();
                if (health > 0 && doShowHealth)
                {
                    spriteBatch.Draw(healthBar,
                    new Vector2(position.X + width / 2 - healthBar.Width / 2, position.Y - 20),
                    new Rectangle((int)(position.X + width / 2 - shownMaxHealth / 2), 
                    (int)(position.Y - 20), shownHealth, healthBarHeight),
                    Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.65f);
                }
            }
        }
        public void Update(GameTime gameTime, Player player, List<Enemy> enemies, SoundManager soundManager, 
            MapController mapController, TreeController treeController)
        {
            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 center = new Vector2(position.X + width / 2, position.Y + height / 2);

            width = (int)(animationManager.FrameWidth * scale);
            height = (int)(animationManager.FrameHeight * scale);

            shownHealth = shownMaxHealth * health / maxHealth;

            showDamage(dt);
            death(mapController, soundManager, dt);
            if (!isDead && !mapController.isInScreenTransit)
            {
                if (getNest(center, mapController) == getNest(player.center, mapController)) 
                {
                    isSameNest = true;
                }
                if (Vector2.Distance(center, player.center) < range)
                {
                    inRange = true;
                }
                if ((isSameNest && getNest(player.position, mapController) != 0) || inRange)
                {
                    if (canMove)
                    {
                        move(player);
                        Rectangle movingXRect = new Rectangle((int)(position.X + velocity.X), (int)(position.Y), width, height);
                        Rectangle movingYRect = new Rectangle((int)(position.X), (int)(position.Y + velocity.Y), width, height);
                        //collideObjects(mapController, treeController.trees, movingXRect, movingYRect, player, dt);
                    }
                    animationManager.FrameSpeed = 0.15f;
                    collideWithEnemies(enemies, dt);
                }
                else
                {
                    animationManager.FrameSpeed = 0.2f;
                }
                if (attackTimer <= 0)
                {
                    attackPlayer(player);
                }
                if (animationManager != null)
                {
                    setAnimation(center, player);
                }
                position += velocity * speed * dt;
                velocity = Vector2.Zero;
                if (attackTimer > 0)
                {
                    attackTimer -= dt;
                }
                inRange = false;
                isSameNest = false;
            }
            else if (isDead)
            {
                respawn(dt);
            }
            if (doRotate)
            {
                if (rotation == 360)
                {
                    rotation = 0;
                }
                rotation += rotationSpeed;
            }
            if (!isRemoved)
            {
                animationManager.Color = color;
                animationManager.Rotation = rotation;
                animationManager.Origin = origin;
                animationManager.Scale = scale;
                animationManager.Update(gameTime);
            }
        }
    }
}
