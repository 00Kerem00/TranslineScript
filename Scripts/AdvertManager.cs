using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public class AdvertManager : MonoBehaviour
{
    static string bannerUnitID = "ca-app-pub-1511132058637905/5246733960";

    private static string interstitialUnitID = "ca-app-pub-1511132058637905/6993686635";
    public static bool interstitialAvailable = true;
    private static int interstitialEnableTime = 60;
    private static int interstitialTimeOut = 5;

    static BannerView banner;
    static InterstitialAd interstitial;

    #region Intersitital

    private static Coroutine cancelableTimeOutCorutine;
    private static IEnumerator afterInterstitial;

    public static IEnumerator ShowInterstitial(float delayTime, IEnumerator afterInterstitial)
    {
        yield return new WaitForSeconds(delayTime);
        ShowInterstitial(afterInterstitial);
    }
    public static void ShowInterstitial(IEnumerator afterInterstitial)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            AdRequest request = new AdRequest.Builder().Build();

            interstitial = new InterstitialAd(interstitialUnitID);

            interstitial.OnAdLoaded += OnInterstitialLoaded;
            interstitial.OnAdFailedToLoad += OnInterstitialFailed;
            interstitial.OnAdClosed += OnInterstitialClosed;
            cancelableTimeOutCorutine = StaticCoroutine.DoCoroutine(OnInterstitialTimeOut());

            interstitial.LoadAd(request);

            AdvertManager.afterInterstitial = afterInterstitial;
            PPRManager.InterruptForLoadingInterstitial();

            interstitialAvailable = false;
        }
        else
            StaticCoroutine.DoCoroutine(afterInterstitial);
    }
    public static void DestroyInterstitial()
    {
        if(interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }
    }

    private static void OnInterstitialLoaded(object sender, EventArgs eventArgs)
    {
        StaticCoroutine.CancelCoroutine(cancelableTimeOutCorutine);
        PPRManager.ContinueAfterInterstitial();
        interstitial.Show();
    }
    private static void OnInterstitialClosed(object sender, EventArgs eventArgs)
    {
        DestroyInterstitial();

        StaticCoroutine.CancelCoroutine(cancelableTimeOutCorutine);
        StaticCoroutine.DoCoroutine(EnableInterstitial());
        StaticCoroutine.DoCoroutine(afterInterstitial);
    }
    private static void OnInterstitialFailed(object sender, EventArgs eventArgs)
    {
        StaticCoroutine.CancelCoroutine(cancelableTimeOutCorutine);
        StaticCoroutine.DoCoroutine(EnableInterstitial());
        StaticCoroutine.DoCoroutine(afterInterstitial);

        DestroyInterstitial();

        PPRManager.ContinueAfterInterstitial();
        AndroidFunctions.ShowToastMessage("Ad Couldn't Load.");
    }
    private static IEnumerator OnInterstitialTimeOut()
    {
        yield return new WaitForSeconds(interstitialTimeOut);

        StaticCoroutine.DoCoroutine(EnableInterstitial());
        StaticCoroutine.DoCoroutine(afterInterstitial);

        DestroyInterstitial();

        PPRManager.ContinueAfterInterstitial();
        AndroidFunctions.ShowToastMessage("Ad Couldn't Load.");
    }

    private static IEnumerator EnableInterstitial()
    {
        yield return new WaitForSeconds(interstitialEnableTime);
        interstitialAvailable = true;
    }
    #endregion

    #region Banner
    public static void ShowBanner()
    {
        AdRequest request = new AdRequest.Builder().Build();

        banner = new BannerView(bannerUnitID, AdSize.SmartBanner, AdPosition.Bottom);
        banner.OnAdLoaded += OnBannerLoaded;
        banner.LoadAd(request);
        
    }
    private static void OnBannerLoaded(object sender, EventArgs eventArgs)
    {
        GameObject.Find("ImgAdPanel").GetComponent<Image>().enabled = false;

        banner.Show();
    }
    public static void DestroyBanner()
    {
        if(banner != null)
        {
            banner.Hide();
            banner.Destroy();
            banner = null;

            GameObject.Find("ImgAdPanel").GetComponent<Image>().enabled = true;
        }
    }
    #endregion
}
