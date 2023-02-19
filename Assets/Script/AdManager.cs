using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using System;

public class AdManager : MonoBehaviour
{   
    [SerializeField] Canvas canvas;

    #if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-2079986244432267/6130186663";
    #elif UNITY_IPHONE
      private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
    #else
      private string _adUnitId = "unused";
    #endif

    public InterstitialAd interstitialAd;

    void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) => { });
        LoadInterstitialAd();
    }

    void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    return;
                }

                interstitialAd = ad;
            });

        RegisterEventHandlers(interstitialAd);
    }

    public void ShowAd()
    {
        canvas.sortingOrder = -1;
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            interstitialAd.Destroy();
            SceneManager.LoadScene(0);
        };
    }
}
