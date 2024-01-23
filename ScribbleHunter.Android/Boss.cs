using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class Boss : ILevel
    {
        #region Members

        private Sprite bossSprite;

        private float speed;

        private const int BossRadiusEasy = 35;
        private const int BossRadiusMedium = 35;
        private const int BossRadiusHard = 35;
        private const int BossRadiusSpeeder = 35;
        private const int BossRadiusTank = 35;

        private const float InitialHitPointsEasy = 2000.0f;
        private const float InitialHitPointsMedium = 2200.0f;
        private const float InitialHitPointsHard = 2400.0f;
        private const float InitialHitPointsSpeeder = 2200.0f;
        private const float InitialHitPointsTank = 2500.0f;

        private readonly Rectangle EasySource = new Rectangle(0, 450,
                                                              100, 100);
        private readonly Rectangle MediumSource = new Rectangle(0, 250,
                                                              100, 100);
        private readonly Rectangle HardSource = new Rectangle(600, 250,
                                                                100, 100);
        private readonly Rectangle SpeederSource = new Rectangle(600, 350,
                                                              100, 100);
        private readonly Rectangle TankSource = new Rectangle(0, 350,
                                                              100, 100);
        private const int EasyFrameCount = 6;
        private const int MediumFrameCount = 6;
        private const int HardFrameCount = 6;
        private const int SpeederFrameCount = 6;
        private const int TankFrameCount = 6;

        private Vector2 previousCenter = Vector2.Zero;

        private float hitPoints;
        public float MaxHitPoints;

        private EnemyType type;

        public const int HitScore = 100;
        public const int KillScore = 2500;

        private readonly float initialShotChance;
        private float shotChance;

        public enum BossState
        {
            Spawning, FollowingPlayer
        }

        private BossState bossState;
        private Vector2 currentTarget;

        private float spawnTimer;
        public const float SPAWN_TIME = 3.0f;

        private float accelerationFactor;

        private bool playerKilled;

        #endregion

        #region Constructors

        private Boss(Texture2D texture, Vector2 locationCenter,
                     float speed, float shotChance,
                     int collisionRadius, EnemyType type)
        {
            Rectangle initialFrame = new Rectangle();
            int frameCount = 0;

            switch (type)
            {
                case EnemyType.Easy:
                    initialFrame = EasySource;
                    frameCount = EasyFrameCount;
                    break;
                case EnemyType.Medium:
                    initialFrame = MediumSource;
                    frameCount = MediumFrameCount;
                    break;
                case EnemyType.Hard:
                    initialFrame = HardSource;
                    frameCount = HardFrameCount;
                    break;
                case EnemyType.Speeder:
                    initialFrame = SpeederSource;
                    frameCount = SpeederFrameCount;
                    break;
                case EnemyType.Tank:
                    initialFrame = TankSource;
                    frameCount = TankFrameCount;
                    break;
            }

            Vector2 topLeft = locationCenter - new Vector2(initialFrame.Width / 2, initialFrame.Height / 2);

            bossSprite = new Sprite(topLeft,
                                     texture,
                                     initialFrame,
                                     Vector2.Zero);

            for (int x = 1; x < frameCount; x++)
            {
                BossSprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
                previousCenter = locationCenter;
                BossSprite.CollisionRadius = collisionRadius;
            }

            this.speed = speed;

            this.initialShotChance = shotChance;
            this.shotChance = shotChance;

            this.type = type;

            this.bossState = BossState.Spawning;

            this.accelerationFactor = 0;
        }

        #endregion

        #region Methods

        public static Boss CreateEasyBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                 location,
                                 25.0f,
                                 0.1f,
                                 Boss.BossRadiusEasy,
                                 EnemyType.Easy);

            boss.hitPoints = InitialHitPointsEasy;
            boss.MaxHitPoints = InitialHitPointsEasy;

            return boss;
        }

        public static Boss CreateMediumBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                 location,
                                 20.0f,
                                 0.15f,
                                 Boss.BossRadiusMedium,
                                 EnemyType.Medium);

            boss.hitPoints = InitialHitPointsMedium;
            boss.MaxHitPoints = InitialHitPointsMedium;

            return boss;
        }

        public static Boss CreateHardBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                location,
                                15.0f,
                                0.175f,
                                Boss.BossRadiusHard,
                                EnemyType.Hard);

            boss.hitPoints = InitialHitPointsHard;
            boss.MaxHitPoints = InitialHitPointsHard;

            return boss;
        }

        public static Boss CreateSpeederBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                location,
                                25.0f,
                                1.25f,
                                Boss.BossRadiusSpeeder,
                                EnemyType.Speeder);

            boss.hitPoints = InitialHitPointsSpeeder;
            boss.MaxHitPoints = InitialHitPointsSpeeder;

            return boss;
        }

        public static Boss CreateTankBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                location,
                                10.0f,
                                0.2f,
                                Boss.BossRadiusTank,
                                EnemyType.Tank);

            boss.hitPoints = InitialHitPointsTank;
            boss.MaxHitPoints = InitialHitPointsTank;

            return boss;
        }

        public bool IsActive()
        {
            if (IsDestroyed)
            {
                return false;
            }

            return true;
        }

        public void Update(float elapsed)
        {
            if (IsActive())
            {
                Vector2 heading;

                switch (bossState)
                {
                    case BossState.Spawning:
                        spawnTimer += elapsed;
                        spawnTimer = Math.Min(SPAWN_TIME, spawnTimer);

                        if (spawnTimer >= SPAWN_TIME)
                        {
                            bossState = BossState.FollowingPlayer;
                        }

                        heading = currentTarget - bossSprite.Center;

                        if (heading != Vector2.Zero)
                        {
                            heading.Normalize();
                        }

                        bossSprite.Update(elapsed);
                        bossSprite.RotateTo(heading);
                        break;

                    case BossState.FollowingPlayer:
                        accelerationFactor += elapsed / 15;

                        if (accelerationFactor > 1)
                            accelerationFactor = 1;

                        // If there is no player then go straight forward
                        if (playerKilled)
                            heading = bossSprite.Velocity;
                        else
                            heading = currentTarget - bossSprite.Center;

                        if (heading != Vector2.Zero)
                        {
                            heading.Normalize();
                        }

                        heading *= (speed * accelerationFactor);
                        bossSprite.Velocity = heading;

                        if (!previousCenter.Equals(bossSprite.Center))
                            previousCenter = bossSprite.Center;

                        bossSprite.Update(elapsed);

                        if (!playerKilled)
                            bossSprite.RotateTo(currentTarget - bossSprite.Center);
                        else
                            bossSprite.RotateTo(bossSprite.Center - previousCenter);
                        break;
                    default:
                        // noting
                        break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
            {
                switch (bossState)
                {
                    case BossState.Spawning:
                        this.bossSprite.TintColor = Color.White * (float)Math.Pow(spawnTimer / SPAWN_TIME, 4);
                        break;

                    case BossState.FollowingPlayer:
                        this.bossSprite.TintColor = Color.White;
                        break;
                }

                BossSprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            this.MaxHitPoints += 25 * (lvl - 1);
            this.HitPoints += 25 * (lvl - 1);
        }

        public void Kill()
        {
            this.hitPoints = 0;
        }

        public void NotifyPlayerKilled()
        {
            this.playerKilled = true;
        }

        public void Explode()
        {
            EffectManager.AddLargeExplosion(bossSprite.Center + new Vector2(-20, 0), Vector2.Zero, true);
            EffectManager.AddLargeExplosion(bossSprite.Center + new Vector2(20, 0), Vector2.Zero, false);
            EffectManager.AddLargeExplosion(bossSprite.Center + new Vector2(-10, -15), Vector2.Zero, false);
            EffectManager.AddLargeExplosion(bossSprite.Center + new Vector2(10, -15), Vector2.Zero, false);
            EffectManager.AddLargeExplosion(bossSprite.Center + new Vector2(-10, 15), Vector2.Zero, false);
            EffectManager.AddLargeExplosion(bossSprite.Center + new Vector2(10, 15), Vector2.Zero, false);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Boss sprite
            bossSprite.Activated(reader);

            this.speed = Single.Parse(reader.ReadLine());

            this.previousCenter = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

            this.hitPoints = Single.Parse(reader.ReadLine());
            this.MaxHitPoints = Single.Parse(reader.ReadLine());

            this.type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.accelerationFactor = Single.Parse(reader.ReadLine());

            this.playerKilled = Boolean.Parse(reader.ReadLine());

            this.spawnTimer = Single.Parse(reader.ReadLine());

            this.bossState = (BossState)Enum.Parse(bossState.GetType(), reader.ReadLine(), false);
            this.currentTarget = new Vector2(Single.Parse(reader.ReadLine()),
                                             Single.Parse(reader.ReadLine()));
        }

        public void Deactivated(StreamWriter writer)
        {
            // Boss sprite
            bossSprite.Deactivated(writer);

            writer.WriteLine(speed);

            writer.WriteLine(previousCenter.X);
            writer.WriteLine(previousCenter.Y);

            writer.WriteLine(hitPoints);
            writer.WriteLine(MaxHitPoints);

            writer.WriteLine(type);

            writer.WriteLine(accelerationFactor);

            writer.WriteLine(playerKilled);

            writer.WriteLine(spawnTimer);

            writer.WriteLine(bossState);
            writer.WriteLine(currentTarget.X);
            writer.WriteLine(currentTarget.Y);
        }

        #endregion

        #region Properties

        public Sprite BossSprite
        {
            get
            {
                return this.bossSprite;
            }
        }

        public EnemyType Type
        {
            get
            {
                return this.type;
            }
        }

        public float ShotChance
        {
            get
            {
                return this.shotChance;
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
            set
            {
                this.hitPoints = MathHelper.Clamp(value, 0.0f, MaxHitPoints);
            }
        }

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        public Vector2 CurrentTarget
        {
            set
            {
                this.currentTarget = value;
            }
        }

        public bool IsSpawning
        {
            get { return bossState == BossState.Spawning; }
        }

        public bool IsHitable
        {
            get { return (spawnTimer / SPAWN_TIME) > 0.9f; }
        }

        #endregion
    }
}