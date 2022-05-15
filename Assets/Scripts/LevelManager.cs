using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UpgradesManager upgradesManager;
    [SerializeField] private AdManager adManager;
    [SerializeField] private PersonalSceneLoader personalSceneLoader;
    private int mapSize;
    private int currentLevel;

    [Header("Menu")]
    [SerializeField] private GameObject levelGrid;
    [SerializeField] private TextMeshProUGUI mapSizeDisplay;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Fader fader;
    private int regularFontSize = 120;
    private int tripleDigitFontSize = 100;

    private int currentGridMapSize;

    private int minMapSize = 5;
    private int maxMapSize = 8;

    /***/

    private void Awake() {
        if (!IsMenu())
            SetCurrentLevelNumber();
    }

    private void Start() {
        // Setting map size in game manager
        ResetCurrentGridMapSize();

        if (!SaveManager.instance.LoadGame())
            SaveManager.instance.InitializeSaveData(GetDistinctMapSizesCount(), upgradesManager.GetUpgradesCount());
        
        if (!IsMenu())
           GenerateAndSetPath();
    }


    private bool IsMenu() {
        if (mapSizeDisplay) // if this is provided it will be a menu scene
            return true;
        else
            return false;
    }


    #region Menu

    public void PrepareLevelChoice() {
        StartCoroutine(PrepareLevelChoiceCo());
    }

    private IEnumerator PrepareLevelChoiceCo() {
        // Small delay to ensure that view is active
        yield return new WaitForSeconds(0.1f);
        
        PrepareLevelButtons();
        AdjustScrollRectContent();
        UpdateMapSizeDisplay();
    }
    
    private void PrepareLevelButtons() {
        Button[] levelButtons = levelGrid.GetComponentsInChildren<Button>();
        int unlockedLevels = GetMaxUnlockedLevel(GetCurrentGridIndex());
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < unlockedLevels)
                levelButtons[i].interactable = true;
            else
                levelButtons[i].interactable = false;

            levelButtons[i].GetComponent<CanvasGroup>().alpha = 0;
            levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();

            if (i + 1 >= 100)   // decreasing size of font if level number is triple digit
                levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().fontSize = tripleDigitFontSize;
        }

        StartCoroutine(TriggerLevelButtonsAnimation(levelButtons));
    }

    private IEnumerator TriggerLevelButtonsAnimation(Button[] levelButtons) {
        foreach (Button b in levelButtons)
        {
            b.GetComponent<Animator>().SetTrigger("In");
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void UnlockNextLevel(int levelNumber) {
        if (levelNumber >= GetMaxUnlockedLevel())
        {
            SaveManager.instance.unlockedLevels[GetCurrentMapSizeIndex()] = levelNumber + 1;
            SaveManager.instance.SaveGame();
        }
    }

    public int GetMaxUnlockedLevel(int currentGridIndex) {
        return SaveManager.instance.unlockedLevels[currentGridIndex];
    }

    public int GetMaxUnlockedLevel() {
        return SaveManager.instance.unlockedLevels[GetCurrentMapSizeIndex()];
    }

    // Called by level buttons
    public void LoadLevel() {
        int lvl = int.Parse(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text);  // getting chosen level number
        CrossSceneInfo.LevelToLoad = lvl;   // saving the level chosen in cross scene static class to later access it and generate proper path
        string name = GetGridMapSizeName();    // getting grid info to load proper scene
        FindObjectOfType<SFXPlayer>().PlayLevelChosenSFX();
        StartCoroutine(fader.FadeOutAndLoad(name));
    }
    
    public void LoadLevel(int lvl) {
        CrossSceneInfo.LevelToLoad = lvl;
        string name = GetGameSceneName();
        FindObjectOfType<SFXPlayer>().PlayLevelChosenSFX();
        StartCoroutine(fader.FadeOutAndLoad(name));
    }

    // used in game scenes
    private string GetGameSceneName() {
        return (GetMapSize() + "x" + GetMapSize());
    }

    // Called by a button
    public void LargerMap() {
        if (GetCurrentGridMapSize() + 1 <= maxMapSize)
        {
            IncreaseCurrentGridMapSize();
            PrepareLevelButtons();
            AdjustScrollRectContent();
            UpdateMapSizeDisplay();
        }
        else
            return;
    }

    // Called by a button
    public void SmallerMap() {
        if (GetCurrentGridMapSize() - 1 >= 0)
        {
            DecreaseCurrentGridMapSize();
            PrepareLevelButtons();
            AdjustScrollRectContent();
            UpdateMapSizeDisplay();
        }
        else
            return;
    }

    private void AdjustScrollRectContent() {
        RectTransform rect = levelGrid.GetComponent<RectTransform>();
        scrollRect.content = rect;              // switching grid with level buttons to the current one
        rect.anchoredPosition = Vector3.zero;   // scrolling it back to top in case it is bugged
    }

    private void UpdateMapSizeDisplay() {
        mapSizeDisplay.text = GetGridMapSizeName();
    }

    private string GetGridMapSizeName() {
        int i = GetCurrentGridMapSize();
        string s = i + "x" + i;
        return s;
    }

    private int GetCurrentGridIndex() {
        return currentGridMapSize - minMapSize;
    }

    private int GetCurrentGridMapSize() { return currentGridMapSize; }
    private void IncreaseCurrentGridMapSize() { currentGridMapSize += 1; currentGridMapSize = Mathf.Clamp(currentGridMapSize, minMapSize, maxMapSize); }
    private void DecreaseCurrentGridMapSize() { currentGridMapSize -= 1; currentGridMapSize = Mathf.Clamp(currentGridMapSize, minMapSize, maxMapSize); }
    private void ResetCurrentGridMapSize() { currentGridMapSize = minMapSize; }

    public void SetMapSize(int size) { mapSize = size; }
    public int GetMapSize() { return mapSize; }

    #endregion


    #region In-Game

    public int GetCurrentLevelNumber() {
        return currentLevel;
    }

    private void SetCurrentLevelNumber() {
        currentLevel = CrossSceneInfo.LevelToLoad;
    }

    // Called by a 'continue' button
    public void ShowAdAndLoadNextLevel() {
        adManager.ShowInterstitialAd();
    }
    
    // called in ad manager
    public void LoadNextLevel() {
        if (GetCurrentLevelNumber() >= gameManager.GetNumberOfLevelsInEachMapSize())
        {
            LoadMainMenu();
            return;
        }

        LoadLevel(GetCurrentLevelNumber() + 1);
    }

    public void LoadMainMenu() {
        StartCoroutine(fader.FadeOutAndLoad(1));
    }

    public int GetDistinctMapSizesCount() { return maxMapSize - minMapSize + 1; }

    public int MinMapSize() { return minMapSize; }
    public int MaxMapSize() { return maxMapSize; }

    private int GetCurrentMapSizeIndex() { return mapSize - minMapSize; }


    public bool GenerateAndSetPath() {
        try
        {
            gameManager.ResetPath();
            int size = GetMapSize();
            int minLength = size + 2;
            int maxLength = size > 6 ? (size * size - size / 2) : (size * size);    // size^2 if map is 5v5; if bigger making the path shorter to make it possible to generate
            List<Cube> path = MapGenerator.GeneratePathForLevel(size, minLength, maxLength, GetCurrentLevelNumber(), gameManager.GetAllCubes());
            gameManager.SetPath(path);
            return true;
        }
        catch
        {
            // Debug.LogErrorFormat("Unable to generate path.");
            return false;
        }
    }

    #endregion


    // DEBUG AND EDITOR

    public void ResetLevelsUnlocked() {
        for (int i = 0; i < GetDistinctMapSizesCount(); i++)
            SaveManager.instance.unlockedLevels[i] = 1;
    }

    public void IncreaseLevelsUnlocked() {
        SaveManager.instance.unlockedLevels[GetCurrentGridIndex()]++;
    }

    public void DecreaseLevelsUnlocked() {
        if (SaveManager.instance.unlockedLevels[GetCurrentGridIndex()] == 1)
            return;
        else
            SaveManager.instance.unlockedLevels[GetCurrentGridIndex()]--;
    }

    
    public void UnlockAllLevels() {
        SaveManager.instance.unlockedLevels[GetCurrentGridIndex()] = 100;
    }
}
