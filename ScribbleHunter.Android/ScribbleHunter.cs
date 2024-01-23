/****************
 * REMARK: - Remember to set the flags in ALL files
 *         - Set up the correct app name: "ScribbleHunter" / "ScribbleHunter Free"
 * */

#define IS_FREE_VERSION
//#define SIMULATE_TRIAL

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using ScribbleHunter.Inputs;

namespace ScribbleHunter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ScribbleHunter : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

#if IS_FREE_VERSION
        /// <summary>
        /// Advertising stuff
        /// </summary>
        //private AdGameComponent adGameComponent;
        //private DrawableAd bannerAd;
        //private AdManager dpManager;
        //private bool isAdDuplexActive = false;
        //private bool bannerLoaded;
        //private readonly Rectangle BannerDummySource = new Rectangle(1100, 600, 480, 80);
#endif

        private const string HighscoreText = "Personal Highscore!";
        private const string GameOverText = "GAME OVER!";

        //private const string KeyboardInLocalMessageFormatText = "You are locally ranked {0}/10!\nPlease enter your name...\n[only: A..Z, a..z, 0..9, 12 characters]";
        //private const string KeyboardNotInLocalMessageFormatText = "You are not locally ranked!\nPlease enter your name for online submission...\n[only: A..Z, a..z, 0..9, 12 characters]";

        private const string ContinueText = "Push to continue...";
        private string VersionText;
        private const string MusicByText = "Music by";
        private const string MusicCreatorText = "PLSQMPRFKT";
        private const string CreatorText = "by Benjamin Kan";

        enum GameStates
        {
            TitleScreen, MainMenu, Highscores, Instructions, Help, Settings, Playing, Paused, GameOver,
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

        LeaderboardManager leaderboardManager;

        SubmissionManager submissionManager;

        MainMenuManager mainMenuManager;

        private float backButtonTimer = 0.0f;
        private const float backButtonDelayTime = 0.25f;

        LevelManager levelManager;

        InstructionManager instructionManager;

        HelpManager helpManager;

        PowerUpManager powerUpManager;
        Texture2D powerUpSheet;

        SettingsManager settingsManager;

        GameInput gameInput = new GameInput();
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

        public ScribbleHunter()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            Content.RootDirectory = "Content";
//#if IS_FREE_VERSION
//#if DEBUG
//            AdGameComponent.Initialize(this, "test_client");
//#else
//            AdGameComponent.Initialize(this, "793d8686-f1d6-4b69-8fd5-bf6fef7997ff");
//            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
//            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
//#endif

//            adGameComponent = AdGameComponent.Current;
//            adGameComponent.Enabled = true;
//#endif
            // Frame rate is 60 fps
            TargetElapsedTime = TimeSpan.FromTicks(166667);

            InitializaPhoneServices();
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
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 480;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            // Aplly the gfx changes
            graphics.ApplyChanges();

            TouchPanel.EnabledGestures = GestureType.Tap;

            loadVersion();

            this.Window.ClientSizeChanged += Window_ClientSizeChanged;

            base.Initialize();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            this.GraphicsDevice.Viewport = new Viewport(0, 0, 800, 480);
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 800;
            // Aplly the gfx changes
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

//#if IS_FREE_VERSION
//            // Create a banner ad for the game.
//#if DEBUG
//            bannerAd = adGameComponent.CreateAd("Image480_80", new Rectangle(0, 0, 480, 80));
//#else
//            bannerAd = adGameComponent.CreateAd("120636", new Rectangle(0, 0, 480, 80));
//#endif
//            bannerAd.BorderEnabled = false;
//            bannerAd.AdRefreshed += new EventHandler(bannerAd_AdRefreshed);
//            bannerAd.ErrorOccurred += new EventHandler<Microsoft.Advertising.AdErrorEventArgs>(bannerAd_ErrorOccurred);
//#endif
            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            menuSheet = Content.Load<Texture2D>(@"Textures\MenuSheet");
            powerUpSheet = Content.Load<Texture2D>(@"Textures\PowerUpSheet");
            planetSheet = Content.Load<Texture2D>(@"Textures\PlanetSheet");
            paperSheet = Content.Load<Texture2D>(@"Textures\PaperSheet");
            handSheet = Content.Load<Texture2D>(@"Textures\HandSheet");

            starFieldManager1 = new StarFieldManager(this.GraphicsDevice.Viewport.Width,
                                                    this.GraphicsDevice.Viewport.Height,
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
                                                            this.GraphicsDevice.Viewport.Width,
                                                            this.GraphicsDevice.Viewport.Height),
                                              gameInput);

            enemyManager = new EnemyManager(spriteSheet,
                                            playerManager,
                                            new Rectangle(0, 0,
                                                          this.GraphicsDevice.Viewport.Width,
                                                          this.GraphicsDevice.Viewport.Height));

            bossManager = new BossManager(spriteSheet,
                                          playerManager,
                                          new Rectangle(0, 0,
                                                        this.GraphicsDevice.Viewport.Width,
                                                        this.GraphicsDevice.Viewport.Height));

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

            zoomTextManager = new ZoomTextManager(new Vector2(this.GraphicsDevice.Viewport.Width / 2,
                                                              this.GraphicsDevice.Viewport.Height / 2),
                                                              pericles20,
                                                              pericles32);

            hud = Hud.GetInstance(GraphicsDevice.Viewport.Bounds,
                                  spriteSheet,
                                  pericles22,
                                  0,
                                  bossManager,
                                  playerManager);

            highscoreManager = HighscoreManager.GetInstance();
            HighscoreManager.Font = pericles20;
            HighscoreManager.Texture = menuSheet;

            leaderboardManager = LeaderboardManager.GetInstance();
            LeaderboardManager.Font = pericles22;
            LeaderboardManager.Texture = menuSheet;
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
                                                        new Rectangle(0, 0,
                                                                      GraphicsDevice.Viewport.Width,
                                                                      GraphicsDevice.Viewport.Height),
                                                        playerManager,
                                                        enemyManager,
                                                        powerUpManager);

            helpManager = new HelpManager(menuSheet, pericles22, new Rectangle(0, 0,
                                                                               GraphicsDevice.Viewport.Width,
                                                                               GraphicsDevice.Viewport.Height));
            HelpManager.GameInput = gameInput;
            SoundManager.PlayBackgroundSound();


            settingsManager = SettingsManager.GetInstance();
            settingsManager.Initialize(menuSheet, pericles22, new Rectangle(0, 0,
                                                                            GraphicsDevice.Viewport.Width,
                                                                            GraphicsDevice.Viewport.Height));
            SettingsManager.GameInput = gameInput;

            handManager = new HandManager(handSheet);

            damageExplosionManager = DamageExplosionManager.Instance;
            DamageExplosionManager.Initialize(spriteSheet);

            phonePositionManager = PhonePositionManager.GetInstance();
            PhonePositionManager.Font = pericles32;
            PhonePositionManager.Texture = menuSheet;
            PhonePositionManager.GameInput = gameInput;

            setupInputs();

//#if IS_FREE_VERSION
//            // ad duplex
//            dpManager = new AdManager(this, "62583");
//            dpManager.LoadContent();
//#endif
        }

//#if IS_FREE_VERSION
//        void bannerAd_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
//        {
//            bannerLoaded = false;

//            // If loading of banner is failed, load an ad duplex banner.
//            isAdDuplexActive = true;
//        }

//        void bannerAd_AdRefreshed(object sender, EventArgs e)
//        {
//            bannerLoaded = true;

//            // If loading of MS banner succeeded, disable ad duplex banner.
//            isAdDuplexActive = false;
//        }
//#endif

        private void setupInputs()
        {
            gameInput.AddTouchGestureInput(TitleAction, GestureType.Tap, new Rectangle(0, 0,
                                                                                   480, 800));
            gameInput.AddTouchGestureInput(BackToGameAction, GestureType.Tap, continueDestination);
            gameInput.AddTouchGestureInput(BackToMainAction, GestureType.Tap, cancelDestination);
            mainMenuManager.SetupInputs();
            playerManager.SetupInputs();
            submissionManager.SetupInputs();
            highscoreManager.SetupInputs();
            settingsManager.SetupInputs();
            helpManager.SetupInputs();
            phonePositionManager.SetupInputs();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        private void InitializaPhoneServices()
        {
            //PhoneApplicationService.Current.Activated += new EventHandler<ActivatedEventArgs>(GameActivated);
            //PhoneApplicationService.Current.Deactivated += new EventHandler<DeactivatedEventArgs>(GameDeactivated);
            //PhoneApplicationService.Current.Closing += new EventHandler<ClosingEventArgs>(GameClosing);
            //PhoneApplicationService.Current.Launching += new EventHandler<LaunchingEventArgs>(GameLaunching);
        }

        /// <summary>
        /// Occurs when the game class (and application) deactivated and tombstoned.
        /// Saves state of: ScribbleHunter, AsteroidManager
        /// Does not save: Starfield-Manager (not necessary)
        /// </summary>
        //void GameDeactivated(object sender, DeactivatedEventArgs e)
        //{
        //    // Save to Isolated Storage file
        //    using (IsolatedStorageFile isolatedStorageFile
        //        = IsolatedStorageFile.GetUserStoreForApplication())
        //    {
        //        // If user choose to save, create a new file
        //        using (IsolatedStorageFileStream fileStream
        //            = isolatedStorageFile.CreateFile("state.dat"))
        //        {
        //            using (StreamWriter writer = new StreamWriter(fileStream))
        //            {
        //                // Write date to the file
        //                writer.WriteLine(this.gameState);
        //                writer.WriteLine(this.stateBeforePaused);
        //                writer.WriteLine(this.playerDeathTimer);
        //                writer.WriteLine(this.titleScreenTimer);
        //                writer.WriteLine(this.highscoreMessageShown);
        //                writer.WriteLine(this.backButtonTimer);

        //                writer.WriteLine(ScribbleHunter.IsTrialMode);

        //                playerManager.Deactivated(writer);

        //                enemyManager.Deactivated(writer);

        //                bossManager.Deactivated(writer);

        //                EffectManager.Deactivated(writer);

        //                powerUpManager.Deactivated(writer);

        //                zoomTextManager.Deactivated(writer);

        //                levelManager.Deactivated(writer);

        //                instructionManager.Deactivated(writer);

        //                mainMenuManager.Deactivated(writer);

        //                highscoreManager.Deactivated(writer);

        //                submissionManager.Deactivated(writer);

        //                starFieldManager1.Deactivated(writer);

        //                handManager.Deactivated(writer);

        //                damageExplosionManager.Deactivated(writer);

        //                phonePositionManager.Deactivated(writer);

        //                settingsManager.Deactivated(writer);

        //                //writer.Close();
        //            }
        //        }
        //    }
        //}

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

        //void GameLaunching(object sender, LaunchingEventArgs e)
        //{
        //    tryLoadGame();

        //    if (gameState == GameStates.MainMenu || gameState == GameStates.Help ||
        //        gameState == GameStates.Highscores || gameState == GameStates.Settings ||
        //        gameState == GameStates.Submittion || gameState == GameStates.PhonePosition)
        //    {
        //        gameState = GameStates.TitleScreen;

        //        // Reset hands:
        //        handManager.Reset();
        //    }
        //}

        /////// <summary>
        /////// Occurs when the game class (and application) activated during return from tombstoned state
        /////// </summary>
        //void GameActivated(object sender, ActivatedEventArgs e)
        //{
        //    // no reload of data required, because we are activated from DORMANT state
        //    if (e.IsApplicationInstancePreserved)
        //        return;

        //    tryLoadGame();
        //}

        //void GameClosing(object sender, ClosingEventArgs e)
        //{
        //    instructionManager.SaveHasDoneInstructions();

        //    // delete saved active-game on real exit
        //    using (IsolatedStorageFile isolatedStorageFile
        //            = IsolatedStorageFile.GetUserStoreForApplication())
        //    {
        //        if (isolatedStorageFile.FileExists("state.dat"))
        //        {
        //            isolatedStorageFile.DeleteFile("state.dat");
        //        }
        //    }
        //}

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

            SoundManager.Update(gameTime);

            handManager.Update(gameTime);

            gameInput.BeginUpdate();

            bool backButtonPressed = false;

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
                            leaderboardManager.Receive();
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

                        case MainMenuManager.MenuItems.Help:
                            gameState = GameStates.Help;
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

                case GameStates.Help:

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    helpManager.IsActive = true;
                    helpManager.Update(gameTime);

                    if (backButtonPressed)
                    {
                        helpManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                        SoundManager.PlayPaperSound();
                    }

                    break;

                case GameStates.Settings:

                    updateBackground(elapsed);

                    EffectManager.Update(elapsed);
                    damageExplosionManager.Update(gameTime);

                    settingsManager.IsActive = true;
                    settingsManager.Update(gameTime);

                    if (backButtonPressed)
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

//#if IS_FREE_VERSION
//            // Advertisment stuff
//            if (isAdDuplexActive)
//                dpManager.Update(gameTime);
//            else
//                adGameComponent.Update(gameTime);
//#endif

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (gameState == GameStates.TitleScreen)
            {
                drawBackground(spriteBatch);

                // title
                spriteBatch.Draw(menuSheet,
                                 new Vector2(0.0f, 200.0f),
                                 new Rectangle(0, 0,
                                               480,
                                               200),
                                 Color.White);

                spriteBatch.DrawString(pericles22,
                                       ContinueText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles22.MeasureString(ContinueText).X / 2,
                                                   455),
                                       Color.Black * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

                spriteBatch.DrawString(pericles20,
                                       MusicByText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles22.MeasureString(MusicByText).X / 2,
                                                   630),
                                       Color.Black);
                spriteBatch.DrawString(pericles20,
                                       MusicCreatorText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles22.MeasureString(MusicCreatorText).X / 2,
                                                   663),
                                       Color.Black);

                spriteBatch.DrawString(pericles20,
                                       VersionText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width - (pericles20.MeasureString(VersionText).X + 15),
                                                   this.GraphicsDevice.Viewport.Height - (pericles20.MeasureString(VersionText).Y + 10)),
                                       Color.Black);

                spriteBatch.DrawString(pericles20,
                                       CreatorText,
                                       new Vector2(15,
                                                   this.GraphicsDevice.Viewport.Height - (pericles20.MeasureString(CreatorText).Y + 10)),
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

            if (gameState == GameStates.Help)
            {
                drawBackground(spriteBatch);

                helpManager.Draw(spriteBatch);

                handManager.Draw(spriteBatch);
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
                                 new Rectangle(0, 0, 480, 800),
                                 new Rectangle(0, 550, 1, 1),
                                 Color.White * 0.5f);

                spriteBatch.Draw(menuSheet,
                                 new Vector2(0.0f, 250.0f),
                                 new Rectangle(0, 200,
                                               480,
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

//#if IS_FREE_VERSION
//            adGameComponent.Draw(gameTime);

//            if (!isAdDuplexActive)
//                adGameComponent.Draw(gameTime);
//#endif

            spriteBatch.End();

//#if IS_FREE_VERSION
//            if (isAdDuplexActive)
//                dpManager.Draw(spriteBatch, Vector2.Zero);
//#endif

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
            //#if IS_FREE_VERSION
            //            if (!adGameComponent.Visible || !bannerLoaded)
            //            {
            //                spriteBatch.Draw(paperSheet,
            //                                 new Rectangle(0, 0,
            //                                               480, 800),
            //                                 new Rectangle(0, 0,
            //                                               480, 800),
            //                                 Color.White);

            //                if (gameState == GameStates.Playing ||
            //                    gameState == GameStates.Paused ||
            //                    gameState == GameStates.Instructions ||
            //                    gameState == GameStates.GameOver)
            //                {
            //                    spriteBatch.Draw(spriteSheet,
            //                                    Vector2.Zero,
            //                                    BannerDummySource,
            //                                    Color.Black * 0.8f);
            //                }
            //            }
            //            else
            //            {
            //                spriteBatch.Draw(paperSheet,
            //                                 new Rectangle(0, 80,
            //                                               480, 720),
            //                                 new Rectangle(0, 80,
            //                                               480, 720),
            //                                 Color.White);
            //            }
            //#else
            //            spriteBatch.Draw(paperSheet,
            //                                 new Rectangle(0, 0,
            //                                               480, 800),
            //                                 new Rectangle(0, 0,
            //                                               480, 800),
            //                                 Color.White);
            //#endif
            spriteBatch.Draw(paperSheet,
                new Rectangle(0, 0, 800, 480),
                new Rectangle(0, 0, 800, 480),
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
    }
}