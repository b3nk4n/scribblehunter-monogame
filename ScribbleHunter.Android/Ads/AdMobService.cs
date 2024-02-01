using Android.App;
using Android.Gms.Ads;
using GmsAds = Android.Gms.Ads;

namespace ScribbleHunter.Android.Ads
{
    public class AdMobService
	{
		private readonly Activity activity;
        internal GmsAds.Interstitial.InterstitialAd interstitialAd;

		public AdMobService(Activity activity)
		{
			this.activity = activity;
        }

		public void LoadInterstitial(string adUnitId)
		{
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

        public bool IsReady
        {
            get
            {
                return interstitialAd != null;
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

