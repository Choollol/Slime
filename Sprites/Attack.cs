using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slime.Models;
using Slime.Managers;
using System.Diagnostics;
using Slime.Controllers;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Slime.Sprites
{
    public class Attack : Sprite
    {
        private Random rand = new Random();
        private bool canDamageEnemy = true;
        private Dictionary<string, Animation> currentAnimations = new Dictionary<string, Animation>();
        private Animation animation;
        private Animation explosionAnimation;
        private int damage;

        public string currentAttack = "normal";
        public bool doPlayExplosion = false;
        public bool doPlayAttackSound = true;
        public Attack(Animation animation, Animation explosionAnimation, Direction direction, float speed, Player player, 
            string currentAttack, int damage)
        {
            this.animation = animation;
            this.explosionAnimation = explosionAnimation;
            animationManager = new AnimationManager(animation);
            animationManager.IsLooping = false;
            this.currentAttack = currentAttack;
            this.damage = damage;

            if (direction == Direction.Up)
            {
                position = new Vector2(player.position.X + (player.width - animationManager.FrameWidth) / 2,
                    player.position.Y - animationManager.FrameHeight);
                velocity.X = 0f;
                velocity.Y = -speed;
            }
            else if (direction == Direction.Down)
            {
                position = new Vector2(player.position.X + (player.width - animationManager.FrameWidth) / 2,
                    player.position.Y + player.height);
                velocity.X = 0f;
                velocity.Y = speed;
            }
            else if (direction == Direction.Left)
            {
                position = new Vector2(player.position.X - animationManager.FrameWidth,
                    player.position.Y);
                velocity.X = -speed;
                velocity.Y = 0f;
            }
            else if (direction == Direction.Right)
            {
                position = new Vector2(player.position.X + player.width,
                    player.position.Y);
                velocity.X = speed;
                velocity.Y = 0f;
            }
            animationManager.Play(animation);
        }
        private void hitEnemy(List<Enemy> enemies, Player player, Vector2 attackCenter,
            SoundManager soundManager, Boss boss, MapController mapController)
        {
            soundManager.enemyHitCount = -1;
            if (mapController.currentMap != Map.temple)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (!enemy.isRemoved && !isRemoved && (Rectangle.Intersects(enemy.Rectangle) || (player.Rectangle.Intersects(enemy.Rectangle) &&
                        Vector2.Distance(player.center, attackCenter) < 120 &&
                        ((player.Direction == Direction.Up && enemy.Rectangle.Top < player.Rectangle.Top + 10) ||
                        (player.Direction == Direction.Down && enemy.Rectangle.Bottom > player.center.Y - 10) ||
                        (player.Direction == Direction.Left && enemy.Rectangle.Left < player.Rectangle.Left + 10) ||
                        (player.Direction == Direction.Right && enemy.Rectangle.Right > player.center.X - 10)))))
                    {
                        if (enemy.canBeHit)
                        {
                            enemy.health -= damage;
                            enemy.canBeHit = false;
                            if (enemy.health > 0)
                            {
                                soundManager.addEnemyHit();
                                enemy.color = Color.Crimson;
                            }
                        }
                        if (currentAttack == "fireball" && !doPlayExplosion && !enemy.isDead)
                        {
                            animationManager.CurrentFrame = animationManager.FrameCount - 1;
                        }
                    }
                }
            }
            else
            {
                if (Rectangle.Intersects(boss.Rectangle) && boss.health > 0 && !boss.isRemoved)
                {
                    if (canDamageEnemy)
                    {
                        boss.health -= damage;
                        canDamageEnemy = false;
                    }
                    if (currentAttack == "fireball" && !doPlayExplosion)
                    {
                        animationManager.CurrentFrame = animationManager.FrameCount - 1;
                    }
                }
                foreach (HealingCrystal healingCrystal in boss.healingCrystals)
                {
                    if (Rectangle.Intersects(healingCrystal.Rectangle))
                    {
                        if (healingCrystal.canBeHit && healingCrystal.doHeal)
                        {
                            healingCrystal.health -= damage;
                            if (healingCrystal.health > 0)
                            {
                                soundManager.playSoundRandomPitch("healingCrystalHit", -30, 30);
                            }
                            healingCrystal.canBeHit = false;
                        }
                        if (currentAttack == "fireball" && !doPlayExplosion && healingCrystal.doHeal)
                        {
                            animationManager.CurrentFrame = animationManager.FrameCount - 1;
                        }
                    }
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
        }
        public void Update(GameTime gameTime, Player player, List<Enemy> enemies, SoundManager soundManager, Boss boss,
            MapController mapController)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kState = Keyboard.GetState();
            Vector2 attackCenter = new Vector2(position.X + width / 2, position.Y + height / 2);
            width = (int)(animationManager.FrameWidth * scale);
            height = (int)(animationManager.FrameHeight * scale);

            animationManager.FrameSpeed = 0.05f;

            if (currentAttack == "fireball")
            {
                if (doPlayExplosion)
                {
                    animationManager.FrameSpeed = 0.0005f;
                    int rotate = (int)rand.Next(0, 3);
                    switch (rotate)
                    {
                        case 1:
                            animationManager.SpriteEffect = SpriteEffects.FlipVertically;
                            break;
                        case 2:
                            animationManager.SpriteEffect = SpriteEffects.FlipHorizontally;
                            break;
                    }
                }
            }
            animationManager.Depth = 0.8f;

            position += velocity * dt;
            if (doPlayAttackSound)
            {
                if (currentAttack == "normal")
                {
                    soundManager.playSound("normalAttackWhoosh");
                }
                else if (currentAttack == "fireball" && !doPlayExplosion)
                {
                    soundManager.playSoundInstance("fireballWhoosh");
                }
                doPlayAttackSound = false;
            }
            if (animationManager.CurrentFrame >= animationManager.FrameCount)
            {
                velocity = Vector2.Zero;
                if (currentAttack == "normal")
                {
                    isRemoved = true;
                }
                else if (currentAttack == "fireball")
                {
                    position = new Vector2(
                            position.X - explosionAnimation.frameWidth / 2 + animation.frameWidth / 2,
                            position.Y - explosionAnimation.frameHeight / 2 + animation.frameHeight / 2);
                    velocity = Vector2.Zero;
                    if (!doPlayExplosion)
                    {
                        CameraController.cameraShakeTimer = 0.15f;
                        CameraController.cameraShakeAmount = 4;
                        soundManager.stopSoundInstance("fireballWhoosh");
                        soundManager.playSoundRandomPitch("explosionSoundEffect", -20, 20);
                        doPlayExplosion = true;
                        animationManager.Play(explosionAnimation);
                        animationManager.IsLooping = false;
                        animationManager.IsDraw = true;
                        canDamageEnemy = true;
                        foreach (Enemy enemy in enemies)
                        {
                            enemy.canBeHit = true;
                        }
                        if (mapController.currentMap == Map.temple)
                        {
                            foreach (HealingCrystal healingCrystal in boss.healingCrystals)
                            {
                                healingCrystal.canBeHit = true;
                            }
                        }
                    }
                    else if (doPlayExplosion)
                    {
                        isRemoved = true;
                    }
                }
            }
            hitEnemy(enemies, player, attackCenter, soundManager, boss, mapController);
            animationManager.Update(gameTime);
        }

    }
}
