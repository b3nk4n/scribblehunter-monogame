using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace ScribbleHunter
{
    class ZoomText
    {
        #region Membsers

        public string text;
        public Color drawColor;
        public int displayCounter;
        private int maxDisplayCount;
        private float lastScaleAmount = 0.0f;
        private float scaleRate;
        private Vector2 location;

        #endregion

        #region Constructors

        public ZoomText()
        {
            this.displayCounter = 0;
        }

        public ZoomText(string text, Color fontColor,
                        int maxDisplayCount, float scaleRate,
                        Vector2 location)
        {
            this.text = text;
            this.drawColor = fontColor;
            this.displayCounter = 0;
            this.maxDisplayCount = maxDisplayCount;
            this.scaleRate = scaleRate;
            this.location = location;
        }

        #endregion

        #region Methods

        public void Update()
        {
            lastScaleAmount += scaleRate;
            displayCounter++;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.text = reader.ReadLine();

            this.drawColor = new Color(Int32.Parse(reader.ReadLine()),
                                       Int32.Parse(reader.ReadLine()),
                                       Int32.Parse(reader.ReadLine()),
                                       Int32.Parse(reader.ReadLine()));

            this.displayCounter = Int32.Parse(reader.ReadLine());
            this.maxDisplayCount = Int32.Parse(reader.ReadLine());

            this.lastScaleAmount = Single.Parse(reader.ReadLine());
            this.scaleRate = Single.Parse(reader.ReadLine());

            this.location.X = Single.Parse(reader.ReadLine());
            this.location.Y = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(text);

            writer.WriteLine((int)drawColor.R);
            writer.WriteLine((int)drawColor.G);
            writer.WriteLine((int)drawColor.B);
            writer.WriteLine((int)drawColor.A);

            writer.WriteLine(displayCounter);
            writer.WriteLine(maxDisplayCount);

            writer.WriteLine(lastScaleAmount);
            writer.WriteLine(scaleRate);

            writer.WriteLine(location.X);
            writer.WriteLine(location.Y);
        }

        #endregion

        #region Properties

        public float Scale
        {
            get
            {
                return scaleRate * displayCounter;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return displayCounter > maxDisplayCount;
            }
        }

        public float Progress
        {
            get
            {
                return (float)displayCounter / maxDisplayCount;
            }
        }

        public Vector2 Location
        {
            get
            {
                return this.location;
            }
        }

        #endregion
    }
}

