using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradesManager : MonoBehaviour
{
    private int upgradesCount = 4;
    private int upgradeMaxLevel = 4;

    private int[] upgradePrices = { 0, 500, 1500, 3000, 5000 };

    enum Upgrades
    {
        searchTime,
        memoryTime,
        levelMoney,
        tileMoney
    }

    private float additionalSearchTime = 2f;
    private float additionalMemoryTime = 1f;
    private int additionalLevelMoney = 10;
    private float additionalTileCoinChance = 0.1f;

    [Header("Sliders")]
    [SerializeField] private Slider searchTimeSlider;
    [SerializeField] private Slider memoryTimeSlider;
    [SerializeField] private Slider levelMoneySlider;
    [SerializeField] private Slider tileMoneySlider;

    [Header("Upgrade icons")]
    [SerializeField] private Sprite searchTimeIcon;
    [SerializeField] private Sprite memoryTimeIcon;
    [SerializeField] private Sprite levelMoneyIcon;
    [SerializeField] private Sprite tileMoneyIcon;

    [Header("Popups")]
    [SerializeField] private GameObject upgradePopup;
    [SerializeField] private Image upgradePopupIcon;
    [SerializeField] private TextMeshProUGUI upgradePopupText;
    [SerializeField] private TextMeshProUGUI upgradePopupPriceText;
    //
    [SerializeField] private GameObject infoPopup;
    [SerializeField] private Image infoIcon;
    [SerializeField] private TextMeshProUGUI infoPopupText;
                                                
    private string searchTimeInfoText = "Lacking time to recreate a given path? Search time will prolong the time you have in each level to make your final decision.";
    private string memoryTimeInfoText = "Need the path revealed to stay on for a longer period of time? Improve Memory time. Useful if you need a few more seconds to memorise the trail.";
    private string levelMoneyInfoText = "Lacking some coins? Level money is the way to go. It will increase the number of coins received for passing each level.";
    private string tileMoneyInfoText = "If you like pleasant surprises, Tile money is for you. This one will increase the chance for a random coin to appear during path check.";

    [Header("Other")]
    [SerializeField] private CoinsManager coinsManager;

    private float popupAnimationDuration = 0.5f;

    /***/

    private void Start() {
        // Making sure that popups are disabled
        if (upgradePopup)
            upgradePopup.SetActive(false);
        if (infoPopup)
            infoPopup.SetActive(false);
    }


    // Called when entering shop
    public void UpdateSliders() {
        searchTimeSlider.value = GetLevel(Upgrades.searchTime);
        memoryTimeSlider.value = GetLevel(Upgrades.memoryTime);
        levelMoneySlider.value = GetLevel(Upgrades.levelMoney);
        tileMoneySlider.value = GetLevel(Upgrades.tileMoney);
    }

    /* UPGRADE */

    // Called by clicking on upgrade
    public void OpenSearchTimeUpgradePopup() {
        OpenUpgradePopup(GetLevel(Upgrades.searchTime), "Search Time", searchTimeIcon);
    }

    public void OpenMemoryTimeUpgradePopup() {
        OpenUpgradePopup(GetLevel(Upgrades.memoryTime), "Memory Time", memoryTimeIcon);
    }

    public void OpenLevelMoneyUpgradePopup() {
        OpenUpgradePopup(GetLevel(Upgrades.levelMoney), "Level Money", levelMoneyIcon);
    }

    public void OpenTileMoneyUpgradePopup() {
        OpenUpgradePopup(GetLevel(Upgrades.tileMoney), "Tile Money", tileMoneyIcon);
    }

    private void OpenUpgradePopup(int level, string name, Sprite icon) {
        if (level >= 4) return;
        
        upgradePopup.SetActive(true);
        upgradePopup.GetComponent<Animator>().SetTrigger("In");

        upgradePopupIcon.sprite = icon;
        upgradePopupText.text = "Would you like to upgrade " + name + "?";
        upgradePopupPriceText.text = upgradePrices[level + 1].ToString();
    }

    // Called by upgrade buttons
    public void UpgradeButton() {
        Sprite icon = upgradePopupIcon.sprite;
        if (icon == searchTimeIcon)
            Upgrade(Upgrades.searchTime);
        else if (icon == memoryTimeIcon)
            Upgrade(Upgrades.memoryTime);
        else if (icon == levelMoneyIcon)
            Upgrade(Upgrades.levelMoney);
        else if (icon == tileMoneyIcon)
            Upgrade(Upgrades.tileMoney);
    }
    
    private void Upgrade(Upgrades u) {
        var level = GetLevel(u);
        if (level >= 4) return;

        int price = upgradePrices[level + 1];
        if (coinsManager.GetPlayerCoins() >= price)
        {
            // Debug.Log("Upgrading " + u + " to level " + (level + 1) + " for " + price + " coins.");
            SaveManager.instance.upgradeLevels[(int)u] = level + 1;
            SaveManager.instance.SaveGame();
            coinsManager.SpendCoins(price);
            UpdateSliders();

            // Turning popup off
            upgradePopup.GetComponent<Animator>().SetTrigger("Out");
            StartCoroutine(TurnOffWithDelay(upgradePopup, popupAnimationDuration));
        }
        else
            return;
    }

    // Called by cancel buttons
    public void CancelUpgradePopup() {
        upgradePopup.GetComponent<Animator>().SetTrigger("Out");
        StartCoroutine(TurnOffWithDelay(upgradePopup, popupAnimationDuration));
    }


    /* INFO */

    public void OpenSearchTimeInfoPopup() {
        OpenInfoPopup(searchTimeInfoText, searchTimeIcon);
    }

    public void OpenMemoryTimeInfoPopup() {
        OpenInfoPopup(memoryTimeInfoText, memoryTimeIcon);
    }

    public void OpenLevelMoneyInfoPopup() {
        OpenInfoPopup(levelMoneyInfoText, levelMoneyIcon);
    }

    public void OpenTileMoneyInfoPopup() {
        OpenInfoPopup(tileMoneyInfoText, tileMoneyIcon);
    }

    private void OpenInfoPopup(string t, Sprite icon) {
        infoPopup.SetActive(true);
        infoPopup.GetComponent<Animator>().SetTrigger("In");

        infoPopupText.text = t;
        infoIcon.sprite = icon;
    }

    public void CloseInfoPopup() {
        infoPopup.GetComponent<Animator>().SetTrigger("Out");
        StartCoroutine(TurnOffWithDelay(infoPopup, popupAnimationDuration));
    }
    
    
    public float GetAdditionalSearchTime() {
        return additionalSearchTime * GetLevel(Upgrades.searchTime);
    }

    public float GetAdditionalMemoryTime() {
        return additionalMemoryTime * GetLevel(Upgrades.memoryTime);
    }

    public int GetAdditionalLevelMoney() {
        return additionalLevelMoney * GetLevel(Upgrades.levelMoney);
    }

    public float GetAdditionalTileMoneyChance() {
        return additionalTileCoinChance * GetLevel(Upgrades.tileMoney);
    }

    private int GetLevel(Upgrades u) {
        return SaveManager.instance.upgradeLevels[(int)u];
    }

    public int GetUpgradesCount() { return upgradesCount; }


    private IEnumerator TurnOffWithDelay(GameObject g, float t) {
        yield return new WaitForSeconds(t);
        g.SetActive(false);
    }

    // Debug purposes
    private void ResetUpgrades() {
        for (int i = 0; i < GetUpgradesCount(); i++)
            SaveManager.instance.upgradeLevels[i] = 0;
    }
}
