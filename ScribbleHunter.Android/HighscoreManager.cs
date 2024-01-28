using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScribbleHunter.Inputs;

namespace ScribbleHunter
{
    class HighscoreManager
    {
        #region Members

        public enum ScoreState { Local };

        private ScoreState scoreState = ScoreState.Local;

        private static HighscoreManager highscoreManager;

        private const string HIGHSCORE_FILE = "highscore.txt";

        private long currentHighScore;

        private List<Highscore> topScores = new List<Highscore>();
        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static SpriteFont Font;
        private readonly Rectangle LocalTitleSource = new Rectangle(0, 1280,
                                                                        240, 80);
        private readonly Vector2 TitlePosition = new Vector2(120.0f, 100.0f);

        private string lastName = "Unknown";

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        // TODO generally replace online highscores with GPGS
        // private WebBrowserTask browser;

        public static GameInput GameInput;

        private const int RankPositionX = 30;
        private const int NamePositionX = 70;
        private const int ScorePositionX = 395;
        private const int LevelPositionX = 450;

        private const string USERDATA_FILE = "user.txt";

        private const string DOTS3 = ". .";
        private const string DOTS6 = ". . . . .";
        private const string DOTS12 = ". . . . . .";
        private const string DOTS21 = ". . . . . . . . . . . . . . .";

        private const int RankPositionStartY = 250;
        private const int RankOffsetY = 40;

        #endregion

        #region Constructors

        private HighscoreManager()
        {
            this.LoadHighScore();

            this.loadUserData();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            // TODO cancel button to go back
            //GameInput.AddTouchGestureInput(RefreshAction,
            //                               GestureType.Tap,
            //                               refreshDestination);
        }

        public static HighscoreManager GetInstance()
        {
            if (highscoreManager == null)
            {
                highscoreManager = new HighscoreManager();
            }

            return highscoreManager;
        }

        private void handleTouchInputs()
        {
            // TODO cancel button to go back
            // Refresh
            //if (GameInput.IsPressed(RefreshAction))
            //{
            //    if (scoreState != ScoreState.Local)
            //    {
            //        SoundManager.PlayPaperSound();
            //        leaderboardManager.Receive();
            //    }
            //}
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (scoreState == ScoreState.Local)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 LocalTitleSource,
                                 Color.White * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (topScores[i].Score > 0)
                    {
                        Highscore h = new Highscore(topScores[i].Name, topScores[i].Score, topScores[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = DOTS12;
                        nameText = DOTS21;
                        levelText = DOTS3;
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(RankPositionX, RankPositionStartY + (i * RankOffsetY)),
                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(NamePositionX, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(ScorePositionX - Font.MeasureString(scoreText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(LevelPositionX - Font.MeasureString(levelText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);
                }
            }
        }


        /// <summary>
        /// Saves the current highscore to a text file.
        /// </summary>
        public void SaveHighScore(string name, long score, int level)
        {
            this.lastName = name;

            if (this.IsInScoreboard(score))
            {
                Highscore newScore = new Highscore(name, score, level);

                topScores.Add(newScore);
                this.sortScoreList();
                this.trimScoreList();

                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(HIGHSCORE_FILE, FileMode.Create, isf))
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            for (int i = 0; i < MaxScores; i++)
                            {
                                sw.WriteLine(topScores[i]);
                            }

                            //sw.Flush();
                            //sw.Close();
                        }
                    }
                }

                this.currentHighScore = maxScore();
            }

            this.saveUserData();
        }

        /// <summary>
        /// Loads the high score from a text file.
        /// </summary>
        private void LoadHighScore()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(HIGHSCORE_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(HIGHSCORE_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            for (int i = 0; i < MaxScores; i++)
                            {
                                topScores.Add(new Highscore(sr.ReadLine()));
                            }

                            this.sortScoreList();
                            this.currentHighScore = this.maxScore();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            for (int i = 0; i < MaxScores; i++)
                            {
                                Highscore newScore = new Highscore();
                                topScores.Add(newScore);
                                sw.WriteLine(newScore);
                            }

                            this.currentHighScore = 0;
                        }
                    }
                }
            }
        }

        private void sortScoreList()
        {
            for (int i = 0; i < topScores.Count; i++)
            {
                for (int j = 0; j < topScores.Count - 1; j++)
                {
                    if (topScores[j].Score < topScores[j + 1].Score)
                    {
                        Highscore tmp = topScores[j];
                        topScores[j] = topScores[j + 1];
                        topScores[j + 1] = tmp;
                    }
                }
            }
        }

        private void trimScoreList()
        {
            while (topScores.Count > MaxScores)
            {
                topScores.RemoveAt(topScores.Count - 1);
            }
        }

        private long maxScore()
        {
            long max = 0;

            for (int i = 0; i < topScores.Count; i++)
            {
                max = Math.Max(max, topScores[i].Score);
            }

            return max;
        }

        /// <summary>
        /// Checks wheather the score reaches top 10.
        /// </summary>
        /// <param name="score">The score to check</param>
        /// <returns>True if the player is under the top 1.</returns>
        public bool IsInScoreboard(long score)
        {
            return score > topScores[MaxScores - 1].Score;
        }

        /// <summary>
        /// Calculates the rank of the new score.
        /// </summary>
        /// <param name="score">The new score</param>
        /// <returns>Returns the calculated rank (-1, if the score is not top 10).</returns>
        public int GetRank(long score)
        {
            if (topScores.Count < 0)
                return 1;

            for (int i = 0; i < topScores.Count; i++)
            {
                if (topScores[i].Score < score)
                    return i + 1;
            }

            return -1;
        }

        public void Save()
        {
            this.saveUserData();
        }

        private void saveUserData()
        {

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(USERDATA_FILE, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.LastName);
                    }
                }
            }
        }

        private void loadUserData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(USERDATA_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(USERDATA_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            this.lastName = sr.ReadLine();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.lastName);
                        }
                    }
                }
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.currentHighScore = Int64.Parse(reader.ReadLine());
            this.lastName = reader.ReadLine();
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.scoreState = (ScoreState)Enum.Parse(scoreState.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(currentHighScore);
            writer.WriteLine(lastName);
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(scoreState);
        }

        #endregion

        #region Properties

        public long CurrentHighscore
        {
            get
            {
                return this.currentHighScore;
            }
        }

        public string LastName
        {
            set
            {
                this.lastName = value;
            }
            get
            {
                return this.lastName;
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