using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class HandManager
    {
        #region Members

        private const float STAGGER_X = 6.0f;
        private const float STAGGER_Y = 8.0f;

        private Texture2D texture;

        private readonly Rectangle leftHandSource = new Rectangle(0, 0, 150, 740);

        private readonly Vector2 leftHandFrom = new Vector2(-160, 70);

        private readonly Vector2 leftHandTo = new Vector2(-50, 80);

        private Vector2 leftHandPosition;

        private Vector2 leftHandStagger;

        private readonly Rectangle rightHandSource = new Rectangle(150, 0, 350, 800);

        private readonly Vector2 rightHandFrom = new Vector2(490, 160);

        private readonly Vector2 rightHandTo = new Vector2(330, 180);

        private Vector2 rightHandPosition;

        private Vector2 rightHandStagger;

        private bool handShown;

        private readonly Vector2 ScripplePosition = new Vector2(208, 170);
        private bool scribbled = true;

        #endregion

        #region Constructors

        public HandManager(Texture2D texture)
        {
            this.texture = texture;

            leftHandPosition = leftHandFrom;
            rightHandPosition = rightHandFrom;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            leftHandPosition = leftHandFrom;
            rightHandPosition = rightHandFrom;
            scribbled = true;
        }

        public void ShowHands()
        {
            handShown = true;
            scribbled = true;
        }

        public void HideHands()
        {
            handShown = false;
            scribbled = true;
        }

        public void HideHandsAndScribble()
        {
            handShown = false;
            scribbled = false;
            SoundManager.PlayWritingSound();
        }

        public void Update(GameTime gameTime)
        {
            Vector2 velocityLeft;
            Vector2 velocityRight;

            if (handShown)
            {
                velocityLeft = (leftHandPosition - leftHandTo) * 0.04f;
                velocityRight = (rightHandPosition - rightHandTo) * 0.04f;
            }
            else
            {
                velocityLeft = -(leftHandTo - (leftHandPosition - leftHandFrom)) * 0.015f;
                velocityRight = -(rightHandTo - (rightHandPosition - rightHandFrom)) * 0.01f;

                if (!scribbled)
                {
                    velocityRight = (rightHandPosition - ScripplePosition) * 0.166f;

                    if ((rightHandPosition - ScripplePosition).Length() < 2)
                    {
                        scribbled = true;
                    }
                }

                if (scribbled)
                {
                    velocityRight = -(rightHandTo - (rightHandPosition - rightHandFrom)) * 0.01f;
                }

            }



            leftHandPosition -= velocityLeft;
            rightHandPosition -= velocityRight;

            // Stagger
            leftHandStagger = new Vector2(
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * STAGGER_X,
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * STAGGER_Y);
            rightHandStagger = new Vector2(
                (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds) * STAGGER_X,
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds / 2) * STAGGER_Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Left hand
            if (leftHandPosition.X > -155)
            {
                spriteBatch.Draw(
                    texture,
                    leftHandPosition + leftHandStagger,
                    leftHandSource,
                    Color.White);
            }

            // Right hand
            if (rightHandPosition.X < 485)
            {
                spriteBatch.Draw(
                    texture,
                    rightHandPosition + rightHandStagger,
                    rightHandSource,
                    Color.White);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Left hand
            this.leftHandPosition = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            this.leftHandStagger = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            // Right hand
            this.rightHandPosition = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            this.rightHandStagger = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            this.scribbled = Boolean.Parse(reader.ReadLine());
        }


        public void Deactivated(StreamWriter writer)
        {
            // Left hand
            writer.WriteLine(leftHandPosition.X);
            writer.WriteLine(leftHandPosition.Y);

            writer.WriteLine(leftHandStagger.X);
            writer.WriteLine(leftHandStagger.Y);

            // Right hand
            writer.WriteLine(rightHandPosition.X);
            writer.WriteLine(rightHandPosition.Y);

            writer.WriteLine(rightHandStagger.X);
            writer.WriteLine(rightHandStagger.Y);

            writer.WriteLine(scribbled);
        }

        #endregion
    }
}