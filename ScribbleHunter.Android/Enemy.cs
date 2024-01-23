using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class Enemy : ILevel
    {
        #region Members

        private Sprite enemySprite;
        private Queue<Vector2> wayPoints = new Queue<Vector2>(32);
        private Vector2 currentWayPoint = Vector2.Zero;

        private float speed;
        private float initialSpeed;
        private const float SPEED_UP_FACTOR = 0.1f;

        private const int EnemyRadiusEasy = 13;
        private const int EnemyRadiusMedium = 13;
        private const int EnemyRadiusHard = 13;
        private const int EnemyRadiusSpeeder = 13;
        private const int EnemyRadiusTank = 13;

        private const float InitialEnemySpeedEasy = 40.0f;
        private const float InitialEnemySpeedMedium = 30.0f;
        private const float InitialEnemySpeedHard = 25.0f;
        private const float InitialEnemySpeedSpeeder = 40.0f;
        private const float InitialEnemySpeedTank = 20.0f;

        private const float InitialHitPoints = 1.0f;

        private readonly Rectangle EasySource = new Rectangle(0, 150,
                                                              40, 40);
        private readonly Rectangle MediumSource = new Rectangle(0, 190,
                                                                40, 40);
        private readonly Rectangle HardSource = new Rectangle(350, 140,
                                                              40, 40);
        private readonly Rectangle SpeederSource = new Rectangle(350, 180,
                                                                 40, 40);
        private readonly Rectangle TankSource = new Rectangle(350, 100,
                                                              40, 40);

        private const int EasyFrameCount = 6;
        private const int MediumFrameCount = 6;
        private const int HardFrameCount = 6;
        private const int SpeederFrameCount = 6;
        private const int TankFrameCount = 6;

        private Vector2 previousCenter = Vector2.Zero;

        private float hitPoints = 1.0f;

        private EnemyType type;

        public const int KillScore = 10;

        public enum EnemyState
        {
            Spawning, Route, FollowingPlayer
        }

        private EnemyState enemyState;
        private Vector2 currentTarget;

        private float spawnTimer;
        public const float SPAWN_TIME = 3.0f;

        private readonly Vector2 WAYPOINT_PLACEHOLDER = new Vector2(-999, -999);

        private float accelerationFactor;

        private bool playerKilled;

        #endregion

        #region Constructors

        private Enemy(Texture2D texture, Vector2 locationCenter,
                     float speed, int collisionRadius, EnemyType type)
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

            this.hitPoints = InitialHitPoints;

            Vector2 topLeft = locationCenter - new Vector2(initialFrame.Width / 2, initialFrame.Height / 2);

            enemySprite = new Sprite(topLeft,
                                     texture,
                                     initialFrame,
                                     Vector2.Zero);

            for (int x = 1; x < frameCount; x++)
            {
                EnemySprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
                previousCenter = locationCenter;
                currentWayPoint = WAYPOINT_PLACEHOLDER;
                EnemySprite.CollisionRadius = collisionRadius;
            }

            this.speed = speed;
            this.initialSpeed = speed;

            this.type = type;

            this.enemyState = EnemyState.Spawning;

            this.accelerationFactor = 0.0f;
        }

        #endregion

        #region Methods

        public static Enemy CreateEasyEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                    location,
                                    InitialEnemySpeedEasy,
            Enemy.EnemyRadiusEasy,
                                    EnemyType.Easy);
            return enemy;
        }

        public static Enemy CreateMediumEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                InitialEnemySpeedMedium,
                                Enemy.EnemyRadiusMedium,
                                EnemyType.Medium);
            return enemy;
        }

        public static Enemy CreateHardEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                InitialEnemySpeedHard,
                                Enemy.EnemyRadiusHard,
                                EnemyType.Hard);
            return enemy;
        }

        public static Enemy CreateSpeederEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                InitialEnemySpeedSpeeder,
                                Enemy.EnemyRadiusSpeeder,
                                EnemyType.Speeder);
            return enemy;
        }

        public static Enemy CreateTankEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                InitialEnemySpeedTank,
                                Enemy.EnemyRadiusTank,
                                EnemyType.Tank);
            return enemy;
        }

        public void AddWayPoint(Vector2 wayPoint)
        {
            wayPoints.Enqueue(wayPoint);
        }

        public bool WayPointReached()
        {
            if (currentWayPoint == WAYPOINT_PLACEHOLDER)
            {
                return true;
            }

            if (Vector2.Distance(enemySprite.Center, currentWayPoint) <
                (float)enemySprite.Source.Width / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsActive()
        {
            if (IsDestroyed)
            {
                return false;
            }

            if (wayPoints.Count > 0)
            {
                return true;
            }

            return true;
        }

        public void Update(float elapsed)
        {
            if (IsActive())
            {
                Vector2 heading;

                switch (enemyState)
                {
                    case EnemyState.Spawning:
                        spawnTimer += elapsed;
                        spawnTimer = Math.Min(SPAWN_TIME, spawnTimer);

                        if (spawnTimer >= SPAWN_TIME)
                        {
                            if (wayPoints.Count > 0)
                                enemyState = EnemyState.Route;
                            else
                                enemyState = EnemyState.FollowingPlayer;
                        }

                        if (currentWayPoint == WAYPOINT_PLACEHOLDER)
                        {
                            if (wayPoints.Count > 0)
                            {
                                currentWayPoint = wayPoints.Peek();
                                heading = currentWayPoint - enemySprite.Center;
                            }
                            else
                            {
                                heading = currentTarget - enemySprite.Center;
                            }
                        }
                        else
                        {
                            heading = currentWayPoint - enemySprite.Center;
                        }


                        if (heading != Vector2.Zero)
                        {
                            heading.Normalize();
                        }

                        enemySprite.Update(elapsed);
                        enemySprite.RotateTo(heading);
                        break;

                    case EnemyState.Route:
                        accelerationFactor += elapsed / 10;

                        if (accelerationFactor > 1)
                            accelerationFactor = 1;

                        heading = currentWayPoint - enemySprite.Center;

                        if (heading != Vector2.Zero)
                        {
                            heading.Normalize();
                        }

                        heading *= (speed * accelerationFactor);
                        enemySprite.Velocity = heading;
                        previousCenter = enemySprite.Center;
                        enemySprite.Update(elapsed);
                        enemySprite.RotateTo(heading);

                        if (WayPointReached())
                        {
                            if (wayPoints.Count > 0)
                            {
                                // next waypoint
                                currentWayPoint = wayPoints.Dequeue();
                            }
                            else
                            {
                                // follow the player
                                enemyState = EnemyState.FollowingPlayer;
                            }
                        }
                        break;
                    case EnemyState.FollowingPlayer:
                        accelerationFactor += elapsed / 10;

                        if (accelerationFactor > 1)
                            accelerationFactor = 1;

                        // If there is no player then go straight forward
                        if (playerKilled && (enemySprite.Center - currentTarget).Length() < 50)
                            heading = enemySprite.Velocity;
                        else
                            heading = currentTarget - enemySprite.Center;

                        if (heading != Vector2.Zero)
                        {
                            heading.Normalize();
                        }

                        this.speed += elapsed * SPEED_UP_FACTOR;

                        heading *= (speed * accelerationFactor);
                        enemySprite.Velocity = heading;

                        if (!previousCenter.Equals(enemySprite.Center))
                            previousCenter = enemySprite.Center;

                        enemySprite.Update(elapsed);

                        if (!playerKilled)
                            enemySprite.RotateTo(currentTarget - enemySprite.Center);
                        else
                            enemySprite.RotateTo(enemySprite.Center - previousCenter);
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
                switch (enemyState)
                {
                    case EnemyState.Spawning:
                        this.enemySprite.TintColor = Color.White * (float)Math.Pow(spawnTimer / SPAWN_TIME, 4);
                        break;

                    case EnemyState.Route:
                        this.enemySprite.TintColor = Color.White;
                        break;
                    case EnemyState.FollowingPlayer:
                        this.enemySprite.TintColor = Color.White;
                        break;
                }

                enemySprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            this.HitPoints += (lvl - 1) * 10;

            if (initialSpeed + (lvl - 1) * 0.25f > this.speed)
                this.speed = initialSpeed + (lvl - 1) * 0.25f;
        }

        public void Kill()
        {
            this.hitPoints = 0.0f;
        }

        public void NotifyPlayerKilled()
        {
            this.playerKilled = true;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemy sprite
            enemySprite.Activated(reader);

            //Waypoints
            int waypointsCount = Int32.Parse(reader.ReadLine());
            wayPoints.Clear();

            for (int i = 0; i < waypointsCount; ++i)
            {
                Vector2 v = new Vector2(Single.Parse(reader.ReadLine()),
                                        Single.Parse(reader.ReadLine()));
                wayPoints.Enqueue(v);
            }

            this.currentWayPoint = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            this.speed = Single.Parse(reader.ReadLine());
            this.initialSpeed = Single.Parse(reader.ReadLine());

            this.previousCenter = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

            this.hitPoints = Single.Parse(reader.ReadLine());

            this.type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.accelerationFactor = Single.Parse(reader.ReadLine());

            this.playerKilled = Boolean.Parse(reader.ReadLine());

            this.spawnTimer = Single.Parse(reader.ReadLine());

            this.enemyState = (EnemyState)Enum.Parse(enemyState.GetType(), reader.ReadLine(), false);
            this.currentTarget = new Vector2(Single.Parse(reader.ReadLine()),
                                             Single.Parse(reader.ReadLine()));
        }

        public void Deactivated(StreamWriter writer)
        {
            // Enemy sprite
            enemySprite.Deactivated(writer);

            // Waypoints
            int wayPointsCount = wayPoints.Count;
            writer.WriteLine(wayPointsCount);

            for (int i = 0; i < wayPointsCount; ++i)
            {
                Vector2 wayPoint = wayPoints.Dequeue();
                writer.WriteLine(wayPoint.X);
                writer.WriteLine(wayPoint.Y);
            }

            writer.WriteLine(currentWayPoint.X);
            writer.WriteLine(currentWayPoint.Y);

            writer.WriteLine(speed);
            writer.WriteLine(initialSpeed);

            writer.WriteLine(previousCenter.X);
            writer.WriteLine(previousCenter.Y);

            writer.WriteLine(hitPoints);

            writer.WriteLine(type);

            writer.WriteLine(accelerationFactor);

            writer.WriteLine(playerKilled);

            writer.WriteLine(spawnTimer);

            writer.WriteLine(enemyState);
            writer.WriteLine(currentTarget.X);
            writer.WriteLine(currentTarget.Y);
        }

        #endregion

        #region Properties

        public Sprite EnemySprite
        {
            get
            {
                return this.enemySprite;
            }
        }

        public EnemyType Type
        {
            get
            {
                return this.type;
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
            private set
            {
                this.hitPoints = value;
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

        public bool IsHitable
        {
            get { return (spawnTimer / SPAWN_TIME) > 0.9f; }
        }

        #endregion
    }
}