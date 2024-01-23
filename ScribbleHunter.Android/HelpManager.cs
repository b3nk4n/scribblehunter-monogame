using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScribbleHunter.Inputs;
using Microsoft.Xna.Framework.Input.Touch;
using ScribbleHunter.Extensions;

namespace ScribbleHunter
{
    class HelpManager
    {
        #region Members

        private Texture2D texture;
        private SpriteFont font;
        private readonly Rectangle HelpTitleSource = new Rectangle(0, 1200,
                                                                   240, 80);
        private readonly Vector2 TitlePosition = new Vector2(120.0f, 100.0f);

        private static readonly string[] Content = {"If you have any further questions,",
                                            "ideas or problems with ScribbleHunter,",
                                            "please do not hesitate to contact us."};

        private const string Email = "bkan@outlook.de";
        private const string EmailSubject = "ScribbleHunter - Support";
        private const string Blog = "b3nk4n.github.io";
        private const string MusicTitle = "Music sponsor:";
        private const string Music = "PLSQMPRFKT";

        private readonly Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        //private WebBrowserTask browser;
        private const string BSAUTERMEISTER_URL = "http://bsautermeister.de";
        private const string SOUND_URL = "https://soundcloud.com/plsqmprfkt";

        private readonly Rectangle EmailDestination = new Rectangle(90, 420,
                                                                    300, 50);
        private readonly Rectangle BlogDestination = new Rectangle(90, 490,
                                                                    300, 50);

        private const int MusicLocationY = 630;

        public static GameInput GameInput;
        private const string EmailAction = "Email";
        private const string BlogAction = "Blog";
        private const string MusicAction = "Music";

        private readonly Rectangle SoundCloudSource = new Rectangle(500, 900,
                                                                   240, 115);

        private readonly Rectangle SoundCloudDestination = new Rectangle(120, 570,
                                                                         240, 115);

        #endregion

        #region Constructors

        public HelpManager(Texture2D tex, SpriteFont font, Rectangle screenBounds)
        {
            //this.browser = new WebBrowserTask();

            this.texture = tex;
            this.font = font;
            this.screenBounds = screenBounds;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(EmailAction,
                                           GestureType.Tap,
                                           EmailDestination);
            GameInput.AddTouchGestureInput(BlogAction,
                                           GestureType.Tap,
                                           BlogDestination);
            GameInput.AddTouchGestureInput(MusicAction,
                                           GestureType.Tap,
                                           SoundCloudDestination);
        }

        private void handleTouchInputs()
        {
            // Email
            if (GameInput.IsPressed(EmailAction))
            {
                // TODO implement actions below if needed
                //EmailComposeTask emailTask = new EmailComposeTask();
                //emailTask.To = Email;
                //emailTask.Subject = EmailSubject;
                //emailTask.Show();
            }
            // Blog
            if (GameInput.IsPressed(BlogAction))
            {
                //this.browser.Uri = new Uri(BSAUTERMEISTER_URL);
                //browser.Show();
            }

            // Music
            if (GameInput.IsPressed(MusicAction))
            {
                //this.browser.Uri = new Uri(SOUND_URL);
                //browser.Show();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             TitlePosition,
                             HelpTitleSource,
                             Color.White * opacity);

            for (int i = 0; i < Content.Length; ++i)
            {
                spriteBatch.DrawString(font,
                       Content[i],
                       new Vector2((screenBounds.Width - font.MeasureString(Content[i]).X) / 2,
                                   245 + (i * 40)),
                       Color.Black * opacity);
            }

            spriteBatch.DrawString(font,
                       Email,
                       new Vector2((screenBounds.Width - font.MeasureString(Email).X) / 2,
                                   EmailDestination.Y + 15),
                       Color.Black * opacity);

            spriteBatch.DrawString(font,
                       Blog,
                       new Vector2((screenBounds.Width - font.MeasureString(Blog).X) / 2,
                                   BlogDestination.Y + 15),
                       Color.Black * opacity);

            spriteBatch.Draw(texture,
                             SoundCloudDestination,
                             SoundCloudSource,
                             Color.White * opacity * 0.66f);

            spriteBatch.DrawStringBordered(
                       font,
                       MusicTitle,
                       new Vector2((screenBounds.Width - font.MeasureString(MusicTitle).X) / 2,
                                   MusicLocationY - 10),
                       Color.Black * opacity,
                       Color.White * opacity);

            spriteBatch.DrawStringBordered(
                       font,
                       Music,
                       new Vector2((screenBounds.Width - font.MeasureString(Music).X) / 2,
                                   MusicLocationY + 15),
                       Color.Black * opacity,
                       Color.White * opacity);
        }

        #endregion

        #region Properties

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                }
            }
        }

        #endregion
    }
}