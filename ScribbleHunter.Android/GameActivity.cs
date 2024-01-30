using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Window;
using GooglePlay.Services.Helpers;
using Microsoft.Xna.Framework;

namespace ScribbleHunter.Android
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@mipmap/ic_launcher",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    // Please note that e.g. on an Android API 31 device (or at least below API 34 for sure),
    // the activity crashes when this class implements any other interface with:
    // java.lang.RuntimeException: Unable to instantiate activity ComponentInfo
    // Because IOnBackInvokedCallback actually has more methods to override
    public class GameActivity : AndroidGameActivity
    {
        private const string HIGHSCORES_ID = "CgkIm_rm-MobEAIQAQ";

        private ScribbleHunter _game;
        private View _view;

        GameHelper helper;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new ScribbleHunter(ShowLeaderboardsHandler, SubmitLeaderboardsScore);
            _view = _game.Services.GetService(typeof(View)) as View;

            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                // Starting from Android API 33, we can support the geasture
                // BACK buttons.
                // When the phone is running on an older API level, we rely
                // on GamePad BACK key handled by the game.
                OnBackInvokedDispatcher.RegisterOnBackInvokedCallback(0, new OnBackInvokedCallback(_game));
            }

            SetContentView(_view);
            InitializeServices();

            if (helper != null && helper.SignedOut)
            {
                helper.SignIn();
            }

            _game.Run();
        }

        void InitializeServices()
        {
            // Setup Google Play Services Helper
            helper = new GameHelper(this);
            // Set Gravity and View for Popups
            helper.GravityForPopups = (GravityFlags.Top | GravityFlags.Center);
            helper.ViewForPopups = _view;
            // Hook up events
            helper.OnSignedIn += (object sender, EventArgs e) => {
                Log.Info("GameActivity", "Signed in");
            };
            helper.OnSignInFailed += (object sender, EventArgs e) => {
                Log.Info("GameActivity", "Signed in failed!");
            };

            helper.Initialize();
        }

        private void ShowLeaderboardsHandler()
        {
            if (helper != null && !helper.SignedOut)
            {
                helper.ShowAllLeaderBoardsIntent();
            }
        }

        private void SubmitLeaderboardsScore(long score)
        {
            if (helper != null && !helper.SignedOut)
            {
                helper.SubmitScore(HIGHSCORES_ID, score);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (helper != null)
                helper.Start();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (helper != null)
                helper.OnActivityResult(requestCode, resultCode, data);
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnStop()
        {
            if (helper != null)
                helper.Stop();
            base.OnStop();
        }
    }

    class OnBackInvokedCallback : Java.Lang.Object, IOnBackInvokedCallback
    {
        private readonly IBackButtonPressedCallback callback;

        public OnBackInvokedCallback(IBackButtonPressedCallback callback)
        {
            this.callback = callback;
        }

        public void OnBackInvoked()
        {
            callback.BackButtonPressed();
        }
    }
}

