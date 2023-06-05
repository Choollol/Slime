using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Slime.Sprites;
using System.Collections;
using System.Diagnostics;
using Comora;
using Slime.Controllers;

namespace Slime.Managers
{
    public class SoundManager
    {
        private Random rand = new Random();
        private MouseState mStateOld;
        private List<bool> ambienceBools = new List<bool>();
        private List<float> ambienceTimers = new List<float>();
        private List<SoundEffectInstance> currentEnemyHitSounds = new List<SoundEffectInstance>();
        private SoundEffect currentEnemyHitSound;
        private Dictionary<string, SoundEffectInstance> soundInstances = new Dictionary<string, SoundEffectInstance>();
        private List<Rectangle> buttonRects = new List<Rectangle>();
        private int currentButtonUnderMouseIndex;
        private int oldButtonUnderMouseIndex;

        public Dictionary<string, SoundEffect> sounds;
        public float masterVolume = 0.5f;
        public SoundEffect currentMusic;
        public bool doPlayMusic = false;
        public float musicTimer;
        public int enemyHitCount = -1;

        private float randomPitch
        {
            get { return (float)rand.Next(-50, 50) / 100; }
        }
        public SoundManager(Dictionary<string, SoundEffect> soundEffects)
        {
            sounds = soundEffects;
            foreach (var sound in sounds)
            {
                soundInstances.Add(sound.Key, sound.Value.CreateInstance());
            }
            musicTimer = rand.Next(7, 14);
            ambienceTimers.Add(rand.Next(90, 120));
            ambienceBools.Add(false);
        }
        private void mapChangeSounds(MapController mapController, AttackController attackController)
        {
            if (mapController.currentMap != mapController.newMap || mapController.doHealPlayer)
            {
                if (!((mapController.currentMap == Map.cave1 && mapController.newMap == Map.cave2) || 
                    mapController.currentMap == Map.cave2 || mapController.currentMap == Map.cave3) || mapController.doHealPlayer || 
                    mapController.newMap == Map.witchHut)
                {
                    stopAllSoundInstances();
                    for (int i = 0; i < ambienceTimers.Count; i++)
                    {
                        ambienceTimers[i] = rand.Next(20, 40);
                        ambienceBools[i] = false;
                    }
                }
                musicTimer = rand.Next(5, 120);
            }
        }
        public void playSound(string key)
        {
            sounds[key].Play();
        }
        public void playSoundRandomPitch(string key, int lowerBound, int upperBound)
        {
            float randomPitch = (float)rand.Next(lowerBound, upperBound) / 100;
            sounds[key].Play(1, randomPitch, 0f);
        }
        public void playSoundInstance(string key)
        {
            if (!soundInstances.ContainsKey(key))
            {
                soundInstances.Add(key, sounds[key].CreateInstance());
            }
            soundInstances[key].Play();
        }
        public void stopSoundInstance(string key)
        {
            if (soundInstances.ContainsKey(key))
            {
                soundInstances[key].Stop();
                soundInstances.Remove(key);
            }
        }
        public void stopAllSoundInstances()
        {
            foreach (var soundInstance in soundInstances)
            {
                soundInstance.Value.Stop();
            }
        }
        public void addButtonRect(Rectangle buttonRect)
        {
            if (!buttonRects.Contains(buttonRect))
            {
                buttonRects.Add(buttonRect);
            }
        }
        public void clearButtonRects()
        {
            buttonRects.Clear();
            currentButtonUnderMouseIndex = -1;
        }
        private void playButtonSounds(MouseState mState, Rectangle mRect)
        {
            currentButtonUnderMouseIndex = -1;
            for (int i = 0; i < buttonRects.Count; i++)
            {
                if (mRect.Intersects(buttonRects[i]))
                {
                    currentButtonUnderMouseIndex = i;
                }
            }
            if (currentButtonUnderMouseIndex != oldButtonUnderMouseIndex && currentButtonUnderMouseIndex != -1)
            {
                playSoundRandomPitch("buttonHover", -40, 40);
            }
            oldButtonUnderMouseIndex = currentButtonUnderMouseIndex;
        }
        public void addEnemyHit()
        {
            if (currentEnemyHitSounds.Count < 1 && currentEnemyHitSound != null)
            {
                currentEnemyHitSounds.Add(currentEnemyHitSound.CreateInstance());
                enemyHitCount++;
            }
        }
        private void enemyHit(MapController mapController)
        {
            foreach (var enemyHitSound in currentEnemyHitSounds.ToArray())
            {
                enemyHitSound.Pitch = randomPitch;
                enemyHitSound.Play();
                currentEnemyHitSounds.Remove(enemyHitSound);
            }
        }
        private void playMusic(MapController mapController, float dt, Boss boss)
        {
            if (musicTimer > 0)
            {
                musicTimer -= dt;
            }
            if (doPlayMusic && musicTimer <= 0)
            {
                if (mapController.currentMap == Map.forest)
                {
                    soundInstances["forestBackgroundMusic"].Play();
                }
                else if (mapController.currentMap == Map.cave1 || mapController.currentMap == Map.cave2 ||
                    mapController.currentMap == Map.cave3)
                {
                    soundInstances["caveBackgroundMusic"].Play();
                }
                else if (mapController.currentMap == Map.cliff)
                {
                    soundInstances["cliffBackgroundMusic"].Play();
                }
                doPlayMusic = false;
            }
            if (mapController.currentMap == Map.temple && mapController.newMap == Map.temple && boss.doAttack && !boss.isRemoved)
            {
                playSoundInstance("bossFightBackgroundMusic");
            }
        }
        private void ambienceSounds(float dt, MapController mapController)
        {
            for (int i = 0; i < ambienceTimers.Count; i++)
            {
                if (ambienceTimers[i] > 0)
                {
                    ambienceTimers[i] -= dt;
                }
                if (ambienceTimers[i] <= 0)
                {
                    ambienceBools[i] = true;
                }
            }

            if (mapController.currentMap == Map.forest && ambienceBools[0])
            {
                playSoundInstance("windHowl");
                ambienceTimers[0] = rand.Next(20, 40);
                ambienceBools[0] = false;
            }
        }
        public void Update(GameTime gameTime, MapController mapController, AttackController attackController, Camera camera, Boss boss)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mState = Mouse.GetState();
            Rectangle mRect = new Rectangle((int)(camera.Position.X - camera.Width / 2 + mState.X),
                (int)(camera.Position.Y - camera.Height / 2 + mState.Y), 1, 1);

            if (masterVolume > 1)
            {
                masterVolume = 1;
            }
            else if (masterVolume < 0)
            {
                masterVolume = 0;
            }
            SoundEffect.MasterVolume = masterVolume;

            if (gameTime.TotalGameTime.TotalSeconds == 0)
            {
                doPlayMusic = true;
            }
            if (mapController.currentMap == Map.forest)
            {
                currentEnemyHitSound = sounds["laskHit"];
            }
            else if (mapController.currentMap == Map.cave1 || mapController.currentMap == Map.cave2 ||
                mapController.currentMap == Map.cave3)
            {
                currentEnemyHitSound = sounds["tarlitHit"];
            }
            else if (mapController.currentMap == Map.cliff)
            {
                currentEnemyHitSound = null;
            }
            if (enemyHitCount >= 0)
            {
                enemyHit(mapController);
            }
            mapChangeSounds(mapController, attackController);
            playMusic(mapController, dt, boss);
            ambienceSounds(dt, mapController);
            playButtonSounds(mState, mRect);
            mStateOld = mState;
        }
    }
}
