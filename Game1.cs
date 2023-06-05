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
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Diagnostics.SymbolStore;

namespace Slime
{
    public enum Direction
    {
        Up, Down, Left, Right, None
    }
    public enum Map
    {
        titleScreen, forest, witchHut, cave1, cave2, cave3, cliff, templePath, temple
    }
    public class Game1 : Game
    {
        public static Game1 game1;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static bool doChangeFullscreen;
        public bool isFullScreen
        {
            get { return graphics.IsFullScreen; }
        }

        public static bool isGameActive = false;

        private SaveManager saveManager;
        private const string PATH = "save.json";
        public static bool doSave;
        public static bool doTransferSaveData;

        private Cheat cheat;

        private Random rand;

        private SpriteFont spriteFont;

        private Texture2D blackScreen;
        private Texture2D whiteScreen;
        private Texture2D skyBackground;

        private MapController mapController;
        private Color[] fillScreenColor;

        private CurrencyController currencyController;
        private ShopController shopController;
        private DialogueController dialogueController;
        private AbilityController abilityController;
        private AbilityIconController abilityIconController;
        private MenuController menuController;
        private ProjectileController projectileController;

        private SoundManager soundManager;

        private Camera camera;
        private CameraController cameraController;
        private int cameraWidth;
        private int cameraHeight;

        private Texture2D slimeUp;
        private Texture2D slimeDown;
        private Texture2D slimeLeft;
        private Texture2D slimeRight;

        private Texture2D attackUp;
        private Texture2D attackDown;
        private Texture2D attackLeft;
        private Texture2D attackRight;
        private Texture2D fireballUp;
        private Texture2D fireballDown;
        private Texture2D fireballLeft;
        private Texture2D fireballRight;

        private Texture2D dialogueBox;
        private Texture2D witchPortraitNormal;

        private Texture2D cursor;
        private Texture2D soulOrb;
        private Texture2D exitButton;
        private Texture2D explosion;
        private Texture2D teleport;
        private Texture2D attackUseIcon;
        private Texture2D teleportUseIcon;
        private Texture2D timeStopUseIcon;
        private Texture2D transitPath;
        private Texture2D transitForest;
        private Texture2D transitCave;
        private Texture2D transitCliff;
        private Texture2D transitTemple;
        private Texture2D cloud1;
        private Texture2D cloud2;

        private Texture2D menuBackground;
        private Texture2D menuButtonMedium;
        private Texture2D soundBar;
        private Texture2D soundSlider;

        private Texture2D shopBackground;
        private Texture2D shopItemLight;
        private Texture2D shopItemDark;
        private Texture2D shopItemLocked;
        private Texture2D itemDescriptionBackground;
        private Texture2D attackUpgradeIcon;
        private Texture2D healthUpgradeIcon;
        private Texture2D speedUpgradeIcon;
        private Texture2D cooldownUpgradeIcon;
        private Texture2D regenerationUpgradeIcon;
        private Texture2D fireballUpgradeIcon;
        private Texture2D ghostUpgradeIcon;
        private Texture2D teleportUpgradeIcon;
        private Texture2D timeStopUpgradeIcon;

        private Texture2D titleScreenButton;
        private Texture2D title;

        private Texture2D forestBackground;
        private Texture2D witchHut;
        private Texture2D treeSprite;
        private Texture2D laskUp;
        private Texture2D laskDown;
        private Texture2D laskDeathUp;
        private Texture2D laskDeathDown;
        private Texture2D laskSpawn;

        private Texture2D witchHutFloor;
        private Texture2D witchHutDoor;
        private Texture2D witch;
        private Texture2D summoningCircle;
        private Texture2D chair;
        private Texture2D table;
        private Texture2D cauldron;
        private Texture2D broom;

        private Texture2D cave1Background;
        private Texture2D cave2Background;
        private Texture2D cave3Background;
        private Texture2D tarlitUp;
        private Texture2D tarlitDown;
        private Texture2D tarlitDeathUp;
        private Texture2D tarlitDeathDown;
        private Texture2D tarlitSpawn;
        private Texture2D tarlitProjectile;

        private Texture2D cliff;
        private Texture2D tempusUp;
        private Texture2D tempusDown;
        private Texture2D tempusDeathUp;
        private Texture2D tempusDeathDown;
        private Texture2D tempusProjectile;

        private Texture2D templePathBackground;
        private Texture2D templeDoorClosed;
        private Texture2D templeDoorOpen;

        private Texture2D templeBackground;
        private Texture2D finalBoss;
        private Texture2D finalBossGrey;
        private Texture2D energyBall;
        private Texture2D healingCrystal;
        private Texture2D healingCrystalDestroyed;
        private Texture2D sunSheet;

        private Player player;
        private AttackController attackController;

        //private List<Enemy> enemies;
        private EnemyController enemyController;
        private List<Vector2> laskSpawnPoints = new List<Vector2>()
        { 
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

        private TreeController treeController;
        private List<Vector2> treePos;
        private Vector2 startPos;
        private int treeRowDist;
        private int treePerRow;

        private Boss boss;

        private SoundEffect forestBackgroundMusic;
        private SoundEffect caveBackgroundMusic;
        private SoundEffect cliffBackgroundMusic;
        private SoundEffect bossFightBackgroundMusic;

        private SoundEffect slimeRustle;
        private SoundEffect slimeDamaged;
        private SoundEffect playerDeath;

        private SoundEffect buttonHover;
        private SoundEffect buttonClick;
        private SoundEffect dialogueSoundEffect;
        private SoundEffect teleportSoundEffect;
        private SoundEffect explosionSoundEffect;
        private SoundEffect timeStopTick;
        private SoundEffect soundTest;
        private SoundEffect doorOpen;

        private SoundEffect normalAttackWhoosh;
        private SoundEffect fireballWhoosh;

        private SoundEffect windHowl;
        private SoundEffect laskHit;
        private SoundEffect laskDeath;

        private SoundEffect tarlitHit;
        private SoundEffect tarlitDeath;
        private SoundEffect tarlitProjectileLaunch;

        private SoundEffect tempusDeath;
        private SoundEffect tempusProjectileLaunch;

        private SoundEffect energyBlast;
        private SoundEffect largeEnergyBlast;
        private SoundEffect healingCrystalShatter;
        private SoundEffect healingCrystalHit;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            game1 = this;
        }

        protected override void Initialize()
        {
            cameraWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            cameraHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = cameraWidth;
            graphics.PreferredBackBufferHeight = cameraHeight;
            graphics.IsFullScreen = true;
            
            graphics.ApplyChanges();

            this.camera = new Camera(graphics.GraphicsDevice);
            cameraController = new CameraController(camera);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            saveManager = new SaveManager();

            cheat = new Cheat();

            rand = new Random();

            spriteFont = Content.Load<SpriteFont>("spriteFont");

            blackScreen = new Texture2D(graphics.GraphicsDevice, cameraWidth, cameraHeight);
            whiteScreen = new Texture2D(graphics.GraphicsDevice, cameraWidth, cameraHeight);
            skyBackground = new Texture2D(graphics.GraphicsDevice, cameraWidth, cameraHeight);
            fillScreenColor = new Color[cameraWidth * cameraHeight];
            for (int i = 0; i < fillScreenColor.Length; i++)
            {
                fillScreenColor[i] = Color.Black;
            }
            blackScreen.SetData(fillScreenColor);
            for (int i = 0; i < fillScreenColor.Length; i++)
            {
                fillScreenColor[i] = Color.White;
            }
            whiteScreen.SetData(fillScreenColor);
            for (int i = 0; i < fillScreenColor.Length; i++)
            {
                fillScreenColor[i] = new Color(155, 238, 255);
            }
            skyBackground.SetData(fillScreenColor);

            slimeUp = Content.Load<Texture2D>("Player/Slime_Up_Sheet");
            slimeDown = Content.Load<Texture2D>("Player/Slime_Down_Sheet");
            slimeLeft = Content.Load<Texture2D>("Player/Slime_Left_Sheet");
            slimeRight = Content.Load<Texture2D>("Player/Slime_Right_Sheet");

            attackUp = Content.Load<Texture2D>("Attacks/Attack_Up_Sheet");
            attackDown = Content.Load<Texture2D>("Attacks/Attack_Down_Sheet");
            attackLeft = Content.Load<Texture2D>("Attacks/Attack_Left_Sheet");
            attackRight = Content.Load<Texture2D>("Attacks/Attack_Right_Sheet");
            fireballUp = Content.Load<Texture2D>("Attacks/Fireball_Up_Sheet");
            fireballDown = Content.Load<Texture2D>("Attacks/Fireball_Down_Sheet");
            fireballLeft = Content.Load<Texture2D>("Attacks/Fireball_Left_Sheet");
            fireballRight = Content.Load<Texture2D>("Attacks/Fireball_Right_Sheet");

            dialogueBox = Content.Load<Texture2D>("Dialogue/DialogueBox");
            witchPortraitNormal = Content.Load<Texture2D>("Dialogue/WitchPortrait_Normal");

            cursor = Content.Load<Texture2D>("Misc/Cursor");
            soulOrb = Content.Load<Texture2D>("Misc/SoulOrb");
            exitButton = Content.Load<Texture2D>("Misc/ExitButton");
            explosion = Content.Load<Texture2D>("Misc/Explosion_Sheet");
            teleport = Content.Load<Texture2D>("Misc/Teleport_Sheet");
            attackUseIcon = Content.Load<Texture2D>("Misc/AttackUse_Icon");
            teleportUseIcon = Content.Load<Texture2D>("Misc/TeleportUse_Icon");
            timeStopUseIcon = Content.Load<Texture2D>("Misc/TimeStopUse_Icon");
            transitPath = Content.Load<Texture2D>("Misc/Transition_Path");
            transitForest = Content.Load<Texture2D>("Misc/Transition_Forest");
            transitCave = Content.Load<Texture2D>("Misc/Transition_Cave");
            transitCliff = Content.Load<Texture2D>("Misc/Transition_Cliff");
            transitTemple = Content.Load<Texture2D>("Misc/Transition_Temple");
            cloud1 = Content.Load<Texture2D>("Misc/Cloud_1");
            cloud2 = Content.Load<Texture2D>("Misc/Cloud_2");

            menuBackground = Content.Load<Texture2D>("Menu/Menu_Background");
            menuButtonMedium = Content.Load<Texture2D>("Menu/MenuButton_Medium");
            soundBar = Content.Load<Texture2D>("Menu/Sound_Bar");
            soundSlider = Content.Load<Texture2D>("Menu/Sound_Slider");

            shopBackground = Content.Load<Texture2D>("Shop/ShopBackground");
            shopItemLight = Content.Load<Texture2D>("Shop/ShopItem_Light");
            shopItemDark = Content.Load<Texture2D>("Shop/ShopItem_Dark");
            shopItemLocked = Content.Load<Texture2D>("Shop/ShopItem_Locked");
            itemDescriptionBackground = Content.Load<Texture2D>("Shop/ItemDescription_Background");
            attackUpgradeIcon = Content.Load<Texture2D>("Shop/AttackUpgrade_Icon");
            healthUpgradeIcon = Content.Load<Texture2D>("Shop/HealthUpgrade_Icon");
            speedUpgradeIcon = Content.Load<Texture2D>("Shop/SpeedUpgrade_Icon");
            cooldownUpgradeIcon = Content.Load<Texture2D>("Shop/CooldownUpgrade_Icon");
            regenerationUpgradeIcon = Content.Load<Texture2D>("Shop/RegenerationUpgrade_Icon");
            fireballUpgradeIcon = Content.Load<Texture2D>("Shop/FireballUpgrade_Icon");
            ghostUpgradeIcon = Content.Load<Texture2D>("Shop/GhostUpgrade_Icon");
            teleportUpgradeIcon = Content.Load<Texture2D>("Shop/TeleportUpgrade_Icon");
            timeStopUpgradeIcon = Content.Load<Texture2D>("Shop/TimeStopUpgrade_Icon");

            titleScreenButton = Content.Load<Texture2D>("TitleScreen/TitleScreen_Button");
            title = Content.Load<Texture2D>("TitleScreen/Title");

            forestBackground = Content.Load<Texture2D>("Forest/Forest_Background");
            witchHut = Content.Load<Texture2D>("Forest/WitchHut");
            treeSprite = Content.Load<Texture2D>("Forest/Tree");
            laskUp = Content.Load<Texture2D>("Forest/Lask_Up_Sheet");
            laskDown = Content.Load<Texture2D>("Forest/Lask_Down_Sheet");
            laskDeathUp = Content.Load<Texture2D>("Forest/Lask_Death_Up_Sheet");
            laskDeathDown = Content.Load<Texture2D>("Forest/Lask_Death_Down_Sheet");
            laskSpawn = Content.Load<Texture2D>("Forest/Lask_Spawn_Sheet");

            witchHutFloor = Content.Load<Texture2D>("WitchHut/WitchHut_Floor");
            witchHutDoor = Content.Load<Texture2D>("WitchHut/WitchHut_Door");
            witch = Content.Load<Texture2D>("WitchHut/Witch");
            summoningCircle = Content.Load<Texture2D>("WitchHut/Summoning_Circle");
            chair = Content.Load<Texture2D>("WitchHut/Chair");
            table = Content.Load<Texture2D>("WitchHut/Table");
            cauldron = Content.Load<Texture2D>("WitchHut/Cauldron");
            broom = Content.Load<Texture2D>("WitchHut/Broom");

            cave1Background = Content.Load<Texture2D>("Cave/Cave1_Background");
            cave2Background = Content.Load<Texture2D>("Cave/Cave2_Background");
            cave3Background = Content.Load<Texture2D>("Cave/Cave3_Background");
            tarlitUp = Content.Load<Texture2D>("Cave/Tarlit_Up_Sheet");
            tarlitDown = Content.Load<Texture2D>("Cave/Tarlit_Down_Sheet");
            tarlitDeathUp = Content.Load<Texture2D>("Cave/Tarlit_Death_Up_Sheet");
            tarlitDeathDown = Content.Load<Texture2D>("Cave/Tarlit_Death_Down_Sheet");
            tarlitSpawn = Content.Load<Texture2D>("Cave/Tarlit_Spawn_Sheet");
            tarlitProjectile = Content.Load<Texture2D>("Cave/Tarlit_Projectile");

            cliff = Content.Load<Texture2D>("Cliff/Cliff");
            tempusUp = Content.Load<Texture2D>("Cliff/Tempus_Up_Sheet");
            tempusDown = Content.Load<Texture2D>("Cliff/Tempus_Down_Sheet");
            tempusDeathUp = Content.Load<Texture2D>("Cliff/Tempus_Death_Up_Sheet");
            tempusDeathDown = Content.Load<Texture2D>("Cliff/Tempus_Death_Down_Sheet");
            tempusProjectile = Content.Load<Texture2D>("Cliff/Tempus_Projectile");

            templePathBackground = Content.Load<Texture2D>("Temple/Temple_Path_Background");
            templeDoorClosed = Content.Load<Texture2D>("Temple/Temple_Door_Closed");
            templeDoorOpen = Content.Load<Texture2D>("Temple/Temple_Door_Open");

            templeBackground = Content.Load<Texture2D>("Temple/Temple_Background");
            finalBoss = Content.Load<Texture2D>("Temple/FinalBoss_Sheet");
            finalBossGrey = Content.Load<Texture2D>("Temple/FinalBoss_Grey");
            energyBall = Content.Load<Texture2D>("Temple/Energy_Ball");
            healingCrystal = Content.Load<Texture2D>("Temple/Healing_Crystal");
            healingCrystalDestroyed = Content.Load<Texture2D>("Temple/Healing_Crystal_Destroyed_Sheet");
            sunSheet = Content.Load<Texture2D>("Temple/Sun_Sheet");

            forestBackgroundMusic = Content.Load<SoundEffect>("BackgroundMusic/Forest_Master_12-13-2022");
            caveBackgroundMusic = Content.Load<SoundEffect>("BackgroundMusic/Cave_Master_1-16-2023");
            cliffBackgroundMusic = Content.Load<SoundEffect>("BackgroundMusic/Cliff_Master_2-21-2023");
            bossFightBackgroundMusic = Content.Load<SoundEffect>("BackgroundMusic/Boss_Fight_Master_2-12-2023");

            slimeRustle = Content.Load<SoundEffect>("SoundEffects/Player/Slime_Rustle");
            slimeDamaged = Content.Load<SoundEffect>("SoundEFfects/Player/Slime_Damaged");
            playerDeath = Content.Load<SoundEffect>("SoundEffects/Player/Player_Death");

            buttonHover = Content.Load<SoundEffect>("SoundEffects/Misc/Button_Hover");
            buttonClick = Content.Load<SoundEffect>("SoundEffects/Misc/Button_Click");
            dialogueSoundEffect = Content.Load<SoundEffect>("SoundEffects/Misc/Dialogue_SoundEffect");
            explosionSoundEffect = Content.Load<SoundEffect>("SoundEffects/Misc/Explosion_SoundEffect");
            teleportSoundEffect = Content.Load<SoundEffect>("SoundEffects/Misc/Teleport_SoundEffect");
            timeStopTick = Content.Load<SoundEffect>("SoundEffects/Misc/TimeStop_Tick");
            soundTest = Content.Load<SoundEffect>("SoundEffects/Misc/Sound_Test");
            doorOpen = Content.Load<SoundEffect>("SoundEffects/Misc/Door_Open");

            normalAttackWhoosh = Content.Load<SoundEffect>("SoundEffects/Attacks/NormalAttack_Whoosh");
            fireballWhoosh = Content.Load<SoundEffect>("SoundEffects/Attacks/Fireball_Whoosh");

            windHowl = Content.Load<SoundEffect>("SoundEffects/Forest/Wind_Howl");
            laskHit = Content.Load<SoundEffect>("SoundEffects/Forest/Lask_Hit");
            laskDeath = Content.Load<SoundEffect>("SoundEffects/Forest/Lask_Death");

            tarlitHit = Content.Load<SoundEffect>("SoundEffects/Cave/Tarlit_Hit");
            tarlitDeath = Content.Load<SoundEffect>("SoundEffects/Cave/Tarlit_Death");
            tarlitProjectileLaunch = Content.Load<SoundEffect>("SoundEffects/Cave/Tarlit_ProjectileLaunch");

            tempusDeath = Content.Load<SoundEffect>("SoundEffects/Cliff/Tempus_Death");
            tempusProjectileLaunch = Content.Load<SoundEffect>("SoundEffects/Cliff/Tempus_ProjectileLaunch");

            energyBlast = Content.Load<SoundEffect>("SoundEffects/Temple/Energy_Blast");
            largeEnergyBlast = Content.Load<SoundEffect>("SoundEffects/Temple/Large_Energy_Blast");
            healingCrystalShatter = Content.Load<SoundEffect>("SoundEffects/Temple/Healing_Crystal_Shatter");
            healingCrystalHit = Content.Load<SoundEffect>("SoundEffects/Temple/Healing_Crystal_Hit");

            var mapTextures = new Dictionary<string, Texture2D>()
            {
                {"blackScreen", blackScreen },
                {"whiteScreen", whiteScreen },

                {"skyBackground", skyBackground },
                {"cloud1", cloud1 },
                {"cloud2", cloud2 },

                {"playerDown", slimeDown },
                {"transitPath", transitPath },
                {"transitForest", transitForest },
                {"transitCave", transitCave },
                {"transitTemple", transitTemple },
                {"transitCliff", transitCliff },

                {"titleScreenButton", titleScreenButton },
                {"title", title },

                {"forestBackground", forestBackground },
                {"witchHut", witchHut },

                {"witchHutFloor", witchHutFloor },
                {"witchHutDoor", witchHutDoor },
                {"witch", witch },
                {"summoningCircle", summoningCircle },
                {"chair", chair },
                {"table", table },
                {"cauldron", cauldron },
                {"broom", broom },

                {"cave1Background", cave1Background },
                {"cave2Background", cave2Background },
                {"cave3Background", cave3Background },

                {"cliff", cliff },

                {"templePathBackground", templePathBackground },
                {"templeDoorClosed", templeDoorClosed},
                {"templeDoorOpen", templeDoorOpen },

                {"templeBackground", templeBackground },
            };
            var shopTextures = new Dictionary<string, Texture2D>()
            {
                {"shopBackground", shopBackground },
                {"blackScreen", blackScreen },
                {"exitButton", exitButton },
                {"shopItemLight", shopItemLight },
                {"shopItemDark", shopItemDark },
                {"shopItemLocked", shopItemLocked },
                {"itemDescriptionBackground", itemDescriptionBackground },
                {"attackUpgradeIcon", attackUpgradeIcon },
                {"healthUpgradeIcon", healthUpgradeIcon },
                {"speedUpgradeIcon", speedUpgradeIcon },
                {"cooldownUpgradeIcon", cooldownUpgradeIcon },
                {"regenerationUpgradeIcon", regenerationUpgradeIcon },
                {"fireballUpgradeIcon", fireballUpgradeIcon },
                {"ghostUpgradeIcon", ghostUpgradeIcon },
                {"teleportUpgradeIcon", teleportUpgradeIcon },
                {"timeStopUpgradeIcon", timeStopUpgradeIcon },
            };
            var dialogueTextures = new Dictionary<string, Texture2D>()
            {
                {"dialogueBox", dialogueBox },
                {"witchPortraitNormal", witchPortraitNormal },
            };
            var playerAnimations = new Dictionary<string, Animation>()
            {
                {"moveUp", new Animation(slimeUp, 6) },
                {"moveDown", new Animation(slimeDown, 6) },
                {"moveLeft", new Animation(slimeLeft, 6) },
                {"moveRight", new Animation(slimeRight, 6) },
            };
            /*var attackAnimations = new Dictionary<string, Animation>()
            {
                {"attackUp", new Animation(attackUp, 5) },
                {"attackDown", new Animation(attackDown, 5) },
                {"attackLeft", new Animation(attackLeft, 5) },
                {"attackRight", new Animation(attackRight, 5) },

                {"fireballUp", new Animation(fireballUp, 8) },
                {"fireballDown", new Animation(fireballDown, 8) },
                {"fireballLeft", new Animation(fireballLeft, 8) },
                {"fireballRight", new Animation(fireballRight, 8) },

                {"explosion", new Animation(explosion, 14) },
            };*/
            var attackSheets = new Dictionary<string, Texture2D>()
            {
                {"attackUp", attackUp },
                {"attackDown", attackDown },
                {"attackLeft", attackLeft },
                {"attackRight", attackRight },

                {"fireballUp", fireballUp },
                {"fireballDown", fireballDown },
                {"fireballLeft", fireballLeft },
                {"fireballRight", fireballRight },

                {"explosion", explosion},
            };
            var abilityAnimations = new Dictionary<string, Animation>()
            {
                {"teleport", new Animation(teleport, 8) },
            };
            var enemySprites = new Dictionary<string, Texture2D>()
            {
                {"laskUp", laskUp},
                {"laskDown", laskDown },
                {"laskDeathUp", laskDeathUp },
                {"laskDeathDown", laskDeathDown },
                {"laskSpawn", laskSpawn },

                {"tarlitUp", tarlitUp },
                {"tarlitDown", tarlitDown },
                {"tarlitDeathUp", tarlitDeathUp },
                {"tarlitDeathDown", tarlitDeathDown },
                {"tarlitSpawn", tarlitSpawn },

                {"tempusUp", tempusUp },
                {"tempusDown", tempusDown },
                {"tempusDeathUp", tempusDeathUp },
                {"tempusDeathDown", tempusDeathDown },
            };
            var bossAnimations = new Dictionary<string, Animation>()
            {
                {"finalBoss", new Animation(finalBoss, 2) },
                {"healingCrystalDestroyed", new Animation(healingCrystalDestroyed, 5) },
                {"sunAnimation", new Animation(sunSheet, 4) },
            };
            var bossTextures = new Dictionary<string, Texture2D>()
            {
                {"grey", finalBossGrey },
                {"energyBall", energyBall },
                {"healingCrystal", healingCrystal },
            };
            var enemyProjectileSprites = new Dictionary<string, Texture2D>()
            {
                {"tarlitProjectile", tarlitProjectile },
                {"tempusProjectile", tempusProjectile },
            };
            var abilityIcons = new Dictionary<string, Texture2D>()
            {
                {"attack", attackUseIcon },
                {"teleport", teleportUseIcon },
                {"timeStop", timeStopUseIcon },
            };
            var menuSprites = new Dictionary<string, Sprite>()
            {
                {"menuBackground", new Sprite(menuBackground, "all")},
                {"exitButton", new Sprite(exitButton, "all") },
                {"blackScreen", new Sprite(blackScreen, "all") },

                {"backButton", new Sprite(menuButtonMedium, "all") },

                {"soundText", new Sprite("SOUND", "main") },
                {"soundBar", new Sprite(soundBar, "main") },
                {"soundSlider", new Sprite(soundSlider, "main") },
                {"controlsButton", new Sprite(menuButtonMedium, "main") },
                {"creditsButton", new Sprite(menuButtonMedium, "main") },
                {"titleScreenButton", new Sprite(menuButtonMedium, "main") },
                {"saveButton", new Sprite(menuButtonMedium, "main") },
                {"autoSaveButton", new Sprite(menuButtonMedium, "main") },
                {"screenShakeButton", new Sprite(menuButtonMedium, "main") },
                {"mouseDirectButton", new Sprite(menuButtonMedium, "main") },
                {"isFullscreenButton", new Sprite (menuButtonMedium, "main") },

                {"movementControls", new Sprite("MOVEMENT:WASD", "controls") },
                {"menuControl", new Sprite("OPEN/CLOSE MENU:ESC", "controls") },
                {"attackControl", new Sprite("ATTACK:LEFT CLICK", "controls") },
                {"interactControl", new Sprite("INTERACT:RIGHT CLICK", "controls") },
                {"teleportControl", new Sprite("TELEPORT FORWARD:SPACE", "controls") },
                {"teleportHomeControl", new Sprite("TELEPORT HOME:H", "controls") },
                {"stopTimeControl", new Sprite("STOP TIME:Q", "controls") },
                {"startTimeControl", new Sprite("START TIME:E", "controls") },

                {"fontCredits", new Sprite("FONT:Coders Crux BY NALGames", "credits") },
                {"otherCredits", new Sprite("EVERYTHING ELSE:Christopher Sun", "credits") },
            };
            var sounds = new Dictionary<string, SoundEffect>()
            {
                {"slimeRustle", slimeRustle },
                {"slimeDamaged", slimeDamaged },
                {"playerDeath", playerDeath },

                {"buttonHover", buttonHover },
                {"buttonClick", buttonClick },
                {"soundTest", soundTest },
                {"doorOpen", doorOpen },
                {"dialogueSoundEffect", dialogueSoundEffect },
                {"explosionSoundEffect", explosionSoundEffect },

                {"timeStopTick", timeStopTick },
                {"teleportSoundEffect", teleportSoundEffect },
                {"normalAttackWhoosh", normalAttackWhoosh },
                {"fireballWhoosh", fireballWhoosh },

                {"forestBackgroundMusic", forestBackgroundMusic },
                {"caveBackgroundMusic", caveBackgroundMusic },
                {"cliffBackgroundMusic", cliffBackgroundMusic },
                {"bossFightBackgroundMusic", bossFightBackgroundMusic },

                {"windHowl", windHowl },
                {"laskHit", laskHit },
                {"laskDeath", laskDeath },

                {"tarlitHit", tarlitHit },
                {"tarlitDeath", tarlitDeath },
                {"tarlitProjectileLaunch", tarlitProjectileLaunch },

                {"tempusDeath", tempusDeath },
                {"tempusProjectileLaunch", tempusProjectileLaunch },

                {"energyBlast", energyBlast },
                {"largeEnergyBlast", largeEnergyBlast },
                {"healingCrystalShatter", healingCrystalShatter },
                {"healingCrystalHit", healingCrystalHit },
            };

            mapController = new MapController(mapTextures);
            currencyController = new CurrencyController(spriteFont, soulOrb);
            shopController = new ShopController(shopTextures, spriteFont);
            dialogueController = new DialogueController(spriteFont, dialogueTextures);
            abilityController = new AbilityController(abilityAnimations);
            abilityIconController = new AbilityIconController(abilityIcons);
            menuController = new MenuController(menuSprites);
            projectileController = new ProjectileController(enemyProjectileSprites);

            soundManager = new SoundManager(sounds);

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

            treeController = new TreeController(treeSprite, treePos);

            player = new Player(playerAnimations, this.graphics, treeController.trees, mapController)
            {
                position = new Vector2(mapController.rightScreenBound / 2, mapController.bottomScreenBound - 70),
                input = new Input()
                {
                    moveUp = Keys.W,
                    moveDown = Keys.S,
                    moveLeft = Keys.A,
                    moveRight = Keys.D,
                    teleport = Keys.Space,
                    teleportHome = Keys.H,
                    stopTime = Keys.Q,
                    startTime = Keys.E,
                },
            };

            attackController = new AttackController(attackSheets);

            enemyController = new EnemyController(enemySprites, graphics, laskSpawnPoints);

            boss = new Boss(bossAnimations, bossTextures);

            if (!File.Exists(PATH))
            {
                Save();
            }
            saveManager = Load();
            TransferSaveData();
            graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                //cheat.Update(gameTime, shopController, currencyController, player, attackController, abilityController, mapController);

                treeController.Update(gameTime, player);

                mapController.Update(gameTime, player, soundManager, attackController, enemyController, abilityController,
                            projectileController, currencyController, cameraController, treeController, boss, spriteFont);

                if (isGameActive)
                {
                    menuController.Update(gameTime, camera, shopController, soundManager, attackController, spriteFont, mapController, player, this);
                    if (!menuController.isMenuOpen)
                    {
                        abilityController.Update(gameTime, player, camera, mapController, currencyController, soundManager, boss);
                        player.Update(gameTime, mapController, soundManager, currencyController, attackController, cameraController);
                        attackController.Update(gameTime, player, enemyController.currentEnemies, soundManager, boss, mapController, 
                            cameraController);
                        enemyController.Update(gameTime, player, mapController, abilityController, soundManager, treeController);
                        projectileController.Update(gameTime, enemyController, player, abilityController, mapController, soundManager);
                        if (mapController.currentMap == Map.temple)
                        {
                            boss.Update(gameTime, projectileController, mapController, player, soundManager, enemyController, cameraController);
                        }

                        shopController.Update(gameTime, player, mapController, dialogueController, currencyController, abilityController,
                            camera, attackController, abilityIconController, soundManager);
                        dialogueController.Update(gameTime, player, mapController, shopController, soundManager, abilityController, cameraController);

                        cameraController.Update(gameTime, player, mapController, boss, soundManager);
                    }
                    currencyController.Update(gameTime, enemyController.currentEnemies, player, camera);
                    abilityIconController.Update(gameTime, camera, abilityController, attackController.timer);
                }
                soundManager.Update(gameTime, mapController, attackController, camera, boss);
                if (mapController.currentMap == Map.titleScreen)
                {
                    projectileController.Update(gameTime, enemyController, player, abilityController, mapController, soundManager);
                }

                if (doChangeFullscreen)
                {
                    graphics.ToggleFullScreen();
                    
                    doChangeFullscreen = false;
                }

                saveManager.Update(gameTime, abilityIconController, shopController);
                if (doSave)
                {
                    Save();
                    doSave = false;
                }
                if (doTransferSaveData)
                {
                    TransferSaveData();
                    doTransferSaveData = false;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(this.camera, SpriteSortMode.FrontToBack);

            Vector2 mousePos = new Vector2(camera.Position.X - camera.Width / 2 + Mouse.GetState().Position.X,
                camera.Position.Y - camera.Height / 2 + Mouse.GetState().Position.Y);
            spriteBatch.Draw(cursor, mousePos, null, Color.White, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 1f);

            //cheat.Draw(spriteBatch, spriteFont, shopController, currencyController, player, abilityController);

            mapController.Draw(spriteBatch, cameraController, player, spriteFont);
            
            if (isGameActive)
            {
                player.Draw(spriteBatch);
                attackController.Draw(spriteBatch);
                if (mapController.currentMap != Map.witchHut)
                {
                    enemyController.Draw(spriteBatch);
                }
                if (mapController.currentMap == Map.forest)
                {
                    treeController.Draw(spriteBatch, player);
                }
                else if (mapController.currentMap == Map.temple)
                {
                    boss.Draw(spriteBatch);
                }

                dialogueController.Draw(spriteBatch, camera, mapController);
                shopController.Draw(spriteBatch, camera);
                abilityController.Draw(spriteBatch);
                currencyController.Draw(spriteBatch, player);
                abilityIconController.Draw(spriteBatch, spriteFont);
                menuController.Draw(spriteBatch, spriteFont, camera);
            }
            projectileController.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        public static void ExitGame()
        {
            game1.Exit();
        }
        public void Save()
        {
            saveManager.isFullScreen = graphics.IsFullScreen;

            saveManager.doAutoSave = menuController.doAutoSave;

            saveManager.doShakeCamera = CameraController.doShakeCamera;

            saveManager.hasMetWitch = player.hasMetWitch;
            saveManager.isGhost = player.isGhost;
            saveManager.speed = player.speed;
            saveManager.maxHealth = player.maxHealth;
            saveManager.health = player.health;
            saveManager.regenerationAmount = player.regenerationAmount;
            saveManager.hasMetBossCurrentRun = player.hasMetBossCurrentRun;
            saveManager.hasDefeatedBossCurrentRun = player.hasDefeatedBossCurrentRun;
            saveManager.bossDefeatCount = player.bossDefeatCount;
            saveManager.doMouseDirect = player.doMouseDirect;

            saveManager.damage = attackController.damage;
            saveManager.cooldown = attackController.cooldown;
            saveManager.currentAttack = attackController.currentAttack;

            saveManager.isTeleportUnlocked = abilityController.isTeleportUnlocked;
            saveManager.teleportCooldown = abilityController.teleportCooldown;
            saveManager.isTimeStopUnlocked = abilityController.isTimeStopUnlocked;
            saveManager.timeStopCooldown = abilityController.timeStopCooldown;

            saveManager.souls = currencyController.souls;

            saveManager.shopItemUnlockStatus.Clear();
            saveManager.shopItemUpgradeCounts.Clear();
            foreach (ShopItem shopItem in shopController.shopItems)
            {
                saveManager.shopItemUnlockStatus.Add(shopItem.isUnlocked);
                saveManager.shopItemUpgradeCounts.Add(shopItem.upgradeCount);
            }

            saveManager.masterVolume = soundManager.masterVolume;

            saveManager.isBossGone = boss.isRemoved;

            string serializedText = JsonSerializer.Serialize<SaveManager>(saveManager);
            Trace.WriteLine(serializedText);
            File.WriteAllText(PATH, serializedText);
        }
        private SaveManager Load() 
        {
            string fileContents;
            fileContents = File.ReadAllText(PATH);
            return JsonSerializer.Deserialize<SaveManager>(fileContents);
        }
        private void TransferSaveData()
        {
            graphics.IsFullScreen = saveManager.isFullScreen;

            menuController.doAutoSave = saveManager.doAutoSave;

            CameraController.doShakeCamera = saveManager.doShakeCamera;

            player.hasMetWitch = saveManager.hasMetWitch;
            player.isGhost = saveManager.isGhost;
            player.speed = saveManager.speed;
            player.maxHealth = saveManager.maxHealth;
            player.health = saveManager.health;
            player.regenerationAmount = saveManager.regenerationAmount;
            player.hasMetBossCurrentRun = saveManager.hasMetBossCurrentRun;
            player.hasDefeatedBossCurrentRun = saveManager.hasDefeatedBossCurrentRun;
            player.bossDefeatCount = saveManager.bossDefeatCount;
            player.doMouseDirect = saveManager.doMouseDirect;

            attackController.damage = saveManager.damage;
            attackController.cooldown = saveManager.cooldown;
            attackController.currentAttack = saveManager.currentAttack;

            abilityController.isTeleportUnlocked = saveManager.isTeleportUnlocked;
            abilityController.teleportCooldown = saveManager.teleportCooldown;
            abilityController.isTimeStopUnlocked = saveManager.isTimeStopUnlocked;
            abilityController.timeStopCooldown = saveManager.timeStopCooldown;

            currencyController.souls = saveManager.souls;

            for (int i = 0; i < shopController.shopItems.Count; i++)
            {
                shopController.shopItems[i].isUnlocked = saveManager.shopItemUnlockStatus[i];
                shopController.shopItems[i].upgradeCount = saveManager.shopItemUpgradeCounts[i];
            }

            soundManager.masterVolume = saveManager.masterVolume;

            boss.isRemoved = saveManager.isBossGone;
        }
    }
}