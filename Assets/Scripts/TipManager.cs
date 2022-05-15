using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    private List<Cube> tipCubes = new List<Cube>();
    
    [SerializeField] private GameObject watchAdPopup;
    [SerializeField] private GameObject inGameWatchAdPopup;

    [SerializeField] private Animator tipAnimator;

    [SerializeField] private GameObject gameTipButton;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private IAPManager iapManager;

    private float popupAnimationDuration = 0.5f;

    private bool tipThisTurn = false;

    /***/

    private void Start() {
        if (watchAdPopup && watchAdPopup.activeInHierarchy) // disabling popup in case it has been left active
            watchAdPopup.SetActive(false);

        if (inGameWatchAdPopup && inGameWatchAdPopup.activeInHierarchy)
            inGameWatchAdPopup.SetActive(false);

        StartCoroutine(SetupGameTipButton());
    }


    // Method to indicate whether there should be tip in level execution, used when the players' turn starts
    private bool IsTip() {
        if (SaveManager.instance.tip)
            return true;
        else
            return false;
    }

    public void HandleTip() {
        if (IsTip())
        {
            SetTipThisTurn(true);
            tipAnimator.gameObject.SetActive(true);
            tipAnimator.SetTrigger("In");

            int count = (int)(Mathf.Ceil((gameManager.GetPathLength() / 3)) + 1);

            for (int i = 0; i < count; i++) 
            {
                Cube c = gameManager.GetPathAt(i);
                if (!tipCubes.Contains(c))
                    tipCubes.Add(c);
            }

            gameManager.SetupLine(count);

            ResetTip();
        }
    }

    public void ActivateTip() { SaveManager.instance.tip = true; SaveManager.instance.SaveGame(); }
    private void ResetTip() { SaveManager.instance.tip = false; SaveManager.instance.SaveGame(); }

    public bool TipContains(Cube c) { return tipCubes.Contains(c); }

    private void SetTipThisTurn(bool b) { tipThisTurn = b; }
    public bool IsTipThisTurn() { return tipThisTurn; }

    // public void OpenTipPopup() { OpenPopup(tipPopup); }
    // public void CloseTipPopup() { ClosePopup(tipPopup); }

    public void OpenInGameWatchAdPopup() {
        if (iapManager.IsRemoveAdsPurchased())
        {
            gameManager.GiveRewardForAd();
            DisableTipButton();
        }
        else
            OpenPopup(inGameWatchAdPopup);
    }

    public void CloseInGameWatchAdPopup() { ClosePopup(inGameWatchAdPopup); }

    // delay to ensure game data is loaded
    private IEnumerator SetupGameTipButton() {
        yield return new WaitForSeconds(0.1f);
        if (gameTipButton && IsTip())
            gameTipButton.SetActive(false);
    }

    public void OpenWatchAdPopup() {
        if (iapManager.IsRemoveAdsPurchased())
            gameManager.GiveRewardForAd();
        else
            OpenPopup(watchAdPopup);
    }

    public void CloseWatchAdPopup() { ClosePopup(watchAdPopup); }

    private void OpenPopup(GameObject popup) {
        popup.SetActive(true);
        popup.GetComponent<Animator>().SetTrigger("In");
    }

    private void ClosePopup(GameObject popup) {
        popup.GetComponent<Animator>().SetTrigger("Out");
        StartCoroutine(TurnOffWithDelay(popup, popupAnimationDuration));
    }

    public void DisableTipButton() {
        if (gameTipButton.activeInHierarchy)
        {
            gameTipButton.GetComponent<Animator>().SetTrigger("Out");
            StartCoroutine(TurnOffWithDelay(gameTipButton, 2f));
        }
    }

    private IEnumerator TurnOffWithDelay(GameObject g, float t) {
        yield return new WaitForSeconds(t);
        g.SetActive(false);
    }
}
