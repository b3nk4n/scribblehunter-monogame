using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using ScribbleHunter.Inputs;
using Android.Window;
using Java.Interop;

namespace ScribbleHunter.Android
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ScribbleHunter : Game, IBackButtonPressedCallback
    {
        /*
         * The game's fixed width and heigth of the screen.
         * Do NOT change this number, because other related code uses hard
         * coded values similar to this one and assumes that this is the screen
         * dimension. This value values was fixed in Windows Phone,
         * but there are very different screen out there in Android.
         */
        const int WIDTH = 480;
        const int HEIGHT = 800;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Matrix screenScaleMatrix;
        Vector2 screenScaleVector;

        private const string HighscoreText = "Personal Highscore!";
        private const string GameOverText = "GAME OVER!";

        private const string ContinueText = "Push to continue...";
        private string VersionText;
        private const string MusicByText = "Music by";
        private const string MusicCreatorText = "PLSQMPRFKT";
        private const string CreatorText = "by Benjamin Kan";

        enum GameStates
        {
            TitleScreen, MainMenu, Highscores, Instructions, Settings, Playing, Paused, GameOver,
            Leaderboards, Submittion, PhonePosition
        };

        GameStates gameState = GameStates.TitleScreen;
        GameStates stateBeforePaused;
        Texture2D spriteSheet;
        Texture2D menuSheet;
        Texture2D planetSheet;
        Texture2D paperSheet;
        Texture2D handSheet;

        StarFieldManager starFieldManager1;

        PlayerManager playerManager;

        EnemyManager enemyManager;
        BossManager bossManager;

        CollisionManager collisionManager;

        SpriteFont pericles20;
        SpriteFont pericles22;
        SpriteFont pericles32;

        ZoomTextManager zoomTextManager;

        private float playerDeathTimer = 0.0f;
        private const float playerDeathDelayTime = 6.0f;
        private const float playerGameOverDelayTime = 5.0f;

        private float titleScreenTimer = 0.0f;
        private const float titleScreenDelayTime = 1.0f;

        private Vector2 highscoreLocation = new Vector2(10, 10);

        Hud hud;

        HighscoreManager highscoreManager;
        private bool highscoreMessageShown = false;

        SubmissionManager submissionManager;

        MainMenuManager mainMenuManager;

        private float backButtonTimer = 0.0f;
        private const float backButtonDelayTime = 0.25f;

        LevelManager levelManager;

        InstructionManager instructionManager;

        PowerUpManager powerUpManager;
        Texture2D powerUpSheet;

        SettingsManager settingsManager;

        GameInput gameInput;
        private const string TitleAction = "Title";
        private const string BackToGameAction = "BackToGame";
        private const string BackToMainAction = "BackToMain";

        private readonly Rectangle cancelSource = new Rectangle(0, 960,
                                                                240, 80);
        private readonly Rectangle cancelDestination = new Rectangle(120, 620,
                                                                     240, 80);

        private readonly Rectangle continueSource = new Rectangle(0, 880,
                                                                  240, 80);
        private readonly Rectangle continueDestination = new Rectangle(120, 510,
                                                                       240, 80);
        HandManager handManager;

        DamageExplosionManager damageExplosionManager;

        PhonePositionManager phonePositionManager;

        private const int SCORE_SUBMIT_LIMIT = 100;

        private bool backButtonPressed = false;

        public nint Handle => throw new NotImplementedException();

        public int JniIdentityHashCode => throw new NotImplementedException();

        public JniObjectReference PeerReference => throw new NotImplementedException();

        public JniPeerMembers JniPeerMembers => throw new NotImplementedException();

        public JniManagedPeerStates JniManagedPeerState => throw new NotImplementedException();

        public ScribbleHunter()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            Content.RootDirectory = "Content";

            // Frame rate is 60 fps
            TargetElapsedTime = TimeSpan.FromTicks(166667);
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            // Aplly the gfx changes
            graphics.ApplyChanges();

            // calculate scaling matrix/vector to fit everything to the assumed screen bounds
            var bw = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var bh = GraphicsDevice.PresentationParameters.BackBufferHeight;
            screenScaleVector = new Vector2((float)bw / WIDTH, (float)bh / HEIGHT);
            screenScaleMatrix = Matrix.Identity * Matrix.CreateScale(screenScaleVector.X, screenScaleVector.Y, 0f);

            gameInput = new GameInput();

            // Because we are using a different virtual scale compared to the
            // physical resolution of the screen, using a transformation matrix
            // of the SpriteBatch, we need to change the display for the touch
            // panel the same way
            TouchPanel.DisplayOrientation = DisplayOrientation.Portrait;
            TouchPanel.DisplayHeight = HEIGHT;
            TouchPanel.DisplayWidth = WIDTH;
            TouchPanel.EnabledGestures = GestureType.Tap;

            loadVersion();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            menuSheet = Content.Load<Texture2D>(@"Textures\MenuSheet");
            powerUpSheet = Content.Load<Texture2D>(@"Textures\PowerUpSheet");
            planetSheet = Content.Load<Texture2D>(@"Textures\PlanetSheet");
            paperSheet = Content.Load<Texture2D>(@"Textures\PaperSheet");
            handSheet = Content.Load<Texture2D>(@"Textures\HandSheet");

            starFieldManager1 = new StarFieldManager(WIDTH,
                                                    HEIGHT,
                                                    100,
                                                    50,
                                                    new Vector2(0, 25.0f),
                                                    new Vector2(0, 50.0f),
                                                    spriteSheet,
                                                    new Rectangle(0, 550, 2, 2),
                                                    new Rectangle(0, 550, 3, 3),
                                                    planetSheet,
                                                    new Rectangle(0, 0, 400, 314),
                                                    2,
                                                    3);

            playerManager = new PlayerManager(spriteSheet,
                                              new Rectangle(0, 100, 45, 45),
                                              6,
                                              new Rectangle(0, 0,
                                                            WIDTH,
                                                            HEIGHT),
                                              gameInput);

            enemyManager = new EnemyManager(spriteSheet,
                                            playerManager,
                                            new Rectangle(0, 0,
                                                          WIDTH,
                                                          HEIGHT));

            bossManager = new BossManager(spriteSheet,
                                          playerManager,
                                          new Rectangle(0, 0,
                                                        WIDTH,
                                                        HEIGHT));

            EffectManager.Initialize(spriteSheet,
                                     new Rectangle(0, 550, 3, 3),
                                     new Rectangle(0, 50, 50, 50),
                                     5);

            powerUpManager = new PowerUpManager(powerUpSheet, playerManager);

            collisionManager = new CollisionManager(playerManager,
                                                    enemyManager,
                                                    bossManager,
                                                    powerUpManager);

            SoundManager.Initialize(Content);

            pericles20 = Content.Load<SpriteFont>(@"Fonts\Pericles20");
            pericles22 = Content.Load<SpriteFont>(@"Fonts\Pericles22");
            pericles32 = Content.Load<SpriteFont>(@"Fonts\Pericles32");

            zoomTextManager = new ZoomTextManager(new Vector2(WIDTH / 2,
                                                              HEIGHT / 2),
                                                              pericles20,
                                                              pericles32);

            hud = Hud.GetInstance(new Rectangle(0, 0, WIDTH, HEIGHT),
                                  spriteSheet,
                                  pericles22,
                                  0,
                                  playerManager);

            highscoreManager = HighscoreManager.GetInstance();
            HighscoreManager.Font = pericles20;
            HighscoreManager.Texture = menuSheet;
            HighscoreManager.GameInput = gameInput;

            submissionManager = SubmissionManager.GetInstance();
            SubmissionManager.FontSmall = pericles20;
            SubmissionManager.FontBig = pericles22;
            SubmissionManager.Texture = menuSheet;
            SubmissionManager.GameInput = gameInput;

            mainMenuManager = new MainMenuManager(menuSheet, gameInput, pericles20);

            levelManager = new LevelManager();
            levelManager.Register(enemyManager);
            levelManager.Register(bossManager);
            levelManager.Register(playerManager);
            levelManager.Register(powerUpManager);

            instructionManager = new InstructionManager(spriteSheet,
                                                        pericles22,
                                                        new Rectangle(0, 0, WIDTH, HEIGHT),
                                                        playerManager,
                                                        enemyManager,
                                                        powerUpManager);

            SoundManager.PlayBackgroundSound();

            settingsManager = SettingsManager.GetInstance();
            settingsManager.Initialize(menuSheet, pericles22,
                new Rectangle(0, 0, WIDTH, HEIGHT));
            SettingsManager.GameInput = gameInput;

            handManager = new HandManager(handSheet);

            damageExplosionManager = DamageExplosionManager.Instance;
            DamageExplosionManager.Initialize(spriteSheet);

            phonePositionManager = PhonePositionManager.GetInstance();
            PhonePositionManager.Font = pericles32;
            PhonePositionManager.Texture = menuSheet;
            PhonePositionManager.GameInput = gameInput;

            setupInputs();
        }

        private void setupInputs()
        {
            gameInput.AddTouchGestureInput(TitleAction, GestureType.Tap, new Rectangle(0, 0, WIDTH, HEIGHT));
            gameInput.AddTouchGestureInput(BackToGameAction, GestureType.Tap, continueDestination);
            gameInput.AddTouchGestureInput(BackToMainAction, GestureType.Tap, cancelDestination);
            mainMenuManager.SetupInputs();
            playerManager.SetupInputs();
            submissionManager.SetupInputs();
            highscoreManager.SetupInputs();
            settingsManager.SetupInputs();
            phonePositionManager.SetupInputs();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Pauses the game when a call is incoming.
        /// Attention: Also called for GUID !!!
        /// </summary>
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            if (gameState == GameStates.Playing
                || gameState == GameStates.Instructions)
            {
                stateBeforePaused = gameState;
                gameState = GameStates.Paused;
            }
        }

        private void tryLoadGame()
        {
            try
            {
                using (IsolatedStorageFile isolatedStorageFile
                    = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageFile.FileExists("state.dat"))
                    {
                        //If user choose to save, create a new file
                        using (IsolatedStorageFileStream fileStream
                            = isolatedStorageFile.OpenFile("state.dat", FileMode.Open))
                        {
                            using (StreamReader reader = new StreamReader(fileStream))
                            {
                                this.gameState = (GameStates)Enum.Parse(gameState.GetType(), reader.ReadLine(), true);
                                this.stateBeforePaused = (GameStates)Enum.Parse(stateBeforePaused.GetType(), reader.ReadLine(), true);

                                if (gameState == GameStates.Playing)
                                {
                                    gameState = GameStates.Paused;
                                    stateBeforePaused = GameStates.Playing;
                                }

                                this.playerDeathTimer = (float)Single.Parse(reader.ReadLine());
                                this.titleScreenTimer = (float)Single.Parse(reader.ReadLine());
                                this.highscoreMessageShown = (bool)Boolean.Parse(reader.ReadLine());
                                this.backButtonTimer = (float)Single.Parse(reader.ReadLine());

                                playerManager.Activated(reader);

                                enemyManager.Activated(reader);

                                bossManager.Activated(reader);

                                EffectManager.Activated(reader);

                                powerUpManager.Activated(reader);

                                zoomTextManager.Activated(reader);

                                levelManager.Activated(reader);

                                instructionManager.Activated(reader);

                                mainMenuManager.Activated(reader);

                                highscoreManager.Activated(reader);

                                submissionManager.Activated(reader);

                                starFieldManager1.Activated(reader);

                                handManager.Activated(reader);

                                damageExplosionManager.Activated(reader);

                                phonePositionManager.Activated(reader);

                                settingsManager.Activated(reader);

                                //reader.Close();
                            }
                        }

                        isolatedStorageFile.DeleteFile("state.dat");
                    }
                }
            }
            catch (Exception)
            {
                // catch end restore in case of incompatible active/deactivate dat-files
                this.resetGame();
                this.gameState = GameStates.TitleScreen;
            }

            GC.Collect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            TouchPanel.DisplayHeight = HEIGHT;
            TouchPanel.DisplayWidth = WIDTH;

            SoundManager.Update(gameTime);

            handManager.Update(gameTime);

            gameInput.BeginUpdate();

            backButtonTimer += elapsed;

            if (backButtonTimer >= backButtonDelayTime)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    backButtonPressed = true;
                    backButtonTimer = 0.0f;
                }
            }

            switch (gameState)
            {
                case GameStates.TitleScreen:

                    titleScreenTimer += elapsed;

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    if (titleScreenTimer >= titleScreenDelayTime)
                    {
                        if (gameInput.IsPressed(TitleAction))
                        {
                            gameState = GameStates.MainMenu;
                            SoundManager.PlayPaperSound();
                        }
                    }

                    if (backButtonPressed)
                        this.Exit();

                    break;

                case GameStates.MainMenu:

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    mainMenuManager.IsActive = true;
                    mainMenuManager.Update(gameTime);

                    handManager.ShowHands();

                    switch (mainMenuManager.LastPressedMenuItem)
                    {
                        case MainMenuManager.MenuItems.Start:
                            gameState = GameStates.PhonePosition;
                            break;

                        case MainMenuManager.MenuItems.Highscores:
                            gameState = GameStates.Highscores;
                            break;

                        case MainMenuManager.MenuItems.Instructions:
                            resetGame();
                            settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle20);
                            instructionManager.Reset();
                            instructionManager.IsAutostarted = false;
                            updateHud(elapsed);
                            gameState = GameStates.Instructions;
                            break;

                        case MainMenuManager.MenuItems.Settings:
                            gameState = GameStates.Settings;
                            break;

                        case MainMenuManager.MenuItems.None:
                            // do nothing
                            break;
                    }

                    if (gameState != GameStates.MainMenu)
                    {
                        mainMenuManager.IsActive = false;
                        if (gameState == GameStates.Instructions)
                            handManager.HideHandsAndScribble();
                        else
                            handManager.HideHands();
                    }

                    if (backButtonPressed)
                        this.Exit();

                    break;

                case GameStates.PhonePosition:

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    phonePositionManager.IsActive = true;
                    phonePositionManager.Update(gameTime);

                    if (phonePositionManager.CancelClicked || backButtonPressed)
                    {
                        phonePositionManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                        SoundManager.PlayPaperSound();
                    }
                    else if (phonePositionManager.StartClicked)
                    {
                        phonePositionManager.IsActive = false;
                        resetGame();
                        updateHud(elapsed);
                        handManager.HideHandsAndScribble();
                        if (instructionManager.HasDoneInstructions)
                        {
                            gameState = GameStates.Playing;
                        }
                        else
                        {
                            instructionManager.Reset();
                            instructionManager.IsAutostarted = true;
                            gameState = GameStates.Instructions;
                        }
                        SoundManager.PlayPaperSound();
                    }

                    break;

                case GameStates.Highscores:

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    highscoreManager.IsActive = true;
                    highscoreManager.Update(gameTime);

                    if (backButtonPressed)
                    {
                        highscoreManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                        SoundManager.PlayPaperSound();
                    }

                    break;

                case GameStates.Submittion:

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    submissionManager.IsActive = true;
                    submissionManager.Update(gameTime);

                    highscoreMessageShown = false;

                    zoomTextManager.Reset();

                    if (submissionManager.CancelClicked || backButtonPressed)
                    {
                        highscoreManager.SaveHighScore(submissionManager.Name,
                                                       playerManager.TotalScore,
                                                       levelManager.CurrentLevel);

                        submissionManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                        SoundManager.PlayPaperSound();
                    }
                    else if (submissionManager.RetryClicked)
                    {
                        highscoreManager.SaveHighScore(submissionManager.Name,
                                                       playerManager.TotalScore,
                                                       levelManager.CurrentLevel);

                        submissionManager.IsActive = false;
                        resetGame();
                        updateHud(elapsed);
                        handManager.HideHandsAndScribble();
                        gameState = GameStates.Playing;
                    }

                    break;

                case GameStates.Instructions:

                    starFieldManager1.Update(elapsed * playerManager.TimeFreezeSpeedFactor);

                    levelManager.SetLevel(1);

                    instructionManager.Update(gameTime);
                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    collisionManager.Update();
                    updateHud(elapsed);

                    zoomTextManager.Update();

                    if (backButtonPressed)
                    {
                        if (!instructionManager.HasDoneInstructions && instructionManager.EnougthInstructionsDone)
                        {
                            instructionManager.InstructionsDone();
                            instructionManager.SaveHasDoneInstructions();
                        }

                        EffectManager.Reset();
                        if (instructionManager.IsAutostarted)
                        {
                            resetGame();
                            updateHud(elapsed);
                            handManager.HideHandsAndScribble();
                            gameState = GameStates.Playing;
                        }
                        else
                        {
                            gameState = GameStates.MainMenu;
                            SoundManager.PlayPaperSound();
                        }
                    }

                    break;

                case GameStates.Settings:

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    settingsManager.IsActive = true;
                    settingsManager.Update(gameTime);

                    if (settingsManager.CancelClicked || backButtonPressed)
                    {
                        settingsManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                        SoundManager.PlayPaperSound();
                    }

                    break;

                case GameStates.Playing:

                    updateBackground(elapsed * playerManager.TimeFreezeSpeedFactor);

                    levelManager.Update(gameTime);

                    playerManager.Update(gameTime);

                    enemyManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                    enemyManager.IsActive = true;

                    bossManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                    bossManager.IsActive = true;

                    EffectManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                    damageExplosionManager.Update(gameTime);

                    collisionManager.Update();

                    powerUpManager.Update(gameTime);

                    zoomTextManager.Update();

                    updateHud(elapsed);

                    if (levelManager.HasChanged)
                    {
                        levelManager.GoToNextState();
                    }

                    if (playerManager.TotalScore > highscoreManager.CurrentHighscore &&
                        highscoreManager.CurrentHighscore != 0 &&
                       !highscoreMessageShown)
                    {
                        zoomTextManager.ShowText(HighscoreText);
                        highscoreMessageShown = true;
                    }

                    if (playerManager.IsDestroyed)
                    {
                        playerDeathTimer = 0.0f;
                        enemyManager.IsActive = false;
                        bossManager.IsActive = false;

                        gameState = GameStates.GameOver;
                        zoomTextManager.ShowText(GameOverText);
                    }

                    if (backButtonPressed)
                    {
                        stateBeforePaused = GameStates.Playing;
                        gameState = GameStates.Paused;
                        SoundManager.PlayPaperSound();
                    }

                    break;

                case GameStates.Paused:

                    if (gameInput.IsPressed(BackToGameAction) || backButtonPressed)
                    {
                        gameState = stateBeforePaused;
                        SoundManager.PlayPaperSound();
                    }

                    if (gameInput.IsPressed(BackToMainAction))
                    {
                        SoundManager.PlayPaperSound();

                        if (playerManager.TotalScore > SCORE_SUBMIT_LIMIT)
                        {
                            gameState = GameStates.Submittion;
                            submissionManager.SetUp(highscoreManager.LastName, playerManager.TotalScore, levelManager.CurrentLevel);
                        }
                        else
                        {
                            gameState = GameStates.MainMenu;
                        }
                    }

                    break;

                case GameStates.GameOver:

                    playerDeathTimer += elapsed;

                    updateBackground(elapsed * playerManager.TimeFreezeSpeedFactor);

                    playerManager.Update(gameTime);
                    powerUpManager.Update(gameTime);
                    enemyManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                    bossManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                    EffectManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                    damageExplosionManager.Update(gameTime);
                    collisionManager.Update();
                    zoomTextManager.Update();

                    updateHud(elapsed);

                    if (playerDeathTimer >= playerGameOverDelayTime)
                    {
                        playerDeathTimer = 0.0f;
                        titleScreenTimer = 0.0f;

                        if (playerManager.TotalScore > SCORE_SUBMIT_LIMIT)
                        {
                            gameState = GameStates.Submittion;
                            submissionManager.SetUp(highscoreManager.LastName, playerManager.TotalScore, levelManager.CurrentLevel);
                        }
                        else
                        {
                            gameState = GameStates.MainMenu;
                        }

                        EffectManager.Reset();
                    }

                    if (backButtonPressed)
                    {
                        stateBeforePaused = GameStates.GameOver;
                        gameState = GameStates.Paused;
                        SoundManager.PlayPaperSound();
                    }

                    break;
            }

            // Reset Back-Button flag
            backButtonPressed = false;

            gameInput.EndUpdate();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(transformMatrix: screenScaleMatrix);

            if (gameState == GameStates.TitleScreen)
            {
                drawBackground(spriteBatch);

                // title
                spriteBatch.Draw(menuSheet,
                                 new Vector2(0.0f, 200.0f),
                                 new Rectangle(0, 0, WIDTH, 200),
                                 Color.White);

                spriteBatch.DrawString(pericles22,
                                       ContinueText,
                                       new Vector2(WIDTH / 2 - pericles22.MeasureString(ContinueText).X / 2, 455),
                                       Color.Black * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

                spriteBatch.DrawString(pericles20,
                                       MusicByText,
                                       new Vector2(WIDTH / 2 - pericles22.MeasureString(MusicByText).X / 2, 630),
                                       Color.Black);
                spriteBatch.DrawString(pericles20,
                                       MusicCreatorText,
                                       new Vector2(WIDTH / 2 - pericles22.MeasureString(MusicCreatorText).X / 2, 663),
                                       Color.Black);

                spriteBatch.DrawString(pericles20,
                                       VersionText,
                                       new Vector2(WIDTH - (pericles20.MeasureString(VersionText).X + 15),
                                                   HEIGHT - (pericles20.MeasureString(VersionText).Y + 10)),
                                       Color.Black);

                spriteBatch.DrawString(pericles20,
                                       CreatorText,
                                       new Vector2(15, HEIGHT - (pericles20.MeasureString(CreatorText).Y + 10)),
                                       Color.Black);
            }

            if (gameState == GameStates.MainMenu)
            {
                drawBackground(spriteBatch);

                mainMenuManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Highscores)
            {
                drawBackground(spriteBatch);

                highscoreManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Submittion)
            {
                drawBackground(spriteBatch);

                submissionManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.PhonePosition)
            {
                drawBackground(spriteBatch);

                phonePositionManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Instructions)
            {
                drawPaper(spriteBatch);

                starFieldManager1.Draw(spriteBatch);

                instructionManager.Draw(spriteBatch);

                EffectManager.Draw(spriteBatch);
                damageExplosionManager.Draw(spriteBatch);

                zoomTextManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);

                hud.Draw(spriteBatch);
            }

            if (gameState == GameStates.Settings)
            {
                drawBackground(spriteBatch);

                settingsManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Paused)
            {
                drawBackground(spriteBatch);

                powerUpManager.Draw(spriteBatch);

                playerManager.Draw(spriteBatch);

                enemyManager.Draw(spriteBatch);

                bossManager.Draw(spriteBatch);

                EffectManager.Draw(spriteBatch);
                damageExplosionManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);

                // Pause title

                spriteBatch.Draw(spriteSheet,
                                 new Rectangle(0, 0, WIDTH, HEIGHT),
                                 new Rectangle(0, 550, 1, 1),
                                 Color.White * 0.5f);

                spriteBatch.Draw(menuSheet,
                                 new Vector2(0.0f, 250.0f),
                                 new Rectangle(0, 200,
                                               WIDTH,
                                               100),
                                 Color.White * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

                spriteBatch.Draw(menuSheet,
                                 cancelDestination,
                                 cancelSource,
                                 Color.White);

                spriteBatch.Draw(menuSheet,
                                 continueDestination,
                                 continueSource,
                                 Color.White);
            }

            if (gameState == GameStates.Playing ||
                gameState == GameStates.GameOver)
            {
                drawBackground(spriteBatch);

                powerUpManager.Draw(spriteBatch);

                playerManager.Draw(spriteBatch);

                enemyManager.Draw(spriteBatch);

                bossManager.Draw(spriteBatch);

                EffectManager.Draw(spriteBatch);
                damageExplosionManager.Draw(spriteBatch);

                zoomTextManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);

                hud.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper method to reduce update redundace.
        /// </summary>
        private void updateBackground(float elapsed)
        {
            starFieldManager1.Update(elapsed);
        }

        /// <summary>
        /// Helper method to reduce draw redundace.
        /// </summary>
        private void drawBackground(SpriteBatch spriteBatch)
        {
            drawPaper(spriteBatch);

            starFieldManager1.Draw(spriteBatch);
        }

        private void drawPaper(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(paperSheet,
                new Rectangle(0, 0, WIDTH, HEIGHT),
                new Rectangle(0, 0, WIDTH, HEIGHT),
                Color.White);
        }

        private void resetRound()
        {
            enemyManager.Reset();
            bossManager.Reset();
            playerManager.Reset();
            EffectManager.Reset();
            powerUpManager.Reset();

            zoomTextManager.Reset();
        }

        private void resetGame()
        {
            resetRound();

            levelManager.Reset();

            playerManager.ResetPlayerScore();

            GC.Collect();
        }

        /// <summary>
        /// Loads the current version from assembly.
        /// </summary>
        private void loadVersion()
        {
            System.Reflection.AssemblyName an = new System.Reflection.AssemblyName(System.Reflection.Assembly
                                                                                   .GetExecutingAssembly()
                                                                                   .FullName);
            this.VersionText = new StringBuilder().Append("v ")
                                                  .Append(an.Version.Major)
                                                  .Append('.')
                                                  .Append(an.Version.Minor)
                                                  .ToString();
        }

        private void updateHud(float elapsed)
        {
            hud.Update(elapsed, levelManager.CurrentLevel);
        }

        public void BackButtonPressed()
        {
            backButtonPressed = true;
        }
    }
}