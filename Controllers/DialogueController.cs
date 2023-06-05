using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slime.Managers;
using Slime.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Slime.Controllers
{
    public class DialogueController
    {
        private KeyboardState kStateOld;
        private MouseState mStateOld;
        private float textScale = 1.6f;
        private int messageCount = -1;
        private SpriteFont spriteFont;
        private Dictionary<string, Texture2D> textures;
        private Random rand = new Random();
        private int letterCount = 0;
        private string currentMessage;
        private string wrappedMessage;
        private List<List<string>> witchEncounterDialogue = new List<List<string>>
        {
            //0
            new List<string>()
            {
                "Oh? A slime?", "I haven't seen one of you in a long time.", "Well, I haven't seen anyone in a long time.",
                "Anyway, you're welcome to make yourself at home here. I wouldn't mind the company.",
                "Oh and while you're here, could you do me a favor?", "You might have seen the creatures with red eyes outside. " +
                "Could you destroy them and collect their life force for me using one of my extra orbs?", "In return, I can make you more powerful.",
                "The more powerful I make you though, the more life force you will have to bring me. A fair trade, I believe.",
                "Well, just think about it. Take this orb for now.",
                "In the meantime, you can decide which upgrades you would like."
            },
            //1
            new List<string>()
            {
                "Oh? A slime?", "I haven't seen one of you in a long time.", "Well, I haven't seen anyone in a long time.",
                "Anyway, you're welcome to make yourself at home here. I wouldn't mind the company.",
                "Oh and while you're here, could you do me a favor?", "You might have seen the creatures with red eyes outside. " +
                "Could you destroy them and collect their life force for me using one of my extra orbs?", "In return, I can make you more powerful.",
                "The more powerful I make you though, the more life force you will have to bring me. A fair trade, I believe.",
                "Well, just think about it. Take this orb for now.",
                "In the meantime, you can decide which upgrades you would like."
            },
            //2
            new List<string>()
            {
                "Oh? A slime?", "I haven't seen one of you in a long time.", "Well, I haven't seen anyone in a long time.", 
                "Wait, This feels familiar...",
                "...Anyway, you're welcome to make yourself at home here. I wouldn't mind the company.",
                "Oh and while you're here, could you do me a favor?", "You might have seen the creatures with red eyes outside. " +
                "Could you destroy them and collect their life force for me using one of my extra orbs?", "In return, I can make you more powerful.",
                "The more powerful I make you though, the more life force you will have to bring me. A fair trade, I believe.",
                "Well, just think about it. Take this orb for now.",
                "In the meantime, you can decide which upgrades you would like."
            },
            //3
            new List<string>()
            {
                "Have I...met you before?", "I get the strong feeling that I have.", "I suspect you know what happens next then."
            },
            //4
            new List<string>()
            {
                "Aha!", "I knew it!", "You're stuck in a time loop!", "...right?", "I have it all wrong don't I?", 
                "Oh well. Back to researching I go."
            },
            //5
            new List<string>()
            {
                "What is happening?", "I don't understand.", "Do you?", "...alright."
            },
            //6
            new List<string>()
            {
                "Ah, there you are.", 
            },
            //7
            new List<string>()
            {
                "I still don't know what's going on, but I remember you for some reason.", "Maybe you're special.", 
                "But you're just a slime.", "Unless you aren't?", "Is that even possible?"
            }
        };
        private List<string> witchRepeatDialogue = new List<string>() { "Welcome back.", "Hello.", "...", "Greetings." };
        private List<List<string>> bossEncounterDialogue = new List<List<string>>()
        {
            //0
            new List<string>()
            {
                "So you finally made it.", "I am the one who holds your fabric of reality together.", "You may call me your Weaver.", 
                "None other than I are permitted here.", "Leave or suffer the consequences.", "You have been warned."
            },
            //1
            new List<string>()
            {
                "We meet again."
            },
            //2
            new List<string>()
            {
                "Why must you be so stubborn?"
            },
            //3
            new List<string>()
            {
                "What is the point in returning?", "You know the result will be be the same.", 
            },
            //4
            new List<string>()
            {
                "Annoying gnat."
            },
            //5
            new List<string>()
            {
                "..."
            },
            //6
            new List<string>()
            {
                "..."
            },
            //7
            new List<string>()
            {
                "This is your last chance to turn away."
            }
        };
        private List<List<string>> bossDeathDialogue = new List<List<string>>()
        {
            //0
            new List<string>()
            {
                "*sigh*", "What a pain."
            },
            //1
            new List<string>()
            {
                "Not again..."
            },
            //2
            new List<string>()
            {
                "What a handful you are."
            },
            //3
            new List<string>()
            {
                "Go.", "Stay AWAY this time."
            },
            //4
            new List<string>()
            {
                "Why?"
            },
            //5
            new List<string>()
            {
                "..."
            },
            //6
            new List<string>()
            {
                "..."
            },
            //7
            new List<string>()
            {
                "Enough.", "I'm leaving.", "It's your responsibility to sustain this reality now."
            },
        };

        public List<string> messages;
        public bool isDialogueOpen = false;
        public bool openShop = false;
        public DialogueController(SpriteFont font, Dictionary<string, Texture2D> dialogueTextures)
        {
            spriteFont = font;
            textures = dialogueTextures;
        }
        private void witchDialogue(MouseState mState, Player player, MapController mapController,
            ShopController shopController, Rectangle witchRect, AbilityController abilityController)
        {
            if (mapController.currentMap == Map.witchHut && player.Rectangle.Intersects(witchRect) &&
                mState.RightButton == ButtonState.Pressed && mStateOld.RightButton == ButtonState.Released &&
                !shopController.isShopOpen && messageCount == -1)
            {
                player.canAttack = false;
                player.isAttacking = false;
                player.canMove = false;
                abilityController.canUseAbilities = false;
                isDialogueOpen = true;

                if (!player.hasMetWitch)
                {
                    messages = witchEncounterDialogue[player.bossDefeatCount];
                }
                else
                {
                    int messageNumber = rand.Next(0, witchRepeatDialogue.Count);
                    messages = new List<string>()
                    {
                        witchRepeatDialogue[messageNumber],
                    };
                }
            }
        }
        private void bossDialogue(CameraController cameraController, Player player)
        {
            if (cameraController.hasReachedCutsceneTarget)
            {
                isDialogueOpen = true;
                messageCount = 0;
                cameraController.hasReachedCutsceneTarget = false;
                if (!player.hasDefeatedBossCurrentRun)
                {
                    messages = bossEncounterDialogue[player.bossDefeatCount];
                }
                else
                {
                    messages = bossDeathDialogue[player.bossDefeatCount];
                }
                wrappedMessage = wrapDialogue(messages[messageCount]);
            }
        }
        private string wrapDialogue(string dialogue)
        {
            int maxTextWidth = 600 * 2;
            if (spriteFont.MeasureString(dialogue).X * textScale < maxTextWidth)
            {
                return dialogue;
            }
            StringBuilder wrapped = new StringBuilder();
            string line = "";
            List<string> wordList = dialogue.Split(" ").ToList();
            float linewidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X * textScale;
            for (int i = 0; i < wordList.Count; i++)
            {
                Vector2 size = spriteFont.MeasureString(wordList[i]) * textScale;
                if (linewidth + size.X < maxTextWidth)
                {
                    linewidth += size.X + spaceWidth;
                }
                else
                {
                    wrapped.Append("\n");
                    linewidth = size.X + spaceWidth;
                }
                wrapped.Append(wordList[i]);
                wrapped.Append(" ");
            }

            return wrapped.ToString();
        }
        private void messageUpdate(KeyboardState kState, MouseState mState, MapController mapController, SoundManager soundManager, 
            CameraController cameraController, Player player)
        {
            if ((kState.IsKeyDown(Keys.Space) && kStateOld.IsKeyUp(Keys.Space) ||
                mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released ||
                mState.RightButton == ButtonState.Pressed && mStateOld.RightButton == ButtonState.Released)
                && messageCount < messages.Count && messages != null && isDialogueOpen)
            {
                if (messageCount != -1 && letterCount < messages[messageCount].Length * 2)
                {
                    letterCount = wrappedMessage.Length * 2;
                }
                else
                {
                    letterCount = 0;
                    messageCount++;
                    if (messageCount < messages.Count)
                    {
                        wrappedMessage = wrapDialogue(messages[messageCount]);
                    }
                }
            }
            if (messageCount < messages.Count && messageCount != -1 && letterCount < wrappedMessage.Length * 2 + 1)
            {
                soundManager.playSoundInstance("dialogueSoundEffect");
                currentMessage = wrappedMessage.Substring(0, letterCount / 2);
                letterCount++;
            }
            else
            {
                soundManager.stopSoundInstance("dialogueSoundEffect");
            }
            if (messageCount == messages.Count)
            {
                isDialogueOpen = false;
                messageCount = -1;
                if (mapController.currentMap == Map.witchHut)
                {
                    openShop = true;
                    if (!player.hasMetWitch)
                    {
                        player.hasMetWitch = true;
                    }
                }
                if (cameraController.doPlayCutscene)
                {
                    cameraController.doCutsceneReturn = true;
                    cameraController.cutsceneStartPos = cameraController.position;
                    cameraController.cutsceneTargetPos = player.center;
                    cameraController.doMoveCutsceneCamera = true;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Camera camera, MapController MapController)
        {
            Vector2 dialogueBoxPos = new Vector2(camera.Position.X - textures["dialogueBox"].Width / 2,
                camera.Position.Y + camera.Height / 2 - textures["dialogueBox"].Height - 30);
            if (isDialogueOpen)
            {
                spriteBatch.Draw(textures["dialogueBox"], dialogueBoxPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
                spriteBatch.DrawString(spriteFont, currentMessage,
                    new Vector2(dialogueBoxPos.X + 60, dialogueBoxPos.Y + 50), Color.Black, 0f, Vector2.Zero,
                    textScale, SpriteEffects.None, 1f);
                if (MapController.currentMap == Map.witchHut)
                {
                    spriteBatch.Draw(textures["witchPortraitNormal"], new Vector2(dialogueBoxPos.X + 1310, dialogueBoxPos.Y + 111),
                        null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }
        }
        public void Update(GameTime gameTime, Player player, MapController mapController, ShopController ShopController, 
            SoundManager soundManager, AbilityController abilityController, CameraController cameraController)
        {
            MouseState mState = Mouse.GetState();
            KeyboardState kState = Keyboard.GetState();
            Rectangle witchRect = new Rectangle((int)mapController.witchPos.X - 20, (int)mapController.witchPos.Y - 20,
            200, 268);

            witchDialogue(mState, player, mapController, ShopController, witchRect, abilityController);
            bossDialogue(cameraController, player);
            if (messages != null)
            {
                messageUpdate(kState, mState, mapController, soundManager, cameraController, player);
            }

            kStateOld = kState;
            mStateOld = mState;
        }
    }
}
