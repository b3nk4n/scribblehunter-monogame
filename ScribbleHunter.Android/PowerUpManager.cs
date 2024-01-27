using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class PowerUpManager : ILevel
    {
        #region Members

        private List<PowerUp> powerUps = new List<PowerUp>(8);
        private Texture2D texture;

        private Random rand = new Random();

        private float spawnTimer;
        private const float InitialSpawnTimerMin = 15.0f;
        private const float InitialSpawnTimerMax = 25.0f;
        private float spawnTimerMin;
        private float spawnTimerMax;

        private int currentLevel;

        private PlayerManager playerManager;

        #endregion

        #region Constructors

        public PowerUpManager(Texture2D texture, PlayerManager player)
        {
            this.texture = texture;
            this.currentLevel = 1;
            this.playerManager = player;
            this.Reset();
        }

        #endregion

        #region Methods

        public void SpawnPowerUp()
        {
            Vector2 location = getRandomPosition();
            SpawnPowerUp(location);
        }

        public void SpawnPowerUp(Vector2 location)
        {
            int rnd = rand.Next(23);

            PowerUp.PowerUpType type = PowerUp.PowerUpType.Mines;
            Rectangle initialFrame = getInitialFrameByType(type);

            switch (rnd)
            {
                case 0:
                case 1:
                    type = PowerUp.PowerUpType.Mines;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 2:
                case 3:
                    type = PowerUp.PowerUpType.Laser;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 4:
                case 5:
                case 6:
                    type = PowerUp.PowerUpType.Nuke;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 7:
                case 8:
                case 9:
                    type = PowerUp.PowerUpType.FastReload;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 10:
                case 11:
                case 12:
                    type = PowerUp.PowerUpType.LargeExplosion;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 13:
                case 14:
                case 15:
                    type = PowerUp.PowerUpType.Bomb;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 16:
                case 17:
                    type = PowerUp.PowerUpType.DirectReload;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 18:
                case 19:
                case 20:
                    type = PowerUp.PowerUpType.TimeFreeze;
                    initialFrame = getInitialFrameByType(type);
                    break;

                case 21:
                case 22:
                    type = PowerUp.PowerUpType.Star;
                    initialFrame = getInitialFrameByType(type);
                    break;
            }

            PowerUp p = new PowerUp(texture,
                                    location - new Vector2(initialFrame.Width / 2, initialFrame.Height / 2),
                                    initialFrame,
                                    type);

            powerUps.Add(p);
        }

        private Vector2 getRandomPosition()
        {
            Vector2 pos;
            do
            {
                int x = rand.Next(30, 450);
                int y = rand.Next(75, 770);
                pos = new Vector2(x, y);
            } while ((playerManager.playerSprite.Center - pos).Length() < 50);

            return pos;
        }

        public void Reset()
        {
            this.PowerUps.Clear();
            this.spawnTimer = spawnTimerMax;
            this.currentLevel = 1;

            this.spawnTimerMax = InitialSpawnTimerMax;
            this.spawnTimerMin = InitialSpawnTimerMin;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            spawnTimer -= elapsed;

            if (spawnTimer <= 0)
            {
                SpawnPowerUp();
                spawnTimer = spawnTimerMin + (float)rand.NextDouble() * (spawnTimerMax - spawnTimerMin);
            }

            for (int x = powerUps.Count - 1; x >= 0; --x)
            {
                powerUps[x].Update(gameTime);

                if (!powerUps[x].IsActive)
                {
                    powerUps.RemoveAt(x);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var powerUp in powerUps)
            {
                powerUp.Draw(spriteBatch);
            }
        }

        private Rectangle getInitialFrameByType(PowerUp.PowerUpType type)
        {
            switch (type)
            {
                case PowerUp.PowerUpType.Mines:
                    return new Rectangle(0, 200, 50, 50);
                case PowerUp.PowerUpType.Laser:
                    return new Rectangle(0, 150, 50, 50);
                case PowerUp.PowerUpType.Nuke:
                    return new Rectangle(0, 250, 50, 50);
                case PowerUp.PowerUpType.LargeExplosion:
                    return new Rectangle(0, 100, 50, 50);
                case PowerUp.PowerUpType.FastReload:
                    return new Rectangle(0, 50, 50, 50);
                case PowerUp.PowerUpType.Bomb:
                    return new Rectangle(0, 300, 50, 50);
                case PowerUp.PowerUpType.DirectReload:
                    return new Rectangle(0, 350, 50, 50);
                case PowerUp.PowerUpType.TimeFreeze:
                    return new Rectangle(0, 400, 50, 50);
                case PowerUp.PowerUpType.Star:
                    return new Rectangle(0, 450, 50, 50);
                default:
                    return new Rectangle(0, 50, 50, 50);
            }
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            spawnTimerMin = Math.Max(InitialSpawnTimerMin / 2.0f, InitialSpawnTimerMin - ((lvl - 1) * 0.25f));
            spawnTimerMin = Math.Max(InitialSpawnTimerMax * 2.0f / 3.0f, InitialSpawnTimerMin - ((lvl - 1) * 0.2f));
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Powerups
            int powerUpsCount = Int32.Parse(reader.ReadLine());

            powerUps.Clear();

            for (int i = 0; i < powerUpsCount; ++i)
            {
                PowerUp.PowerUpType type = PowerUp.PowerUpType.FastReload;
                type = (PowerUp.PowerUpType)Enum.Parse(type.GetType(), reader.ReadLine(), false);
                PowerUp p = new PowerUp(texture,
                                    Vector2.Zero,
                                    getInitialFrameByType(type),
                                    type);
                p.Activated(reader);
                powerUps.Add(p);
            }

            this.spawnTimer = Single.Parse(reader.ReadLine());
            this.spawnTimerMin = Single.Parse(reader.ReadLine());
            this.spawnTimerMax = Single.Parse(reader.ReadLine());

            this.currentLevel = Int32.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Powerups
            writer.WriteLine(powerUps.Count);

            for (int i = 0; i < powerUps.Count; ++i)
            {
                writer.WriteLine(powerUps[i].Type);
                powerUps[i].Deactivated(writer);
            }

            writer.WriteLine(spawnTimer);
            writer.WriteLine(spawnTimerMin);
            writer.WriteLine(spawnTimerMax);

            writer.WriteLine(currentLevel);
        }

        #endregion

        #region Properties

        public List<PowerUp> PowerUps
        {
            get
            {
                return powerUps;
            }
        }

        #endregion
    }
}