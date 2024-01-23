using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    static class EffectManager
    {
        #region Members

        private const int PRELOADED_EXPLOSION_EFFECTS = 256;
        private const int PRELOADED_POINT_EFFECTS = 2048;

        public static List<Particle> ExplosionEffects = new List<Particle>(PRELOADED_EXPLOSION_EFFECTS);
        public static List<Particle> PointEffects = new List<Particle>(PRELOADED_POINT_EFFECTS);

        private static Random rand = new Random();
        public static Texture2D Texture;
        public static Rectangle ParticleFrame;
        public static List<Rectangle> ExplosionFrames = new List<Rectangle>(8);

        private static Queue<Particle> freeExplosionParticles = new Queue<Particle>(PRELOADED_EXPLOSION_EFFECTS);
        private static Queue<Particle> freePointParticles = new Queue<Particle>(PRELOADED_POINT_EFFECTS);

        #endregion

        #region Methods

        public static void Initialize(Texture2D texture, Rectangle particleFrame,
                                      Rectangle initialExplosionFrame, int explosionFameCount)
        {
            Texture = texture;
            ParticleFrame = particleFrame;
            ExplosionFrames.Clear();

            for (int x = 1; x < explosionFameCount; x++)
            {
                initialExplosionFrame.Offset(initialExplosionFrame.Width, 0);
                ExplosionFrames.Add(initialExplosionFrame);
            }

            // Generate free explosion particles:
            for (int i = 0; i < PRELOADED_EXPLOSION_EFFECTS; i++)
            {
                freeExplosionParticles.Enqueue(new Particle(Vector2.Zero,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.Transparent,
                                         Color.Transparent,
                                         0.0f));
            }

            // Generate free explosion particles:
            for (int i = 0; i < PRELOADED_POINT_EFFECTS; i++)
            {
                freePointParticles.Enqueue(new Particle(Vector2.Zero,
                                         Texture,
                                         ParticleFrame,
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.Transparent,
                                         Color.Transparent,
                                         0.0f));
            }
        }

        public static Vector2 RandomDirection(float scale)
        {
            Vector2 direction;

            do
            {
                direction = new Vector2(rand.Next(0, 101) - 50,
                                        rand.Next(0, 101) - 50);
            } while (direction.Length() == 0);

            direction.Normalize();
            direction *= scale;

            return direction;
        }

        public static void Update(float elapsed)
        {
            // Explosion effects
            for (int x = ExplosionEffects.Count - 1; x >= 0; --x)
            {
                if (ExplosionEffects[x].IsActive)
                {
                    ExplosionEffects[x].Update(elapsed);
                }
                else
                {
                    freeExplosionParticles.Enqueue(ExplosionEffects[x]);
                    ExplosionEffects.RemoveAt(x);
                }
            }

            // Point effects
            for (int x = PointEffects.Count - 1; x >= 0; --x)
            {
                if (PointEffects[x].IsActive)
                {
                    PointEffects[x].Update(elapsed);
                }
                else
                {
                    freePointParticles.Enqueue(PointEffects[x]);
                    PointEffects.RemoveAt(x);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in ExplosionEffects)
            {
                particle.Draw(spriteBatch);
            }

            foreach (var particle in PointEffects)
            {
                particle.Draw(spriteBatch);
            }
        }

        public static void AddCustomExplosion(Vector2 location, Vector2 momentum, int minPointCount,
                                              int maxPointCount, int minPieceCount, int maxPieceCount,
                                              float pieceSpeedScale, float duration, Color initialColor,
                                              Color finalColor, float rotationSpeed, bool sound)
        {
            float explosionMaxSpeed = pieceSpeedScale;

            int pointSpeedMin = (int)pieceSpeedScale * 2;
            int pointSpeedMax = (int)pieceSpeedScale * 3;

            Vector2 pieceLocation = location - new Vector2(ExplosionFrames[0].Width / 2,
                                                           ExplosionFrames[0].Height / 2);

            int pieceCount = rand.Next(minPieceCount, maxPieceCount + 1);

            for (int x = 0; x < pieceCount; x++)
            {
                if (freeExplosionParticles.Count == 0)
                {
                    ExplosionEffects.Add(new Particle(pieceLocation,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         RandomDirection(pieceSpeedScale) + momentum,
                                         Vector2.Zero,
                                         ((float)rand.NextDouble()) * explosionMaxSpeed,
                                         duration,
                                         initialColor,
                                         finalColor,
                                         rotationSpeed));
                }
                else
                {
                    Particle p = freeExplosionParticles.Dequeue();
                    p.Reinitialize(pieceLocation,
                                   Texture,
                                   RandomDirection(pieceSpeedScale) + momentum,
                                   Vector2.Zero,
                                   ((float)rand.NextDouble()) * explosionMaxSpeed,
                                   duration,
                                   initialColor,
                                   finalColor,
                                   rotationSpeed);
                    ExplosionEffects.Add(p);
                }
            }

            int pointsCount = rand.Next(minPointCount, maxPointCount + 1);

            for (int x = 0; x < maxPointCount; x++)
            {
                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location,
                                             Texture,
                                             ParticleFrame,
                                             RandomDirection((float)rand.Next(pointSpeedMin, pointSpeedMax + 1)) + momentum,
                                             Vector2.Zero,
                                             ((float)rand.NextDouble()) * explosionMaxSpeed,
                                             duration,
                                             initialColor,
                                             finalColor,
                                             0.0f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location,
                                   Texture,
                                   RandomDirection((float)rand.Next(pointSpeedMin, pointSpeedMax + 1)) + momentum,
                                   Vector2.Zero,
                                   ((float)rand.NextDouble()) * explosionMaxSpeed,
                                   duration,
                                   initialColor,
                                   finalColor,
                                   0.0f);
                    PointEffects.Add(p);
                }
            }

            if (sound)
                SoundManager.PlayExplosion();
        }

        public static void AddExplosion(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               5,
                               8,
                               3,
                               5,
                               20.0f,
                               0.833f,
                               Color.Black * (0.4f + (float)rand.NextDouble() * 0.4f),
                               Color.Black * 0.0f,
                               0.025f,
                               true);
        }

        public static void AddLargeExplosion(Vector2 location, Vector2 momentum, bool sound)
        {
            AddCustomExplosion(location,
                               momentum,
                               7,
                               10,
                               5,
                               8,
                               30.0f,
                               1.66f,
                               Color.Black * (0.4f + (float)rand.NextDouble() * 0.4f),
                               Color.Black * 0.0f,
                               0.025f,
                               false);
            if (sound)
                SoundManager.PlayLargeExplosion();
        }

        public static void AddPlayerExplosion(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               7,
                               10,
                               5,
                               8,
                               30.0f,
                               1.66f,
                               Color.Purple * (0.4f + (float)rand.NextDouble() * 0.4f),
                               Color.Purple * 0.0f,
                               0.025f,
                               false);

            SoundManager.PlayLargeExplosion();
        }

        public static void AddRocketExplosion(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               8,
                               12,
                               12,
                               15,
                               175.0f,
                               0.6f,
                               Color.Black * (0.4f + (float)rand.NextDouble() * 0.4f),
                               Color.Black * 0.0f,
                               0.025f,
                               true);
        }

        public static void AddSparksEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum, Color tintColor, bool sound)
        {
            int particleCount = rand.Next(3, 5);

            for (int x = 0; x < particleCount; x++)
            {
                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location - (impectVelocity / 60),
                                                  Texture,
                                                  ParticleFrame,
                                                  RandomDirection((float)rand.Next(10, 20)) + momentum * 0.75f,
                                                  Vector2.Zero,
                                                  40.0f,
                                                  0.66f,
                                                  tintColor,
                                                  tintColor * 0.0f,
                                                  0.01f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location - (impectVelocity / 60),
                                   Texture,
                                   RandomDirection((float)rand.Next(10, 20)) + momentum * 0.75f,
                                   Vector2.Zero,
                                   40.0f,
                                   0.66f,
                                   tintColor,
                                   tintColor * 0.0f,
                                   0.01f);
                    PointEffects.Add(p);
                }
            }

            if (sound)
                SoundManager.PlayHitSound();
        }

        public static void AddLargeSparksEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum, Color tintColor)
        {
            int particleCount = rand.Next(7, 10);

            for (int x = 0; x < particleCount; x++)
            {
                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location - (impectVelocity / 60),
                                                  Texture,
                                                  ParticleFrame,
                                                  RandomDirection((float)rand.Next(10, 20)) * 3 + momentum * 0.25f,
                                                  Vector2.Zero,
                                                  60.0f,
                                                  0.5f,
                                                  tintColor * 0.75f,
                                                  tintColor * 0.0f,
                                                  0.0f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location - (impectVelocity / 60),
                                   Texture,
                                   RandomDirection((float)rand.Next(10, 20)) * 3 + momentum * 0.25f,
                                   Vector2.Zero,
                                   60.0f,
                                   0.5f,
                                   tintColor * 0.75f,
                                   tintColor * 0.0f,
                                   0.0f);
                    PointEffects.Add(p);
                }
            }

            SoundManager.PlayHitSound();
        }

        public static void AddSmallSplatterExplosionEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum)
        {
            int particleCount = rand.Next(15, 20);

            for (int x = 0; x < particleCount; x++)
            {
                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location - (impectVelocity / 60),
                                                  Texture,
                                                  ParticleFrame,
                                                  RandomDirection((float)rand.Next(5, 20)) * 3 + momentum * 0.25f,
                                                  Vector2.Zero,
                                                  80,
                                                  1.166f,
                                                  Color.Black * 0.75f,
                                                  Color.Black * 0.0f,
                                                  0.0f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location - (impectVelocity / 60),
                                   Texture,
                                   RandomDirection((float)rand.Next(5, 20)) * 3 + momentum * 0.25f,
                                   Vector2.Zero,
                                   80,
                                   1.166f,
                                   Color.Black * 0.75f,
                                   Color.Black * 0.0f,
                                   0.0f);
                    PointEffects.Add(p);
                }
            }
        }

        public static void AddMediumSplatterExplosionEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum)
        {
            int particleCount = rand.Next(20, 25);

            for (int x = 0; x < particleCount; x++)
            {
                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location - (impectVelocity / 60),
                                                  Texture,
                                                  ParticleFrame,
                                                  RandomDirection((float)rand.Next(5, 20)) * 6 + momentum * 0.25f,
                                                  Vector2.Zero,
                                                  250,
                                                  1.5f,
                                                  Color.Black * 0.75f,
                                                  Color.Black * 0.0f,
                                                  0.0f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location - (impectVelocity / 60),
                                   Texture,
                                   RandomDirection((float)rand.Next(5, 20)) * 6 + momentum * 0.25f,
                                   Vector2.Zero,
                                   250,
                                   1.5f,
                                   Color.Black * 0.75f,
                                   Color.Black * 0.0f,
                                   0.0f);
                    PointEffects.Add(p);
                }
            }
        }

        public static void AddLargeSplatterExplosionEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum)
        {
            int particleCount = rand.Next(25, 30);

            for (int x = 0; x < particleCount; x++)
            {
                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location - (impectVelocity / 60),
                                                  Texture,
                                                  ParticleFrame,
                                                  RandomDirection((float)rand.Next(5, 20)) * 8 + momentum * 0.25f,
                                                  Vector2.Zero,
                                                  400,
                                                  1.66f,
                                                  Color.Black * 0.75f,
                                                  Color.Black * 0.0f,
                                                  0.0f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location - (impectVelocity / 60),
                                   Texture,
                                   RandomDirection((float)rand.Next(5, 20)) * 8 + momentum * 0.25f,
                                   Vector2.Zero,
                                   400,
                                   1.66f,
                                   Color.Black * 0.75f,
                                   Color.Black * 0.0f,
                                   0.0f);
                    PointEffects.Add(p);
                }
            }
        }

        public static void AddRocketSmoke(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               0,
                               0,
                               1,
                               1,
                               20.0f,
                               0.5f,
                               Color.Black * 0.25f,
                               Color.Black * 0.0f,
                               0.025f,
                               false);
        }

        public static void AddPlayerSmoke(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               0,
                               0,
                               1,
                               1,
                               10.0f,
                               0.66f,
                               Color.Purple * 0.3f,
                               Color.Purple * 0.0f,
                               0.025f,
                               false);
        }

        public static void AddSpawnSmoke(Vector2 location)
        {
            AddCustomExplosion(location,
                               Vector2.Zero,
                               0,
                               0,
                               1,
                               1,
                               0.01f,
                               3.0f,
                               Color.Black * 0.1f,
                               Color.Black * 0.75f,
                               0.025f,
                               false);
        }

        public static void Reset()
        {
            ExplosionEffects.Clear();
            PointEffects.Clear();
        }

        #endregion

        #region Activated / Deactivated

        public static void Activated(StreamReader reader)
        {
            // Explosion effects
            int expCount = Int32.Parse(reader.ReadLine());
            ExplosionEffects.Clear();

            for (int i = 0; i < expCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.Black,
                                         Color.Black,
                                         0.0f);
                p.Activated(reader);
                ExplosionEffects.Add(p);
            }

            // Point effects
            int pointsCount = Int32.Parse(reader.ReadLine());
            PointEffects.Clear();

            for (int i = 0; i < pointsCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ParticleFrame,
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.Black,
                                         Color.Black,
                                         0.0f);
                p.Activated(reader);
                PointEffects.Add(p);
            }

            // Free explosion effects
            int freeExpCount = Int32.Parse(reader.ReadLine());
            freeExplosionParticles.Clear();

            for (int i = 0; i < freeExpCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.Black,
                                         Color.Black,
                                         0.0f);
                p.Activated(reader);
                freeExplosionParticles.Enqueue(p);
            }

            // Free point effects
            int freePointsCount = Int32.Parse(reader.ReadLine());
            freePointParticles.Clear();

            for (int i = 0; i < freePointsCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ParticleFrame,
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.Black,
                                         Color.Black,
                                         0.0f);
                p.Activated(reader);
                freePointParticles.Enqueue(p);
            }
        }

        public static void Deactivated(StreamWriter writer)
        {
            // Explosion effects
            writer.WriteLine(ExplosionEffects.Count);

            for (int i = 0; i < ExplosionEffects.Count; ++i)
            {
                ExplosionEffects[i].Deactivated(writer);
            }

            // Point effects
            writer.WriteLine(PointEffects.Count);

            for (int i = 0; i < PointEffects.Count; ++i)
            {
                PointEffects[i].Deactivated(writer);
            }

            // Free explosion effects
            writer.WriteLine(freeExplosionParticles.Count);

            foreach (var freeExplosion in freeExplosionParticles)
            {
                freeExplosion.Deactivated(writer);
            }

            // Free point effects
            writer.WriteLine(freePointParticles.Count);

            foreach (var freePoints in freePointParticles)
            {
                freePoints.Deactivated(writer);
            }
        }

        #endregion
    }
}