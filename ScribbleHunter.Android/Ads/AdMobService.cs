using Android.App;
using Android.Gms.Ads;
using GmsAds = Android.Gms.Ads;
using Xamarin.Google.UserMesssagingPlatform;
using Android.Content;
using Log = System.Diagnostics;

namespace ScribbleHunter.Android.Ads
{
    public class AdMobService : Java.Lang.Object, IConsentFormOnConsentFormDismissedListener,
        IConsentInformationOnConsentInfoUpdateSuccessListener, IConsentInformationOnConsentInfoUpdateFailureListener
    {
		private readonly Activity activity;
        internal GmsAds.Interstitial.InterstitialAd interstitialAd;
        private IConsentInformation consentInformation;

        private volatile bool isMobileAdsInitializeCalled = false;


        public AdMobService(Activity activity)
		{
			this.activity = activity;
        }

        public void Initialize()
        {
            consentInformation = UserMessagingPlatform.GetConsentInformation(activity);

            var requsetParams = buildConsentRequestParams(activity);
            consentInformation.RequestConsentInfoUpdate(activity, requsetParams, this, this);

            if (consentInformation.CanRequestAds())
            {
                initializeMobileAdsSdk();
            }
        }

        private ConsentRequestParameters buildConsentRequestParams(Context context)
        {
            return new ConsentRequestParameters.Builder()
#if DEBUG
                .SetConsentDebugSettings(
                    new ConsentDebugSettings.Builder(activity)
                        .SetDebugGeography(ConsentDebugSettings.DebugGeography.DebugGeographyEea)
                        .AddTestDeviceHashedId("9FF27230C699AF49D14EBB6EC8E11F11") // Pixel 4       
                        .Build())
#endif
                .Build();
        }

        private void initializeMobileAdsSdk()
        {
            if (isMobileAdsInitializeCalled)
            {
                return;
            }

            MobileAds.Initialize(activity);
        }

		public void LoadInterstitial(string adUnitId)
		{
            if (!consentInformation.CanRequestAds())
            {
                return;
            }

            GmsAds.Interstitial.InterstitialAd.Load(
                activity, adUnitId, new AdRequest.Builder().Build(),
                new InterstitialCallbackImpl(this));
        }

        public void ShowInterstitial()
        {
            if (!IsReady)
            {
                return;
            }

            interstitialAd.Show(activity);
        }

        public void ShowPrivacyConsentForm()
        {
            UserMessagingPlatform.ShowPrivacyOptionsForm(activity, this);
        }

        public bool IsReady
        {
            get
            {
                return interstitialAd != null;
            }
        }

        public bool IsPrivacyOptionsRequired
        {
            get
            {
                return consentInformation.PrivacyOptionsRequirementStatus
                    == ConsentInformationPrivacyOptionsRequirementStatus.Required;
            }
        }

        public void OnConsentInfoUpdateSuccess()
        {
            UserMessagingPlatform.LoadAndShowConsentFormIfRequired(activity, this);
        }

        public void OnConsentInfoUpdateFailure(FormError formError)
        {
            Log.Debug.WriteLine("{0}: {1}", formError.ErrorCodeData(), formError.Message);
        }

        public void OnConsentFormDismissed(FormError formError)
        {
            if (formError != null)
            {
                Log.Debug.WriteLine("{0}: {1}", formError.ErrorCodeData(), formError.Message);
            }

            if (consentInformation.CanRequestAds())
            {
                initializeMobileAdsSdk();
            }
        }
    }

    class InterstitialCallbackImpl : InterstitialCallback
    {
        private readonly AdMobService adMobService;

        public InterstitialCallbackImpl(AdMobService adMobService)
        {
            this.adMobService = adMobService;
        }

        public override void OnAdLoaded(GmsAds.Interstitial.InterstitialAd interstitialAd)
        {
            adMobService.interstitialAd = interstitialAd;
            base.OnAdLoaded(interstitialAd);

        }
        public override void OnAdFailedToLoad(LoadAdError loadAdError)
        {
            base.OnAdFailedToLoad(loadAdError);
        }
    }
}

