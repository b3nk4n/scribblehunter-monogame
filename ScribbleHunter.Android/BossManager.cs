using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace ScribbleHunter
{
    /// <summary>
    /// Manages the bosses.
    /// </summary>
    class BossManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Boss> Bosses = new List<Boss>();

        public ShotManager BossShotManager;
        private PlayerManager playerManager;

        public const int SOFT_ROCKET_EXPLOSION_RADIUS = 100;
        public const float ROCKET_POWER_AT_CENTER = 200.0f;

        public const int DAMAGE_ROCKET_MIN = 30;
        public const int DAMAGE_ROCKET_MAX = 35;
        public const int DAMAGE_LASER_MIN = 10;
        public const int DAMAGE_LASER_MAX = 12;

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

        private readonly Rectangle screen = new Rectangle(0, 0, 480, 800);

        private float nextSingleBossTimer;
        public const float InitialNextSingleBossMinTimer = 20.0f;
        private float nextSingleBossMinTimer;
        public const float InitialNextSingleBossMaxTimer = 40.0f;
        private float nextSingleBossMaxTimer;

        public const int InitialShotChance = 2000;
        private int shotChance;
        private float minShotDelayTimer;
        private const float MinShotDelay = 3.0f;


        #endregion

        #region Constructors

        public BossManager(Texture2D texture, PlayerManager playerManager,
                           Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            BossShotManager = new ShotManager(texture,
                                               new Rectangle(650, 160, 20, 20),
                                               4,
                                               2,
                                               200.0f,
                                               new Rectangle(screenBounds.X - 50, screenBounds.Y - 50,
                                                             screenBounds.Width + 100, screenBounds.Height + 100));

            this.currentLevel = 1;
        }

        #endregion

        #region Methods

        public void SpawnSingleBoss()
        {
            Boss newBoss;
            int rnd = rand.Next(0, 5);

            Vector2 randomPosition = getRandomPosition();

            switch (rnd)
            {
                case 0:
                    newBoss = Boss.CreateMediumBoss(texture,
                                                       randomPosition);
                    break;

                case 1:
                    newBoss = Boss.CreateHardBoss(texture,
                                                     randomPosition);
                    break;

                case 2:
                    newBoss = Boss.CreateSpeederBoss(texture,
                                                        randomPosition);
                    break;

                case 3:
                    newBoss = Boss.CreateTankBoss(texture,
                                                     randomPosition);
                    break;

                default:
                    newBoss = Boss.CreateEasyBoss(texture,
                                                     randomPosition);
                    break;
            }

            newBoss.SetLevel(currentLevel);

            Bosses.Add(newBoss);

            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center);
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(-20, -10));
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(-20, 10));
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(20, -10));
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(20, 10));
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(-10, -20));
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(10, -20));
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(-10, 20));
            EffectManager.AddSpawnSmoke(newBoss.BossSprite.Center + new Vector2(10, 20));
        }

        private Vector2 getRandomPosition()
        {
            Vector2 pos;
            do
            {
                int x = rand.Next(50, 430);
                int y = rand.Next(50, 750);
                pos = new Vector2(x, y);
            } while ((playerManager.playerSprite.Center - pos).Length() < 50);

            return pos;
        }

        private void updateSingleBossSpawns(float elapsed)
        {
            nextSingleBossTimer -= elapsed;

            if (nextSingleBossTimer <= 0.0f)
            {
                SpawnSingleBoss();

                nextSingleBossTimer = nextSingleBossMinTimer + (float)rand.NextDouble() * (nextSingleBossMaxTimer - nextSingleBossMinTimer);
            }
        }

        public void Update(float elapsed)
        {
            BossShotManager.Update(elapsed);

            minShotDelayTimer += elapsed;

            for (int x = Bosses.Count - 1; x >= 0; --x)
            {
                Boss boss = Bosses[x];

                if (!playerManager.IsDestroyed)
                    boss.CurrentTarget = playerManager.playerSprite.Center;
                else
                    boss.NotifyPlayerKilled();

                boss.Update(elapsed);

                if (!boss.IsActive())
                {
                    Bosses.RemoveAt(x);
                }
                else
                {
                    probablyfireShot(boss);
                }
            }

            if (this.IsActive)
            {
                updateSingleBossSpawns(elapsed);
            }
        }

        private void probablyfireShot(Boss boss)
        {
            float rndShot;
            if ((boss.BossSprite.Center - playerManager.playerSprite.Center).Length() > 200)
                rndShot = (float)rand.Next(0, ((int)(shotChance))) / 10;
            else
                rndShot = (float)rand.Next(0, ((int)(shotChance * 5))) / 10;

            if (!boss.IsSpawning &&
                minShotDelayTimer > MinShotDelay &&
                rndShot <= boss.ShotChance &&
                !playerManager.IsDestroyed &&
                 screen.Contains((int)boss.BossSprite.Center.X,
                                 (int)boss.BossSprite.Center.Y))
            {
                Vector2 fireLocation = boss.BossSprite.Center;

                Vector2 shotDirection = ((playerManager.playerSprite.Center + playerManager.playerSprite.Velocity / (1.75f + (float)rand.NextDouble() * 3.25f)) - fireLocation);

                shotDirection.Normalize();

                Matrix m = new Matrix();
                m.M11 = (float)Math.Cos(Math.PI / 2);
                m.M12 = (float)-Math.Sin(Math.PI / 2);
                m.M21 = (float)Math.Sin(Math.PI / 2);
                m.M22 = (float)Math.Cos(Math.PI / 2);

                Vector2 fireLocationOffset15 = Vector2.Transform(shotDirection, m) * 15;
                Vector2 fireLocationOffset8 = Vector2.Transform(shotDirection, m) * 8;

                if (boss.Type == EnemyType.Easy)
                {
                    BossShotManager.FireShot(fireLocation,
                                            shotDirection,
                                            false,
                                            Color.DarkRed,
                                            true);
                }
                else if (boss.Type == EnemyType.Medium)
                {
                    BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                shotDirection,
                                                false,
                                                Color.DarkRed,
                                                true);
                    BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                shotDirection,
                                                false,
                                                Color.DarkRed,
                                                false);
                }
                else if (boss.Type == EnemyType.Hard)
                {
                    Color c = new Color(0.8f, 0.5f, 1.0f);

                    BossShotManager.FireRocket(fireLocation,
                                                    shotDirection,
                                                    false,
                                                    Color.DarkRed,
                                                    true);
                }
                else if (boss.Type == EnemyType.Speeder)
                {
                    BossShotManager.FireSonic(fireLocation,
                                              shotDirection,
                                              Color.DarkRed,
                                              true);
                }
                else if (boss.Type == EnemyType.Tank)
                {
                    BossShotManager.FireRocket(fireLocation,
                                                shotDirection,
                                                false,
                                                Color.DarkRed,
                                                true);
                    BossShotManager.FireShot(fireLocation - fireLocationOffset15,
                                                Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(10f))),
                                                false,
                                                Color.DarkRed,
                                                true);
                    BossShotManager.FireShot(fireLocation + fireLocationOffset15,
                                                Vector2.Transform(shotDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(-10f))),
                                                false,
                                                Color.DarkRed,
                                                false);
                }

                minShotDelayTimer = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            BossShotManager.Draw(spriteBatch);

            foreach (var boss in Bosses)
            {
                boss.Draw(spriteBatch);
            }
        }

        public void Reset()
        {
            this.Bosses.Clear();
            this.BossShotManager.Reset();

            this.nextSingleBossTimer = (InitialNextSingleBossMaxTimer + InitialNextSingleBossMinTimer) / 2.0f;
            this.nextSingleBossMinTimer = InitialNextSingleBossMinTimer;
            this.nextSingleBossMaxTimer = InitialNextSingleBossMaxTimer;

            this.IsActive = true;

            this.minShotDelayTimer = 0.0f;
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            this.shotChance = InitialShotChance - (lvl - 1) * 10;

            float tmpSingleMin = (int)(InitialNextSingleBossMinTimer - (float)Math.Sqrt(lvl - 1) * 0.25f - 0.2 * (lvl - 1)); // 20 - WURZEL(A2-1) / 4 - 0,2 * (A2 - 1)
            this.nextSingleBossMinTimer = Math.Max(tmpSingleMin, 10.0f);

            float tmpSingleMax = (int)(InitialNextSingleBossMaxTimer - (float)Math.Sqrt(lvl - 1) * 0.5f - 0.2 * (lvl - 1)); // 40 - WURZEL(A2-1) / 2 - 0,2 * (A2 - 1)
            this.nextSingleBossMaxTimer = Math.Max(tmpSingleMax, 20.0f);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Bosses
            int bossesCount = Int32.Parse(reader.ReadLine());

            Bosses.Clear();

            for (int i = 0; i < bossesCount; ++i)
            {
                EnemyType type = EnemyType.Easy;
                Boss b;

                type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                switch (type)
                {
                    case EnemyType.Easy:
                        b = Boss.CreateEasyBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Medium:
                        b = Boss.CreateMediumBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Hard:
                        b = Boss.CreateHardBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Speeder:
                        b = Boss.CreateSpeederBoss(texture, Vector2.Zero);
                        break;
                    case EnemyType.Tank:
                        b = Boss.CreateTankBoss(texture, Vector2.Zero);
                        break;
                    default:
                        b = Boss.CreateEasyBoss(texture, Vector2.Zero);
                        break;
                }
                b.Activated(reader);

                Bosses.Add(b);
            }

            BossShotManager.Activated(reader);

            this.nextSingleBossTimer = Single.Parse(reader.ReadLine());
            this.nextSingleBossMinTimer = Single.Parse(reader.ReadLine());
            this.nextSingleBossMaxTimer = Single.Parse(reader.ReadLine());

            this.IsActive = Boolean.Parse(reader.ReadLine());

            this.currentLevel = Int32.Parse(reader.ReadLine());

            this.minShotDelayTimer = Single.Parse(reader.ReadLine());

            this.shotChance = Int32.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            //Bosses
            writer.WriteLine(Bosses.Count);

            for (int i = 0; i < Bosses.Count; ++i)
            {
                writer.WriteLine(Bosses[i].Type);
                Bosses[i].Deactivated(writer);
            }

            BossShotManager.Deactivated(writer);


            writer.WriteLine(nextSingleBossTimer);
            writer.WriteLine(nextSingleBossMinTimer);
            writer.WriteLine(nextSingleBossMaxTimer);

            writer.WriteLine(IsActive);

            writer.WriteLine(currentLevel);

            writer.WriteLine(minShotDelayTimer);

            writer.WriteLine(shotChance);
        }

        #endregion
    }
}