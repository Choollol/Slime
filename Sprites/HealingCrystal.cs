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
using Slime.Controllers;

namespace Slime.Sprites
{
    public class HealingCrystal : Sprite
    {
        private Animation deathAnimation;
        private int healAmount;
        private float healCooldown;
        private float healTimer = 0;
        private float riseSpeed;
        private bool doPlayDeath = true;

        public int health;
        public bool doHeal = false;
        public bool canBeHit = true;

        public HealingCrystal(Texture2D texture, Animation deathAnimation, int healAmount, int health, float riseSpeed, float healCooldown)
        {
            this.texture = texture;
            this.deathAnimation = deathAnimation;
            animationManager = new AnimationManager(deathAnimation);
            animationManager.IsLooping = false;
            animationManager.FrameSpeed = 0.1f;
            this.healAmount = healAmount;
            width = texture.Width;
            height = texture.Height;
            sourceRect = new Rectangle(0, 0, width, 0);
            this.health = health;
            this.riseSpeed = riseSpeed;
            this.healCooldown = healCooldown;
            useSourceRect = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (health > 0)
            {
                spriteBatch.Draw(texture, position, sourceRect, color * opacity, rotation, Vector2.Zero, scale, spriteEffect, depth);
            }
            else
            {
                animationManager.Draw(spriteBatch);
            }
        }
        public void Update(GameTime gameTime, float dt, Boss boss, SoundManager soundManager) 
        {
            if (sourceRect.Height < height)
            {
                sourceRect.Height += (int)(riseSpeed * dt);
                position -= new Vector2(0, riseSpeed * dt);
                CameraController.cameraShakeTimer = 0.1f;
                CameraController.cameraShakeAmount  = 5f;
            }
            else
            {
                doHeal = true;
            }
            if (doHeal)
            {
                if (healTimer <= 0 && boss.health < boss.maxHealth)
                {
                    boss.health += healAmount;
                    healTimer = healCooldown;
                }
                else if (healTimer > 0)
                {
                    healTimer -= dt;
                }
            }
            if (health <= 0)
            {
                if (doPlayDeath)
                {
                    animationManager.CurrentFrame = 0;
                    soundManager.playSoundRandomPitch("healingCrystalShatter", -30, 30);
                    animationManager.Play(deathAnimation);
                    position -= new Vector2(deathAnimation.frameWidth / 2 - texture.Width / 2, 0);
                    CameraController.cameraShakeTimer = 0.2f;
                    CameraController.cameraShakeAmount = 3f;
                    doPlayDeath = false;
                }
                if (animationManager.CurrentFrame > animationManager.FrameCount)
                {
                    isRemoved = true;
                }
                animationManager.Update(gameTime);
            }
        }
    }
}
