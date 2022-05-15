using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using UnityEngine.Purchasing.Security;

public class IAPManager : MonoBehaviour
{
    [SerializeField] private GameObject removeAdsPopup;
    [SerializeField] private GameObject removeAdsButton;
    private float popupAnimationDuration = 0.5f;

    /***/

    private void Start() {
        InitializePurchases();
        SetUpRemoveAdsPopup();
        SetupRemoveAdsButton();
    }
    
    /***/

    public void PurchaseRemoveAds() {
        if (!IsRemoveAdsPurchased())
            InAppPurchasing.Purchase(EM_IAPConstants.Product_Remove_Ads);
    }

    public void OpenRemoveAdsPopup() {
        removeAdsPopup.SetActive(true);
        removeAdsPopup.GetComponent<Animator>().SetTrigger("In");
    }

    public void CloseRemoveAdsPopup() {
        removeAdsPopup.GetComponent<Animator>().SetTrigger("Out");
        StartCoroutine(TurnOffWithDelay(removeAdsPopup, popupAnimationDuration));
    }

    public void CloseRemoveAdsPopupInLevel() {
        removeAdsPopup.GetComponent<Animator>().SetTrigger("Out");
        StartCoroutine(TurnOffWithDelay(removeAdsPopup, popupAnimationDuration));
        FindObjectOfType<LevelManager>().LoadNextLevel();
    }

    private IEnumerator TurnOffWithDelay(GameObject g, float t) {
        yield return new WaitForSeconds(t);
        g.SetActive(false);
    }

    private void SetUpRemoveAdsPopup() {
        if (removeAdsPopup)
            removeAdsPopup.SetActive(false);
    }

    public void SetupRemoveAdsButton() {
        if (removeAdsButton && IsRemoveAdsPurchased())
            removeAdsButton.SetActive(false);
        else if (removeAdsButton)
            removeAdsButton.SetActive(true);
    }


    void PurchaseCompletedHandler(IAPProduct product) {
        // Compare product name to the generated name constants to determine which product was bought
        switch (product.Name)
        {
            case EM_IAPConstants.Product_Remove_Ads:
                //Debug.Log("Sample_Product was purchased. The user should be granted it now.");
                break;
        }
    }

    // Failed purchase handler
    void PurchaseFailedHandler(IAPProduct product) {
        //Debug.Log("The purchase of product " + product.Name + " has failed.");
    }


    // Subscribe to IAP purchase events
    void OnEnable() {
        InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
    }

    // Unsubscribe when the game object is disabled
    void OnDisable() {
        InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
    }

    private void InitializePurchases() {
        if (!InAppPurchasing.IsInitialized())
            InAppPurchasing.InitializePurchasing();
    }

    public bool IsRemoveAdsPurchased() {
        if (Application.platform == RuntimePlatform.Android)
        {
            GooglePlayReceipt receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_Remove_Ads);
            if (receipt == null)
                return false;
            else if (receipt.purchaseState == GooglePurchaseState.Purchased)
                return true;
            else if (receipt.purchaseState == GooglePurchaseState.Cancelled || receipt.purchaseState == GooglePurchaseState.Refunded)
                return false;
            else    // extra precautions
                return false;
        }
        else
        {
            // Leaving place for other platforms, e.g. ios
            return false;
        }
    }
}
