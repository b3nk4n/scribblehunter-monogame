using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScribbleHunter
{
    class CollisionManager
    {
        #region Members

        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private BossManager bossManager;
        private PowerUpManager powerUpManager;
        private DamageExplosionManager damageExplosionManager;
        private Vector2 offScreen = new Vector2(-500, -500);

        private const string INFO_LASER = "Laser";
        private const string INFO_MINES = "Mines";
        private const string INFO_RELOAD = "Fast Reload";
        private const string INFO_NUKE = "Nuke";
        private const string INFO_LARGEEXPLOSION = "Explosive Power";
        private const string INFO_BOMB = "Bomb";
        private const string INFO_DIRECTRELOAD = "Direct Reload";
        private const string INFO_TIMEFREEZE = "Time Freeze";
        private const string INFO_STAR = "Inviolability";

        Random rand = new Random();

        private const float MINE_DAMAGE = 300.0f;

        #endregion

        #region Constructors

        public CollisionManager(PlayerManager playerManager,
                                EnemyManager enemyManager, BossManager bossManager,
                                PowerUpManager powerUpManager)
        {
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
            this.bossManager = bossManager;
            this.powerUpManager = powerUpManager;
            this.damageExplosionManager = DamageExplosionManager.Instance;
        }

        #endregion

        #region Methods

        private void checkShotToEnemyCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = enemies[i];

                Vector2 locationEnemy = Vector2.Zero;
                Vector2 locationShot = Vector2.Zero;
                Vector2 shotVelocity = Vector2.Zero;

                List<Sprite> shots = playerManager.PlayerShotManager.Shots;

                for (int j = 0; j < shots.Count; ++j)
                {
                    Sprite shot = shots[j];

                    if (enemy.IsHitable &&
                        shot.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.Kill();

                        locationShot = shot.Center;
                        locationEnemy = enemy.EnemySprite.Center;
                        shotVelocity = shot.Velocity;

                        shot.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }

                if (locationShot != Vector2.Zero)
                {
                    if (enemy.IsDestroyed)
                    {
                        playerManager.IncreaseScoreWithCombo(Enemy.KillScore);

                        damageExplosionManager.AddSmallExplosion(locationEnemy);
                        EffectManager.AddSparksEffect(locationShot, shotVelocity, shotVelocity / 4, Color.Black, true);
                    }
                }
            }
        }

        private void checkShotToBossCollisions()
        {
            List<Boss> bosses = bossManager.Bosses;

            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                List<Sprite> shots = playerManager.PlayerShotManager.Shots;

                for (int j = 0; j < shots.Count; ++j)
                {
                    Sprite shot = shots[j];

                    if (boss.IsHitable &&
                        shot.IsCircleColliding(boss.BossSprite.Center,
                                               boss.BossSprite.CollisionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= playerManager.ShotPower;

                        location = shot.Location;
                        velocity = shot.Velocity;

                        shot.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (boss.IsDestroyed)
                    {
                        playerManager.IncreaseScore(Boss.KillScore);

                        damageExplosionManager.AddSuperExplosion(location);

                        boss.Explode();
                    }
                    else
                    {
                        playerManager.IncreaseScore(Boss.HitScore);

                        EffectManager.AddLargeSparksEffect(location,
                                                           velocity,
                                                           -velocity,
                                                           Color.DarkGray);
                    }
                }
            }
        }

        private void checkMineToEnemyCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = enemies[i];

                Vector2 location = Vector2.Zero;

                List<Sprite> mines = playerManager.PlayerShotManager.Mines;

                for (int j = 0; j < mines.Count; ++j)
                {
                    Sprite mine = mines[j];

                    if (enemy.IsHitable &&
                        mine.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        location = mine.Center;

                        mine.Location = offScreen;

                        break; // Skip next comparisons
                    }
                }

                if (location != Vector2.Zero)
                {
                    damageExplosionManager.AddSmallExplosion(location);
                }
            }
        }

        private void checkMineToBossCollisions()
        {
            List<Boss> bosses = bossManager.Bosses;

            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                Vector2 location = Vector2.Zero;

                List<Sprite> mines = playerManager.PlayerShotManager.Mines;

                for (int j = 0; j < mines.Count; ++j)
                {
                    Sprite mine = mines[j];

                    if (boss.IsHitable &&
                        mine.IsCircleColliding(boss.BossSprite.Center,
                                               boss.BossSprite.CollisionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= MINE_DAMAGE;

                        location = mine.Center;

                        mine.Location = offScreen;

                        break; // Skip next comparisons
                    }
                }

                if (location != Vector2.Zero)
                {
                    damageExplosionManager.AddSmallExplosion(location);
                }
            }
        }

        private void checkDamageExplosionToEnemyCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = enemies[i];

                Vector2 location = Vector2.Zero;

                List<DamageExplosion> dExplosions = damageExplosionManager.DamageExplosions;

                for (int j = 0; j < dExplosions.Count; ++j)
                {
                    DamageExplosion de = dExplosions[j];

                    if (enemy.IsHitable &&
                        enemy.EnemySprite.IsCircleColliding(de.Location,
                                                            de.ExplosionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.Kill();

                        location = enemy.EnemySprite.Center;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (enemy.IsDestroyed)
                    {
                        playerManager.IncreaseScoreWithCombo(Enemy.KillScore);

                        damageExplosionManager.AddSmallExplosion(location);
                    }
                }
            }
        }

        private void checkDamageExplosionToBossCollisions()
        {
            List<Boss> bosses = bossManager.Bosses;

            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                Vector2 location = Vector2.Zero;

                List<DamageExplosion> dExplosions = damageExplosionManager.DamageExplosions;

                for (int j = 0; j < dExplosions.Count; ++j)
                {
                    DamageExplosion de = dExplosions[j];

                    if (boss.IsHitable &&
                        boss.BossSprite.IsCircleColliding(de.Location,
                                                            de.ExplosionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= 20; // decrease, because there are multiple hits

                        location = boss.BossSprite.Center;

                        break;
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (boss.IsDestroyed)
                    {
                        playerManager.IncreaseScore(Boss.KillScore);

                        damageExplosionManager.AddSuperExplosion(location);

                        boss.Explode();
                    }
                }
            }
        }

        private void checkBossShotToPlayerCollisions()
        {
            List<Sprite> shots = bossManager.BossShotManager.Shots;

            for (int i = 0; i < shots.Count; ++i)
            {
                Sprite shot = shots[i];

                if (playerManager.IsHitable &&
                    shot.IsCircleColliding(playerManager.playerSprite.Center,
                                           playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.Kill();

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddPlayerExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }

                    shot.Location = offScreen;
                }
            }
        }

        private void checkBossRocketToPlayerCollisions()
        {
            List<Sprite> rockets = bossManager.BossShotManager.Rockets;

            for (int i = 0; i < rockets.Count; ++i)
            {
                Sprite rocket = rockets[i];

                if (playerManager.IsHitable &&
                    rocket.IsCircleColliding(playerManager.playerSprite.Center,
                                             playerManager.playerSprite.CollisionRadius) &&
                    !playerManager.IsDestroyed)
                {
                    playerManager.Kill();

                    EffectManager.AddRocketExplosion(rocket.Center,
                                                     rocket.Velocity / 10);
                    EffectManager.AddPlayerExplosion(playerManager.playerSprite.Center,
                                               playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.3f);

                    rocket.Location = offScreen;
                }
            }
        }

        private void checkBossSonicToPlayerCollisions()
        {
            List<Sprite> sonics = bossManager.BossShotManager.Sonic;

            for (int i = 0; i < sonics.Count; ++i)
            {
                Sprite sonic = sonics[i];

                if (playerManager.IsHitable &&
                    sonic.IsCircleColliding(playerManager.playerSprite.Center,
                                           playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.Kill();

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddPlayerExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }
                }
            }
        }

        private void checkBossSonicToEnemiesCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                List<Sprite> sonics = bossManager.BossShotManager.Sonic;

                for (int j = 0; j < sonics.Count; ++j)
                {
                    Sprite sonic = sonics[j];

                    if (sonic.IsCircleColliding(enemies[i].EnemySprite.Center,
                                                enemies[i].EnemySprite.CollisionRadius))
                    {
                        enemies[i].Kill();

                        EffectManager.AddLargeExplosion(enemies[i].EnemySprite.Center,
                                                        enemies[i].EnemySprite.Velocity / 10,
                                                        true);
                    }
                }
            }
        }

        private void checkEnemyToPlayerCollisions()
        {
            List<Enemy> enemies = enemyManager.Enemies;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = enemies[i];

                if (playerManager.IsHitable &&
                    enemy.IsHitable &&
                    enemy.EnemySprite.IsCircleColliding(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.Kill();
                    EffectManager.AddPlayerExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.5f);
                }
            }
        }

        private void checkBossToPlayerCollisions()
        {
            List<Boss> bosses = bossManager.Bosses;

            for (int i = 0; i < bosses.Count; ++i)
            {
                Boss boss = bosses[i];

                if (playerManager.IsHitable &&
                    boss.IsHitable &&
                    boss.BossSprite.IsCircleColliding(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.Kill();
                    EffectManager.AddPlayerExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.5f);
                }
            }
        }

        private void checkPowerUpToPlayerCollision()
        {
            List<PowerUp> powerUps = powerUpManager.PowerUps;

            for (int i = 0; i < powerUps.Count; ++i)
            {
                PowerUp powerUp = powerUps[i];

                if (powerUp.isCircleColliding(playerManager.playerSprite.Center, playerManager.playerSprite.CollisionRadius))
                {
                    // Activate power-up
                    switch (powerUp.Type)
                    {
                        case PowerUp.PowerUpType.Laser:
                            playerManager.ActivateLaser();
                            SoundManager.PlayLaserSelectSound();
                            ZoomTextManager.ShowInfo(INFO_LASER);
                            break;

                        case PowerUp.PowerUpType.Mines:
                            playerManager.ActivateMines();
                            SoundManager.PlayMinesSelectSound();
                            ZoomTextManager.ShowInfo(INFO_MINES);
                            break;

                        case PowerUp.PowerUpType.Nuke:
                            playerManager.ReleaseNuke();
                            ZoomTextManager.ShowInfo(INFO_NUKE);
                            break;

                        case PowerUp.PowerUpType.LargeExplosion:
                            playerManager.ActivateLargeExplosion();
                            SoundManager.PlayLargeExplosionSelectSound();
                            ZoomTextManager.ShowInfo(INFO_LARGEEXPLOSION);
                            break;

                        case PowerUp.PowerUpType.FastReload:
                            playerManager.ActivateFastReload();
                            SoundManager.PlayFastReloadSelectSound();
                            ZoomTextManager.ShowInfo(INFO_RELOAD);
                            break;

                        case PowerUp.PowerUpType.Bomb:
                            playerManager.ReleaseBomb();
                            ZoomTextManager.ShowInfo(INFO_BOMB);
                            break;

                        case PowerUp.PowerUpType.DirectReload:
                            playerManager.DirectReload();
                            SoundManager.PlayDirectReloadSelectSound();
                            ZoomTextManager.ShowInfo(INFO_DIRECTRELOAD);
                            break;

                        case PowerUp.PowerUpType.TimeFreeze:
                            playerManager.ActivateTimeFreeze();
                            SoundManager.PlayTimeFreezeSelectSound();
                            ZoomTextManager.ShowInfo(INFO_TIMEFREEZE);
                            break;

                        case PowerUp.PowerUpType.Star:
                            playerManager.ActivateStar();
                            SoundManager.PlayStarSelectSound();
                            ZoomTextManager.ShowInfo(INFO_STAR);
                            break;
                    }

                    powerUp.IsActive = false;
                }
            }
        }

        public void Update()
        {
            checkShotToEnemyCollisions();
            checkShotToBossCollisions();
            checkDamageExplosionToEnemyCollisions();
            checkDamageExplosionToBossCollisions();
            checkMineToEnemyCollisions();
            checkMineToBossCollisions();
            checkBossSonicToEnemiesCollisions();

            if (!playerManager.IsDestroyed)
            {
                checkBossShotToPlayerCollisions();
                checkBossRocketToPlayerCollisions();
                checkBossSonicToPlayerCollisions();
                checkEnemyToPlayerCollisions();
                checkBossToPlayerCollisions();
                checkPowerUpToPlayerCollision();
            }
        }

        #endregion
    }
}