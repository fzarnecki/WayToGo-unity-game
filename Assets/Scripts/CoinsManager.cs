using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    private int levelReward;
    private int passedLevelDivider = 10; // determines how many times smaller the reward will be if lvl has already been passed

    private int bonusReward = 0;
    private int bonusMin = 1;
    private int bonusMax = 10;
    private float bonusProb = 0.1f;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject coinDespawnParticlesPrefab;
    private float destroyDelay = 2f;
    private float animationDespawnDelay = 1.5f;
    private float coinHeightChange = 3.5f;
    private float particlesDespawnDelay = 1.5f;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private UpgradesManager upgradesManager;

    [SerializeField] private TextMeshProUGUI earnedCoinsText;

    /***/

    private void Start() {
        UpdateCoinsDisplays();

        if (IsGameScene())
            SetLevelReward();
    }


    private bool IsGameScene() {
        if (gameManager)
            return true;
        else
            return false;
    }

    public void SetLevelReward() {
        int size = levelManager.GetMapSize();
        int pathLength = gameManager.GetPathLength();

        int reward = size * pathLength + size * size;
        if (levelManager.GetMaxUnlockedLevel() > levelManager.GetCurrentLevelNumber())   // If level already passed, making the reward smaller
            levelReward = Mathf.FloorToInt(reward / passedLevelDivider);
        else
            levelReward = reward;

        levelReward += upgradesManager.GetAdditionalLevelMoney();
    }

    public int GetLevelReward() {
        return levelReward;
    }

    public void AddLevelReward() {
        StartCoroutine(RewardAnimation(GetPlayerCoins() + GetLevelReward()));        
    }

    private IEnumerator RewardAnimation(int newCoins) {
        yield return new WaitForSeconds(0.1f);
        UpdateCoinsDisplays();
        DisplayEarnedCoins();
        yield return new WaitForSeconds(1f);
        var displays = FindObjectsOfType<DisplayCoins>();
        while (GetPlayerCoins() < newCoins)
        {
            AddCoins(1);
            UpdateCoinsDisplays(displays);
            yield return 0;
        }
        SaveManager.instance.SaveGame();
    }

    public bool TileMoneyChance(Transform t) {
        if (Random.Range(0f,1f) < GetBonusProb())
        {
            AddTileMoney(t);
            return true;
        }
        else
            return false;
    }

    public void AddTileMoney(Transform t) {
        AddCoins(Random.Range(bonusMin, bonusMax));
        
        SpawnCoin(t);
        StartCoroutine(PlayCoinDespawnParticles(t));
        // sfx in Cube script

        UpdateCoinsDisplays();
    }

    private void UpdateCoinsDisplays(DisplayCoins[] displays) {
        foreach (DisplayCoins d in displays)
            d.UpdateCoins();
    }

    private void UpdateCoinsDisplays() {
        var displays = FindObjectsOfType<DisplayCoins>();
        foreach (DisplayCoins d in displays)
            d.UpdateCoins();
    }

    private void DisplayEarnedCoins() {
        earnedCoinsText.text = GetLevelReward().ToString();
    }

    private void AddCoins(int c) {
        SaveManager.instance.playerCoins += c;
        SaveManager.instance.SaveGame();
    }

    public void SpendCoins(int c) {
        SaveManager.instance.playerCoins -= c;
        SaveManager.instance.SaveGame();
        UpdateCoinsDisplays();
    }

    private void SpawnCoin(Transform t) {
        GameObject newCoin = Instantiate(coinPrefab, t.position, Quaternion.identity);
        Destroy(newCoin, destroyDelay);
    }

    private IEnumerator PlayCoinDespawnParticles(Transform trans) {
        yield return new WaitForSeconds(animationDespawnDelay);
        GameObject newParticles = Instantiate(coinDespawnParticlesPrefab, trans.position + new Vector3(0, coinHeightChange, 0), Quaternion.identity);
        Destroy(newParticles, particlesDespawnDelay);
    }


    public float GetBonusProb() { return bonusProb + upgradesManager.GetAdditionalTileMoneyChance(); }

    public int GetPlayerCoins() { return SaveManager.instance.playerCoins; }
}
