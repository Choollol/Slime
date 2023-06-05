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
using Transform;

namespace Slime.Sprites
{
    public class Projectile : Sprite
    {
        private float lifeTime;
        private float timer;
        private float projectileSpeed;
        private Vector2 targetPos;
        private Vector2 startPos;
        private int damage;
        private float rotationSpeed;
        private bool doMove = true;
        private float targetScale;
        private bool doGrow = false;

        public float growSpeed;
        public bool doDestroyOnTargetReach = false;
        public bool doHitPlayer = true;
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), width, height); }
        }
        public Projectile(Texture2D newTexture, Vector2 startPosition, Vector2 targetPosition, int enemyDamage, 
            float projectileSpeed, float lifeTime, float rotationSpeed)
        {
            texture = newTexture;
            targetPos = targetPosition;
            startPos = startPosition;
            position = startPos;
            damage = enemyDamage;
            timer = lifeTime;
            this.lifeTime = lifeTime;
            this.projectileSpeed = projectileSpeed;
            this.rotationSpeed = rotationSpeed;
            depth = 0.65f;
        }
        public Projectile(Texture2D newTexture, Vector2 startPosition, Vector2 targetPosition, int enemyDamage,
            float projectileSpeed, float lifeTime, float rotationSpeed, float scale, float growSpeed)
        {
            texture = newTexture;
            targetPos = targetPosition;
            startPos = startPosition;
            position = startPos;
            damage = enemyDamage;
            timer = lifeTime;
            this.lifeTime = lifeTime;
            this.projectileSpeed = projectileSpeed;
            this.rotationSpeed = rotationSpeed;
            depth = 0.65f;

            doGrow = true;
            doMove = false;
            targetScale = scale;
            this.scale = 0f;
            this.growSpeed = growSpeed;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, color * opacity, rotation, new Vector2(texture.Width / 2, texture.Height / 2), 
                scale, spriteEffect, depth);
        }
        public void Update(GameTime gameTime, Player player) 
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            width = (int)(texture.Width * scale);
            height = (int)(texture.Height * scale);
            if (doGrow)
            {
                if (scale < targetScale)
                {
                    scale += dt * growSpeed;
                }
                else
                {
                    doMove = true;
                    doGrow = false;
                }
            }
            if (doMove)
            {
                if (this.Rectangle.Intersects(player.Rectangle) && player.canBeDamaged && doHitPlayer)
                {
                    player.health -= damage;
                    isRemoved = true;
                }
                position = Vector2.Lerp(startPos, targetPos, (1f - timer / lifeTime) * projectileSpeed /
                    Vector2.Distance(startPos, targetPos) * 100);
                if (timer <= 0f || (doDestroyOnTargetReach && position == targetPos))
                {
                    isRemoved = true;
                }

                timer -= dt;
            }
            rotation += rotationSpeed * dt;
        }
    }
}
