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

namespace Slime.Sprites
{
    public class Player : Sprite
    {
        private AnimationManager animationManager;
        private Dictionary<string, Animation> animations;
        private Vector2 pos;
        private Texture2D texture;
        private KeyboardState kStateOld;
        private MouseState mStateOld;
        private bool moveUp;
        private bool moveDown;
        private bool moveLeft;
        private bool moveRight;
        private float regenTimer = 0;
        private List<Sprite> trees;
        private MapController mapController;
        private float damagedDuration = 50f;
        private float damagedTimer = 0f;
        private int shownMaxHealth = 50;
        private int shownHealth = 50;
        private int oldHealth;

        public Input input;
        public Vector2 position
        {
            get { return pos; }
            set
            {
                pos = value;
                if (animationManager != null)
                {
                    animationManager.position = pos;
                }
            }
        }
        public float speed = 200f;
        public int width = 70;
        public int height = 64;
        public int maxHealth = 50;
        public int health = 50;
        public Color[] healthData;
        public int healthBarHeight = 10;
        public Texture2D healthBar;
        public bool canBeDamaged = true;
        public int regenerationAmount = 0;
        public GraphicsDeviceManager graphics;
        public bool canMove = true;
        public bool canAttack = true;
        public bool hasMetWitch = false;
        public bool isGhost = false;
        public Direction dir;
        public int playerTopBound;
        public int playerBottomBound;
        public int playerLeftBound;
        public int playerRightBound;
        public Vector2 center;
        public bool doPlayDeathSound = true;
        public bool hasMetBossCurrentRun = false;
        public bool hasDefeatedBossCurrentRun = false;
        public int bossDefeatCount = 0;
        public bool doMouseDirect = false;
        public Player(Dictionary<string, Animation> anims, GraphicsDeviceManager graphics, List<Sprite> trees, 
            MapController newMapController)
        {
            animations = anims;
            animationManager = new AnimationManager(animations.First().Value);

            healthData = new Color[health * healthBarHeight];
            for (int i = 0; i < health * healthBarHeight; i++)
            {
                healthData[i] = Color.Green;
            }
            healthBar = new Texture2D(graphics.GraphicsDevice, health, healthBarHeight);
            healthBar.SetData(healthData);
            this.trees = trees;
            mapController = newMapController;
            animationManager.Depth = 0.5f;
            oldHealth = health;
        }
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)position.X, (int)position.Y, width, height); }
        }

        public Vector2 velocity;
        public bool isAttacking = false;
        public float FrameSpeed
        {
            get { return animationManager.FrameSpeed; }
            set { animationManager.FrameSpeed = value; }
        }
        public Direction Direction
        {
            get { return dir; }
            set { dir = value; }
        }
        private void setBounds(MapController mapController)
        {
            playerTopBound = mapController.topScreenBound;
            playerBottomBound = mapController.bottomScreenBound;
            playerLeftBound = mapController.leftScreenBound;
            playerRightBound = mapController.rightScreenBound;

            if (mapController.currentMap == Map.cave1)
            {
                playerLeftBound = mapController.leftScreenBound + 40;
                playerRightBound = mapController.rightScreenBound - 40;
            }
            else if (mapController.currentMap == Map.cave2)
            {
                playerLeftBound = mapController.leftScreenBound + 50;
                playerRightBound = mapController.rightScreenBound - 50;
            }
            else if (mapController.currentMap == Map.cave3)
            {
                playerLeftBound = mapController.leftScreenBound + 97;
                playerRightBound = mapController.rightScreenBound - 97;
                playerTopBound = mapController.topScreenBound + 97;
            }
            else if (mapController.currentMap == Map.cliff)
            {
                if (position.Y > 810)
                {
                    playerRightBound = 483;
                }
                else if (position.Y > 532)
                {
                    playerRightBound = 463;
                }
                else if (position.Y > 62)
                {
                    playerRightBound = 459;
                }
                else
                {
                    playerRightBound = 404;
                }
                if (position.Y > 640)
                {
                    playerLeftBound = 0;
                }
                else if (position.Y > 40)
                {
                    playerLeftBound = 24;
                }
                else
                {
                    playerLeftBound = 74;
                }
                playerTopBound = -20;
            }
            else if (mapController.currentMap == Map.templePath)
            {
                playerTopBound = 0;
            }
            if (position.X > playerRightBound - width)
            {
                position = new Vector2(playerRightBound - width, position.Y);
            }
            else if (position.X < playerLeftBound)
            {
                position = new Vector2(playerLeftBound, position.Y);
            }
        }
        private void move(GameTime gameTime, KeyboardState kState)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float tempSpeed = speed;

            if (input == null)
            {
                return;
            }
            if (kState.IsKeyDown(input.moveUp) && moveUp)
            {
                dir = Direction.Up;
                if (kState.IsKeyDown(input.moveLeft) || kState.IsKeyDown(input.moveRight))
                {
                    tempSpeed = (float)(speed * 0.70710678118);
                }
                velocity.Y = -tempSpeed * dt;
                if (position.Y + velocity.Y < playerTopBound)
                {
                    velocity.Y = 0;
                }
                moveDown = false;
            }
            else
            {
                moveDown = true;
            }
            if (kState.IsKeyDown(input.moveDown) && moveDown)
            {
                dir = Direction.Down;
                if (kState.IsKeyDown(input.moveLeft) || kState.IsKeyDown(input.moveRight))
                {
                    tempSpeed = (float)(speed * 0.70710678118);
                }
                velocity.Y = tempSpeed * dt;
                if (position.Y + velocity.Y > playerBottomBound - height)
                {
                    velocity.Y = 0;
                }
                moveUp = false;
            }
            else
            {
                moveUp = true;
            }
            if (kState.IsKeyDown(input.moveLeft) && moveLeft)
            {
                dir = Direction.Left;
                if (kState.IsKeyDown(input.moveUp) || kState.IsKeyDown(input.moveDown))
                {
                    tempSpeed = (float)(speed * 0.70710678118);
                }
                velocity.X = -tempSpeed * dt;
                if (position.X + velocity.X < playerLeftBound)
                {
                    velocity.X = 0;
                }
                moveRight = false;
            }
            else
            {
                moveRight = true;
            }
            if (kState.IsKeyDown(input.moveRight) && moveRight)
            {
                dir = Direction.Right;
                if (kState.IsKeyDown(input.moveUp) || kState.IsKeyDown(input.moveDown))
                {
                    tempSpeed = (float)(speed * 0.70710678118);
                }
                velocity.X = tempSpeed * dt;
                if (position.X + velocity.X > playerRightBound - width)
                {
                    velocity.X = 0;
                }
                moveLeft = false;
            }
            else
            {
                moveLeft = true;
            }
        }
        
        private void setAnimation()
        {
            if (dir == Direction.Left)
            {
                animationManager.Play(animations["moveLeft"]);
            }
            else if (dir == Direction.Right)
            {
                animationManager.Play(animations["moveRight"]);
            }
            else if (dir == Direction.Up)
            {
                animationManager.Play(animations["moveUp"]);
            }
            else if (dir == Direction.Down)
            {
                animationManager.Play(animations["moveDown"]);
            }
            if (velocity == Vector2.Zero)
            {
                animationManager.Stop();
            }

        }
        private void collideObjects(Rectangle movingXRect, Rectangle movingYRect, MapController mapController)
        {
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
        private void damaged(GameTime gameTime, SoundManager soundManager)
        {
            if (health < oldHealth)
            {
                canBeDamaged = false;
            }
            if (!canBeDamaged)
            {
                if (damagedTimer == 0)
                {
                    soundManager.playSound("slimeDamaged");
                }
                if (damagedTimer < 50)
                {
                    animationManager.Opacity = 0.4f;
                }
                else
                {
                    animationManager.Opacity = 1f;
                }
                if (damagedTimer < damagedDuration)
                {
                    damagedTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                else
                {
                    damagedTimer = 0f;
                    animationManager.Opacity = 1f;
                    canBeDamaged = true;
                }
            }
        }
        private void death(MapController mapController, CurrencyController currencyController, SoundManager soundManager)
        {
            if (health <= 0)
            {
                mapController.isInScreenTransit = true;
                if (hasMetWitch)
                {
                    mapController.newMap = Map.witchHut;
                    mapController.newPlayerPos = mapController.summoningCirclePos + new Vector2(5, 5);
                    mapController.newPlayerDir = Direction.Down;
                }
                else
                {
                    mapController.newMap = Map.forest;
                    mapController.newPlayerPos = new Vector2(2000 - width / 2, 4000 - height);
                    mapController.newPlayerDir = Direction.Up;
                }
                mapController.doHealPlayer = true;
                mapController.currencyDeductAmount = currencyController.souls;
                if (doPlayDeathSound)
                {
                    soundManager.playSound("playerDeath");
                    doPlayDeathSound = false;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
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
            if (health > 0)
            {
                spriteBatch.Draw(healthBar,
                new Vector2(position.X + width / 2 - shownMaxHealth / 2, position.Y - 20),
                new Rectangle((int)(position.X + width / 2 - shownMaxHealth / 2), (int)(position.Y - 20), shownHealth, healthBarHeight),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.65f);
            }
        }
        public void Update(GameTime gameTime, MapController mapController, SoundManager soundManager, 
            CurrencyController currencyController, AttackController attackController, CameraController cameraController)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            Rectangle mRect = new Rectangle((int)(cameraController.position.X - cameraController.width / 2 + mState.Position.X),
                (int)(cameraController.position.Y - cameraController.height / 2 + mState.Position.Y), 1, 1);
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            regenTimer += dt;
            animationManager.Color = color;
            damaged(gameTime, soundManager);
            death(mapController, currencyController, soundManager);
            if (regenTimer > 1 && health < maxHealth && health > 0)
            {
                health += regenerationAmount;
                regenTimer = 0;
            }
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            shownHealth = shownMaxHealth * health / maxHealth;

            if (canMove)
            {
                setBounds(mapController);
                move(gameTime, kState);
            }
            if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released && 
                canAttack && attackController.timer <= 0.1f)
            {
                if (doMouseDirect)
                {
                    float mouseDistX = mRect.X - center.X;
                    float mouseDistY = mRect.Y - center.Y;
                    if (Math.Abs(mouseDistX) >= Math.Abs(mouseDistY))
                    {
                        if (mouseDistX < 0)
                        {
                            dir = Direction.Left;
                        }
                        else
                        {
                            dir = Direction.Right;
                        }
                    }
                    else
                    {
                        if (mouseDistY < 0)
                        {
                            dir = Direction.Up;
                        }
                        else
                        {
                            dir = Direction.Down;
                        }
                    }
                }
                isAttacking = true;
            }
            animationManager.FrameSpeed = 0.1f;
            if (animationManager != null)
            {
                setAnimation();
                animationManager.Update(gameTime);
            }
            Rectangle movingXRect = new Rectangle((int)(position.X + velocity.X), (int)(position.Y), width, height);
            Rectangle movingYRect = new Rectangle((int)(position.X), (int)(position.Y + velocity.Y), width, height);
            if (!isGhost)
            {
                collideObjects(movingXRect, movingYRect, mapController);
            }
            if (velocity.X != 0 || velocity.Y != 0)
            {
                soundManager.playSoundInstance("slimeRustle");
            }
            else
            {
                soundManager.stopSoundInstance("slimeRustle");
            }
            position += velocity;
            velocity = Vector2.Zero;
            center = new Vector2(position.X + width / 2, position.Y + height / 2);
            kStateOld = kState;
            mStateOld = mState;
            oldHealth = health;
        }


    }
}
