using System;
using GmsAds = Android.Gms.Ads;
using Android.Runtime;

namespace ScribbleHunter.Android.Ads
{
    /// <summary>
	/// We need to replace the official callback with our own one, because the
	/// official one is based on a deprecated GMS version and is missing the OnAdLoad callback.
    /// See https://learn.microsoft.com/en-us/answers/questions/508242/how-to-add-interstitial-ads-in-xamarin-android.
    /// </summary>
    public abstract class InterstitialCallback : GmsAds.Interstitial.InterstitialAdLoadCallback
    {
        [Register("onAdLoaded", "(Lcom/google/android/gms/ads/interstitial/InterstitialAd;)V", "GetOnAdLoadedHandler")]
        public virtual void OnAdLoaded(GmsAds.Interstitial.InterstitialAd interstitialAd)
        {
        }

        private static Delegate cb_onAdLoaded;
        private static Delegate GetOnAdLoadedHandler()
        {
            if (cb_onAdLoaded is null)
                cb_onAdLoaded = JNINativeWrapper.CreateDelegate((Action<IntPtr, IntPtr, IntPtr>)onAdLoaded);
            return cb_onAdLoaded;
        }
        private static void onAdLoaded(IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
        {
            InterstitialCallback thisobject = GetObject<InterstitialCallback>(jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            GmsAds.Interstitial.InterstitialAd resultobject = GetObject<GmsAds.Interstitial.InterstitialAd>(native_p0, JniHandleOwnership.DoNotTransfer);
            thisobject.OnAdLoaded(resultobject);
        }
    }
}

