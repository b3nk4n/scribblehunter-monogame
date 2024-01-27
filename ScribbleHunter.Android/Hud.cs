using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using ScribbleHunter.Extensions;
using System;

namespace ScribbleHunter
{
    class Hud
    {
        #region Members

        private static Hud hud;

        private long score;
        private int level;

        private PlayerManager playerManager;

        private Rectangle screenBounds;
        private Texture2D texture;
        private SpriteFont font;

        private readonly Vector2 ScoreLocation = new Vector2(5, 3);
        private readonly Vector2 ComboLocation = new Vector2(5, 25);
        private readonly Vector2 UpgradesLocation = new Vector2(325, 10);

        private readonly Rectangle UpgradeSymbolDestination = new Rectangle(298, 4, 24, 24);

        private readonly Rectangle UpgradeLargeExplosionSource = new Rectangle(800, 50, 24, 24);
        private readonly Rectangle UpgradeFastReloadSource = new Rectangle(824, 50, 24, 24);
        private readonly Rectangle UpgradeLaserSource = new Rectangle(848, 50, 24, 24);
        private readonly Rectangle UpgradeMinesSource = new Rectangle(872, 50, 24, 24);
        private readonly Rectangle UpgradeTimeFreezeSource = new Rectangle(896, 50, 24, 24);
        private readonly Rectangle UpgradeStarSource = new Rectangle(920, 50, 24, 24);

        private const int PROGRESSBAR_OFFSET = 24;

        private Vector2 playerReloadLocation = new Vector2(0, 780);

        private readonly Vector2 SmallBarOverlayStart = new Vector2(600, 950);
        private readonly Vector2 BigBarOverlayStart = new Vector2(600, 930);
        private const int BarSourceWidth = 150;
        private const int BarSourceHeight = 10;
        private const int BarWidth = 150;
        private const int BarHeight = 12;
        private const int ReloadBarWidth = 480;
        private const int ReloadBarHeight = 20;

        private int lastLevel = -1;
        private StringBuilder currentLevelText = new StringBuilder(16);
        private const string LEVEL_PRE_TEXT = "Level: ";

        private const int OFFSET_Y = 0;
        private readonly Vector2 OFFSET_VEC_Y = new Vector2(0, 0);

        private bool canFireDisplayed = false;
        private const float DisplayCanFireTimerLimit = 0.5f;
        private float displayCanFireTimer = 0.0f;

        #endregion

        #region Constructors

        private Hud(Rectangle screen, Texture2D texture, SpriteFont font,
                    int level, PlayerManager player)
        {
            this.screenBounds = screen;
            this.texture = texture;
            this.font = font;
            this.score = player.PlayerScore;
            this.level = level;
            this.playerManager = player;
        }

        #endregion

        #region Methods

        public static Hud GetInstance(Rectangle screen, Texture2D texture,
                                      SpriteFont font, int level,
                                      PlayerManager player)
        {
            if (hud == null)
            {
                hud = new Hud(screen,
                              texture,
                              font,
                              level,
                              player);
            }

            return hud;
        }

        public void Update(float elapsed, int level)
        {
            this.score = playerManager.PlayerScore;

            this.level = level;

            if (playerManager.CanFire)
            {
                if (canFireDisplayed)
                {
                    displayCanFireTimer -= elapsed;
                }
                else
                {
                    displayCanFireTimer = DisplayCanFireTimerLimit;
                    canFireDisplayed = true;
                }
            }
            else
            {
                canFireDisplayed = false;
                displayCanFireTimer = 0.0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawScore(spriteBatch);
            drawCombo(spriteBatch);

            drawLevel(spriteBatch);
            drawUpgrades(spriteBatch);

            drawPlayerReload(spriteBatch, playerManager.ShotTimer, PlayerManager.ShotTimerMin);

            drawCanFire(spriteBatch);
        }

        private void drawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawInt64WithZerosBordered(
                               font,
                               score,
                               ScoreLocation + OFFSET_VEC_Y,
                               Color.Purple * 0.8f,
                               Color.White * 0.8f,
                               11);
        }

        private void drawCombo(SpriteBatch spriteBatch)
        {
            if (playerManager.HasCombo)
            {
                float factor = 0.8f * (0.1f + 0.9f * (1.0f - (float)Math.Pow(1.0f - playerManager.ComboProgress, 2)));
                Color c = Color.Purple * factor;
                Color cBorder = Color.White * factor;

                Vector2 multi = spriteBatch.DrawInt64Bordered(
                    font,
                    playerManager.ComboMulti,
                    ComboLocation + OFFSET_VEC_Y,
                    c,
                    cBorder);

                spriteBatch.DrawStringBordered(
                    font,
                    "x",
                    new Vector2(multi.X + 5, multi.Y),
                    c,
                    cBorder);

                spriteBatch.DrawInt64Bordered(
                    font,
                    playerManager.ComboScore,
                    new Vector2(multi.X + 20, multi.Y),
                    c,
                    cBorder);
            }

        }

        private void drawUpgrades(SpriteBatch spriteBatch)
        {
            Rectangle symbolDestination = UpgradeSymbolDestination;
            symbolDestination.Y += OFFSET_Y;
            Vector2 progressbarLocation = UpgradesLocation;
            progressbarLocation.Y += OFFSET_Y;

            if (playerManager.IsLaserActive())
            {
                drawUpgradeProgressBar(spriteBatch, symbolDestination, UpgradeLaserSource, progressbarLocation, playerManager.LaserValue());

                symbolDestination.Y += PROGRESSBAR_OFFSET;
                progressbarLocation.Y += PROGRESSBAR_OFFSET;
            }

            if (playerManager.IsMineActive())
            {
                drawUpgradeProgressBar(spriteBatch, symbolDestination, UpgradeMinesSource, progressbarLocation, playerManager.MineValue());

                symbolDestination.Y += PROGRESSBAR_OFFSET;
                progressbarLocation.Y += PROGRESSBAR_OFFSET;
            }

            if (playerManager.IsFastReloadActive())
            {
                drawUpgradeProgressBar(spriteBatch, symbolDestination, UpgradeFastReloadSource, progressbarLocation, playerManager.FastReloadValue());

                symbolDestination.Y += PROGRESSBAR_OFFSET;
                progressbarLocation.Y += PROGRESSBAR_OFFSET;
            }

            if (playerManager.IsLargeExplosionActive())
            {
                drawUpgradeProgressBar(spriteBatch, symbolDestination, UpgradeLargeExplosionSource, progressbarLocation, playerManager.LargeExplosionValue());

                symbolDestination.Y += PROGRESSBAR_OFFSET;
                progressbarLocation.Y += PROGRESSBAR_OFFSET;
            }

            if (playerManager.IsTimeFreezeActive())
            {
                drawUpgradeProgressBar(spriteBatch, symbolDestination, UpgradeTimeFreezeSource, progressbarLocation, playerManager.TimeFreezeValue());
                symbolDestination.Y += PROGRESSBAR_OFFSET;
                progressbarLocation.Y += PROGRESSBAR_OFFSET;
            }

            if (playerManager.IsStarActive())
            {
                drawUpgradeProgressBar(spriteBatch, symbolDestination, UpgradeStarSource, progressbarLocation, playerManager.StarValue());
            }
        }

        private void drawUpgradeProgressBar(SpriteBatch spriteBatch, Rectangle symbolDestination, Rectangle symbolSource,
                                            Vector2 progressBarLocation, float value)
        {
            spriteBatch.DrawBordered(
                texture,
                symbolDestination,
                symbolSource,
                Color.Purple * 0.8f,
                Color.White * 0.8f);

            spriteBatch.DrawBordered(
                    texture,
                    new Rectangle(
                        (int)progressBarLocation.X,
                        (int)progressBarLocation.Y,
                        BarWidth,
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        BarSourceWidth,
                        BarSourceHeight),
                    Color.Purple * 0.3f,
                    Color.White * 0.3f);

            spriteBatch.DrawBordered(
                texture,
                    new Rectangle(
                        (int)progressBarLocation.X,
                        (int)progressBarLocation.Y,
                        (int)(BarWidth * value),
                        BarHeight),
                    new Rectangle(
                        (int)SmallBarOverlayStart.X,
                        (int)SmallBarOverlayStart.Y,
                        (int)(BarSourceWidth * value),
                        BarSourceHeight),
                    Color.Purple * 0.75f,
                    Color.White * 0.75f);
        }

        private void drawLevel(SpriteBatch spriteBatch)
        {
            if (level == 0)
                return;

            if (lastLevel != level || currentLevelText.Length == 0)
            {
                if (currentLevelText.Length != 0)
                    currentLevelText.Clear();

                lastLevel = level;

                currentLevelText.Append(LEVEL_PRE_TEXT)
                                .Append(level);
            }

            spriteBatch.DrawStringBordered(
                font,
                currentLevelText.ToString(),
                new Vector2(screenBounds.Width / 2 - (font.MeasureString(currentLevelText).Y / 2) - 30,
                            5 + OFFSET_Y),
                Color.Purple * 0.8f,
                Color.White * 0.8f);
        }

        private void drawPlayerReload(SpriteBatch spriteBatch, float reloadValue, float reloadValueMax)
        {
            float factor = ReloadBarWidth / reloadValueMax;
            float value = reloadValueMax - MathHelper.Max(0.0f, reloadValue);

            spriteBatch.DrawBordered(
                    texture,
                    new Rectangle(
                        (int)playerReloadLocation.X,
                        (int)playerReloadLocation.Y,
                        ReloadBarWidth,
                        ReloadBarHeight),
                    new Rectangle(
                        (int)BigBarOverlayStart.X,
                        (int)BigBarOverlayStart.Y,
                        ReloadBarWidth,
                        ReloadBarHeight),
                    Color.Purple * 0.3f,
                    Color.White * 0.3f);

            spriteBatch.DrawBordered(
                texture,
                    new Rectangle(
                        (int)playerReloadLocation.X,
                        (int)playerReloadLocation.Y,
                        (int)(factor * value),
                        ReloadBarHeight),
                    new Rectangle(
                        (int)BigBarOverlayStart.X,
                        (int)BigBarOverlayStart.Y,
                        (int)(factor * value),
                        ReloadBarHeight),
                    Color.Purple * 0.75f,
                    Color.White * 0.75f);
        }

        private void drawCanFire(SpriteBatch spriteBatch)
        {
            if (displayCanFireTimer > 0)
            {
                float colorFactor = MathHelper.Clamp(displayCanFireTimer / DisplayCanFireTimerLimit, 0.0f, 1.0f);

                spriteBatch.Draw(
                    texture,
                    new Rectangle(0, 0, 480, 4),
                    new Rectangle(0, 550, 4, 4),
                    Color.Purple * colorFactor);
                spriteBatch.Draw(
                    texture,
                    new Rectangle(0, 796, 480, 4),
                    new Rectangle(0, 550, 4, 4),
                    Color.Purple * colorFactor);
                spriteBatch.Draw(
                    texture,
                    new Rectangle(0, 4, 4, 792),
                    new Rectangle(0, 550, 4, 4),
                    Color.Purple * colorFactor);
                spriteBatch.Draw(
                    texture,
                    new Rectangle(476, 4, 4, 792),
                    new Rectangle(0, 550, 4, 4),
                    Color.Purple * colorFactor);
            }
        }

        #endregion
    }
}
