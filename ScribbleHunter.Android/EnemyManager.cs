#define IS_FREE_VERSION

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    /// <summary>
    /// Manages the enemies.
    /// </summary>
    class EnemyManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Enemy> Enemies = new List<Enemy>(64);

        private PlayerManager playerManager;

        public const int SOFT_ROCKET_EXPLOSION_RADIUS = 100;
        public const float ROCKET_POWER_AT_CENTER = 40.0f;

        public const int DAMAGE_LASER_MIN = 7;
        public const int DAMAGE_LASER_MAX = 10;

        public int MinShipsPerWave = 5;
        public int MaxShipsPerWave = 8;
        private float nextWaveTimer;
        public const float InitialNextWaveMinTimer = 5.0f;
        private float nextWaveMinTimer;
        public const float InitialNextWaveMaxTimer = 7.0f;
        private float nextWaveMaxTimer;

        private float nextSingleEnemyTimer;
        public const float InitialNextSingleEnemyMinTimer = 1.5f;
        private float nextSingleEnemyMinTimer;
        public const float InitialNextSingleEnemyMaxTimer = 3.0f;
        private float nextSingleEnemyMaxTimer;

        private List<Wave> waves = new List<Wave>(32);

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

#if IS_FREE_VERSION
        private const int OFFSET_Y = 80;
#else
        private const int OFFSET_Y = 0;
#endif

        #endregion

        #region Constructors

        public EnemyManager(Texture2D texture, PlayerManager playerManager,
                            Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            setUpWayPoints();

            this.currentLevel = 1;
        }

        #endregion

        #region Methods

        private void setUpWayPoints()
        {
            Wave wave1 = new Wave();
#if !IS_FREE_VERSION
            WaveEntity entry01_1 = new WaveEntity(new Vector2(25,75));
            entry01_1.AddRoutePoint(new Vector2(455, 75));
            wave1.AddEntry(entry01_1);
#endif
            WaveEntity entry01_2 = new WaveEntity(new Vector2(25, 150));
            entry01_2.AddRoutePoint(new Vector2(455, 150));
            wave1.AddEntry(entry01_2);
            WaveEntity entry01_3 = new WaveEntity(new Vector2(25, 225));
            entry01_3.AddRoutePoint(new Vector2(455, 225));
            wave1.AddEntry(entry01_3);
            WaveEntity entry01_4 = new WaveEntity(new Vector2(25, 300));
            entry01_4.AddRoutePoint(new Vector2(455, 300));
            wave1.AddEntry(entry01_4);
            WaveEntity entry01_5 = new WaveEntity(new Vector2(25, 375));
            entry01_5.AddRoutePoint(new Vector2(455, 375));
            wave1.AddEntry(entry01_5);
            waves.Add(wave1);

            Wave wave2 = new Wave();
#if !IS_FREE_VERSION
            WaveEntity entry02_1 = new WaveEntity(new Vector2(455, 100));
            entry02_1.AddRoutePoint(new Vector2(25, 100));
            wave2.AddEntry(entry02_1);
#endif
            WaveEntity entry02_2 = new WaveEntity(new Vector2(455, 175));
            entry02_2.AddRoutePoint(new Vector2(25, 175));
            wave2.AddEntry(entry02_2);
            WaveEntity entry02_3 = new WaveEntity(new Vector2(455, 250));
            entry02_3.AddRoutePoint(new Vector2(25, 250));
            wave2.AddEntry(entry02_3);
            WaveEntity entry02_4 = new WaveEntity(new Vector2(455, 325));
            entry02_4.AddRoutePoint(new Vector2(25, 325));
            wave2.AddEntry(entry02_4);
            WaveEntity entry02_5 = new WaveEntity(new Vector2(455, 400));
            entry02_5.AddRoutePoint(new Vector2(25, 400));
            wave2.AddEntry(entry02_5);
            waves.Add(wave2);

            Wave wave3 = new Wave();
            WaveEntity entry03_1 = new WaveEntity(new Vector2(25, 425));
            entry03_1.AddRoutePoint(new Vector2(455, 425));
            wave3.AddEntry(entry03_1);
            WaveEntity entry03_2 = new WaveEntity(new Vector2(25, 500));
            entry03_2.AddRoutePoint(new Vector2(455, 500));
            wave3.AddEntry(entry03_2);
            WaveEntity entry03_3 = new WaveEntity(new Vector2(25, 575));
            entry03_3.AddRoutePoint(new Vector2(455, 575));
            wave3.AddEntry(entry03_3);
            WaveEntity entry03_4 = new WaveEntity(new Vector2(25, 650));
            entry03_4.AddRoutePoint(new Vector2(455, 650));
            wave3.AddEntry(entry03_4);
            WaveEntity entry03_5 = new WaveEntity(new Vector2(25, 725));
            entry03_5.AddRoutePoint(new Vector2(455, 725));
            wave3.AddEntry(entry03_5);
            waves.Add(wave3);

            Wave wave4 = new Wave();
            WaveEntity entry04_1 = new WaveEntity(new Vector2(455, 450));
            entry04_1.AddRoutePoint(new Vector2(25, 450));
            wave4.AddEntry(entry04_1);
            WaveEntity entry04_2 = new WaveEntity(new Vector2(455, 525));
            entry04_2.AddRoutePoint(new Vector2(25, 525));
            wave4.AddEntry(entry04_2);
            WaveEntity entry04_3 = new WaveEntity(new Vector2(455, 600));
            entry04_3.AddRoutePoint(new Vector2(25, 600));
            wave4.AddEntry(entry04_3);
            WaveEntity entry04_4 = new WaveEntity(new Vector2(455, 675));
            entry04_4.AddRoutePoint(new Vector2(25, 675));
            wave4.AddEntry(entry04_4);
            WaveEntity entry04_5 = new WaveEntity(new Vector2(455, 750));
            entry04_5.AddRoutePoint(new Vector2(25, 750));
            wave4.AddEntry(entry04_5);
            waves.Add(wave4);

            Wave wave5 = new Wave();
            WaveEntity entry05_1 = new WaveEntity(new Vector2(40, 100 + OFFSET_Y));
            entry05_1.AddRoutePoint(new Vector2(40, 700));
            entry05_1.AddRoutePoint(new Vector2(120, 700));
            entry05_1.AddRoutePoint(new Vector2(120, 100 + OFFSET_Y));
            wave5.AddEntry(entry05_1);
            WaveEntity entry05_2 = new WaveEntity(new Vector2(120, 100 + OFFSET_Y));
            entry05_2.AddRoutePoint(new Vector2(120, 700));
            entry05_2.AddRoutePoint(new Vector2(200, 700));
            entry05_2.AddRoutePoint(new Vector2(200, 100 + OFFSET_Y));
            wave5.AddEntry(entry05_2);
            WaveEntity entry05_3 = new WaveEntity(new Vector2(200, 100 + OFFSET_Y));
            entry05_3.AddRoutePoint(new Vector2(200, 700));
            entry05_3.AddRoutePoint(new Vector2(280, 700));
            entry05_3.AddRoutePoint(new Vector2(280, 100 + OFFSET_Y));
            wave5.AddEntry(entry05_3);
            WaveEntity entry05_4 = new WaveEntity(new Vector2(280, 100 + OFFSET_Y));
            entry05_4.AddRoutePoint(new Vector2(280, 700));
            entry05_4.AddRoutePoint(new Vector2(360, 700));
            entry05_4.AddRoutePoint(new Vector2(360, 100 + OFFSET_Y));
            wave5.AddEntry(entry05_4);
            WaveEntity entry05_5 = new WaveEntity(new Vector2(360, 100 + OFFSET_Y));
            entry05_5.AddRoutePoint(new Vector2(360, 700));
            entry05_5.AddRoutePoint(new Vector2(440, 700));
            entry05_5.AddRoutePoint(new Vector2(440, 100 + OFFSET_Y));
            wave5.AddEntry(entry05_5);
            waves.Add(wave5);

            Wave wave6 = new Wave();
            WaveEntity entry06_1 = new WaveEntity(new Vector2(120, 100 + OFFSET_Y));
            entry06_1.AddRoutePoint(new Vector2(120, 700));
            entry06_1.AddRoutePoint(new Vector2(40, 700));
            entry06_1.AddRoutePoint(new Vector2(40, 100 + OFFSET_Y));
            wave6.AddEntry(entry06_1);
            WaveEntity entry06_2 = new WaveEntity(new Vector2(200, 100 + OFFSET_Y));
            entry06_2.AddRoutePoint(new Vector2(200, 700));
            entry06_2.AddRoutePoint(new Vector2(120, 700));
            entry06_2.AddRoutePoint(new Vector2(120, 100 + OFFSET_Y));
            wave6.AddEntry(entry06_2);
            WaveEntity entry06_3 = new WaveEntity(new Vector2(280, 100 + OFFSET_Y));
            entry06_3.AddRoutePoint(new Vector2(280, 700));
            entry06_3.AddRoutePoint(new Vector2(200, 700));
            entry06_3.AddRoutePoint(new Vector2(200, 100 + OFFSET_Y));
            wave6.AddEntry(entry06_3);
            WaveEntity entry06_4 = new WaveEntity(new Vector2(360, 100 + OFFSET_Y));
            entry06_4.AddRoutePoint(new Vector2(360, 700));
            entry06_4.AddRoutePoint(new Vector2(280, 700));
            entry06_4.AddRoutePoint(new Vector2(280, 100 + OFFSET_Y));
            wave6.AddEntry(entry06_4);
            WaveEntity entry06_5 = new WaveEntity(new Vector2(440, 100 + OFFSET_Y));
            entry06_5.AddRoutePoint(new Vector2(440, 700));
            entry06_5.AddRoutePoint(new Vector2(360, 700));
            entry06_5.AddRoutePoint(new Vector2(360, 100 + OFFSET_Y));
            wave6.AddEntry(entry06_5);
            waves.Add(wave6);

            Wave wave7 = new Wave();
            WaveEntity entry07_1 = new WaveEntity(new Vector2(40, 700));
            entry07_1.AddRoutePoint(new Vector2(40, 100 + OFFSET_Y));
            entry07_1.AddRoutePoint(new Vector2(120, 100 + OFFSET_Y));
            entry07_1.AddRoutePoint(new Vector2(120, 700));
            wave7.AddEntry(entry07_1);
            WaveEntity entry07_2 = new WaveEntity(new Vector2(120, 700));
            entry07_2.AddRoutePoint(new Vector2(120, 100 + OFFSET_Y));
            entry07_2.AddRoutePoint(new Vector2(200, 100 + OFFSET_Y));
            entry07_2.AddRoutePoint(new Vector2(200, 700));
            wave7.AddEntry(entry07_2);
            WaveEntity entry07_3 = new WaveEntity(new Vector2(200, 700));
            entry07_3.AddRoutePoint(new Vector2(200, 100 + OFFSET_Y));
            entry07_3.AddRoutePoint(new Vector2(280, 100 + OFFSET_Y));
            entry07_3.AddRoutePoint(new Vector2(280, 700));
            wave7.AddEntry(entry07_3);
            WaveEntity entry07_4 = new WaveEntity(new Vector2(280, 700));
            entry07_4.AddRoutePoint(new Vector2(280, 100 + OFFSET_Y));
            entry07_4.AddRoutePoint(new Vector2(360, 100 + OFFSET_Y));
            entry07_4.AddRoutePoint(new Vector2(360, 700));
            wave7.AddEntry(entry07_4);
            WaveEntity entry07_5 = new WaveEntity(new Vector2(360, 700));
            entry07_5.AddRoutePoint(new Vector2(360, 100 + OFFSET_Y));
            entry07_5.AddRoutePoint(new Vector2(440, 100 + OFFSET_Y));
            entry07_5.AddRoutePoint(new Vector2(440, 700));
            wave7.AddEntry(entry07_5);
            waves.Add(wave7);

            Wave wave8 = new Wave();
            WaveEntity entry08_1 = new WaveEntity(new Vector2(120, 700));
            entry08_1.AddRoutePoint(new Vector2(120, 100 + OFFSET_Y));
            entry08_1.AddRoutePoint(new Vector2(40, 100 + OFFSET_Y));
            entry08_1.AddRoutePoint(new Vector2(40, 700));
            wave8.AddEntry(entry08_1);
            WaveEntity entry08_2 = new WaveEntity(new Vector2(200, 700));
            entry08_2.AddRoutePoint(new Vector2(200, 100 + OFFSET_Y));
            entry08_2.AddRoutePoint(new Vector2(120, 100 + OFFSET_Y));
            entry08_2.AddRoutePoint(new Vector2(120, 700));
            wave8.AddEntry(entry08_2);
            WaveEntity entry08_3 = new WaveEntity(new Vector2(280, 700));
            entry08_3.AddRoutePoint(new Vector2(280, 100 + OFFSET_Y));
            entry08_3.AddRoutePoint(new Vector2(200, 100 + OFFSET_Y));
            entry08_3.AddRoutePoint(new Vector2(200, 700));
            wave8.AddEntry(entry08_3);
            WaveEntity entry08_4 = new WaveEntity(new Vector2(360, 700));
            entry08_4.AddRoutePoint(new Vector2(360, 100 + OFFSET_Y));
            entry08_4.AddRoutePoint(new Vector2(280, 100 + OFFSET_Y));
            entry08_4.AddRoutePoint(new Vector2(280, 700));
            wave8.AddEntry(entry08_4);
            WaveEntity entry08_5 = new WaveEntity(new Vector2(440, 700));
            entry08_5.AddRoutePoint(new Vector2(440, 100 + OFFSET_Y));
            entry08_5.AddRoutePoint(new Vector2(360, 100 + OFFSET_Y));
            entry08_5.AddRoutePoint(new Vector2(360, 700));
            wave8.AddEntry(entry08_5);
            waves.Add(wave8);

            Wave wave9 = new Wave();
#if !IS_FREE_VERSION
            WaveEntity entry09_1 = new WaveEntity(new Vector2(240, 100));
            entry09_1.AddRoutePoint(new Vector2(25, 100));
            wave9.AddEntry(entry09_1);
#endif
            WaveEntity entry09_2 = new WaveEntity(new Vector2(240, 200));
            entry09_2.AddRoutePoint(new Vector2(25, 200));
            wave9.AddEntry(entry09_2);
            WaveEntity entry09_3 = new WaveEntity(new Vector2(240, 300));
            entry09_3.AddRoutePoint(new Vector2(25, 300));
            wave9.AddEntry(entry09_3);
            WaveEntity entry09_4 = new WaveEntity(new Vector2(240, 400));
            entry09_4.AddRoutePoint(new Vector2(25, 400));
            wave9.AddEntry(entry09_4);
            WaveEntity entry09_5 = new WaveEntity(new Vector2(240, 500));
            entry09_5.AddRoutePoint(new Vector2(25, 500));
            wave9.AddEntry(entry09_5);
            WaveEntity entry09_6 = new WaveEntity(new Vector2(240, 600));
            entry09_6.AddRoutePoint(new Vector2(25, 600));
            wave9.AddEntry(entry09_6);
            WaveEntity entry09_7 = new WaveEntity(new Vector2(240, 700));
            entry09_7.AddRoutePoint(new Vector2(25, 700));
            wave9.AddEntry(entry09_7);
            waves.Add(wave9);

            Wave wave10 = new Wave();
#if !IS_FREE_VERSION
            WaveEntity entry10_1 = new WaveEntity(new Vector2(240, 100));
            entry10_1.AddRoutePoint(new Vector2(455, 100));
            wave10.AddEntry(entry10_1);
#endif
            WaveEntity entry10_2 = new WaveEntity(new Vector2(240, 200));
            entry10_2.AddRoutePoint(new Vector2(455, 200));
            wave10.AddEntry(entry10_2);
            WaveEntity entry10_3 = new WaveEntity(new Vector2(240, 300));
            entry10_3.AddRoutePoint(new Vector2(455, 300));
            wave10.AddEntry(entry10_3);
            WaveEntity entry10_4 = new WaveEntity(new Vector2(240, 400));
            entry10_4.AddRoutePoint(new Vector2(455, 400));
            wave10.AddEntry(entry10_4);
            WaveEntity entry10_5 = new WaveEntity(new Vector2(240, 500));
            entry10_5.AddRoutePoint(new Vector2(455, 500));
            wave10.AddEntry(entry10_5);
            WaveEntity entry10_6 = new WaveEntity(new Vector2(240, 600));
            entry10_6.AddRoutePoint(new Vector2(455, 600));
            wave10.AddEntry(entry10_6);
            WaveEntity entry10_7 = new WaveEntity(new Vector2(240, 700));
            entry10_7.AddRoutePoint(new Vector2(455, 700));
            wave10.AddEntry(entry10_7);
            waves.Add(wave10);

            Wave wave11 = new Wave();
            WaveEntity entry11_1 = new WaveEntity(new Vector2(220, 25 + OFFSET_Y));
            entry11_1.AddRoutePoint(new Vector2(220, 750));
            wave11.AddEntry(entry11_1);
            WaveEntity entry11_2 = new WaveEntity(new Vector2(180, 25 + OFFSET_Y));
            entry11_2.AddRoutePoint(new Vector2(180, 750));
            wave11.AddEntry(entry11_2);
            WaveEntity entry11_3 = new WaveEntity(new Vector2(140, 25 + OFFSET_Y));
            entry11_3.AddRoutePoint(new Vector2(140, 750));
            wave11.AddEntry(entry11_3);
            WaveEntity entry11_4 = new WaveEntity(new Vector2(100, 25 + OFFSET_Y));
            entry11_4.AddRoutePoint(new Vector2(100, 750));
            wave11.AddEntry(entry11_4);
            WaveEntity entry11_5 = new WaveEntity(new Vector2(60, 25 + OFFSET_Y));
            entry11_5.AddRoutePoint(new Vector2(60, 750));
            wave11.AddEntry(entry11_5);
            WaveEntity entry11_6 = new WaveEntity(new Vector2(20, 25 + OFFSET_Y));
            entry11_6.AddRoutePoint(new Vector2(20, 750));
            wave11.AddEntry(entry11_6);
            WaveEntity entry11_7 = new WaveEntity(new Vector2(260, 25 + OFFSET_Y));
            entry11_7.AddRoutePoint(new Vector2(260, 750));
            wave11.AddEntry(entry11_7);
            WaveEntity entry11_8 = new WaveEntity(new Vector2(300, 25 + OFFSET_Y));
            entry11_8.AddRoutePoint(new Vector2(300, 750));
            wave11.AddEntry(entry11_8);
            WaveEntity entry11_9 = new WaveEntity(new Vector2(340, 25 + OFFSET_Y));
            entry11_9.AddRoutePoint(new Vector2(340, 750));
            wave11.AddEntry(entry11_9);
            WaveEntity entry11_10 = new WaveEntity(new Vector2(380, 25 + OFFSET_Y));
            entry11_10.AddRoutePoint(new Vector2(380, 750));
            wave11.AddEntry(entry11_10);
            WaveEntity entry11_11 = new WaveEntity(new Vector2(420, 25 + OFFSET_Y));
            entry11_11.AddRoutePoint(new Vector2(420, 750));
            wave11.AddEntry(entry11_11);
            WaveEntity entry11_12 = new WaveEntity(new Vector2(460, 25 + OFFSET_Y));
            entry11_12.AddRoutePoint(new Vector2(460, 750));
            wave11.AddEntry(entry11_12);
            waves.Add(wave11);

            Wave wave12 = new Wave();
            WaveEntity entry12_1 = new WaveEntity(new Vector2(220, 750));
            entry12_1.AddRoutePoint(new Vector2(220, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_1);
            WaveEntity entry12_2 = new WaveEntity(new Vector2(180, 750));
            entry12_2.AddRoutePoint(new Vector2(180, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_2);
            WaveEntity entry12_3 = new WaveEntity(new Vector2(140, 750));
            entry12_3.AddRoutePoint(new Vector2(140, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_3);
            WaveEntity entry12_4 = new WaveEntity(new Vector2(100, 750));
            entry12_4.AddRoutePoint(new Vector2(100, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_4);
            WaveEntity entry12_5 = new WaveEntity(new Vector2(60, 750));
            entry12_5.AddRoutePoint(new Vector2(60, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_5);
            WaveEntity entry12_6 = new WaveEntity(new Vector2(20, 750));
            entry12_6.AddRoutePoint(new Vector2(20, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_6);
            WaveEntity entry12_7 = new WaveEntity(new Vector2(260, 750));
            entry12_7.AddRoutePoint(new Vector2(260, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_7);
            WaveEntity entry12_8 = new WaveEntity(new Vector2(300, 750));
            entry12_8.AddRoutePoint(new Vector2(300, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_8);
            WaveEntity entry12_9 = new WaveEntity(new Vector2(340, 750));
            entry12_9.AddRoutePoint(new Vector2(340, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_9);
            WaveEntity entry12_10 = new WaveEntity(new Vector2(380, 750));
            entry12_10.AddRoutePoint(new Vector2(380, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_10);
            WaveEntity entry12_11 = new WaveEntity(new Vector2(420, 750));
            entry12_11.AddRoutePoint(new Vector2(420, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_11);
            WaveEntity entry12_12 = new WaveEntity(new Vector2(460, 750));
            entry12_12.AddRoutePoint(new Vector2(460, 25 + OFFSET_Y));
            wave12.AddEntry(entry12_12);
            waves.Add(wave12);

            Wave wave13 = new Wave();
            WaveEntity entry13_1 = new WaveEntity(new Vector2(25, 380));
            entry13_1.AddRoutePoint(new Vector2(455, 380));
            wave13.AddEntry(entry13_1);
            WaveEntity entry13_2 = new WaveEntity(new Vector2(25, 340));
            entry13_2.AddRoutePoint(new Vector2(455, 340));
            wave13.AddEntry(entry13_2);
            WaveEntity entry13_3 = new WaveEntity(new Vector2(25, 300));
            entry13_3.AddRoutePoint(new Vector2(455, 300));
            wave13.AddEntry(entry13_3);
            WaveEntity entry13_4 = new WaveEntity(new Vector2(25, 260));
            entry13_4.AddRoutePoint(new Vector2(455, 260));
            wave13.AddEntry(entry13_4);
            WaveEntity entry13_5 = new WaveEntity(new Vector2(25, 220));
            entry13_5.AddRoutePoint(new Vector2(455, 220));
            wave13.AddEntry(entry13_5);
            WaveEntity entry13_6 = new WaveEntity(new Vector2(25, 180));
            entry13_6.AddRoutePoint(new Vector2(455, 180));
            wave13.AddEntry(entry13_6);
            WaveEntity entry13_7 = new WaveEntity(new Vector2(25, 140));
            entry13_7.AddRoutePoint(new Vector2(455, 140));
            wave13.AddEntry(entry13_7);
#if !IS_FREE_VERSION
            WaveEntity entry13_8 = new WaveEntity(new Vector2(25, 100));
            entry13_8.AddRoutePoint(new Vector2(455, 100));
            wave13.AddEntry(entry13_8);
            WaveEntity entry13_9 = new WaveEntity(new Vector2(25, 60));
            entry13_9.AddRoutePoint(new Vector2(455, 60));
            wave13.AddEntry(entry13_9);
#endif
            WaveEntity entry13_10 = new WaveEntity(new Vector2(25, 420));
            entry13_10.AddRoutePoint(new Vector2(455, 420));
            wave13.AddEntry(entry13_10);
            WaveEntity entry13_11 = new WaveEntity(new Vector2(25, 460));
            entry13_11.AddRoutePoint(new Vector2(455, 460));
            wave13.AddEntry(entry13_11);
            WaveEntity entry13_12 = new WaveEntity(new Vector2(25, 500));
            entry13_12.AddRoutePoint(new Vector2(455, 500));
            wave13.AddEntry(entry13_12);
            WaveEntity entry13_13 = new WaveEntity(new Vector2(25, 540));
            entry13_13.AddRoutePoint(new Vector2(455, 540));
            wave13.AddEntry(entry13_13);
            WaveEntity entry13_14 = new WaveEntity(new Vector2(25, 580));
            entry13_14.AddRoutePoint(new Vector2(455, 580));
            wave13.AddEntry(entry13_14);
            WaveEntity entry13_15 = new WaveEntity(new Vector2(25, 620));
            entry13_15.AddRoutePoint(new Vector2(455, 620));
            wave13.AddEntry(entry13_15);
            WaveEntity entry13_16 = new WaveEntity(new Vector2(25, 660));
            entry13_16.AddRoutePoint(new Vector2(455, 660));
            wave13.AddEntry(entry13_16);
            WaveEntity entry13_17 = new WaveEntity(new Vector2(25, 700));
            entry13_17.AddRoutePoint(new Vector2(455, 700));
            wave13.AddEntry(entry13_17);
            WaveEntity entry13_18 = new WaveEntity(new Vector2(25, 740));
            entry13_18.AddRoutePoint(new Vector2(455, 740));
            wave13.AddEntry(entry13_18);
            waves.Add(wave13);

            Wave wave14 = new Wave();
            WaveEntity entry14_1 = new WaveEntity(new Vector2(455, 380));
            entry14_1.AddRoutePoint(new Vector2(25, 380));
            wave14.AddEntry(entry14_1);
            WaveEntity entry14_2 = new WaveEntity(new Vector2(455, 340));
            entry14_2.AddRoutePoint(new Vector2(25, 340));
            wave14.AddEntry(entry14_2);
            WaveEntity entry14_3 = new WaveEntity(new Vector2(455, 300));
            entry14_3.AddRoutePoint(new Vector2(25, 300));
            wave14.AddEntry(entry14_3);
            WaveEntity entry14_4 = new WaveEntity(new Vector2(455, 260));
            entry14_4.AddRoutePoint(new Vector2(25, 260));
            wave14.AddEntry(entry14_4);
            WaveEntity entry14_5 = new WaveEntity(new Vector2(455, 220));
            entry14_5.AddRoutePoint(new Vector2(25, 220));
            wave14.AddEntry(entry14_5);
            WaveEntity entry14_6 = new WaveEntity(new Vector2(455, 180));
            entry14_6.AddRoutePoint(new Vector2(25, 180));
            wave14.AddEntry(entry14_6);
            WaveEntity entry14_7 = new WaveEntity(new Vector2(455, 140));
            entry14_7.AddRoutePoint(new Vector2(25, 140));
            wave14.AddEntry(entry14_7);
#if !IS_FREE_VERSION
            WaveEntity entry14_8 = new WaveEntity(new Vector2(455, 100));
            entry14_8.AddRoutePoint(new Vector2(25, 100));
            wave14.AddEntry(entry14_8);
            WaveEntity entry14_9 = new WaveEntity(new Vector2(455, 60));
            entry14_9.AddRoutePoint(new Vector2(25, 60));
            wave14.AddEntry(entry14_9);
#endif
            WaveEntity entry14_10 = new WaveEntity(new Vector2(455, 420));
            entry14_10.AddRoutePoint(new Vector2(25, 420));
            wave14.AddEntry(entry14_10);
            WaveEntity entry14_11 = new WaveEntity(new Vector2(455, 460));
            entry14_11.AddRoutePoint(new Vector2(25, 460));
            wave14.AddEntry(entry14_11);
            WaveEntity entry14_12 = new WaveEntity(new Vector2(455, 500));
            entry14_12.AddRoutePoint(new Vector2(25, 500));
            wave14.AddEntry(entry14_12);
            WaveEntity entry14_13 = new WaveEntity(new Vector2(455, 540));
            entry14_13.AddRoutePoint(new Vector2(25, 540));
            wave14.AddEntry(entry14_13);
            WaveEntity entry14_14 = new WaveEntity(new Vector2(455, 580));
            entry14_14.AddRoutePoint(new Vector2(25, 580));
            wave14.AddEntry(entry14_14);
            WaveEntity entry14_15 = new WaveEntity(new Vector2(455, 620));
            entry14_15.AddRoutePoint(new Vector2(25, 620));
            wave14.AddEntry(entry14_15);
            WaveEntity entry14_16 = new WaveEntity(new Vector2(455, 660));
            entry14_16.AddRoutePoint(new Vector2(25, 660));
            wave14.AddEntry(entry14_16);
            WaveEntity entry14_17 = new WaveEntity(new Vector2(455, 700));
            entry14_17.AddRoutePoint(new Vector2(25, 700));
            wave14.AddEntry(entry14_17);
            WaveEntity entry14_18 = new WaveEntity(new Vector2(455, 740));
            entry14_18.AddRoutePoint(new Vector2(25, 740));
            wave14.AddEntry(entry14_18);
            waves.Add(wave14);

            /* since Version 1.3 */

            Wave wave15 = new Wave();
            WaveEntity entry15_1 = new WaveEntity(new Vector2(240, 250));
            entry15_1.AddRoutePoint(new Vector2(240, 550));
            wave15.AddEntry(entry15_1);
            WaveEntity entry15_2 = new WaveEntity(new Vector2(205, 230));
            entry15_2.AddRoutePoint(new Vector2(205, 530));
            wave15.AddEntry(entry15_2);
            WaveEntity entry15_3 = new WaveEntity(new Vector2(275, 230));
            entry15_3.AddRoutePoint(new Vector2(275, 530));
            wave15.AddEntry(entry15_3);
            WaveEntity entry15_4 = new WaveEntity(new Vector2(170, 210));
            entry15_4.AddRoutePoint(new Vector2(170, 510));
            wave15.AddEntry(entry15_4);
            WaveEntity entry15_5 = new WaveEntity(new Vector2(310, 210));
            entry15_5.AddRoutePoint(new Vector2(310, 510));
            wave15.AddEntry(entry15_5);
            WaveEntity entry15_6 = new WaveEntity(new Vector2(135, 190));
            entry15_6.AddRoutePoint(new Vector2(135, 490));
            wave15.AddEntry(entry15_6);
            WaveEntity entry15_7 = new WaveEntity(new Vector2(345, 190));
            entry15_7.AddRoutePoint(new Vector2(345, 490));
            wave15.AddEntry(entry15_7);
            WaveEntity entry15_8 = new WaveEntity(new Vector2(100, 170));
            entry15_8.AddRoutePoint(new Vector2(100, 470));
            wave15.AddEntry(entry15_8);
            WaveEntity entry15_9 = new WaveEntity(new Vector2(380, 170));
            entry15_9.AddRoutePoint(new Vector2(380, 470));
            wave15.AddEntry(entry15_9);
            WaveEntity entry15_10 = new WaveEntity(new Vector2(65, 150));
            entry15_10.AddRoutePoint(new Vector2(65, 450));
            wave15.AddEntry(entry15_10);
            WaveEntity entry15_11 = new WaveEntity(new Vector2(415, 150));
            entry15_11.AddRoutePoint(new Vector2(415, 450));
            wave15.AddEntry(entry15_11);
            waves.Add(wave15);

            Wave wave16 = new Wave();
            WaveEntity entry16_1 = new WaveEntity(new Vector2(240, 250));
            entry16_1.AddRoutePoint(new Vector2(240, 550));
            wave16.AddEntry(entry16_1);
            WaveEntity entry16_2 = new WaveEntity(new Vector2(205, 230));
            entry16_2.AddRoutePoint(new Vector2(205, 550));
            wave16.AddEntry(entry16_2);
            WaveEntity entry16_3 = new WaveEntity(new Vector2(275, 230));
            entry16_3.AddRoutePoint(new Vector2(275, 550));
            wave16.AddEntry(entry16_3);
            WaveEntity entry16_4 = new WaveEntity(new Vector2(170, 210));
            entry16_4.AddRoutePoint(new Vector2(170, 550));
            wave16.AddEntry(entry16_4);
            WaveEntity entry16_5 = new WaveEntity(new Vector2(310, 210));
            entry16_5.AddRoutePoint(new Vector2(310, 550));
            wave16.AddEntry(entry16_5);
            WaveEntity entry16_6 = new WaveEntity(new Vector2(135, 190));
            entry16_6.AddRoutePoint(new Vector2(135, 550));
            wave16.AddEntry(entry16_6);
            WaveEntity entry16_7 = new WaveEntity(new Vector2(345, 190));
            entry16_7.AddRoutePoint(new Vector2(345, 550));
            wave16.AddEntry(entry16_7);
            WaveEntity entry16_8 = new WaveEntity(new Vector2(100, 170));
            entry16_8.AddRoutePoint(new Vector2(100, 550));
            wave16.AddEntry(entry16_8);
            WaveEntity entry16_9 = new WaveEntity(new Vector2(380, 170));
            entry16_9.AddRoutePoint(new Vector2(380, 550));
            wave16.AddEntry(entry16_9);
            WaveEntity entry16_10 = new WaveEntity(new Vector2(65, 150));
            entry16_10.AddRoutePoint(new Vector2(65, 550));
            wave16.AddEntry(entry16_10);
            WaveEntity entry16_11 = new WaveEntity(new Vector2(415, 150));
            entry16_11.AddRoutePoint(new Vector2(415, 550));
            wave16.AddEntry(entry16_11);
            waves.Add(wave16);

            Wave wave17 = new Wave();
            WaveEntity entry17_1 = new WaveEntity(new Vector2(240, 450));
            entry17_1.AddRoutePoint(new Vector2(240, 250));
            wave17.AddEntry(entry17_1);
            WaveEntity entry17_2 = new WaveEntity(new Vector2(205, 470));
            entry17_2.AddRoutePoint(new Vector2(205, 230));
            wave17.AddEntry(entry17_2);
            WaveEntity entry17_3 = new WaveEntity(new Vector2(275, 470));
            entry17_3.AddRoutePoint(new Vector2(275, 230));
            wave17.AddEntry(entry17_3);
            WaveEntity entry17_4 = new WaveEntity(new Vector2(170, 490));
            entry17_4.AddRoutePoint(new Vector2(170, 210));
            wave17.AddEntry(entry17_4);
            WaveEntity entry17_5 = new WaveEntity(new Vector2(310, 490));
            entry17_5.AddRoutePoint(new Vector2(310, 210));
            wave17.AddEntry(entry17_5);
            WaveEntity entry17_6 = new WaveEntity(new Vector2(135, 510));
            entry17_6.AddRoutePoint(new Vector2(135, 190));
            wave17.AddEntry(entry17_6);
            WaveEntity entry17_7 = new WaveEntity(new Vector2(345, 510));
            entry17_7.AddRoutePoint(new Vector2(345, 190));
            wave17.AddEntry(entry17_7);
            WaveEntity entry17_8 = new WaveEntity(new Vector2(100, 530));
            entry17_8.AddRoutePoint(new Vector2(100, 170));
            wave17.AddEntry(entry17_8);
            WaveEntity entry17_9 = new WaveEntity(new Vector2(380, 530));
            entry17_9.AddRoutePoint(new Vector2(380, 170));
            wave17.AddEntry(entry17_9);
            WaveEntity entry17_10 = new WaveEntity(new Vector2(65, 550));
            entry17_10.AddRoutePoint(new Vector2(65, 150));
            wave17.AddEntry(entry17_10);
            WaveEntity entry17_11 = new WaveEntity(new Vector2(415, 550));
            entry17_11.AddRoutePoint(new Vector2(415, 150));
            wave17.AddEntry(entry17_11);
            waves.Add(wave17);

            Wave wave18 = new Wave();
            WaveEntity entry18_1 = new WaveEntity(new Vector2(240, 540));
            entry18_1.AddRoutePoint(new Vector2(240, 250));
            wave18.AddEntry(entry18_1);
            WaveEntity entry18_2 = new WaveEntity(new Vector2(205, 560));
            entry18_2.AddRoutePoint(new Vector2(205, 230));
            wave18.AddEntry(entry18_2);
            WaveEntity entry18_3 = new WaveEntity(new Vector2(275, 560));
            entry18_3.AddRoutePoint(new Vector2(275, 230));
            wave18.AddEntry(entry18_3);
            WaveEntity entry18_4 = new WaveEntity(new Vector2(170, 540));
            entry18_4.AddRoutePoint(new Vector2(170, 210));
            wave18.AddEntry(entry18_4);
            WaveEntity entry18_5 = new WaveEntity(new Vector2(310, 540));
            entry18_5.AddRoutePoint(new Vector2(310, 210));
            wave18.AddEntry(entry18_5);
            WaveEntity entry18_6 = new WaveEntity(new Vector2(135, 560));
            entry18_6.AddRoutePoint(new Vector2(135, 190));
            wave18.AddEntry(entry18_6);
            WaveEntity entry18_7 = new WaveEntity(new Vector2(345, 560));
            entry18_7.AddRoutePoint(new Vector2(345, 190));
            wave18.AddEntry(entry18_7);
            WaveEntity entry18_8 = new WaveEntity(new Vector2(100, 540));
            entry18_8.AddRoutePoint(new Vector2(100, 170));
            wave18.AddEntry(entry18_8);
            WaveEntity entry18_9 = new WaveEntity(new Vector2(380, 540));
            entry18_9.AddRoutePoint(new Vector2(380, 170));
            wave18.AddEntry(entry18_9);
            WaveEntity entry18_10 = new WaveEntity(new Vector2(65, 560));
            entry18_10.AddRoutePoint(new Vector2(65, 150));
            wave18.AddEntry(entry18_10);
            WaveEntity entry18_11 = new WaveEntity(new Vector2(415, 560));
            entry18_11.AddRoutePoint(new Vector2(415, 150));
            wave18.AddEntry(entry18_11);
            waves.Add(wave18);

            Wave wave19 = new Wave();
            WaveEntity entry19_1 = new WaveEntity(new Vector2(50, 150));
            entry19_1.AddRoutePoint(new Vector2(430, 150));
            wave19.AddEntry(entry19_1);
            WaveEntity entry19_2 = new WaveEntity(new Vector2(430, 200));
            entry19_2.AddRoutePoint(new Vector2(50, 200));
            wave19.AddEntry(entry19_2);
            WaveEntity entry19_3 = new WaveEntity(new Vector2(50, 250));
            entry19_3.AddRoutePoint(new Vector2(430, 250));
            wave19.AddEntry(entry19_3);
            WaveEntity entry19_4 = new WaveEntity(new Vector2(430, 300));
            entry19_4.AddRoutePoint(new Vector2(50, 300));
            wave19.AddEntry(entry19_4);
            WaveEntity entry19_5 = new WaveEntity(new Vector2(50, 350));
            entry19_5.AddRoutePoint(new Vector2(430, 350));
            wave19.AddEntry(entry19_5);
            WaveEntity entry19_6 = new WaveEntity(new Vector2(430, 400));
            entry19_6.AddRoutePoint(new Vector2(50, 400));
            wave19.AddEntry(entry19_6);
            WaveEntity entry19_7 = new WaveEntity(new Vector2(50, 450));
            entry19_7.AddRoutePoint(new Vector2(430, 450));
            wave19.AddEntry(entry19_7);
            WaveEntity entry19_8 = new WaveEntity(new Vector2(430, 500));
            entry19_8.AddRoutePoint(new Vector2(50, 500));
            wave19.AddEntry(entry19_8);
            WaveEntity entry19_9 = new WaveEntity(new Vector2(50, 550));
            entry19_9.AddRoutePoint(new Vector2(430, 550));
            wave19.AddEntry(entry19_9);
            WaveEntity entry19_10 = new WaveEntity(new Vector2(430, 600));
            entry19_10.AddRoutePoint(new Vector2(50, 600));
            wave19.AddEntry(entry19_10);
            WaveEntity entry19_11 = new WaveEntity(new Vector2(50, 650));
            entry19_11.AddRoutePoint(new Vector2(430, 650));
            wave19.AddEntry(entry19_11);
            waves.Add(wave19);
        }

        public void SpawnEnemy(WaveEntity waveEntry, EnemyType type)
        {
            Enemy newEnemy;

            switch (type)
            {
                case EnemyType.Medium:
                    newEnemy = Enemy.CreateMediumEnemy(texture,
                                                       waveEntry.StartPosition);
                    break;

                case EnemyType.Hard:
                    newEnemy = Enemy.CreateHardEnemy(texture,
                                                     waveEntry.StartPosition);
                    break;

                case EnemyType.Speeder:
                    newEnemy = Enemy.CreateSpeederEnemy(texture,
                                                        waveEntry.StartPosition);
                    break;

                case EnemyType.Tank:
                    newEnemy = Enemy.CreateTankEnemy(texture,
                                                     waveEntry.StartPosition);
                    break;

                default:
                    newEnemy = Enemy.CreateEasyEnemy(texture,
                                                     waveEntry.StartPosition);
                    break;
            }

            newEnemy.SetLevel(currentLevel);

            for (int x = 0; x < waveEntry.RoutePoints.Count; x++)
            {
                newEnemy.AddWayPoint(waveEntry.RoutePoints[x]);
            }

            Enemies.Add(newEnemy);

            EffectManager.AddSpawnSmoke(newEnemy.EnemySprite.Center);
        }

        public void SpawnWave(Wave wave)
        {
            EnemyType type;
            int rnd = rand.Next(0, 5);

            switch (rnd)
            {
                case 0:
                    type = EnemyType.Medium;
                    break;

                case 1:
                    type = EnemyType.Hard;
                    break;

                case 2:
                    type = EnemyType.Speeder;
                    break;

                case 3:
                    type = EnemyType.Tank;
                    break;

                default:
                    type = EnemyType.Easy;
                    break;
            }

            for (int i = 0; i < wave.Entries.Count; ++i)
            {
                SpawnEnemy(wave.Entries[i], type);
            }
        }

        public void SpawnSingleEnemy()
        {
            Enemy newEnemy;
            int rnd = rand.Next(0, 5);

            Vector2 randomPosition = getRandomPosition();

            switch (rnd)
            {
                case 0:
                    newEnemy = Enemy.CreateMediumEnemy(texture,
                                                       randomPosition);
                    break;

                case 1:
                    newEnemy = Enemy.CreateHardEnemy(texture,
                                                     randomPosition);
                    break;

                case 2:
                    newEnemy = Enemy.CreateSpeederEnemy(texture,
                                                        randomPosition);
                    break;

                case 3:
                    newEnemy = Enemy.CreateTankEnemy(texture,
                                                     randomPosition);
                    break;

                default:
                    newEnemy = Enemy.CreateEasyEnemy(texture,
                                                     randomPosition);
                    break;
            }

            newEnemy.SetLevel(currentLevel);

            Enemies.Add(newEnemy);

            EffectManager.AddSpawnSmoke(newEnemy.EnemySprite.Center);
        }

        private Vector2 getRandomPosition()
        {
            Vector2 pos;
            do
            {
                int x = rand.Next(25, 455);
#if IS_FREE_VERSION
                int y = rand.Next(105, 775);
#else
                int y = rand.Next(25, 775);
#endif
                pos = new Vector2(x, y);
            } while ((playerManager.playerSprite.Center - pos).Length() < 100);

            return pos;
        }

        private void updateWaveSpawns(float elapsed)
        {
            nextWaveTimer -= elapsed;

            if (nextWaveTimer <= 0)
            {
                int rnd1 = rand.Next(0, waves.Count);

                SpawnWave(waves[rnd1]);

                nextWaveTimer = nextWaveMinTimer + (float)rand.NextDouble() * (nextWaveMaxTimer - nextWaveMinTimer);
            }
        }

        private void updateSingleEnemySpawns(float elapsed)
        {
            nextSingleEnemyTimer -= elapsed;

            if (nextSingleEnemyTimer <= 0.0f)
            {
                SpawnSingleEnemy();

                nextSingleEnemyTimer = nextSingleEnemyMinTimer + (float)rand.NextDouble() * (nextSingleEnemyMaxTimer - nextSingleEnemyMinTimer);
            }
        }

        public void Update(float elapsed)
        {
            for (int x = Enemies.Count - 1; x >= 0; --x)
            {
                Enemy enemy = Enemies[x];

                if (!playerManager.IsDestroyed)
                    enemy.CurrentTarget = playerManager.playerSprite.Center;
                else
                    enemy.NotifyPlayerKilled();

                enemy.Update(elapsed);

                if (!enemy.IsActive())
                {
                    Enemies.RemoveAt(x);
                }
            }

            if (this.IsActive)
            {
                updateWaveSpawns(elapsed);
                updateSingleEnemySpawns(elapsed);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        public void Reset()
        {
            this.Enemies.Clear();

            this.nextWaveTimer = InitialNextWaveMaxTimer;

            this.nextWaveMinTimer = InitialNextWaveMinTimer;
            this.nextWaveMaxTimer = InitialNextWaveMaxTimer;

            this.nextSingleEnemyTimer = InitialNextSingleEnemyMaxTimer;
            this.nextSingleEnemyMinTimer = InitialNextSingleEnemyMinTimer;
            this.nextSingleEnemyMaxTimer = InitialNextSingleEnemyMaxTimer;

            this.IsActive = true;
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            float tmpMin = (int)(InitialNextWaveMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f - 0.02 * (lvl - 1)); // 5,0 - WURZEL(A2-1) / 2 * 0,15 - 0,02 * (A2 - 1)
            this.nextWaveMinTimer = Math.Max(tmpMin, 2.5f);

            float tmpMax = (int)(InitialNextWaveMaxTimer - (float)Math.Sqrt(lvl - 1) * 0.075f - 0.01 * (lvl - 1)); // 7,0 - WURZEL(A2-1) / 2 * 0,15 - 0,01 * (A2 - 1)
            this.nextWaveMaxTimer = Math.Max(tmpMax, 4.0f);

            float tmpSingleMin = (int)(InitialNextSingleEnemyMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f - 0.0025 * (lvl - 1)); // 1,5 - WURZEL(A2-1) / 2 * 0,15 - 0,0025 * (A2 - 1)
            this.nextSingleEnemyMinTimer = Math.Max(tmpSingleMin, 1.0f);

            float tmpSingleMax = (int)(InitialNextSingleEnemyMaxTimer - (float)Math.Sqrt(lvl - 1) * 0.15f - 0.005 * (lvl - 1)); // 3 - WURZEL(A2-1) / 2 * 0,3 - 0,005 * (A2 - 1)
            this.nextSingleEnemyMaxTimer = Math.Max(tmpSingleMax, 1.5f);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemies
            int enemiesCount = Int32.Parse(reader.ReadLine());
            Enemies.Clear();

            for (int i = 0; i < enemiesCount; ++i)
            {
                EnemyType type = EnemyType.Easy;
                Enemy e;

                type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                switch (type)
                {
                    case EnemyType.Easy:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Medium:
                        e = Enemy.CreateMediumEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Hard:
                        e = Enemy.CreateHardEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Speeder:
                        e = Enemy.CreateSpeederEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Tank:
                        e = Enemy.CreateTankEnemy(texture, Vector2.Zero);
                        break;
                    default:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                }
                e.Activated(reader);

                Enemies.Add(e);
            }

            this.MinShipsPerWave = Int32.Parse(reader.ReadLine());
            this.MaxShipsPerWave = Int32.Parse(reader.ReadLine());

            this.nextWaveTimer = Single.Parse(reader.ReadLine());
            this.nextWaveMinTimer = Single.Parse(reader.ReadLine());
            this.nextWaveMaxTimer = Single.Parse(reader.ReadLine());

            this.nextSingleEnemyTimer = Single.Parse(reader.ReadLine());
            this.nextSingleEnemyMinTimer = Single.Parse(reader.ReadLine());
            this.nextSingleEnemyMaxTimer = Single.Parse(reader.ReadLine());

            this.IsActive = Boolean.Parse(reader.ReadLine());

            this.currentLevel = Int32.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            //Enemies
            writer.WriteLine(Enemies.Count);

            for (int i = 0; i < Enemies.Count; ++i)
            {
                writer.WriteLine(Enemies[i].Type);
                Enemies[i].Deactivated(writer);
            }

            writer.WriteLine(MinShipsPerWave);
            writer.WriteLine(MaxShipsPerWave);

            writer.WriteLine(nextWaveTimer);
            writer.WriteLine(nextWaveMinTimer);
            writer.WriteLine(nextWaveMaxTimer);

            writer.WriteLine(nextSingleEnemyTimer);
            writer.WriteLine(nextSingleEnemyMinTimer);
            writer.WriteLine(nextSingleEnemyMaxTimer);

            writer.WriteLine(IsActive);

            writer.WriteLine(currentLevel);
        }

        #endregion
    }
}