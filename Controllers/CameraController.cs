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
    public class CameraController
    {
        private Camera camera;
        private float dt;
        private float cutsceneLerpAmount;
        private float cutsceneCameraSpeed = 0.3f;

        public bool doCenterPlayer = true;
        public bool doPlayCutscene;
        public Vector2 cutsceneStartPos;
        public Vector2 cutsceneTargetPos;
        public bool doMoveCutsceneCamera;
        public bool hasReachedCutsceneTarget;
        public bool doCutsceneReturn;
        public Vector2 cameraOffset;
        public static float cameraShakeTimer;
        public static float cameraShakeAmount = 10;
        public static bool doShakeCamera = true;
        public Vector2 position
        {
            get { return camera.Position; }
            set { camera.Position = value; }
        }
        public float width
        {
            get { return camera.Width; }
        }
        public float height
        {
            get { return camera.Height; }
        }
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), (int)width, (int)height); }
        }
        public CameraController(Camera camera)
        {
            this.camera = camera;
        }
        private void shakeCamera(float dt)
        {
            cameraOffset = Vector2.Zero;
            if (cameraShakeTimer > 0)
            {
                Random rand = new Random();

                cameraOffset = new Vector2((float)rand.NextDouble() * cameraShakeAmount, (float)rand.NextDouble() * cameraShakeAmount);
                switch (rand.Next(0, 3))
                {
                    case 0:
                        cameraOffset.X *= -1;
                        break;
                    case 1:
                        cameraOffset.Y *= -1;
                        break;
                    case 2:
                        cameraOffset.X *= -1;
                        cameraOffset.Y *= -1;
                        break;
                }

                cameraShakeTimer -= dt;
            }
        }
        private void cutscene(Vector2 startPos, Vector2 targetPos, float speed, Player player, Boss boss, MapController mapController, 
            SoundManager soundManager)
        {
            doCenterPlayer = false;
            camera.Position = Vector2.Lerp(startPos, targetPos, cutsceneLerpAmount);
            if (cutsceneLerpAmount < 1 && doMoveCutsceneCamera)
            {
                cutsceneLerpAmount += speed * dt;
            }
            if (cutsceneLerpAmount >= 1)
            {
                if (!doCutsceneReturn)
                {
                    hasReachedCutsceneTarget = true;
                    doMoveCutsceneCamera = false;
                    cutsceneStartPos = targetPos;
                    cutsceneTargetPos = startPos;
                }
                else
                {
                    doCutsceneReturn = false;
                    doPlayCutscene = false;
                    doCenterPlayer = true;
                    player.canMove = true;
                    player.canAttack = true;
                    if (mapController.currentMap == Map.temple && !player.hasMetBossCurrentRun)
                    {
                        boss.doAttack = true;
                        player.hasMetBossCurrentRun = true;
                    }
                    if (player.hasDefeatedBossCurrentRun)
                    {
                        if (player.bossDefeatCount < 7)
                        {
                            mapController.doUseWhiteScreen = true;
                        }
                        else
                        {
                            boss.doFade = true;
                        }
                    }
                }
                cutsceneLerpAmount = 0;
            }
        }
        public void Update(GameTime gameTime, Player player, MapController mapController, Boss boss, SoundManager soundManager)
        {
            dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (doCenterPlayer)
            {
                camera.Position = player.center;
            }
            if (doPlayCutscene)
            {
                cutscene(cutsceneStartPos, cutsceneTargetPos, cutsceneCameraSpeed, player, boss, mapController, soundManager);
            }
            if (camera.Position.Y > mapController.bottomScreenBound - camera.Height / 2)
            {
                camera.Position = new Vector2(camera.Position.X, mapController.bottomScreenBound - camera.Height / 2);
            }
            if (camera.Position.Y < mapController.topScreenBound + camera.Height / 2)
            {
                camera.Position = new Vector2(camera.Position.X, mapController.topScreenBound + camera.Height / 2);
            }
            if (camera.Position.X > mapController.rightScreenBound - camera.Width / 2)
            {
                camera.Position = new Vector2(mapController.rightScreenBound - camera.Width / 2, camera.Position.Y);
            }
            if (camera.Position.X < mapController.leftScreenBound + camera.Width / 2)
            {
                camera.Position = new Vector2(mapController.leftScreenBound + camera.Width / 2, camera.Position.Y);
            }
            if (camera.Width > mapController.rightScreenBound)
            {
                camera.Position = new Vector2(mapController.currentBackground.Width / 2, camera.Position.Y);
            }
            if (camera.Height > mapController.bottomScreenBound)
            {
                camera.Position = new Vector2(camera.Position.X, mapController.currentBackground.Height / 2);
            }
            if (doShakeCamera)
            {
                shakeCamera(dt);
            }
            camera.Position += cameraOffset;
            
            camera.Update(gameTime);
        }
    }
}
