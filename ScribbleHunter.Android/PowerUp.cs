using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class PowerUp
    {
        #region Members

        private Sprite powerUpSprite;

        private const float SPEED = 0.0f;
        private const int RADIUS = 13;

        public enum PowerUpType { Mines, Laser, Nuke, FastReload, LargeExplosion, Bomb, DirectReload, TimeFreeze, Star };

        private PowerUpType type;

        private bool isActive = true;

        private const int ScreenHeight = 800;
        private const int ScreenWidth = 480;

        private const int FramesCount = 2;

        private float opacity;

        private readonly Rectangle RotationUpgradeFrame = new Rectangle(0, 0, 50, 50);
        private const float UpgradeFrameRotationSpeed = 0.05f;
        private float upgradeRotation;
        private readonly Sprite rotationSprite;

        #endregion

        #region Constructor

        public PowerUp(Texture2D texture, Vector2 location, Rectangle initialFrame,
                       PowerUpType type)
        {
            powerUpSprite = new Sprite(location, texture, initialFrame, new Vector2(0, 1) * SPEED);

            for (int x = 1; x < FramesCount; x++)
            {
                this.powerUpSprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                         initialFrame.Y,
                                                         initialFrame.Width,
                                                         initialFrame.Height));
            }

            powerUpSprite.CollisionRadius = RADIUS;

            this.type = type;

            this.rotationSprite = new Sprite(
                location,
                texture,
                RotationUpgradeFrame,
                Vector2.Zero);
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                opacity += elapsed;

                if (opacity > 1)
                {
                    opacity = 1;
                }

                powerUpSprite.Update(elapsed);

                if (!IsInScreen)
                {
                    IsActive = false;
                }

                checkBoundsX();

                rotationSprite.Update(elapsed);
                rotationSprite.Rotation = upgradeRotation;
                upgradeRotation += UpgradeFrameRotationSpeed;
                rotationSprite.Location = powerUpSprite.Location;
            }
        }

        private void checkBoundsX()
        {
            if (powerUpSprite.Location.X < -powerUpSprite.Source.Width / 2)
            {
                powerUpSprite.Location = new Vector2(-powerUpSprite.Source.Width / 2, powerUpSprite.Location.Y);
            }
            else if (powerUpSprite.Location.X > ScreenWidth - powerUpSprite.Source.Width / 2)
            {
                powerUpSprite.Location = new Vector2(ScreenWidth - powerUpSprite.Source.Width / 2, powerUpSprite.Location.Y);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                Color c = powerUpSprite.TintColor;
                Color newColor = new Color((int)c.R, (int)c.G, (int)c.B, (int)(c.A * opacity));

                powerUpSprite.TintColor = newColor;
                powerUpSprite.Draw(spriteBatch);
                powerUpSprite.TintColor = c;

                rotationSprite.TintColor = newColor * 0.5f;
                rotationSprite.Draw(spriteBatch);
                rotationSprite.TintColor = c;
            }
        }

        #region Methods

        public bool isCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            return this.powerUpSprite.IsCircleColliding(otherCenter, otherRadius) && opacity > 0.33f;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            powerUpSprite.Activated(reader);

            this.type = (PowerUpType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.isActive = Boolean.Parse(reader.ReadLine());

            this.opacity = Single.Parse(reader.ReadLine());

            rotationSprite.Activated(reader);
            this.upgradeRotation = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Powerup sprite
            powerUpSprite.Deactivated(writer);

            writer.WriteLine(type);

            writer.WriteLine(isActive);

            writer.WriteLine(opacity);

            rotationSprite.Deactivated(writer);
            writer.WriteLine(upgradeRotation);
        }

        #endregion

        #region Properties

        public PowerUpType Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                this.isActive = value;
            }
        }

        public bool IsInScreen
        {
            get
            {
                return powerUpSprite.Location.Y < ScreenHeight;
            }
        }

        public Vector2 Center
        {
            get
            {
                return powerUpSprite.Center;
            }
        }

        #endregion
    }
}

