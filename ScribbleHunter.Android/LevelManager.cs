using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace ScribbleHunter
{
    class LevelManager
    {
        #region Members

        private List<ILevel> components = new List<ILevel>();

        public const int StartLevel = 0;

        private float levelTimer = 0.0f;
        public const float TimeForStart = 1.0f;
        public const float TimeForLevel = 30.0f;

        public enum LevelStates
        {
            Starting, Running
        }

        private LevelStates levelState;

        private static int currentLevel;
        private int lastLevel;

        private bool hasChanged = false;

        Random rand = new Random();

        #endregion

        #region Constructors

        public LevelManager()
        {
            Reset();
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            levelTimer -= elapsed;

            if (levelTimer <= 0.0f)
            {
                this.hasChanged = true;
            }
            else
            {
                this.hasChanged = false;
            }
        }

        public void GoToNextState()
        {
            switch (levelState)
            {
                case LevelStates.Starting:
                    levelState = LevelStates.Running;
                    break;
                case LevelStates.Running:
                    // no change
                    break;
            }

            SetLevelAll(currentLevel + 1);

            resetTimer();
        }

        private void resetTimer()
        {
            switch (levelState)
            {
                case LevelStates.Starting:
                    levelTimer = TimeForStart;
                    break;
                case LevelStates.Running:
                    levelTimer = TimeForLevel;
                    break;
            }
        }

        public void Register(ILevel comp)
        {
            if (!components.Contains(comp))
            {
                components.Add(comp);
            }
        }

        private void SetLevelAll(int lvl)
        {
            this.lastLevel = currentLevel;
            currentLevel = lvl;

            foreach (var comp in components)
            {
                comp.SetLevel(lvl);
            }
        }

        public void SetLevel(int level)
        {
            currentLevel = level;

            SetLevelAll(currentLevel);
        }


        public void Reset()
        {
            levelState = LevelStates.Starting;

            levelTimer = TimeForStart;

            currentLevel = 0;
            this.lastLevel = 0;

            SetLevelAll(LevelManager.StartLevel);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.levelTimer = Single.Parse(reader.ReadLine());
            currentLevel = Int32.Parse(reader.ReadLine());
            this.lastLevel = Int32.Parse(reader.ReadLine());
            this.hasChanged = Boolean.Parse(reader.ReadLine());
            this.levelState = (LevelStates)Enum.Parse(levelState.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(levelTimer);
            writer.WriteLine(currentLevel);
            writer.WriteLine(lastLevel);
            writer.WriteLine(hasChanged);
            writer.WriteLine(levelState);
        }

        #endregion

        #region Properties

        public int CurrentLevel
        {
            get
            {
                return currentLevel;
            }
        }

        /// <summary>
        /// Implemented to let the powerup-manager have easy access to the current level.
        /// Very ugly code! Shame on you...
        /// </summary>
        public static int CurrentLevelStatic
        {
            get
            {
                return currentLevel;
            }
        }

        public bool HasChanged
        {
            get
            {
                return hasChanged;
            }
        }

        public LevelStates LevelState
        {
            get
            {
                return this.levelState;
            }
        }

        #endregion
    }
}