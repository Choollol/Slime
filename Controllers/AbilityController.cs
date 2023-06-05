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
using Slime.Sprites;
using System.Threading;
using Comora;

namespace Slime.Controllers
{
    public class AbilityController
    {
        private KeyboardState kStateOld;
        private MouseState mStateOld;
        private Dictionary<string, Animation> animations;
        private Vector2 position;
        private AnimationManager animationManager;
        private bool doUpdateAnimationManager = false;
        private bool isTeleporting = false;
        private bool doTeleportPlayer = false;
        private float timeStoppedDuration = 5f;
        private float timeStoppedTimer = 0f;

        public bool canUseAbilities = true;
        public bool isTeleportUnlocked = false;
        public float teleportCooldown = 5f;
        public float teleportTimer = 0f;
        public bool isTimeStopUnlocked = false;
        public bool isTimeStopped = false;
        public float timeStopCooldown = 30f;
        public float timeStopTimer = 0f;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, animationManager.FrameWidth, animationManager.FrameHeight);
            }
        }
        public AbilityController(Dictionary<string, Animation> anims)
        {
            animations = anims;
            animationManager = new AnimationManager(animations.First().Value);
            animationManager.Depth = 0.6f;
            animationManager.IsLooping = false;
        }

        private void teleport(float dt, KeyboardState kState, Player player, Camera camera, MapController mapController, 
            CurrencyController currencyController, SoundManager soundManager, Boss boss)
        {
            float teleportDistance = 240f;
            animationManager.FrameSpeed = 0.05f;

            if (!isTeleporting)
            {
                animationManager.position = position;
            }
            if (teleportTimer > 0)
            {
                teleportTimer -= dt;
            }
            if (isTeleportUnlocked)
            {
                if (kState.IsKeyDown(player.input.teleport) && kStateOld.IsKeyUp(player.input.teleport) &&
                    teleportTimer <= 0)
                {
                    animationManager.Play(animations["teleport"]);
                    isTeleporting = true;
                    teleportTimer = teleportCooldown;
                    doTeleportPlayer = true;
                    doUpdateAnimationManager = true;
                    soundManager.playSound("teleportSoundEffect");
                }
                if (kState.IsKeyDown(player.input.teleportHome) && kStateOld.IsKeyUp(player.input.teleportHome) && 
                    (mapController.currentMap != Map.temple || boss.isRemoved))
                {
                    mapController.newMap = Map.witchHut;
                    mapController.newPlayerPos = mapController.summoningCirclePlayerPos;
                    mapController.newPlayerDir = Direction.Down;
                    soundManager.playSound("teleportSoundEffect");
                }
            }
            if (isTeleporting)
            {
                animationManager.IsDraw = true;
                if (doTeleportPlayer)
                {
                    switch (player.Direction)
                    {
                        case Direction.Up:
                            currencyController.soulOrbPos.Y -= teleportDistance;
                            player.position -= new Vector2(0, teleportDistance);
                            if (player.position.Y <= player.playerTopBound)
                            {
                                player.position = new Vector2(player.position.X, player.playerTopBound);
                            }
                            if (camera.Height < mapController.bottomScreenBound)
                            {
                                camera.Position -= new Vector2(0, teleportDistance);
                            }
                            break;
                        case Direction.Down:
                            currencyController.soulOrbPos.Y += teleportDistance;
                            player.position += new Vector2(0, teleportDistance);
                            if (player.position.Y + player.height >= player.playerBottomBound)
                            {
                                player.position = new Vector2(player.position.X, player.playerBottomBound - player.height);
                            }
                            if (camera.Height < mapController.bottomScreenBound)
                            {
                                camera.Position += new Vector2(0, teleportDistance);
                            }
                            break;
                        case Direction.Left:
                            currencyController.soulOrbPos.X -= teleportDistance;
                            player.position -= new Vector2(teleportDistance, 0);
                            if (player.position.X <= player.playerLeftBound)
                            {
                                player.position = new Vector2(player.playerLeftBound, player.position.Y);
                            }
                            if (camera.Width < mapController.rightScreenBound)
                            {
                                camera.Position -= new Vector2(teleportDistance, 0);
                            }
                            break;
                        case Direction.Right:
                            currencyController.soulOrbPos.X += teleportDistance;
                            player.position += new Vector2(teleportDistance, 0);
                            if (player.position.X + player.width >= player.playerRightBound)
                            {
                                player.position = new Vector2(player.playerRightBound - player.width, player.position.Y);
                            }
                            if (camera.Width < mapController.rightScreenBound)
                            {
                                camera.Position += new Vector2(teleportDistance, 0);
                            }
                            break;
                    }
                    doTeleportPlayer = false;
                }
                if (animationManager.CurrentFrame >= animationManager.FrameCount)
                {
                    animationManager.Stop();
                    isTeleporting = false;
                    animationManager.IsDraw = false;
                    doUpdateAnimationManager = false;
                }
            }
        }
        private void timeStop(KeyboardState kState, float dt, SoundManager soundManager, Player player, MapController mapController)
        {
            if (isTimeStopUnlocked && kState.IsKeyDown(player.input.stopTime) && kStateOld.IsKeyUp(player.input.stopTime) && timeStopTimer <= 0)
            {
                isTimeStopped = true;
                timeStoppedTimer = timeStoppedDuration;
                if (!mapController.doOpenTempleDoor)
                {
                    mapController.doOpenTempleDoor = true;
                }
            }
            if (timeStopTimer > 0)
            {
                timeStopTimer -= dt;
            }
            if (isTimeStopped)
            {
                if (timeStoppedTimer == timeStoppedDuration)
                {
                    soundManager.playSoundInstance("timeStopTick");
                }
                if (timeStoppedTimer > 0)
                {
                    timeStoppedTimer -= dt;
                }
                if (timeStoppedTimer <= 0 || kState.IsKeyDown(player.input.startTime))
                {
                    isTimeStopped = false;
                    timeStopTimer = timeStopCooldown;
                    mapController.doOpenTempleDoor = false;
                    soundManager.stopSoundInstance("timeStopTick");
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isTeleporting)
            {
                if (animationManager != null)
                {
                    animationManager.Draw(spriteBatch);
                }
                else throw new Exception();
            }

        }
        public void Update(GameTime gameTime, Player player, Camera camera, MapController mapController, 
            CurrencyController currencyController, SoundManager soundManager, Boss boss)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position = new Vector2(player.position.X - (animationManager.FrameWidth - player.width) / 2, 
                player.position.Y - (animationManager.FrameHeight - player.height) / 2);

            if (canUseAbilities && player.canMove)
            {
                teleport(dt, kState, player, camera, mapController, currencyController, soundManager, boss);
                timeStop(kState, dt, soundManager, player, mapController);
            }

            if (doUpdateAnimationManager)
            {
                animationManager.Update(gameTime);
            }

            kStateOld = kState;
            mStateOld = mState;
        }
    }
}
