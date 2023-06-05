using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slime.Sprites;
using System.Diagnostics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Slime.Managers;
using System.Numerics;
using Comora;
using static System.Formats.Asn1.AsnWriter;

namespace Slime.Controllers
{
    public class MapController
    {
        private Random rand = new Random();
        private Dictionary<string, Texture2D> textures;
        private float hutDepth;
        private MouseState mStateOld;
        private KeyboardState kStateOld;
        private float screenTransitOpacity = 0f;
        private float screenTransitOpacityIncrement = 0.05f;
        private bool transitPhase1 = true;
        private bool transitPhase2 = false;
        private float transitTimer = 0f;
        private float transitDuration = 800f;
        private bool doSwitchMap = false;
        private float witchHutDoorOpacity = 1f;
        private bool doPlayDoorOpenSound = true;
        private Texture2D transitTextureOld;
        private Texture2D transitTextureNew;
        private Vector2 transitOldPos;
        private Vector2 transitNewPos;
        private Vector2 transitPathPos;
        private float transitTextureOpacity = 0;
        private float transitTextureOpacityIncrement = 0.1f;
        private Vector2 playerDownPos;
        private int playerDownSpeed = 200;
        private bool doUpdateTransitTexture = false;
        private bool letPlayerMove = true;
        private Vector2 backgroundPos;
        private Vector2 titleScreenBackgroundPos;
        private Dictionary<string, Sprite> titleScreenButtons;
        private List<Projectile> clouds = new List<Projectile>();
        private float spawnCloudTimer = 0;
        private Texture2D cloudTexture;
        private Dictionary<string, Sprite> witchHutFurniture;

        public Map currentMap = Map.titleScreen;
        public Map newMap;
        public bool isInScreenTransit = false;
        public Vector2 newPlayerPos;
        public int leftScreenBound = 0;
        public int rightScreenBound = 4000;
        public int topScreenBound = 0;
        public int bottomScreenBound = 4000;
        public Direction newPlayerDir;
        public bool doHealPlayer;
        public int currencyDeductAmount;
        public Texture2D currentBackground;
        public Vector2 witchHutPos = new Vector2(1600, 1200);
        public Vector2 witchPos = new Vector2(700, 50);
        public Vector2 summoningCirclePos;
        public Vector2 summoningCirclePlayerPos { get { return summoningCirclePos + new Vector2(5, 5); } }
        public bool doOpenTempleDoor = false;
        public List<Rectangle> objectRects = new List<Rectangle>();
        public Vector2 bossPos;
        public bool doUseWhiteScreen = false;
        public float whiteScreenOpacity = 0f;
        public float whiteScreenOpacityIncrement = 0.01f;
        public MapController(Dictionary<string, Texture2D> newTextures)
        {
            textures = newTextures;
            summoningCirclePos = new Vector2(textures["witchHutFloor"].Width / 2 -
                    textures["summoningCircle"].Width / 2, textures["witchHutFloor"].Height / 2 -
                    textures["summoningCircle"].Height / 2);

            currentBackground = textures["skyBackground"];
            titleScreenBackgroundPos = new Vector2(-textures["skyBackground"].Width / 2, -textures["skyBackground"].Height / 2);
            backgroundPos = titleScreenBackgroundPos;
            Texture2D tsb = textures["titleScreenButton"];
            titleScreenButtons = new Dictionary<string, Sprite>()
            {
                {"playButton", new Sprite(tsb) { text = "PLAY", position = new Vector2(-tsb.Width / 2, titleScreenBackgroundPos.Y + 630) } },
                {"exitButton", new Sprite(tsb) { text = "EXIT GAME", position = new Vector2(-tsb.Width / 2, titleScreenBackgroundPos.Y + 870) } },
            };
            witchHutFurniture = new Dictionary<string, Sprite>()
            {
                {"table", new Sprite(textures["table"]) { position = new Vector2(40, 370), depth = 0.6f } },
                {"chair", new Sprite(textures["chair"]) { depth = 0.55f } },
                {"cauldron", new Sprite(textures["cauldron"]) { position = new Vector2(40, 20), depth = 0.6f } },
                {"broom", new Sprite(textures["broom"]) { position = new Vector2(820, 650), rotation = (float)rand.Next(-3, 3) / 10, depth = 0.4f, 
                    origin = new Vector2(textures["broom"].Width / 2, textures["broom"].Height / 2) } }
            };
            witchHutFurniture["chair"].position = witchHutFurniture["table"].position + new Vector2(110, -130);
        }
        private void screenTransit(float dtM, Player player, SoundManager soundManager, AttackController attackController, 
            AbilityController abilityController, CurrencyController currencyController, CameraController cameraController, Boss boss)
        {
            player.canMove = false;
            player.canAttack = false;
            player.isAttacking = false;
            abilityController.teleportTimer = 0.1f;
            currencyController.souls -= currencyDeductAmount;
            transitPathPos = new Vector2(cameraController.position.X -
                    textures["transitPath"].Width / 2, cameraController.position.Y - textures["transitPath"].Height / 2);

            if (doHealPlayer)
            {
                transitDuration = 5000f;
            }
            else if (transitTextureNew != null)
            {
                transitDuration = 4200f;
            }
            else
            {
                transitDuration = 800f;
            }

            if (transitPhase1)
            {
                screenTransitOpacity += screenTransitOpacityIncrement;
                if (screenTransitOpacity >= 1f)
                {
                    transitPhase1 = false;
                    doSwitchMap = true;
                }
            }
            if (!transitPhase1)
            {
                doUpdateTransitTexture = true;
                if (transitTimer >= transitDuration)
                {
                    transitPhase2 = true;
                    transitTimer = 0f;
                }
                transitTimer += dtM;
            }
            if (transitPhase2)
            {
                screenTransitOpacity -= screenTransitOpacityIncrement;
                if (screenTransitOpacity <= 0)
                {
                    transitPhase2 = false;
                    transitPhase1 = true;
                    isInScreenTransit = false;
                    if (letPlayerMove)
                    {
                        player.canMove = true;
                        player.canAttack = true;
                    }
                    else if (!player.hasMetBossCurrentRun && !boss.isRemoved)
                    {
                        cameraController.doPlayCutscene = true;
                        cameraController.cutsceneStartPos = cameraController.position;
                        cameraController.cutsceneTargetPos = boss.center;
                        cameraController.doMoveCutsceneCamera = true;
                    }
                    doHealPlayer = false;
                    doPlayDoorOpenSound = true;
                    if (rand.Next(0, 2) == 0)
                    {
                        soundManager.doPlayMusic = true;
                    }
                    transitTextureNew = null;
                    transitTextureOpacity = 0;
                    doUpdateTransitTexture = false;
                }
            }
        }
        private void transitTextureUpdate(CameraController cameraController, Player player, float dt)
        {
            transitPathPos = new Vector2(cameraController.position.X - textures["transitPath"].Width / 2,
                cameraController.position.Y - textures["transitPath"].Height / 2);
            if (transitTextureOld != null)
            {
                transitOldPos = new Vector2(transitPathPos.X - transitTextureOld.Width,
                    cameraController.position.Y - transitTextureOld.Height / 2 + 50);
            }
            if (transitTextureNew != null)
            {
                transitNewPos = new Vector2(transitPathPos.X + textures["transitPath"].Width,
                    cameraController.position.Y - transitTextureNew.Height / 2 + 50);
            }

            if (transitTextureOpacity < 1 && transitTimer < transitDuration - 500)
            {
                transitTextureOpacity += transitTextureOpacityIncrement;
                playerDownPos = new Vector2(transitPathPos.X, cameraController.position.Y - player.height / 2);
            }
            if (transitTextureOpacity >= 1)
            {
                if (playerDownPos.X < transitPathPos.X + textures["transitPath"].Width - player.width)
                {
                    playerDownPos.X += playerDownSpeed * dt;
                }
            }
            if (transitTimer >= transitDuration - 500)
            {
                transitTextureOpacity -= transitTextureOpacityIncrement;
            }
        }
        private void switchMap(Player player, SoundManager soundManager, EnemyController enemyController,
            ProjectileController projectileController, CameraController cameraController, TreeController treeController,
            Boss boss, AbilityController abilityController)
        {
            letPlayerMove = true;
            player.dir = newPlayerDir;
            if (doHealPlayer)
            {
                player.health = player.maxHealth;
            }
            player.doPlayDeathSound = true;
            if (newPlayerPos != Vector2.Zero)
            {
                player.position = newPlayerPos;
            }
            enemyController.doSwitchEnemy = true;
            projectileController.currentProjectiles.Clear();
            abilityController.isTimeStopped = false;
            backgroundPos = Vector2.Zero;
            doUseWhiteScreen = false;

            objectRects.Clear();

            leftScreenBound = 0;
            topScreenBound = 0;
            if (currentMap == Map.titleScreen)
            {
                Game1.isGameActive = true;
            }
            if (newMap == Map.titleScreen)
            {
                currentBackground = textures["skyBackground"];
                backgroundPos = new Vector2(-textures["skyBackground"].Width / 2, -textures["skyBackground"].Height / 2);
                Game1.isGameActive = false;
                cameraController.position = Vector2.Zero;
            }
            else if (newMap == Map.witchHut)
            {
                currentBackground = textures["witchHutFloor"];

                Rectangle witchFeetRect = new Rectangle((int)witchPos.X,
                    (int)witchPos.Y + 228 - 50, 160, 50);
                objectRects.Add(witchFeetRect);
                foreach (var furniture in witchHutFurniture)
                {
                    if (furniture.Key == "chair")
                    {
                        Rectangle chairRect = new Rectangle((int)furniture.Value.position.X, (int)furniture.Value.position.Y + 110, 
                            furniture.Value.width, furniture.Value.height - 110);
                        objectRects.Add(chairRect);
                    }
                    else if (furniture.Key != "broom")
                    {
                        objectRects.Add(furniture.Value.Rectangle);
                    }
                }
            }
            else if (newMap == Map.forest)
            {
                currentBackground = textures["forestBackground"];

                Rectangle hutRect = new Rectangle((int)witchHutPos.X + 50,
                    (int)witchHutPos.Y + 330, 410, 182);
                foreach (Sprite tree in treeController.trees)
                {
                    Rectangle treeRootsRect = new Rectangle((int)tree.position.X + 130, (int)tree.position.Y + 382, 118, 76);
                    objectRects.Add(treeRootsRect);
                }
                objectRects.Add(hutRect);
            }
            else if (newMap == Map.cave1)
            {
                currentBackground = textures["cave1Background"];
            }
            else if (newMap == Map.cave2)
            {
                currentBackground = textures["cave2Background"];
            }
            else if (newMap == Map.cave3)
            {
                currentBackground = textures["cave3Background"];
            }
            else if (newMap == Map.cliff)
            {
                currentBackground = textures["cliff"];
                topScreenBound = -2000;
            }
            else if (newMap == Map.templePath)
            {
                currentBackground = textures["templePathBackground"];
                topScreenBound = -textures["templeDoorClosed"].Height;
            }
            else if (newMap == Map.temple)
            {
                currentBackground = textures["templeBackground"];
                bossPos = new Vector2(currentBackground.Width / 2 - boss.width / 2, currentBackground.Height / 2 - boss.height / 2 - 400);
                boss.health = boss.maxHealth;
                if (!player.hasMetBossCurrentRun)
                {
                    letPlayerMove = false;
                }
            }
            
            rightScreenBound = currentBackground.Width;
            bottomScreenBound = currentBackground.Height;
            if (newMap == Map.cliff)
            {
                rightScreenBound = 2000;
                leftScreenBound = -2000;
            }

            currentMap = newMap;
            doSwitchMap = false;
        }
        private void whiteScreen()
        {
            if (doUseWhiteScreen)
            {
                whiteScreenOpacity += whiteScreenOpacityIncrement;
            }
            else
            {
                if (whiteScreenOpacity > 0)
                {
                    whiteScreenOpacity -= whiteScreenOpacityIncrement;
                }
            }
        }
        private void spawnCloud(ProjectileController projectileController, CameraController cameraController)
        {
            switch (rand.Next(0, 2))
            {
                case 0:
                    cloudTexture = textures["cloud1"];
                    break;
                case 1:
                    cloudTexture = textures["cloud2"];
                    break;
            }
            float cloudScale = (float)rand.Next(8, 20) / 10;
            float cloudXPos = 0;
            switch (rand.Next(0, 2))
            {
                case 0:
                    cloudXPos = cameraController.Rectangle.Left - cloudTexture.Width * cloudScale;
                    break;
                case 1:
                    cloudXPos = cameraController.Rectangle.Right + cloudTexture.Width * cloudScale;
                    break;
            }
            SpriteEffects cloudFlip = SpriteEffects.None;
            switch (rand.Next(0, 2))
            {
                case 0:
                    cloudFlip = SpriteEffects.None;
                    break;
                case 1:
                    cloudFlip = SpriteEffects.FlipHorizontally;
                    break;
            }
            Vector2 cloudStartPos = new Vector2(cloudXPos, rand.Next(-currentBackground.Height / 2, currentBackground.Height / 2 -
                (int)(cloudTexture.Height * cloudScale)));
            Vector2 cloudTargetPos;
            if (cloudStartPos.X < 0)
            {
                cloudTargetPos = new Vector2(currentBackground.Width / 2 + cloudTexture.Width * cloudScale + 200, cloudStartPos.Y);
            }
            else
            {
                cloudTargetPos = new Vector2(-currentBackground.Width / 2 - cloudTexture.Width * cloudScale - 200, cloudStartPos.Y);
            }
            Projectile cloud = new Projectile(cloudTexture, cloudStartPos, cloudTargetPos, 0, rand.Next(200, 600), 300f, 0f)
            {
                scale = cloudScale,
                depth = (float)rand.Next(55, 70) / 100,
                doDestroyOnTargetReach = true,
                spriteEffect = cloudFlip,
                doHitPlayer = false,
                opacity = (float)rand.Next(8, 10) / 10,
            };
            projectileController.currentProjectiles.Add(cloud);
        }
        private void titleScreenUpdate(GameTime gameTime, SpriteFont spriteFont, MouseState mState, 
            Rectangle mRect, SoundManager soundManager, Player player, ProjectileController projectileController, CameraController cameraController)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnCloudTimer <= 0)
            {
                spawnCloud(projectileController, cameraController);
                spawnCloudTimer = rand.Next(1, 6);
            }
            else
            {
                spawnCloudTimer -= dt;
            }

            foreach (var tsb in titleScreenButtons)
            {
                soundManager.addButtonRect(tsb.Value.Rectangle);
                tsb.Value.depth = 0.9f;
                tsb.Value.textScale = 3f;
                tsb.Value.Update(gameTime, spriteFont);

                if (tsb.Value.Rectangle.Intersects(mRect))
                {
                    tsb.Value.color = Color.Bisque;
                    if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                    {
                        soundManager.playSound("buttonClick");
                        if (tsb.Key == "playButton")
                        {
                            if (player.hasMetWitch)
                            {
                                summonPlayer();
                            }
                            else
                            {
                                restartPlayer();
                            }
                        }
                        else if (tsb.Key == "exitButton")
                        {
                            Game1.ExitGame();
                        }
                    }
                }
                else
                {
                    tsb.Value.color = Color.White;
                }
            }
        }
        private void forestUpdate(Player player, MouseState mState, SoundManager soundManager)
        {
            Rectangle witchHutDoor = new Rectangle((int)witchHutPos.X + 216, (int)witchHutPos.Y + 432, 78, 100);
            Rectangle forestToCave = new Rectangle(-5, 1575, 20, 160);
            Rectangle forestToCliff = new Rectangle(3985, 1840, 15, 140);
            Rectangle forestToTemplePath = new Rectangle(1985, -5, 90, 20);

            if (!isInScreenTransit)
            {
                transitTextureOld = textures["transitForest"];
            }

            if (player.position.Y > 1600 + 354)
            {
                hutDepth = 0.4f;
            }
            else
            {
                hutDepth = 0.6f;
            }
            if (player.Rectangle.Intersects(witchHutDoor) && mState.RightButton == ButtonState.Pressed && 
                screenTransitOpacity <= 0)
            {
                newMap = Map.witchHut;
                newPlayerPos = new Vector2(textures["witchHutFloor"].Width / 2 - player.width / 2,
                    textures["witchHutFloor"].Height - player.height);
                newPlayerDir = Direction.Up;
                if (doPlayDoorOpenSound)
                {
                    soundManager.playSound("doorOpen");
                    doPlayDoorOpenSound = false;
                }
            }
            else if (player.Rectangle.Intersects(forestToCave) && player.dir == Direction.Left)
            {
                newMap = Map.cave1;
                newPlayerPos = new Vector2(textures["cave1Background"].Width / 2 - player.width / 2, 
                    textures["cave1Background"].Height - player.height);
                newPlayerDir = Direction.Up;
                transitTextureNew = textures["transitCave"];
            }
            else if (player.Rectangle.Intersects(forestToCliff) && player.dir == Direction.Right)
            {
                newMap = Map.cliff;
                newPlayerPos = new Vector2(textures["cliff"].Width / 2 - player.width / 2,
                    textures["cliff"].Height - player.height);
                newPlayerDir = Direction.Up;
                transitTextureNew = textures["transitCliff"];
            }
            else if (player.Rectangle.Intersects(forestToTemplePath) && player.dir == Direction.Up)
            {
                newMap = Map.templePath;
                newPlayerPos = new Vector2(textures["templePathBackground"].Width / 2 - player.width / 2,
                    textures["templePathBackground"].Height - player.height);
                newPlayerDir = player.dir;
                transitTextureNew = textures["transitTemple"];
            }
        }
        private void witchHutUpdate(Player player, MouseState mState, SoundManager soundManager)
        {
            Texture2D witchHutFloor = textures["witchHutFloor"];
            Texture2D witchHutDoor = textures["witchHutDoor"];
            Rectangle witchHutDoorRect = new Rectangle(witchHutFloor.Width / 2 - witchHutDoor.Width / 2,
                   witchHutFloor.Height - witchHutDoor.Height,
                   witchHutDoor.Width, witchHutDoor.Height);

            if (player.health < player.maxHealth)
            {
                player.health = player.maxHealth;
            }
            if (player.Rectangle.Intersects(witchHutDoorRect))
            {
                witchHutDoorOpacity = 0.5f;
            }
            else
            {
                witchHutDoorOpacity = 1f;
            }
            if (player.Rectangle.Intersects(witchHutDoorRect) &&
                player.position.Y > witchHutFloor.Height - 90 &&
                mState.RightButton == ButtonState.Pressed && screenTransitOpacity <= 0)
            {
                newMap = Map.forest;
                newPlayerPos = new Vector2(witchHutPos.X + 221, witchHutPos.Y + 512);
                newPlayerDir = Direction.Down;
                if (doPlayDoorOpenSound)
                {
                    soundManager.playSound("doorOpen");
                    doPlayDoorOpenSound = false;
                }
            }
        }
        private void caveUpdate(Player player)
        {
            Rectangle cave1ToForest = new Rectangle(textures["cave1Background"].Width / 2 - 10,
                textures["cave1Background"].Height - 15, 20, 20);
            Rectangle cave1ToCave2 = new Rectangle(340, 0, 120, 15);
            Rectangle cave2ToCave1 = new Rectangle(850, textures["cave2Background"].Height - 20, 120, 20);
            Rectangle cave2ToCave3 = new Rectangle(850, 0, 120, 15);
            Rectangle cave3ToCave2 = new Rectangle(1185, textures["cave3Background"].Height - 20, 120, 20);
            if (!isInScreenTransit)
            {
                transitTextureOld = textures["transitCave"];
            }

            if (currentMap == Map.cave1)
            {
                if (player.Rectangle.Intersects(cave1ToForest) && player.dir == Direction.Down)
                {
                    newMap = Map.forest;
                    newPlayerPos = new Vector2(2, 1595);
                    newPlayerDir = Direction.Right;
                    transitTextureNew = textures["transitForest"];
                }
                else if (player.Rectangle.Intersects(cave1ToCave2) && player.dir == Direction.Up)
                {
                    newMap = Map.cave2;
                    newPlayerPos = new Vector2(textures["cave2Background"].Width / 2 - player.width / 2,
                        textures["cave2Background"].Height - player.height);
                    newPlayerDir = player.dir;
                }
            }
            else if (currentMap == Map.cave2)
            {
                if (player.Rectangle.Intersects(cave2ToCave1) && player.dir == Direction.Down)
                {
                    newMap = Map.cave1;
                    newPlayerPos = new Vector2(textures["cave1Background"].Width / 2 - player.width / 2, player.height);
                    newPlayerDir = player.dir;
                }
                else if (player.Rectangle.Intersects(cave2ToCave3) && player.dir == Direction.Up)
                {
                    newMap = Map.cave3;
                    newPlayerPos = new Vector2(textures["cave3Background"].Width / 2 - player.width / 2,
                        textures["cave3Background"].Height - player.height);
                    newPlayerDir = player.dir;
                }
            }
            else if (currentMap == Map.cave3)
            {
                if (player.Rectangle.Intersects(cave3ToCave2) && player.dir == Direction.Down)
                {
                    newMap = Map.cave2;
                    newPlayerPos = new Vector2(textures["cave2Background"].Width / 2 - player.width / 2, player.height);
                    newPlayerDir = player.dir;
                }
            }
        }
        private void cliffUpdate(Player player, ProjectileController projectileController, float dt, CameraController cameraController)
        {
            if (!isInScreenTransit)
            {
                transitTextureOld = textures["transitCliff"];
            }

            if (player.position.Y > textures["cliff"].Height - player.height - 15 && player.dir == Direction.Down)
            {
                newMap = Map.forest;
                newPlayerPos = new Vector2(textures["forestBackground"].Width - player.width, 1850);
                newPlayerDir = Direction.Left;
                transitTextureNew = textures["transitForest"];
            }
        }
        private void templePathUpdate(Player player)
        {
            if (!isInScreenTransit)
            {
                transitTextureOld = textures["transitTemple"];
            }

            if (player.position.Y < 15 && player.dir == Direction.Up && doOpenTempleDoor)
            {
                newMap = Map.temple;
                newPlayerPos = new Vector2(textures["templeBackground"].Width / 2 - player.width / 2,
                    textures["templeBackground"].Height - player.height);
                newPlayerDir = player.dir;
            }
            else if (player.position.Y > bottomScreenBound - player.height - 15 && player.dir == Direction.Down)
            {
                newMap = Map.forest;
                newPlayerPos = new Vector2(textures["forestBackground"].Width / 2, 0);
                newPlayerDir = player.dir;
                transitTextureNew = textures["transitForest"];
            }
        }
        public void summonPlayer()
        {
            newPlayerPos = summoningCirclePlayerPos;
            newPlayerDir = Direction.Down;
            newMap = Map.witchHut;
        }
        public void restartPlayer()
        {
            newPlayerPos = new Vector2(textures["forestBackground"].Width / 2, textures["forestBackground"].Height - 70);
            newPlayerDir = Direction.Up;
            newMap = Map.forest;
        }
        public void Draw(SpriteBatch spriteBatch, CameraController cameraController, Player player, SpriteFont spriteFont)
        {
            spriteBatch.Draw(textures["blackScreen"], new Vector2(cameraController.position.X - cameraController.width / 2, 
                cameraController.position.Y - cameraController.height / 2), null,
                Color.Black * screenTransitOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
            spriteBatch.Draw(textures["whiteScreen"], new Vector2(cameraController.position.X - cameraController.width / 2,
                cameraController.position.Y - cameraController.height / 2), null,
                Color.White * whiteScreenOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.981f);
            spriteBatch.Draw(currentBackground, backgroundPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            if (isInScreenTransit && transitTextureNew != null)
            {
                spriteBatch.Draw(textures["transitPath"], transitPathPos, 
                    null, Color.White * transitTextureOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.981f);
                spriteBatch.Draw(transitTextureOld, transitOldPos,
                    null, Color.White * transitTextureOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.981f);
                spriteBatch.Draw(transitTextureNew, transitNewPos,
                    null, Color.White * transitTextureOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.981f);
                spriteBatch.Draw(textures["playerDown"], playerDownPos, 
                    new Rectangle(0, 0, player.width, player.height), 
                    Color.White * transitTextureOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.982f);
            }
            if (currentMap == Map.titleScreen)
            {
                foreach (Sprite titleScreenButton in titleScreenButtons.Values)
                {
                    titleScreenButton.Draw(spriteBatch, spriteFont);
                }
                spriteBatch.Draw(textures["title"], new Vector2(-textures["title"].Width / 2, -cameraController.height / 2 + 40), null, Color.White,
                    0f, Vector2.Zero, 1f, SpriteEffects.None, 0.8f);
            }
            else if (currentMap == Map.forest)
            {
                spriteBatch.Draw(textures["witchHut"], witchHutPos, null, Color.White, 0, Vector2.Zero, 1f,
                    SpriteEffects.None, hutDepth);
            }
            else if (currentMap == Map.witchHut)
            {
                spriteBatch.Draw(textures["witchHutDoor"],
                    new Vector2(textures["witchHutFloor"].Width / 2 - textures["witchHutDoor"].Width / 2,
                    textures["witchHutFloor"].Height - textures["witchHutDoor"].Height),
                    null, Color.White * witchHutDoorOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
                spriteBatch.Draw(textures["witch"], witchPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                spriteBatch.Draw(textures["summoningCircle"], summoningCirclePos, null, Color.White, 0f, 
                    Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
                foreach (Sprite furniture in witchHutFurniture.Values)
                {
                    furniture.Draw(spriteBatch);
                }
            }
            else if (currentMap == Map.cliff)
            {
                spriteBatch.Draw(textures["skyBackground"], cameraController.position, null, Color.White, 0f, 
                    new Vector2(textures["skyBackground"].Width / 2, textures["skyBackground"].Height / 2), 1f, SpriteEffects.None, 0.05f);
            }
            else if (currentMap == Map.templePath)
            {
                if (!doOpenTempleDoor) 
                {
                    spriteBatch.Draw(textures["templeDoorClosed"], new Vector2(rightScreenBound / 2 - 
                        textures["templeDoorClosed"].Width / 2, -textures["templeDoorClosed"].Height), null, Color.White, 0f, 
                        Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
                }
                else
                {
                    spriteBatch.Draw(textures["templeDoorOpen"], new Vector2(rightScreenBound / 2 -
                        textures["templeDoorOpen"].Width / 2, -textures["templeDoorOpen"].Height), null, Color.White, 0f,
                        Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
                }
            }
        }
        public void Update(GameTime gameTime, Player player, SoundManager soundManager, AttackController attackController, 
            EnemyController enemyController, AbilityController abilityController, 
            ProjectileController projectileController, CurrencyController currencyController, 
            CameraController cameraController, TreeController treeController, Boss boss, SpriteFont spriteFont)
        {
            MouseState mState = Mouse.GetState();
            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float dtM = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Rectangle mRect = new Rectangle((int)(cameraController.position.X - cameraController.width / 2 + mState.Position.X),
                (int)(cameraController.position.Y - cameraController.height / 2 + mState.Position.Y), 1, 1);

            whiteScreen();
            if (currentMap == Map.titleScreen)
            {
                titleScreenUpdate(gameTime, spriteFont, mState, mRect, soundManager, player, projectileController, cameraController);
            }
            else if (currentMap == Map.forest)
            {
                forestUpdate(player, mState, soundManager);
            }
            else if (currentMap == Map.witchHut)
            {
                witchHutUpdate(player, mState, soundManager);
            }
            else if (currentMap == Map.cave1 || currentMap == Map.cave2 || currentMap == Map.cave3)
            {
                caveUpdate(player);
            }
            else if (currentMap == Map.cliff)
            {
                cliffUpdate(player, projectileController, dt, cameraController);
            }
            else if (currentMap == Map.templePath)
            {
                templePathUpdate(player);
            }
            if (newMap != currentMap)
            {
                isInScreenTransit = true;
            }
            if (isInScreenTransit)
            {
                screenTransit(dtM, player, soundManager, attackController, abilityController, currencyController, cameraController, boss);
            }
            if (doUpdateTransitTexture)
            {
                transitTextureUpdate(cameraController, player, dt);
            }
            if (doSwitchMap)
            {
                switchMap(player, soundManager, enemyController, projectileController, cameraController, treeController, 
                    boss, abilityController);
            }
            if (isInScreenTransit)
            {
                player.isAttacking = false;
            }
            mStateOld = mState;
            kStateOld = kState;
        }
    }
}
