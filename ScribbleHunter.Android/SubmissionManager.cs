using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using ScribbleHunter.Inputs;
using ScribbleHunter.Extensions;

namespace ScribbleHunter
{
    class SubmissionManager
    {
        #region Members

        LeaderboardManager leaderboardManager;

        private readonly Rectangle submitSource = new Rectangle(0, 1440,
                                                                480, 100);
        private readonly Rectangle submitDestination = new Rectangle(5, 610,
                                                                     470, 97);

        private readonly Rectangle cancelSource = new Rectangle(0, 800,
                                                                240, 80);
        private readonly Rectangle cancelDestination = new Rectangle(245, 710,
                                                                     230, 77);

        private readonly Rectangle retrySource = new Rectangle(0, 1040,
                                                                  240, 80);
        private readonly Rectangle retryDestination = new Rectangle(5, 710,
                                                                    230, 77);

        private static SubmissionManager submissionManager;

        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static SpriteFont FontSmall;
        public static SpriteFont FontBig;
        private readonly Rectangle TitleSource = new Rectangle(0, 300,
                                                               480, 100);
        private readonly Vector2 TitlePosition = new Vector2(0.0f, 100.0f);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private string name = string.Empty;
        private long score;
        private int level;

        private bool cancelClicked = false;
        private bool retryClicked = false;
        private bool changeNameClicked = false;


        private readonly string[] TEXT_SUBMIT = {"You have now the ability to share your",
                                                 "score with players from all over the world!"};
        private const string TEXT_NAME = "Name:";
        private const string TEXT_SCORE = "Score:";
        private const string TEXT_LEVEL = "Level:";

        private enum SubmitState { Submit, Submitted };
        private SubmitState submitState = SubmitState.Submit;

        public static GameInput GameInput;
        private const string SubmitAction = "Submit";
        private const string CancelAction = "Cancel";
        private const string RetryAction = "Retry";
        private const string ChangeNameAction = "ChangeName";

        private readonly Rectangle formLeftSource = new Rectangle(480, 1350,
                                                                  20, 60);
        private readonly Rectangle formContentSource = new Rectangle(500, 1350,
                                                                     160, 60);
        private readonly Rectangle formRightSource = new Rectangle(670, 1350,
                                                                   70, 60);
        private readonly Rectangle formClickDestination = new Rectangle(60, 372,
                                                                   360, 60);

        #endregion

        #region Constructors

        private SubmissionManager()
        {
            leaderboardManager = LeaderboardManager.GetInstance();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(SubmitAction,
                                           GestureType.Tap,
                                           submitDestination);
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);
            GameInput.AddTouchGestureInput(RetryAction,
                                           GestureType.Tap,
                                           retryDestination);
            GameInput.AddTouchGestureInput(ChangeNameAction,
                                           GestureType.Tap,
                                           formClickDestination);
        }

        public static SubmissionManager GetInstance()
        {
            if (submissionManager == null)
            {
                submissionManager = new SubmissionManager();
            }

            return submissionManager;
        }

        private void handleTouchInputs()
        {
            if (submitState == SubmitState.Submit)
            {
                // Submit
                if (GameInput.IsPressed(SubmitAction))
                {
                    SoundManager.PlayPaperSound();
                    leaderboardManager.Submit(LeaderboardManager.SUBMIT,
                                              name,
                                              score,
                                              level);
                    submitState = SubmitState.Submitted;
                }

                if (GameInput.IsPressed(ChangeNameAction))
                {
                    changeNameClicked = true;
                }
            }

            // Retry
            if (GameInput.IsPressed(RetryAction))
            {
                retryClicked = true;
            }

            // Cancel
            if (GameInput.IsPressed(CancelAction))
            {
                leaderboardManager.StatusText = LeaderboardManager.TEXT_NONE;
                cancelClicked = true;
            }
        }

        public void SetUp(string name, long score, int level)
        {
            this.name = name;
            this.score = score;
            this.level = level;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                                 cancelDestination,
                                 cancelSource,
                                 Color.White * opacity);

            spriteBatch.Draw(Texture,
                                 retryDestination,
                                 retrySource,
                                 Color.White * opacity);

            if (submitState == SubmitState.Submit)
            {
                spriteBatch.Draw(Texture,
                                 submitDestination,
                                 submitSource,
                                 Color.White * opacity);
            }
            else if (submitState == SubmitState.Submitted)
            {
                spriteBatch.DrawString(FontSmall,
                                   leaderboardManager.StatusText,
                                   new Vector2(240 - FontSmall.MeasureString(leaderboardManager.StatusText).X / 2,
                                               645),
                                   Color.Black * opacity);

            }

            for (int i = 0; i < TEXT_SUBMIT.Length; i++)
            {
                spriteBatch.DrawString(FontBig,
                                   TEXT_SUBMIT[i],
                                   new Vector2(240 - FontBig.MeasureString(TEXT_SUBMIT[i]).X / 2,
                                               240 + (i * 35)),
                                   Color.Black * opacity);
            }

            // Title:
            spriteBatch.DrawString(FontSmall,
                                   TEXT_NAME,
                                   new Vector2(240 - FontSmall.MeasureString(TEXT_NAME).X / 2,
                                               350),
                                   Color.Black * opacity);

            spriteBatch.DrawString(FontSmall,
                                   TEXT_SCORE,
                                   new Vector2(240 - FontSmall.MeasureString(TEXT_SCORE).X / 2,
                                               450),
                                   Color.Black * opacity);

            spriteBatch.DrawString(FontSmall,
                                   TEXT_LEVEL,
                                   new Vector2(240 - FontSmall.MeasureString(TEXT_LEVEL).X / 2,
                                               530),
                                   Color.Black * opacity);

            // Content:

            // Form:
            int nameWidth = Math.Max((int)FontBig.MeasureString(name).X, 100);

            Color formColor;

            if (submitState == SubmitState.Submit)
            {
                formColor = Color.White;
            }
            else
            {
                formColor = Color.White * 0.5f;
            }

            spriteBatch.Draw(
                Texture,
                new Rectangle(240 - nameWidth / 2 - formLeftSource.Width - 10, formClickDestination.Y,
                              formLeftSource.Width,
                              formLeftSource.Height),
                formLeftSource,
                formColor);

            spriteBatch.Draw(
                Texture,
                new Rectangle(240 - nameWidth / 2 - 10, formClickDestination.Y,
                              nameWidth,
                              formContentSource.Height),
                formContentSource,
                formColor);

            spriteBatch.Draw(
                Texture,
                new Rectangle(240 + nameWidth / 2 - 10, formClickDestination.Y,
                              formRightSource.Width,
                              formRightSource.Height),
                formRightSource,
                formColor);


            spriteBatch.DrawString(FontBig,
                                   name,
                                   new Vector2(240 - nameWidth / 2 - 10,
                                               390),
                                   Color.Black * opacity);

            String scoreString = score.ToString();

            spriteBatch.DrawString(FontBig,
                                  scoreString,
                                  new Vector2(240 - FontBig.MeasureString(scoreString).X / 2,
                                              480),
                                  Color.Black * opacity);

            String levelString = level.ToString();

            spriteBatch.DrawString(FontBig,
                                  levelString,
                                  new Vector2(240 - FontBig.MeasureString(levelString).X / 2,
                                              560),
                                  Color.Black * opacity);

            spriteBatch.Draw(Texture,
                             TitlePosition,
                             TitleSource,
                             Color.White * opacity);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.name = reader.ReadLine();
            this.score = Int64.Parse(reader.ReadLine());
            this.level = Int32.Parse(reader.ReadLine());
            this.submitState = (SubmitState)Enum.Parse(submitState.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(name);
            writer.WriteLine(score);
            writer.WriteLine(level);
            writer.WriteLine(submitState);
        }

        #endregion

        #region Properties

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
                    this.retryClicked = false;
                    this.cancelClicked = false;
                    this.submitState = SubmitState.Submit;
                }
            }
        }

        public bool CancelClicked
        {
            get
            {
                return this.cancelClicked;
            }
        }

        public bool RetryClicked
        {
            get
            {
                return this.retryClicked;
            }
        }

        public bool ChangeNameClicked
        {
            set
            {
                this.changeNameClicked = value;
            }
            get
            {
                return this.changeNameClicked;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion
    }
}