using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Comora;
using System.Collections.Generic;
using Slime.Sprites;
using Slime.Models;
using Slime.Managers;
using System.Diagnostics;
using System;

namespace Slime
{
    public enum Direction
    {
        Up, Down, Left, Right
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Cheat cheat;

        private Random rand;

        private SpriteFont spriteFont;

        private MapManager mapManager;
        private Texture2D screenTransition;
        private Color[] screenTransitionColor;

        private SoulManager soulManager;
        private ShopManager shopManager;
        private DialogueManager dialogueManager;

        private int cameraWidth = 960;
        private int cameraHeight = 540;

        private Texture2D slimeUp;
        private Texture2D slimeDown;
        private Texture2D slimeLeft;
        private Texture2D slimeRight;

        private Texture2D attackUp;
        private Texture2D attackDown;
        private Texture2D attackLeft;
        private Texture2D attackRight;

        private Texture2D soulOrb;
        private Vector2 soulOrbPos;
        private Texture2D dialogueBox;
        private Texture2D exitButton;

        private Texture2D shopBackground;
        private Texture2D shopItemLight;
        private Texture2D shopItemDark;
        private Texture2D shopItemLocked;
        private Texture2D itemDescriptionBackground;
        private Texture2D attackUpgradeIcon;
        private Texture2D healthUpgradeIcon;
        private Texture2D speedUpgradeIcon;

        private Texture2D forestBackground;
        private Texture2D witchHut;
        private Texture2D treeSprite;
        private Texture2D laskUp;
        private Texture2D laskDown;

        private Texture2D witchHutFloor;
        private Texture2D witchHutDoor;
        private Texture2D witch;

        private Player player;
        private Attack attack;

        private List<Enemy> enemies;
        Vector2[] laskSpawnpoints = { 
        //Bottom right nest
        new Vector2(2570, 2900), new Vector2(2740, 2920), new Vector2(2570, 3150), new Vector2(2700, 3220), 
        new Vector2(2800, 3210), new Vector2(3250, 2570), new Vector2(3400, 2600), new Vector2(3540, 2670), 
        new Vector2(3450, 2780), new Vector2(3200, 2700), new Vector2(3355, 2710), 
        //Bottom left nest
        new Vector2(570, 2450), new Vector2(430, 2600), new Vector2(470, 2780), new Vector2(565, 2900), 
        new Vector2(710, 3000), new Vector2(920, 2860), new Vector2(750, 2770), new Vector2(690, 2760),
        //Top left nest
        new Vector2(550, 265), new Vector2(665, 230), new Vector2(770, 255), new Vector2(880, 330),
        new Vector2(940, 425), new Vector2(935, 525), new Vector2(455, 465), new Vector2(600, 550),
        new Vector2(630, 350), new Vector2(765, 370), new Vector2(710, 450), new Vector2(680, 540),
        //Top right nest
        new Vector2(3370, 105), new Vector2(3500, 150), new Vector2(3675, 190), new Vector2(3745, 280),
        new Vector2(3835, 390), new Vector2(3850, 510), new Vector2(3795, 685), new Vector2(3780, 825),
        new Vector2(3170, 180), new Vector2(3070, 240), new Vector2(3030, 375), new Vector2(3010, 570),
        new Vector2(3040, 735), new Vector2(3280, 250), new Vector2(3510, 285), new Vector2(3600, 450),
        new Vector2(3275, 440), new Vector2(3500, 455), new Vector2(3610, 360), new Vector2(3175, 665),
        new Vector2(3615, 670), new Vector2(3355, 720), new Vector2(3605, 745),
        };


        private TreeManager treeManager;
        private List<Vector2> treePos;
        private Vector2 startPos;
        private int treeRowDist;
        private int treePerRow;

        private Camera camera;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920; graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = cameraWidth; graphics.PreferredBackBufferHeight = cameraHeight;
            graphics.ApplyChanges();

            this.camera = new Camera(graphics.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cheat = new Cheat();

            rand = new Random();

            spriteFont = Content.Load<SpriteFont>("spriteFont");

            screenTransition = new Texture2D(graphics.GraphicsDevice, cameraWidth, cameraHeight);
            screenTransitionColor = new Color[cameraWidth * cameraHeight];
            for (int i = 0; i < screenTransitionColor.Length; i++)
            {
                screenTransitionColor[i] = Color.Black;
            }
            screenTransition.SetData(screenTransitionColor);

            slimeUp = Content.Load<Texture2D>("Player/Slime_Up_Sheet");
            slimeDown = Content.Load<Texture2D>("Player/Slime_Down_Sheet");
            slimeLeft = Content.Load<Texture2D>("Player/Slime_Left_Sheet");
            slimeRight = Content.Load<Texture2D>("Player/Slime_Right_Sheet");

            attackUp = Content.Load<Texture2D>("Attacks/Attack_Up_Sheet");
            attackDown = Content.Load<Texture2D>("Attacks/Attack_Down_Sheet");
            attackLeft = Content.Load<Texture2D>("Attacks/Attack_Left_Sheet");
            attackRight = Content.Load<Texture2D>("Attacks/Attack_Right_Sheet");

            soulOrb = Content.Load<Texture2D>("Misc/SoulOrb");
            dialogueBox = Content.Load<Texture2D>("Misc/DialogueBox");
            exitButton = Content.Load<Texture2D>("Misc/ExitButton");

            shopBackground = Content.Load<Texture2D>("Shop/ShopBackground");
            shopItemLight = Content.Load<Texture2D>("Shop/ShopItem_Light");
            shopItemDark = Content.Load<Texture2D>("Shop/ShopItem_Dark");
            shopItemLocked = Content.Load<Texture2D>("Shop/ShopItem_Locked");
            itemDescriptionBackground = Content.Load<Texture2D>("Shop/ItemDescription_Background");
            attackUpgradeIcon = Content.Load<Texture2D>("Shop/AttackUpgrade_Icon");
            healthUpgradeIcon = Content.Load<Texture2D>("Shop/HealthUpgrade_Icon");
            speedUpgradeIcon = Content.Load<Texture2D>("Shop/SpeedUpgrade_Icon");

            forestBackground = Content.Load<Texture2D>("Forest/Forest");
            witchHut = Content.Load<Texture2D>("Forest/WitchHut");
            treeSprite = Content.Load<Texture2D>("Forest/Tree");
            laskUp = Content.Load<Texture2D>("Forest/Lask_Up_Sheet");
            laskDown = Content.Load<Texture2D>("Forest/Lask_Down_Sheet");

            witchHutFloor = Content.Load<Texture2D>("WitchHut/WitchHut_Floor");
            witchHutDoor = Content.Load<Texture2D>("WitchHut/WitchHut_Door");
            witch = Content.Load<Texture2D>("WitchHut/Witch");

            var mapSprites = new Dictionary<string, Texture2D>()
            {
                {"screenTransition", screenTransition },
                {"forest", forestBackground },
                {"witchHut", witchHut },
                {"witchHutFloor", witchHutFloor },
                {"witchHutDoor", witchHutDoor },
                {"witch", witch },
            };
            var shopSprites = new Dictionary<string, Texture2D>()
            {
                {"shopBackground", shopBackground },
                {"exitButton", exitButton },
                {"shopItemLight", shopItemLight },
                {"shopItemDark", shopItemDark },
                {"shopItemLocked", shopItemLocked },
                {"itemDescriptionBackground", itemDescriptionBackground },
                {"attackUpgradeIcon", attackUpgradeIcon },
                {"healthUpgradeIcon", healthUpgradeIcon },
                {"speedUpgradeIcon", speedUpgradeIcon },
            };
            var dialoguePortraitSprites = new Dictionary<string, Texture2D>()
            {
                {"witch", witch },
            };
            var playerAnimations = new Dictionary<string, Animation>()
            {
                {"moveUp", new Animation(slimeUp, 6) },
                {"moveDown", new Animation(slimeDown, 6) },
                {"moveLeft", new Animation(slimeLeft, 6) },
                {"moveRight", new Animation(slimeRight, 6) },
            };
            var attackAnimations = new Dictionary<string, Animation>()
            {
                {"up", new Animation(attackUp, 5) },
                {"down", new Animation(attackDown, 5) },
                {"left", new Animation(attackLeft, 5) },
                {"right", new Animation(attackRight, 5) },
            };

            mapManager = new MapManager(mapSprites);
            soulManager = new SoulManager();
            shopManager = new ShopManager(shopSprites, spriteFont);
            dialogueManager = new DialogueManager(dialogueBox, spriteFont, dialoguePortraitSprites);

            player = new Player(playerAnimations, this.graphics)
            {
                position = new Vector2(mapManager.rightScreenBound / 2, mapManager.bottomScreenBound - 70),
                input = new Input()
                {
                    Up = Keys.W,
                    Down = Keys.S,
                    Left = Keys.A,
                    Right = Keys.D,
                    Attack = Keys.Space,
                },
            };

            attack = new Attack(attackAnimations)
            {
                position = player.position
            };

            enemies = new List<Enemy>();
            int spawnPointsIndex = 0;
            foreach (Vector2 spawnpoint in laskSpawnpoints)
            {
                int addHealth;
                int addDamage;
                int addSpeed;
                if (spawnPointsIndex < 11)
                {
                    addHealth = 0;
                    addDamage = 0;
                    addSpeed = 0;
                }
                else if (spawnPointsIndex < 19)
                {
                    addHealth = 10;
                    addDamage = 5;
                    addSpeed = 10;
                }
                else if (spawnPointsIndex < 31)
                {
                    addHealth = 20;
                    addDamage = 10;
                    addSpeed = 20;
                }
                else
                {
                    addHealth = 30;
                    addDamage = 15;
                    addSpeed = 30;
                }
                var laskAnimations = new Dictionary<string, Animation>()
                {
                    {"moveDown", new Animation(laskDown, 8) },
                    {"moveUp", new Animation(laskUp, 8) },
                };
                Enemy enemy = new Enemy(laskAnimations, graphics)
                {
                    startPos = spawnpoint,
                    position = spawnpoint,
                };
                enemy.maxHealth += addHealth;
                enemy.damage += addDamage;
                enemy.speed += addSpeed;
                enemies.Add(enemy);
                spawnPointsIndex++;
            }

            treePos = new List<Vector2>();
            treeRowDist = 200;
            treePerRow = 24;
            for (int row = 0; row < 4000 / treeRowDist; row++)
            {
                startPos = new Vector2(-100, -300);
                for (int col = 0; col < treePerRow; col++)
                {
                    int xRand = rand.Next(150, 190);
                    int yRand = rand.Next(-40, 40);
                    if (row % 2 == 0)
                    {
                        if (col == 0)
                        {
                            treePos.Add(new Vector2(startPos.X, startPos.Y + yRand + row * treeRowDist));
                        }
                        else
                        {
                            treePos.Add(new Vector2(treePos[row * treePerRow + col - 1].X + xRand, 
                                startPos.Y + yRand + row * treeRowDist));
                        }
                    }
                    else
                    {
                        if (col == 0)
                        {
                            treePos.Add(new Vector2(startPos.X + 50, startPos.Y + yRand + row * treeRowDist));
                        }
                        else
                        {
                            treePos.Add(new Vector2(treePos[row * treePerRow + col - 1].X + xRand,
                                startPos.Y + yRand + row * treeRowDist));
                        }
                    }
                }
            }

            treeManager = new TreeManager(treeSprite, treePos);

        }

        protected override void Update(GameTime gameTime)
        {
            cheat.Update(gameTime, shopManager, soulManager, player, attack);

            this.camera.Position = new Vector2(player.position.X + player.width / 2, player.position.Y + player.height / 2);
            if (camera.Position.Y > mapManager.bottomScreenBound - cameraHeight / 2)
            {
                camera.Position = new Vector2(camera.Position.X, mapManager.bottomScreenBound - cameraHeight / 2);
            }
            if (camera.Position.Y < mapManager.topScreenBound + cameraHeight / 2)
            {
                camera.Position = new Vector2(camera.Position.X, mapManager.topScreenBound + cameraHeight / 2);
            }
            if (camera.Position.X > mapManager.rightScreenBound - cameraWidth / 2)
            {
                camera.Position = new Vector2(mapManager.rightScreenBound - cameraWidth / 2, camera.Position.Y);
            }
            if (camera.Position.X < mapManager.leftScreenBound + cameraWidth / 2)
            {
                camera.Position = new Vector2(mapManager.leftScreenBound + cameraWidth / 2, camera.Position.Y);
            }
            this.camera.Update(gameTime);
            player.Update(gameTime, treeManager.trees, mapManager);
            attack.Update(gameTime, player, enemies);
            if (mapManager.currentMap != Map.witchHut)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.Update(gameTime, player, enemies);
                }
            }
            if (mapManager.currentMap == Map.forest)
            {
                treeManager.Update(gameTime, player);
            }
            mapManager.Update(gameTime, player, dialogueManager);

            soulManager.Update(gameTime, enemies, player);

            soulOrbPos = new Vector2(camera.Position.X - cameraWidth / 2 + 20, camera.Position.Y - cameraHeight / 2 + 20);

            dialogueManager.Update(gameTime, player, mapManager, shopManager);
            
            shopManager.Update(gameTime, player, mapManager, dialogueManager, soulManager, camera, attack);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(this.camera, SpriteSortMode.FrontToBack);

            mapManager.Draw(spriteBatch, new Vector2(camera.Position.X - cameraWidth / 2, camera.Position.Y - cameraHeight / 2));

            player.Draw(spriteBatch);
            if (player.isAttacking)
            {
                attack.Draw(spriteBatch);
            }
            if (mapManager.currentMap != Map.witchHut)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(spriteBatch);
                }
            }
            if (mapManager.currentMap == Map.forest)
            {
                treeManager.Draw(spriteBatch, player);
            }
            if (player.hasMetWitch)
            {
                spriteBatch.Draw(soulOrb, soulOrbPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.95f);
                spriteBatch.DrawString(spriteFont, soulManager.Souls.ToString(),
                    new Vector2(soulOrbPos.X + 40, soulOrbPos.Y + 5), Color.White, 0, Vector2.Zero, 1f, 
                    SpriteEffects.None, 0.95f);
            }
            dialogueManager.Draw(spriteBatch, camera, mapManager);
            shopManager.Draw(spriteBatch, player, attack);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}