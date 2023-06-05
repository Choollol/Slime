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
using System.Text;
using Transform;

namespace Slime.Controllers
{
    public class AttackController
    {
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private Dictionary<string, Animation> currentAnimations = new Dictionary<string, Animation>();
        private Animation currentAnimation;

        public List<Attack> attacks = new List<Attack>();
        public float cooldown = 2f;
        public float timer;
        public string currentAttack = "normal";
        public int damage = 5;
        public float speed = 240f;

        public AttackController(Dictionary<string, Texture2D> textures)
        {
            this.textures = textures;
        }
        private void setCurrentAnimations(Player player)
        {
            if (currentAttack == "normal")
            {
                currentAnimations["up"] = new Animation(textures["attackUp"], 5);
                currentAnimations["down"] = new Animation(textures["attackDown"], 5);
                currentAnimations["left"] = new Animation(textures["attackLeft"], 5);
                currentAnimations["right"] = new Animation(textures["attackRight"], 5);
            }
            else if (currentAttack == "fireball")
            {
                currentAnimations["up"] = new Animation(textures["fireballUp"], 8);
                currentAnimations["down"] = new Animation(textures["fireballDown"], 8);
                currentAnimations["left"] = new Animation(textures["fireballLeft"], 8);
                currentAnimations["right"] = new Animation(textures["fireballRight"], 8);
            }
            if (player.Direction == Direction.Up)
            {
                currentAnimation = currentAnimations["up"];
            }
            else if (player.Direction == Direction.Down)
            {
                currentAnimation = currentAnimations["down"];
            }
            else if (player.Direction == Direction.Left)
            {
                currentAnimation = currentAnimations["left"];
            }
            else if (player.Direction == Direction.Right)
            {
                currentAnimation = currentAnimations["right"];
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Attack attack in attacks)
            {
                attack.Draw(spriteBatch);
            }
        }
        public void Update(GameTime gameTime, Player player, List<Enemy> enemies, SoundManager soundManager, Boss boss,
            MapController mapController, CameraController cameraController)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mState = Mouse.GetState();
            Rectangle mRect = new Rectangle((int)(cameraController.position.X - cameraController.width / 2), 
                (int)(cameraController.position.Y - cameraController.height / 2), 1, 1);

            setCurrentAnimations(player);
            if (currentAttack == "normal")
            {
                speed = 240;
            }
            else if (currentAttack == "fireball")
            {
                speed = 480;
            }
            if (timer <= 0 && player.isAttacking)
            { 
                Attack newAttack = new Attack(currentAnimation, new Animation(textures["explosion"], 14), player.dir, speed, player, currentAttack, damage);

                attacks.Add(newAttack);
                player.isAttacking = false;
                timer = cooldown;
            }
            foreach (Attack attack in attacks.ToArray())
            {
                if (attack.isRemoved)
                {
                    attacks.Remove(attack);
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
                else
                {
                    attack.Update(gameTime, player, enemies, soundManager, boss, mapController);
                }
            }
            if (timer > 0)
            {
                timer -= dt;
            }
        }
    }
}
