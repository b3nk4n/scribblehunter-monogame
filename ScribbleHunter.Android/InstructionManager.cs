using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;
using ScribbleHunter.Extensions;

namespace ScribbleHunter
{
    class InstructionManager
    {
        #region Members

        private float progressTimer = 0.0f;

        public enum InstructionStates
        {
            Welcome,
            Movement,
            Shot,
            Reload,
            Survive,
            PowerUps,
            KillEnemies,
            Combos,
            GoodLuck,
            Finished
        };

        private InstructionStates state = InstructionStates.Welcome;

        private const float WelcomeLimit = 2.0f;
        private const float MovementLimit = 6.0f;
        private const float ShotLimit = 10.0f;
        private const float ReloadLimit = 14.0f;
        private const float SurviveLimit = 18.0f;
        private const float PowerUpsLimit = 22.0f;
        private const float KillEnemiesLimit = 26.0f;
        private const float CombosLimit = 30.0f;
        private const float GoodLuckLimit = 36.0f;
        private const float FinishedLimit = 39.0f;

        private SpriteFont font;

        private Texture2D texture;

        private Rectangle screenBounds;

        private readonly Rectangle ArrowRightSource = new Rectangle(650, 140, 40, 20);

        private readonly Rectangle ReloadDestination = new Rectangle(235, 770, 50, 30);
        private readonly Rectangle ComboDestination = new Rectangle(45, 60, 50, 30);
        private readonly Rectangle PowerUpArrowDestination = new Rectangle(255, 535, 50, 30);

        private readonly Rectangle FingerprintSource = new Rectangle(1100, 700, 200, 200);
        private readonly Rectangle FingerprintDestination = new Rectangle(220, 460, 200, 200);
        private float fingerprintOpacity;

        private Color arrowTint = Color.Black * 0.8f;

        private EnemyManager enemyManager;

        private PlayerManager playerManager;

        private PowerUpManager powerUpManager;

        private readonly string WelcomeText = "Welcome to ScribbleHunter!";
        private readonly string[] MovementText = { "Move the spaceship", "by tilting your phone" };
        private readonly string[] ShotText = { "Touch the screen", "to release an explosion" };
        private readonly string[] ReloadText = { "At the bottom you can", "check your reload status" };
        private readonly string SurviveText = "Survive as long as you can";
        private readonly string[] PowerUpsText = { "Collect power ups", "and special weapons" };
        private readonly string KillEnemiesText = "Kill enemies to score";
        private readonly string[] CombosText = { "Do combos to", "maximize your highscore" };
        private readonly string GoodLuckText = "Good luck commander!";
        private readonly string ReturnWithBackButtonText = "Press BACK to return...";
        private readonly string ContinueWithBackButtonText = "Press BACK to start the game...";

        private bool hasDoneInstructions = false;

        private const string INSTRUCTION_FILE = "instructions.txt";

        private bool powerUpsDropped;

        private bool isInvalidated = false;

        private bool isAutostarted;

        #endregion

        #region Constructors

        public InstructionManager(Texture2D texture, SpriteFont font, Rectangle screenBounds,
                                  PlayerManager playerManager, EnemyManager enemyManager,
                                  PowerUpManager powerUpManager)
        {
            this.texture = texture;
            this.font = font;
            this.screenBounds = screenBounds;

            this.enemyManager = enemyManager;
            this.enemyManager.Reset();

            this.playerManager = playerManager;
            this.playerManager.Reset();

            this.powerUpManager = powerUpManager;
            this.powerUpManager.Reset();

            loadHasDoneInstructions();
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            progressTimer += elapsed;

            if (playerManager.IsDestroyed)
            {
                this.state = InstructionStates.Finished;

                powerUpManager.Update(gameTime);
                enemyManager.Update(elapsed);
            }
            else if (progressTimer < WelcomeLimit)
            {
                fingerprintOpacity = 0.0f;
                this.state = InstructionStates.Welcome;
            }
            else if (progressTimer < MovementLimit)
            {
                this.state = InstructionStates.Movement;
            }
            else if (progressTimer < ShotLimit)
            {
                this.state = InstructionStates.Shot;
                this.fingerprintOpacity += 0.025f;

                if (fingerprintOpacity > 1)
                    fingerprintOpacity = 1;
            }
            else if (progressTimer < ReloadLimit)
            {
                this.state = InstructionStates.Reload;

                this.fingerprintOpacity -= 0.033f;

                if (fingerprintOpacity < 0)
                    fingerprintOpacity = 0;
            }
            else if (progressTimer < SurviveLimit)
            {
                this.state = InstructionStates.Survive;
            }
            else if (progressTimer < PowerUpsLimit)
            {
                this.state = InstructionStates.PowerUps;

                if (!powerUpsDropped)
                {
                    powerUpManager.SpawnPowerUp(new Vector2(240, 500));
                    powerUpsDropped = true;
                }
                enemyManager.Update(elapsed);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < KillEnemiesLimit)
            {
                this.state = InstructionStates.KillEnemies;

                enemyManager.Update(elapsed);
                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < CombosLimit)
            {
                this.state = InstructionStates.Combos;

                enemyManager.Update(elapsed);
                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < GoodLuckLimit)
            {
                this.state = InstructionStates.GoodLuck;

                enemyManager.Update(elapsed);
                powerUpManager.Update(gameTime);
            }
            else
            {
                this.state = InstructionStates.Finished;

                enemyManager.Update(elapsed);
                powerUpManager.Update(gameTime);
            }

            playerManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (this.state)
            {
                case InstructionStates.Welcome:
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, WelcomeText);
                    break;

                case InstructionStates.Movement:
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, MovementText);
                    break;

                case InstructionStates.Shot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(
                        texture,
                        FingerprintDestination,
                        FingerprintSource,
                        Color.White * 0.4f * fingerprintOpacity);

                    drawRedCenteredText(spriteBatch, ShotText);
                    break;

                case InstructionStates.Reload:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(
                        texture,
                        FingerprintDestination,
                        FingerprintSource,
                        Color.White * 0.5f * fingerprintOpacity);

                    spriteBatch.DrawBordered(
                        texture,
                        ReloadDestination,
                        ArrowRightSource,
                        arrowTint,
                        3 * (float)Math.PI / 2.0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0.0f,
                        Color.White);

                    drawRedCenteredText(spriteBatch, ReloadText);
                    break;

                case InstructionStates.Survive:
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, SurviveText);
                    break;

                case InstructionStates.PowerUps:
                    powerUpManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(
                        texture,
                        PowerUpArrowDestination,
                        ArrowRightSource,
                        arrowTint,
                        (float)Math.PI / 2.0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0.0f,
                        Color.White);

                    drawRedCenteredText(spriteBatch, PowerUpsText);
                    break;

                case InstructionStates.KillEnemies:
                    powerUpManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, KillEnemiesText);
                    break;

                case InstructionStates.Combos:
                    powerUpManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawBordered(
                        texture,
                        ComboDestination,
                        ArrowRightSource,
                        arrowTint,
                        (float)Math.PI / 2.0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0.0f,
                        Color.White);

                    drawRedCenteredText(spriteBatch, CombosText);
                    break;

                case InstructionStates.GoodLuck:
                    powerUpManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, GoodLuckText);
                    break;

                case InstructionStates.Finished:
                    powerUpManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    if (isAutostarted)
                        drawRedCenteredText(spriteBatch, ContinueWithBackButtonText);
                    else
                        drawRedCenteredText(spriteBatch, ReturnWithBackButtonText);
                    break;
            }
        }

        private void drawRedCenteredText(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawStringBordered(font,
                                   text,
                                   new Vector2(screenBounds.Width / 2 - font.MeasureString(text).X / 2,
                                               screenBounds.Height / 2 - font.MeasureString(text).Y / 2),
                                   Color.Black,
                                   Color.White);
        }

        private void drawRedCenteredText(SpriteBatch spriteBatch, string[] texts)
        {
            for (var i = 0; i < texts.Length; ++i)
            {
                spriteBatch.DrawStringBordered(font,
                                   texts[i],
                                   new Vector2(screenBounds.Width / 2 - font.MeasureString(texts[i]).X / 2,
                                               (screenBounds.Height / 2 - font.MeasureString(texts[i]).Y / 2) - 16 + (i * 32)),
                                   Color.Black,
                                   Color.White);
            }
        }

        public void Reset()
        {
            this.progressTimer = 0.0f;
            this.state = InstructionStates.Welcome;
            this.powerUpsDropped = false;
            this.isAutostarted = false;
            this.fingerprintOpacity = 0.0f;
        }

        public void InstructionsDone()
        {
            if (!hasDoneInstructions)
            {
                hasDoneInstructions = true;
                isInvalidated = true;
            }
        }

        public void SaveHasDoneInstructions()
        {
            if (!isInvalidated)
                return;

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(INSTRUCTION_FILE, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(hasDoneInstructions);

                        //sw.Flush();
                        //sw.Close();
                    }
                }
            }
        }

        private void loadHasDoneInstructions()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(INSTRUCTION_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(INSTRUCTION_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    isInvalidated = false;

                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            hasDoneInstructions = Boolean.Parse(sr.ReadLine());
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(hasDoneInstructions);

                            // ... ? 
                        }
                    }
                }
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.progressTimer = Single.Parse(reader.ReadLine());
            this.hasDoneInstructions = Boolean.Parse(reader.ReadLine());
            this.powerUpsDropped = Boolean.Parse(reader.ReadLine());
            this.isInvalidated = Boolean.Parse(reader.ReadLine());
            this.isAutostarted = Boolean.Parse(reader.ReadLine());
            this.fingerprintOpacity = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(progressTimer);
            writer.WriteLine(hasDoneInstructions);
            writer.WriteLine(powerUpsDropped);
            writer.WriteLine(isInvalidated);
            writer.WriteLine(isAutostarted);
            writer.WriteLine(fingerprintOpacity);
        }

        #endregion

        #region Properties

        public bool HasDoneInstructions
        {
            get
            {
                return hasDoneInstructions;
            }
        }

        public bool IsAutostarted
        {
            set
            {
                this.isAutostarted = value;
            }
            get
            {
                return this.isAutostarted;
            }
        }

        public bool EnougthInstructionsDone
        {
            get
            {
                return (progressTimer > 5.0f);
            }
        }

        #endregion
    }
}