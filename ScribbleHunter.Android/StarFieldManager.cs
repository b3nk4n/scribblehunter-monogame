using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class StarFieldManager
    {
        #region Members

        private List<Sprite> stars1;
        private List<Sprite> stars2;
        private int screenWidth;
        private int screenHeight;
        private Random rand = new Random();
        private Color[] colors = { Color.Gray * 0.66f,
                                   Color.Wheat * 0.75f,
                                   Color.LightGray * 0.75f,
                                   Color.SlateGray * 0.66f};

        Sprite planet;
        Texture2D planetSheet;
        Rectangle initialPlanetRect;
        int planetImagesCountX;
        int planetImagesCountY;
        private int currentIndexX;
        private int currentIndexY;

        #endregion

        #region Constructors

        public StarFieldManager(int screenWidth, int screenHeight, int starCount1, int starCount2,
                                Vector2 starVelocity1, Vector2 starVelocity2, Texture2D texture, Rectangle frameRect1, Rectangle frameRect2,
                                Texture2D planetSheet, Rectangle initialPlanetRect, int planetImagesCountX, int planetImagesCountY)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            // Layer 1
            stars1 = new List<Sprite>(starCount1 + 1);

            for (int x = 0; x < starCount1; x++)
            {
                stars1.Add(new Sprite(new Vector2(rand.Next(0, screenWidth), rand.Next(0, screenHeight)),
                                     texture,
                                     frameRect1,
                                     starVelocity1));
                Color starColor = colors[rand.Next(0, colors.Length)];
                starColor *= (float)rand.Next(30, 80) / 100.0f;
                stars1[stars1.Count - 1].TintColor = starColor;
            }

            // Layer 2
            stars2 = new List<Sprite>(starCount2 + 1);

            for (int x = 0; x < starCount2; x++)
            {
                stars2.Add(new Sprite(new Vector2(rand.Next(0, screenWidth), rand.Next(0, screenHeight)),
                                     texture,
                                     frameRect2,
                                     starVelocity2));
                Color starColor = colors[rand.Next(0, colors.Length)];
                starColor *= (float)rand.Next(30, 80) / 100.0f;
                stars2[stars2.Count - 1].TintColor = starColor;
            }

            // Planet
            this.planetSheet = planetSheet;
            this.initialPlanetRect = initialPlanetRect;
            this.planetImagesCountX = planetImagesCountX;
            this.planetImagesCountY = planetImagesCountY;
            resetPlanet();
        }

        #endregion

        #region Methods

        private void resetPlanet()
        {
            currentIndexX = rand.Next(planetImagesCountX);
            currentIndexY = rand.Next(planetImagesCountY);

            planet = new Sprite(getPlanetStartLocation(),
                                planetSheet,
                                new Rectangle(initialPlanetRect.X + (currentIndexX * initialPlanetRect.Width), initialPlanetRect.Y + (currentIndexY * initialPlanetRect.Height),
                                              initialPlanetRect.Width, initialPlanetRect.Height),
                                new Vector2(0, 15));
            planet.TintColor *= 0.15f;
            planet.Rotation = MathHelper.ToRadians(rand.Next(-30, 30));
        }

        private Vector2 getPlanetStartLocation()
        {
            return new Vector2(rand.Next(0, screenWidth) - initialPlanetRect.Width / 2,
                               rand.Next(-500, -350));
        }

        public void Update(float elapsed)
        {
            foreach (var star in stars1)
            {
                star.Update(elapsed);

                if (star.Location.Y > screenHeight)
                {
                    star.Location = new Vector2(rand.Next(0, screenWidth), 0);
                }
            }

            foreach (var star in stars2)
            {
                star.Update(elapsed);

                if (star.Location.Y > screenHeight)
                {
                    star.Location = new Vector2(rand.Next(0, screenWidth), 0);
                }
            }

            planet.Update(elapsed);

            if (planet.Location.Y > screenHeight)
                resetPlanet();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var star in stars1)
            {
                star.Draw(spriteBatch);
            }

            foreach (var star in stars2)
            {
                star.Draw(spriteBatch);
            }

            planet.Draw(spriteBatch);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.currentIndexX = Int32.Parse(reader.ReadLine());
            this.currentIndexY = Int32.Parse(reader.ReadLine());
            this.planet.Activated(reader);
            this.planet.ReplaceFrames(new Rectangle(initialPlanetRect.X + (currentIndexX * initialPlanetRect.Width), initialPlanetRect.Y + (currentIndexY * initialPlanetRect.Height),
                                                    initialPlanetRect.Width, initialPlanetRect.Height));
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(currentIndexX);
            writer.WriteLine(currentIndexY);
            this.planet.Deactivated(writer);
        }

        #endregion
    }
}