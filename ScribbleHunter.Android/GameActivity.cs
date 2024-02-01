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
using ScribbleHunter.Android.Ads;

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

#if DEBUG
        private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/1033173712";
#else
        private const string AD_UNIT_ID = "ca-app-pub-8102925760359189/2412240412";
#endif

        private ScribbleHunter _game;
        private View _view;

        private GameHelper helper;
        private AdMobService adMobService;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new ScribbleHunter(
                ShowLeaderboardsHandler, SubmitLeaderboardsScore,
                StartNewGameHandler, GameOverEndedHandler,
                IsPrivacyOptionsRequiredHanlder, ShowPrivacyConsentFormHandler);
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
            InitializeServices(bundle);

            if (helper != null && helper.SignedOut)
            {
                helper.SignIn();
            }

            _game.Run();
        }

        void InitializeServices(Bundle bundle)
        {
            Xamarin.Essentials.Platform.Init(this, bundle);

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

            adMobService = new AdMobService(this);
            adMobService.Initialize();
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

        private void StartNewGameHandler()
        {
            adMobService.LoadInterstitial(AD_UNIT_ID);
        }

        private void GameOverEndedHandler()
        {
            adMobService.ShowInterstitial();
        }

        private bool IsPrivacyOptionsRequiredHanlder()
        {
            return adMobService.IsPrivacyOptionsRequired;
        }

        private void ShowPrivacyConsentFormHandler()
        {
            adMobService.ShowPrivacyConsentForm();
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

