using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class Particle : Sprite
    {
        #region Members

        private Vector2 acceleration;
        private float maxSpeed;
        private float initialDuration;
        private float remainingDuration;
        private Color initialColor;
        private Color finalColor;
        private float rotationSpeed;

        #endregion

        #region Constructors

        public Particle(Vector2 location, Texture2D texture, Rectangle initialFrame,
                        Vector2 velocity, Vector2 acceleration, float maxSpeed,
                        float duration, Color initialColor, Color finalColor, float rotationSpeed)
            : base(location, texture, initialFrame, velocity)
        {
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;
            this.rotationSpeed = rotationSpeed;
        }

        #endregion

        #region Methods

        public void Reinitialize(Vector2 location, Texture2D texture,
                                 Vector2 velocity, Vector2 acceleration, float maxSpeed,
                                 float duration, Color initialColor, Color finalColor, float rotationSpeed)
        {
            this.Location = location;
            this.Velocity = velocity;
            this.Frame = 0;

            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;

            this.rotationSpeed = rotationSpeed;
        }

        public override void Update(float elapsed)
        {
            if (IsActive)
            {
                velocity += acceleration;

                if (velocity.Length() > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }

                if (remainingDuration > 0.25f)
                {
                    TintColor = Color.Lerp(initialColor,
                                           finalColor,
                                           this.DurationProgress);
                }
                else
                {
                    TintColor = Color.Lerp(initialColor,
                                           finalColor,
                                           this.DurationProgress) * (remainingDuration / 0.25f);
                }
                remainingDuration -= elapsed;

                base.Rotation += rotationSpeed;

                base.Update(elapsed);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
            }

        }

        #endregion

        #region Activate/Deactivate

        public new void Activated(StreamReader reader)
        {
            // Sprite
            base.Activated(reader);

            this.acceleration.X = Single.Parse(reader.ReadLine());
            this.acceleration.Y = Single.Parse(reader.ReadLine());

            this.maxSpeed = Single.Parse(reader.ReadLine());

            this.initialDuration = Single.Parse(reader.ReadLine());
            this.remainingDuration = Single.Parse(reader.ReadLine());

            this.initialColor = new Color(Int32.Parse(reader.ReadLine()),
                                          Int32.Parse(reader.ReadLine()),
                                          Int32.Parse(reader.ReadLine()),
                                          Int32.Parse(reader.ReadLine()));

            this.finalColor = new Color(Int32.Parse(reader.ReadLine()),
                                        Int32.Parse(reader.ReadLine()),
                                        Int32.Parse(reader.ReadLine()),
                                        Int32.Parse(reader.ReadLine()));

            this.rotationSpeed = Single.Parse(reader.ReadLine());
        }

        public new void Deactivated(StreamWriter writer)
        {
            // Sprite
            base.Deactivated(writer);

            writer.WriteLine(acceleration.X);
            writer.WriteLine(acceleration.Y);

            writer.WriteLine(maxSpeed);

            writer.WriteLine(initialDuration);
            writer.WriteLine(remainingDuration);

            writer.WriteLine((int)initialColor.R);
            writer.WriteLine((int)initialColor.G);
            writer.WriteLine((int)initialColor.B);
            writer.WriteLine((int)initialColor.A);

            writer.WriteLine((int)finalColor.R);
            writer.WriteLine((int)finalColor.G);
            writer.WriteLine((int)finalColor.B);
            writer.WriteLine((int)finalColor.A);

            writer.WriteLine(rotationSpeed);
        }

        #endregion

        #region Properties

        public float ElapsedDuration
        {
            get
            {
                return initialDuration - remainingDuration;
            }
        }

        public float DurationProgress
        {
            get
            {
                return (float)ElapsedDuration / (float)initialDuration;
            }
        }

        public bool IsActive
        {
            get
            {
                return remainingDuration > 0;
            }
        }

        #endregion
    }
}