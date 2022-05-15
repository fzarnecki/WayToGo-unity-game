using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using EasyMobile;

public class AdManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private IAPManager iapManager;

    private bool testMode = true;

    /***/

    // Initialization
    void Awake() {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }

    // CONSENT

    public void GrantAdPrivacyConsent() {
        ConsentStatus adConsent = Advertising.DataPrivacyConsent;
        if (adConsent == ConsentStatus.Unknown)
            Advertising.GrantDataPrivacyConsent();
    }

    public void GrantAdVendorConsent() {
        ConsentStatus unityadsConsent = Advertising.GetDataPrivacyConsent(AdNetwork.UnityAds);
        if (unityadsConsent == ConsentStatus.Unknown)
            Advertising.GrantDataPrivacyConsent(AdNetwork.UnityAds);
    }


    // INTERSTITIAL

    public void ShowInterstitialAd() {
        // Check if interstitial ad is ready
        bool isReady = Advertising.IsInterstitialAdReady();
        
        // Not showing ads if the iap has been purchased or if its not available; loading level immediately
        if (!isReady || iapManager.IsRemoveAdsPurchased())
        {
            levelManager.LoadNextLevel();
            return;
        }

        // 40% chance for an ad, 40% for a popup and 20% chance for nothing
        int rand = Random.Range(0, 10);
        if (rand >= 0 && rand < 2)
        {
            // 20% for a lucky one to get no ad and no popup
            return;
        }
        else if (rand >= 2 && rand < 6)
        {
            // Popusk asking to purchase the RemoveAds option
            iapManager.OpenRemoveAdsPopup();
            return;
        }
        else
        {
            // Show ad if it's ready
            if (isReady)
                Advertising.ShowInterstitialAd();
        }
    }

    // The event handler
    void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdPlacement location) {
        // Debug.Log("Interstitial ad has been closed.");
        levelManager.LoadNextLevel();
    }


    // REWARDED

    public void ShowRewardedAd() {
        // If iap purchased then immediately giving the reward for watching (probably useless since checked in tip manager; leaving for extra safety)
        if (iapManager.IsRemoveAdsPurchased())
        {
            gameManager.GiveRewardForAd();
            return;
        }

        // Check if rewarded ad is ready
        bool isReady = Advertising.IsRewardedAdReady();

        // Show it if it's ready
        if (isReady)
            Advertising.ShowRewardedAd();
    }

    // Event handler called when a rewarded ad has completed
    void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement location) {
        // Debug.Log("Rewarded ad has completed. The user should be rewarded now.");
        gameManager.GiveRewardForAd();
    }

    // Event handler called when a rewarded ad has been skipped
    void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement location) {
        // Debug.Log("Rewarded ad was skipped. The user should NOT be rewarded.");
        gameManager.FailedToShowAd();
    }


    //

    // Subscribe to the event
    void OnEnable() {
        Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;

        Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
        Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
    }

    // Unsubscribe
    void OnDisable() {
        Advertising.InterstitialAdCompleted -= InterstitialAdCompletedHandler;

        Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
        Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
    }
}
