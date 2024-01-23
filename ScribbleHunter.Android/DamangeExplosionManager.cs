using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScribbleHunter
{
    public class DamageExplosionManager
    {
        private static DamageExplosionManager manager;

        private List<DamageExplosion> damageExplosions = new List<DamageExplosion>(128);

        public static Texture2D Texture;

        private Random rand = new Random();

        private DamageExplosionManager()
        {

        }

        public static void Initialize(Texture2D tex)
        {
            Texture = tex;
        }

        public void Reset()
        {
            this.damageExplosions.Clear();
        }

        public void AddSmallExplosion(Vector2 location)
        {
            this.damageExplosions.Add(new DamageExplosion(location, 10.0f, rand.Next(25, 40), 1.0f, Color.Black));
            EffectManager.AddExplosion(location, Vector2.Zero);
            EffectManager.AddSmallSplatterExplosionEffect(location,
                                                     Vector2.Zero,
                                                     Vector2.Zero);
        }

        public void AddPlayerExplosion(Vector2 location)
        {
            this.damageExplosions.Add(new DamageExplosion(location, 25.0f, 100.0f, 1.11f, Color.Purple));
            EffectManager.AddPlayerExplosion(location, Vector2.Zero);
            EffectManager.AddMediumSplatterExplosionEffect(location,
                                                     Vector2.Zero,
                                                     Vector2.Zero);
        }

        public void AddLargePlayerExplosion(Vector2 location)
        {
            this.damageExplosions.Add(new DamageExplosion(location, 25.0f, 175.0f, 1.25f, Color.Purple));
            EffectManager.AddLargeExplosion(location, Vector2.Zero, true);
            EffectManager.AddLargeSplatterExplosionEffect(location,
                                                     Vector2.Zero,
                                                     Vector2.Zero);
        }

        public void AddBombExplosion(Vector2 location)
        {
            this.damageExplosions.Add(new DamageExplosion(location, 25.0f, 125.0f, 1.11f, Color.Black));
            EffectManager.AddLargeExplosion(location, Vector2.Zero, true);
            EffectManager.AddLargeSplatterExplosionEffect(location,
                                                     Vector2.Zero,
                                                     Vector2.Zero);
        }

        public void AddSuperExplosion(Vector2 location)
        {
            this.damageExplosions.Add(new DamageExplosion(location, 30.0f, 215.0f, 1.25f, Color.Black));
            EffectManager.AddLargeExplosion(location, Vector2.Zero, true);
            EffectManager.AddLargeSplatterExplosionEffect(location,
                                                     Vector2.Zero,
                                                     Vector2.Zero);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < damageExplosions.Count; i++)
            {
                damageExplosions[i].Update(gameTime);

                if (damageExplosions[i].IsElapsed)
                {
                    damageExplosions.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (var de in damageExplosions)
            {
                de.Draw(batch);
            }
        }

        #region Activated/Deactivated

        public void Activated(StreamReader reader)
        {
            int count = Int32.Parse(reader.ReadLine());

            damageExplosions.Clear();

            for (int i = 0; i < count; i++)
            {
                DamageExplosion de = new DamageExplosion();
                de.Activated(reader);

                damageExplosions.Add(de);
            }
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(this.damageExplosions.Count);

            for (int i = 0; i < this.damageExplosions.Count; i++)
            {
                this.damageExplosions[i].Deactivated(writer);
            }
        }

        #endregion

        /// <summary>
        /// Gets a singleton instance of the DamageExplosionManager.
        /// </summary>
        public static DamageExplosionManager Instance
        {
            get
            {
                if (manager == null)
                    manager = new DamageExplosionManager();
                return manager;
            }
        }

        /// <summary>
        /// Gets all damage explosions.
        /// </summary>
        public List<DamageExplosion> DamageExplosions
        {
            get
            {
                return this.damageExplosions;
            }
        }
    }
}