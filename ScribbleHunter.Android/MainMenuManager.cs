using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using ScribbleHunter.Inputs;

namespace ScribbleHunter
{
    class MainMenuManager
    {
        #region Members

        public enum MenuItems { None, Start, Highscores, Instructions, Settings };

        private MenuItems lastPressedMenuItem = MenuItems.None;

        private Texture2D texture;

        private Rectangle ScribbleHunterSource = new Rectangle(0, 0,
                                                          480, 200);
        private Rectangle ScribbleHunterDestination = new Rectangle(0, 100,
                                                               480, 200);

        private Rectangle startSource = new Rectangle(0, 400,
                                                      240, 80);
        private Rectangle startDestination = new Rectangle(120, 320,
                                                           240, 80);

        private Rectangle instructionsSource = new Rectangle(0, 480,
                                                             240, 80);
        private Rectangle instructionsDestination = new Rectangle(120, 415,
                                                                  240, 80);

        private Rectangle highscoresSource = new Rectangle(0, 560,
                                                           240, 80);
        private Rectangle highscoresDestination = new Rectangle(120, 510,
                                                                240, 80);

        private Rectangle settingsSource = new Rectangle(0, 640,
                                                     240, 80);
        private Rectangle settingsDestination = new Rectangle(120, 605,
                                                          240, 80);

        private Rectangle reviewSource = new Rectangle(240, 700,
                                                       100, 100);
        private Rectangle moreGamesSource = new Rectangle(240, 500,
                                                       100, 100);
        private Rectangle moreGamesDestination = new Rectangle(15, 690,
                                                               100, 100);

        private Rectangle reviewDestination = new Rectangle(370, 690,
                                                            100, 100);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private float time = 0.0f;

        private GameInput gameInput;
        private const string StartAction = "Start";
        private const string InstructionsAction = "Instructions";
        private const string HighscoresAction = "Highscores";
        private const string SettingsAction = "Settings";
        private const string ReviewAction = "Review";
        private const string MoreGamesAction = "MoreGames";

        private SpriteFont font;

        #endregion

        #region Constructors

        public MainMenuManager(Texture2D spriteSheet, GameInput input, SpriteFont font)
        {
            this.texture = spriteSheet;
            this.gameInput = input;
            this.font = font;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            gameInput.AddTouchGestureInput(StartAction,
                                           GestureType.Tap,
                                           startDestination);
            gameInput.AddTouchGestureInput(InstructionsAction,
                                           GestureType.Tap,
                                           instructionsDestination);
            gameInput.AddTouchGestureInput(HighscoresAction,
                                           GestureType.Tap,
                                           highscoresDestination);
            gameInput.AddTouchGestureInput(SettingsAction,
                                           GestureType.Tap,
                                           settingsDestination);
            gameInput.AddTouchGestureInput(ReviewAction,
                                           GestureType.Tap,
                                           reviewDestination);

            gameInput.AddTouchGestureInput(MoreGamesAction,
                                           GestureType.Tap,
                                           moreGamesDestination);
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            time = (float)gameTime.TotalGameTime.TotalSeconds;

            this.handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             ScribbleHunterDestination,
                             ScribbleHunterSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                             startDestination,
                             startSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                             highscoresDestination,
                             highscoresSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                                instructionsDestination,
                                instructionsSource,
                                Color.White * opacity);

            spriteBatch.Draw(texture,
                             reviewDestination,
                             reviewSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                             settingsDestination,
                             settingsSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                            moreGamesDestination,
                            moreGamesSource,
                            Color.White * opacity);
        }

        private void handleTouchInputs()
        {
            // Start
            if (gameInput.IsPressed(StartAction))
            {
                this.lastPressedMenuItem = MenuItems.Start;
                SoundManager.PlayPaperSound();
            }
            // Highscores
            else if (gameInput.IsPressed(HighscoresAction))
            {
                this.lastPressedMenuItem = MenuItems.Highscores;
                SoundManager.PlayPaperSound();
            }
            // Instructions
            else if (gameInput.IsPressed(InstructionsAction))
            {
                this.lastPressedMenuItem = MenuItems.Instructions;
                SoundManager.PlayPaperSound();
            }
            // Settings
            else if (gameInput.IsPressed(SettingsAction))
            {
                this.lastPressedMenuItem = MenuItems.Settings;
                SoundManager.PlayPaperSound();
            }
            // More games
            else if (gameInput.IsPressed(MoreGamesAction))
            {
                // TODO open more games in store
                SoundManager.PlayPaperSound();

            }
            else if (gameInput.IsPressed(ReviewAction))
            {
                // TODO open review app in store
                SoundManager.PlayPaperSound();
            }
            else
            {
                this.lastPressedMenuItem = MenuItems.None;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.lastPressedMenuItem = (MenuItems)Enum.Parse(lastPressedMenuItem.GetType(), reader.ReadLine(), false);
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.time = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(lastPressedMenuItem);
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(time);
        }

        #endregion

        #region Properties

        public MenuItems LastPressedMenuItem
        {
            get
            {
                return this.lastPressedMenuItem;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                }
            }
        }

        #endregion
    }
}