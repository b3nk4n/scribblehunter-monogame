using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace ScribbleHunter
{
    public class DamageExplosion
    {
        /// <summary>
        /// The location center of the explosion.
        /// </summary>
        private Vector2 location;

        /// <summary>
        /// The starting radius.
        /// </summary>
        private float radiusStart;

        /// <summary>
        /// The final radius.
        /// </summary>
        private float radiusEnd;

        /// <summary>
        /// The current explosion radius.
        /// </summary>
        private float currentRadius;

        /// <summary>
        /// The duration of the explosion in seconds.
        /// </summary>
        private float duration;

        /// <summary>
        /// The current duration.
        /// </summary>
        private float currentDuration;

        /// <summary>
        /// The rotation.
        /// </summary>
        private float rotation;

        /// <summary>
        /// The color.
        /// </summary>
        private Color tintColor;

        /// <summary>
        /// The rotation per frame.
        /// </summary>
        private const float ROTATION_VALUE = 1.00f;

        private readonly Rectangle ExplosionSource = new Rectangle(1200, 0, 400, 400);

        /// <summary>
        /// Constructor just for activation/deactivation.
        /// </summary>
        public DamageExplosion()
        {

        }

        /// <summary>
        /// Creates a new damage explosion.
        /// </summary>
        /// <param name="location">The location center</param>
        /// <param name="radiusStart">The starting radius</param>
        /// <param name="radiusEnd">The final radius</param>
        /// <param name="duration">The duration of the explosion</param>
        public DamageExplosion(Vector2 location, float radiusStart, float radiusEnd, float duration, Color tint)
        {
            this.location = location;
            this.radiusStart = radiusStart;
            this.radiusEnd = radiusEnd;
            this.duration = duration;
            this.currentDuration = 0.0f;
            this.tintColor = tint;
        }

        /// <summary>
        /// Updates the damage explosion.
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentDuration += elapsed;
            rotation += ROTATION_VALUE;

            this.currentRadius = radiusStart + ((radiusEnd - radiusStart) * currentDuration / duration);
        }

        /// <summary>
        /// Draws the explosion border for debugging
        /// </summary>
        /// <param name="batch">The sprite batch</param>
        public void Draw(SpriteBatch batch)
        {
            Rectangle destination = new Rectangle((int)(location.X),
                                                  (int)(location.Y),
                                                  (int)(currentRadius * 2),
                                                  (int)(currentRadius * 2));

            Color c;

            if (duration - currentRadius > 0.33f)
                c = tintColor;
            else
                c = tintColor * ((duration - currentDuration) / 0.33f);

            batch.Draw(
                DamageExplosionManager.Texture,
                destination,
                ExplosionSource,
                c,
                rotation,
                new Vector2(200, 200),
                SpriteEffects.None,
                0.0f);
        }

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.location.X = Single.Parse(reader.ReadLine());
            this.location.Y = Single.Parse(reader.ReadLine());

            this.radiusStart = Single.Parse(reader.ReadLine());
            this.radiusEnd = Single.Parse(reader.ReadLine());
            this.currentRadius = Single.Parse(reader.ReadLine());

            this.duration = Single.Parse(reader.ReadLine());
            this.currentDuration = Single.Parse(reader.ReadLine());

            this.rotation = Single.Parse(reader.ReadLine());

            this.tintColor = new Color(Int32.Parse(reader.ReadLine()),
                                       Int32.Parse(reader.ReadLine()),
                                       Int32.Parse(reader.ReadLine()),
                                       Int32.Parse(reader.ReadLine()));
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(this.location.X);
            writer.WriteLine(this.location.Y);

            writer.WriteLine(this.radiusStart);
            writer.WriteLine(this.radiusEnd);
            writer.WriteLine(this.currentRadius);

            writer.WriteLine(this.duration);
            writer.WriteLine(this.currentDuration);

            writer.WriteLine(this.rotation);

            writer.WriteLine((int)tintColor.R);
            writer.WriteLine((int)tintColor.G);
            writer.WriteLine((int)tintColor.B);
            writer.WriteLine((int)tintColor.A);
        }

        #endregion

        /// <summary>
        /// Gets the center location of the explosion.
        /// </summary>
        public Vector2 Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Gets the current explosion radius of the explosion.
        /// </summary>
        public float ExplosionRadius
        {
            get
            {
                return this.currentRadius;
            }
        }

        /// <summary>
        /// Gets the value indication whether the exposion is elapsed or not.
        /// </summary>
        public bool IsElapsed
        {
            get
            {
                return currentDuration >= duration;
            }
        }
    }
}