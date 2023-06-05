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
using System.Linq;

namespace Slime.Controllers
{
    internal class MenuController
    {
        private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        private Dictionary<string, Sprite> currentSprites = new Dictionary<string, Sprite>();
        private KeyboardState kStateOld;
        private MouseState mStateOld;
        private float largeTextScale = 2f;
        private float mediumTextScale = 1.5f;
        private bool doMoveSoundSlider = false;
        private bool doMoveSoundSliderOld = false;
        private string currentMenu = "main";
        private int controlsMenuSpacing = 80;
        private int spriteIDCount = 0;
        private int buttonYOffset = 40;
        private int buttonXOffset = 80;

        public bool isMenuOpen = false;
        public bool doAutoSave = true;
        public MenuController(Dictionary<string, Sprite> menuSprites)
        {
            foreach (var sprite in menuSprites)
            {
                sprites.Add(sprite.Key, sprite.Value);
                sprites[sprite.Key].numID = spriteIDCount;
                spriteIDCount++;
            }
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, Camera camera)
        {
            if (isMenuOpen)
            {
                foreach (Sprite sprite in currentSprites.Values)
                {
                    sprite.Draw(spriteBatch, spriteFont);
                }
            }
        }
        public void Update(GameTime gameTime, Camera camera, ShopController shopController, SoundManager soundManager,
            AttackController attackController, SpriteFont spriteFont, MapController mapController, Player player, Game1 game1)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            Rectangle mRect = new Rectangle((int)(camera.Position.X - camera.Width / 2 + mState.Position.X),
                (int)(camera.Position.Y - camera.Height / 2 + mState.Position.Y), 1, 1);

            if (!shopController.isShopOpen && kState.IsKeyDown(Keys.Escape) && kStateOld.IsKeyUp(Keys.Escape) &&
                !mapController.isInScreenTransit)
            {
                if (!isMenuOpen)
                {
                    isMenuOpen = true;
                    soundManager.stopSoundInstance("slimeRustle");
                }
                else if (isMenuOpen)
                {
                    if (currentMenu == "main")
                    {
                        isMenuOpen = false;
                    }
                    else
                    {
                        currentMenu = "main";
                    }
                    soundManager.clearButtonRects();
                }
            }

            float leftButtonX = sprites["menuBackground"].position.X + buttonXOffset;
            float middleButtonX = sprites["menuBackground"].position.X + sprites["menuBackground"].width / 2 - sprites["backButton"].width / 2;
            float rightButtonX = sprites["menuBackground"].position.X + sprites["menuBackground"].width - sprites["backButton"].width -
                buttonXOffset;
            float bottomButtonY = sprites["menuBackground"].position.Y + sprites["menuBackground"].height - sprites["backButton"].height -
                buttonYOffset;
            float middleButtonY = bottomButtonY - 100 - buttonYOffset;
            float topButtonY = middleButtonY - 100 - buttonYOffset;

            foreach (var sprite in sprites)
            {
                sprite.Value.Update(gameTime, spriteFont);
                sprite.Value.depth = 0.91f;
                if (sprite.Key == "menuBackground")
                {
                    sprite.Value.position = new Vector2(camera.Position.X - sprites["menuBackground"].width / 2,
                        camera.Position.Y - sprites["menuBackground"].height / 2);
                    sprite.Value.depth = 0.9f;
                }
                else if (sprite.Key == "blackScreen")
                {
                    sprite.Value.position = new Vector2(camera.Position.X - camera.Width / 2,
                        camera.Position.Y - camera.Height / 2);
                    sprite.Value.depth = 0.81f;
                    sprite.Value.opacity = 0.6f;
                }
                else if (sprite.Key == "exitButton")
                {
                    sprite.Value.scale = 1.6f;
                    sprite.Value.position = new Vector2(sprites["menuBackground"].position.X + sprites["menuBackground"].width -
                        sprites["exitButton"].width / 2,
                        sprites["menuBackground"].position.Y - sprites["exitButton"].height / 2);
                    sprite.Value.opacity = 0.6f;
                }
                else if (sprite.Key == "backButton")
                {
                    sprite.Value.position = new Vector2(rightButtonX, bottomButtonY);
                    if (currentMenu == "main")
                    {
                        sprite.Value.textScale = mediumTextScale;
                        sprite.Value.text = "EXIT GAME";
                    }
                    else
                    {
                        sprite.Value.textScale = largeTextScale;
                        sprite.Value.text = "BACK";
                    }
                }
                if (sprite.Value.ID == "main")
                {
                    if (sprite.Key == "soundBar")
                    {
                        sprite.Value.position = new Vector2(sprites["menuBackground"].position.X + sprites["menuBackground"].width / 2
                            - sprite.Value.width / 2, sprites["menuBackground"].position.Y + 200);
                    }
                    else if (sprite.Key == "soundSlider")
                    {
                        sprite.Value.position = new Vector2(sprites["soundBar"].position.X + soundManager.masterVolume * 1000 -
                            sprite.Value.width / 2,
                            sprites["soundBar"].position.Y + sprites["soundBar"].height / 2 - sprite.Value.height / 2);
                        sprite.Value.depth = 0.92f;
                    }
                    else if (sprite.Key == "soundText")
                    {
                        sprite.Value.textScale = largeTextScale;
                        sprite.Value.position = new Vector2(sprites["soundBar"].position.X +
                            sprites["soundBar"].width / 2 - spriteFont.MeasureString("SOUND").X / 2 * largeTextScale,
                            sprites["soundBar"].position.Y - 100);
                    }
                    else if (sprite.Key == "controlsButton")
                    {
                        sprite.Value.position = new Vector2(leftButtonX, middleButtonY);
                        sprite.Value.text = "CONTROLS";
                        sprite.Value.textScale = mediumTextScale;
                    }
                    else if (sprite.Key == "creditsButton")
                    {
                        sprite.Value.position = new Vector2(middleButtonX, bottomButtonY);
                        sprite.Value.text = "CREDITS";
                        sprite.Value.textScale = mediumTextScale;
                    }
                    else if (sprite.Key == "titleScreenButton")
                    {
                        sprite.Value.position = new Vector2(leftButtonX, bottomButtonY);
                        sprite.Value.text = "TITLE SCREEN";
                        sprite.Value.textScale = mediumTextScale - 0.25f;
                    }
                    else if (sprite.Key == "saveButton")
                    {
                        sprite.Value.position = new Vector2(middleButtonX, middleButtonY);
                        sprite.Value.text = "SAVE";
                        sprite.Value.textScale = mediumTextScale;
                    }
                    else if (sprite.Key == "autoSaveButton")
                    {
                        sprite.Value.position = new Vector2(rightButtonX, middleButtonY);
                        if (doAutoSave)
                        {
                            sprite.Value.text = "AUTOSAVE:ON";
                        }
                        else
                        {
                            sprite.Value.text = "AUTOSAVE:OFF";
                        }
                        sprite.Value.textScale = mediumTextScale - 0.2f;
                    }
                    else if (sprite.Key == "screenShakeButton")
                    {
                        sprite.Value.position = new Vector2(leftButtonX, topButtonY);
                        if (CameraController.doShakeCamera)
                        {
                            sprite.Value.text = "SCREENSHAKE:ON";
                        }
                        else
                        {
                            sprite.Value.text = "SCREENSHAKE:OFF";
                        }
                        sprite.Value.textScale = mediumTextScale - 0.5f;
                    }
                    else if (sprite.Key == "mouseDirectButton")
                    {
                        sprite.Value.position = new Vector2(middleButtonX, topButtonY);
                        if (player.doMouseDirect)
                        {
                            sprite.Value.text = "ATTACK:\n MOUSE";
                        }
                        else
                        {
                            sprite.Value.text = " ATTACK:\nDIRECTION";
                        }
                        sprite.Value.textScale = mediumTextScale - 0.3f;
                    }
                    else if (sprite.Key == "isFullscreenButton")
                    {
                        sprite.Value.position = new Vector2(rightButtonX, topButtonY);
                        if (game1.isFullScreen)
                        {
                            sprite.Value.text = "FULLSCREEN:ON";
                        }
                        else
                        {
                            sprite.Value.text = "FULLSCREEN:OFF";
                        }
                        sprite.Value.textScale = mediumTextScale - 0.4f;
                    }
                }
                else if (sprite.Value.ID == "controls")
                {
                    sprite.Value.textScale = largeTextScale;
                    if (sprite.Key == "movementControls")
                    {
                        sprite.Value.position = new Vector2(sprites["menuBackground"].position.X + 80,
                            sprites["menuBackground"].position.Y + 60);
                    }
                    else
                    {
                        sprite.Value.position = new Vector2(sprites.ElementAt(sprite.Value.numID - 1).Value.position.X,
                            sprites.ElementAt(sprite.Value.numID - 1).Value.position.Y + controlsMenuSpacing);
                    }
                }
                else if (sprite.Value.ID == "credits")
                {
                    sprite.Value.textScale = mediumTextScale;
                    if (sprite.Key == "fontCredits")
                    {
                        sprite.Value.position = new Vector2(sprites["movementControls"].position.X,
                            sprites["menuBackground"].position.Y + 80);
                    }
                    else if (sprite.Key == "otherCredits")
                    {
                        sprite.Value.position = new Vector2(sprites.ElementAt(sprite.Value.numID - 1).Value.position.X,
                            sprites.ElementAt(sprite.Value.numID - 1).Value.position.Y + 80);
                    }
                }

                if (isMenuOpen)
                {
                    if (mRect.Intersects(sprite.Value.Rectangle))
                    {
                        if (sprite.Key == "exitButton")
                        {
                            soundManager.addButtonRect(sprite.Value.Rectangle);
                            sprite.Value.opacity = 1f;
                            if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                            {
                                soundManager.playSound("buttonClick");
                                isMenuOpen = false;
                                if (attackController.timer <= 0)
                                {
                                    attackController.timer = 0.1f;
                                }
                                currentMenu = "main";
                            }
                        }
                        if (sprite.Key == "backButton")
                        {
                            soundManager.addButtonRect(sprite.Value.Rectangle);
                            sprite.Value.color = Color.Bisque;
                            sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                            if (mState.LeftButton == ButtonState.Released && mStateOld.LeftButton == ButtonState.Pressed)
                            {
                                if (currentMenu == "main")
                                {
                                    if (doAutoSave)
                                    {
                                        Game1.doSave = true;
                                    }
                                    Game1.ExitGame();
                                }
                                else
                                {
                                    soundManager.playSound("buttonClick");
                                    currentMenu = "main";
                                }
                            }
                        }
                        if (currentMenu == "main")
                        {
                            if (sprite.Key == "soundBar")
                            {
                                if (mState.LeftButton == ButtonState.Pressed)
                                {
                                    soundManager.masterVolume = (mRect.Left - sprites["soundBar"].position.X) / sprites["soundBar"].width;
                                    doMoveSoundSlider = true;
                                }
                            }
                            else if (sprite.Key == "controlsButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    currentMenu = "controls";
                                    soundManager.playSound("buttonClick");
                                    soundManager.clearButtonRects();
                                }
                            }
                            else if (sprite.Key == "creditsButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    currentMenu = "credits";
                                    soundManager.playSound("buttonClick");
                                    soundManager.clearButtonRects();
                                }
                            }
                            else if (sprite.Key == "titleScreenButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    soundManager.playSound("buttonClick");
                                    soundManager.clearButtonRects();
                                    mapController.newMap = Map.titleScreen;
                                    isMenuOpen = false;
                                    if (doAutoSave)
                                    {
                                        Game1.doSave = true;
                                    }
                                }
                            }
                            else if (sprite.Key == "saveButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    soundManager.playSound("buttonClick");
                                    Game1.doSave = true;
                                }
                            }
                            else if (sprite.Key == "autoSaveButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    soundManager.playSound("buttonClick");
                                    if (doAutoSave)
                                    {
                                        doAutoSave = false;
                                    }
                                    else
                                    {
                                        doAutoSave = true;
                                    }
                                }
                            }
                            else if (sprite.Key == "screenShakeButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    soundManager.playSound("buttonClick");
                                    if (CameraController.doShakeCamera)
                                    {
                                        CameraController.doShakeCamera = false;
                                    }
                                    else
                                    {
                                        CameraController.doShakeCamera = true;
                                    }
                                }
                            }
                            else if (sprite.Key == "mouseDirectButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    soundManager.playSound("buttonClick");
                                    if (player.doMouseDirect)
                                    {
                                        player.doMouseDirect = false;
                                    }
                                    else
                                    {
                                        player.doMouseDirect = true;
                                    }
                                }
                            }
                            else if (sprite.Key == "isFullscreenButton")
                            {
                                soundManager.addButtonRect(sprite.Value.Rectangle);
                                sprite.Value.color = Color.Bisque;
                                sprite.Value.position = new Vector2(sprite.Value.position.X, sprite.Value.position.Y - 5);
                                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                                {
                                    soundManager.playSound("buttonClick");
                                    Game1.doChangeFullscreen = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        sprite.Value.color = Color.White;
                    }
                }
            }
            if (currentMenu == "main")
            {
                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Pressed && doMoveSoundSlider)
                {
                    soundManager.masterVolume = (mRect.Left - sprites["soundBar"].position.X) / sprites["soundBar"].width;
                }
                else
                {
                    doMoveSoundSlider = false;
                }
                if (!doMoveSoundSlider && doMoveSoundSliderOld)
                {
                    soundManager.playSound("soundTest");
                }
            }

            currentSprites.Clear();
            foreach (var sprite in sprites)
            {
                if (sprite.Value.ID == "all" || currentMenu == sprite.Value.ID)
                {
                    currentSprites[sprite.Key] = sprites[sprite.Key];
                }
            }

            doMoveSoundSliderOld = doMoveSoundSlider;
            kStateOld = kState;
            mStateOld = mState;
        }
    }
}
