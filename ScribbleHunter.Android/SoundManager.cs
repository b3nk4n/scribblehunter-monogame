using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace ScribbleHunter
{
    public static class SoundManager
    {
        #region Members

        private static SettingsManager settings;

        private static List<SoundEffect> explosions = new List<SoundEffect>();
        private static int explosionsCount = 4;
        private static SoundEffect largeExplosion;

        private static SoundEffect playerShot;
        private static SoundEffect enemyShot;

        private static SoundEffect fastReloadSound;
        private static SoundEffect minesSound;
        private static SoundEffect largeSound;
        private static SoundEffect extraSonicSound;
        private static SoundEffect rocketSound;
        private static SoundEffect laserSound;
        private static SoundEffect enemyRocketSound;

        private static SoundEffect directReloadSound;

        private static List<SoundEffect> hitSounds = new List<SoundEffect>();
        private static int hitSoundsCount = 6;

        private static Random rand = new Random();

        private static Song backgroundSound;
        //private static SoundEffectInstance backgroundSound;

        private static List<SoundEffect> paperSounds = new List<SoundEffect>();
        private static int papersCount = 3;

        // performance improvements
        public const float MinTimeBetweenHitSound = 0.2f;
        private static float hitTimer = 0.0f;

        public const float MinTimeBetweenExplosionSound = 0.1f;
        private static float explosionTimer = 0.0f;

        private static SoundEffect writingSound;
        private static SoundEffect timeFreezeSound;
        private static SoundEffect starSound;

        private static SoundEffect piepSound;
        private static SoundEffect mineSound;

        #endregion

        #region Methods

        public static void Initialize(ContentManager content)
        {
            try
            {
                settings = SettingsManager.GetInstance();

                playerShot = content.Load<SoundEffect>(@"Sounds\Shot2");
                enemyShot = content.Load<SoundEffect>(@"Sounds\Shot1");

                fastReloadSound = content.Load<SoundEffect>(@"Sounds\FastReload");
                minesSound = content.Load<SoundEffect>(@"Sounds\Mines");
                largeSound = content.Load<SoundEffect>(@"Sounds\Large");
                extraSonicSound = content.Load<SoundEffect>(@"Sounds\Sonic");
                rocketSound = content.Load<SoundEffect>(@"Sounds\Rocket");
                laserSound = content.Load<SoundEffect>(@"Sounds\Laser");
                enemyRocketSound = content.Load<SoundEffect>(@"Sounds\EnemyRocket");

                directReloadSound = content.Load<SoundEffect>(@"Sounds\DirectReload");

                for (int x = 1; x <= explosionsCount; x++)
                {
                    explosions.Add(content.Load<SoundEffect>(@"Sounds\Explosion"
                                                             + x.ToString()));
                }

                largeExplosion = content.Load<SoundEffect>(@"Sounds\LargeExplosion");

                for (int x = 1; x <= hitSoundsCount; x++)
                {
                    hitSounds.Add(content.Load<SoundEffect>(@"Sounds\Hit"
                                                             + x.ToString()));
                }

                backgroundSound = content.Load<Song>(@"Sounds\GameSound");

                for (int x = 1; x <= papersCount; x++)
                {
                    paperSounds.Add(content.Load<SoundEffect>(@"Sounds\Paper"
                                                             + x.ToString()));
                }

                writingSound = content.Load<SoundEffect>(@"Sounds\Writing");

                timeFreezeSound = content.Load<SoundEffect>(@"Sounds\TimeFreeze");
                starSound = content.Load<SoundEffect>(@"Sounds\Star");

                piepSound = content.Load<SoundEffect>(@"Sounds\Piep");
                mineSound = content.Load<SoundEffect>(@"Sounds\Mine");
            }
            catch
            {
                Debug.WriteLine("SoundManager: Content not found.");
            }
        }

        public static void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            SoundManager.hitTimer += elapsed;
            SoundManager.explosionTimer += elapsed;
        }

        public static void PlayExplosion()
        {
            if (SoundManager.explosionTimer > SoundManager.MinTimeBetweenExplosionSound)
            {
                try
                {
                    SoundEffectInstance s = explosions[rand.Next(0, explosionsCount)].CreateInstance();
                    s.Volume = settings.GetSfxValue();
                    s.Play();
                }
                catch
                {
                    Debug.WriteLine("SoundManager: Play explosion failed.");
                }

                SoundManager.explosionTimer = 0.0f;
            }
        }

        public static void PlayLargeExplosion()
        {
            try
            {
                SoundEffectInstance s = largeExplosion.CreateInstance();
                s.Volume = 0.75f * settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play large explosion failed.");
            }
        }

        public static void PlayPlayerShot()
        {
            try
            {
                SoundEffectInstance s = playerShot.CreateInstance();
                s.Volume = 0.75f * settings.GetSfxValue() * 0.6f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play player shot failed.");
            }
        }

        public static void PlayEnemyShot()
        {
            try
            {
                SoundEffectInstance s = enemyShot.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.8f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayFastReloadSelectSound()
        {
            try
            {
                SoundEffectInstance s = fastReloadSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play fast reload sound failed.");
            }
        }

        public static void PlayLargeExplosionSelectSound()
        {
            try
            {
                SoundEffectInstance s = largeSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play large sound failed.");
            }
        }

        public static void PlayUpgradeSound()
        {
            try
            {
                SoundEffectInstance s = minesSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.7f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlaySonicSound()
        {
            try
            {
                SoundEffectInstance s = extraSonicSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayHitSound()
        {
            if (SoundManager.hitTimer > SoundManager.MinTimeBetweenHitSound)
            {
                try
                {
                    SoundEffectInstance s = hitSounds[rand.Next(0, hitSoundsCount)].CreateInstance();
                    s.Volume = settings.GetSfxValue();
                    s.Play();
                }
                catch
                {
                    Debug.WriteLine("SoundManager: Play explosion failed.");
                }

                SoundManager.hitTimer = 0.0f;
            }
        }

        public static void PlayRocketSound()
        {
            try
            {
                SoundEffectInstance s = rocketSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayLaserSelectSound()
        {
            try
            {
                SoundEffectInstance s = laserSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play laser select failed.");
            }
        }

        public static void PlayMinesSelectSound()
        {
            try
            {
                SoundEffectInstance s = minesSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play mines select failed.");
            }
        }

        public static void PlayEnemyRocketSound()
        {
            try
            {
                SoundEffectInstance s = enemyRocketSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayPaperSound()
        {
            try
            {
                SoundEffectInstance s = paperSounds[rand.Next(0, papersCount)].CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.5f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play paper sound failed.");
            }
        }

        public static void PlayWritingSound()
        {
            try
            {
                SoundEffectInstance s = writingSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play writing sound failed.");
            }
        }

        public static void PlayTimeFreezeSelectSound()
        {
            try
            {
                SoundEffectInstance s = timeFreezeSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play time freeze sound failed.");
            }
        }

        public static void PlayStarSelectSound()
        {
            try
            {
                SoundEffectInstance s = starSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.9f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play star sound failed.");
            }
        }

        public static void PlayDirectReloadSelectSound()
        {
            try
            {
                SoundEffectInstance s = directReloadSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.8f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play direct reload sound failed.");
            }
        }

        public static void PlayPiepSound()
        {
            try
            {
                SoundEffectInstance s = piepSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.5f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play piep sound failed.");
            }
        }

        public static void PlayMineSound()
        {
            try
            {
                SoundEffectInstance s = mineSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.9f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play mine sound failed.");
            }
        }

        public static void PlayBackgroundSound()
        {
            try
            {
                if (MediaPlayer.GameHasControl)
                {
                    MediaPlayer.Play(backgroundSound);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = settings.GetMusicValue();
                }
            }
            catch (UnauthorizedAccessException)
            {
                // play no music...
            }
            catch (InvalidOperationException)
            {
                // play no music (because of Zune on PC)
            }
        }

        public static void RefreshMusicVolume()
        {
            float val = settings.GetMusicValue();
            MediaPlayer.Volume = val;
        }

        #endregion
    }
}