using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using ScribbleHunter.Inputs;

namespace ScribbleHunter
{
    class SubmissionManager
    {
        #region Members

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
        private readonly Rectangle TitleSource = new Rectangle(0, 0, 480, 200);
        private readonly Rectangle TitleDestination = new Rectangle(0, 100, 480, 200);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private long score;
        private int level;

        private bool cancelClicked = false;
        private bool retryClicked = false;
        private bool changeNameClicked = false;

        private const string TEXT_SCORE = "Score:";
        private const string TEXT_LEVEL = "Level:";

        public static GameInput GameInput;

        private const string CancelAction = "Cancel";
        private const string RetryAction = "Retry";

        #endregion

        #region Constructors

        private SubmissionManager()
        {
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);
            GameInput.AddTouchGestureInput(RetryAction,
                                           GestureType.Tap,
                                           retryDestination);
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
            if (GameInput.IsPressed(RetryAction))
            {
                retryClicked = true;
            }
            else if (GameInput.IsPressed(CancelAction))
            {
                cancelClicked = true;
            }
        }

        public void SetUp(long score, int level)
        {
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

            // Title
            spriteBatch.DrawString(FontSmall,
                                   TEXT_SCORE,
                                   new Vector2(240 - FontSmall.MeasureString(TEXT_SCORE).X / 2,
                                               400),
                                   Color.Black * opacity);

            spriteBatch.DrawString(FontSmall,
                                   TEXT_LEVEL,
                                   new Vector2(240 - FontSmall.MeasureString(TEXT_LEVEL).X / 2,
                                               500),
                                   Color.Black * opacity);

            String scoreString = score.ToString();

            spriteBatch.DrawString(FontBig,
                                  scoreString,
                                  new Vector2(240 - FontBig.MeasureString(scoreString).X / 2,
                                              425),
                                  Color.Black * opacity);

            String levelString = level.ToString();

            spriteBatch.DrawString(FontBig,
                                  levelString,
                                  new Vector2(240 - FontBig.MeasureString(levelString).X / 2,
                                              525),
                                  Color.Black * opacity);

            spriteBatch.Draw(Texture,
                             TitleDestination,
                             TitleSource,
                             Color.White * opacity);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.score = Int64.Parse(reader.ReadLine());
            this.level = Int32.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(score);
            writer.WriteLine(level);
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

        #endregion
    }
}