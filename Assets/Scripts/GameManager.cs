using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
    [Header("Game variables")]
    [SerializeField] private CoinsManager coinsManager;
    private bool gameLost = false;
    private bool gameWon = false;
    private bool countTime = true;
    private bool checking = false;

    private bool revealPhase = false;
    private bool searchPhase = false;

    [Header("Animations")]
    [SerializeField] private string buttonEntryAnimationName = "Entry";
    [SerializeField] private string buttonLeaveAnimationName = "Leave";

    [Header("Text displayed on top of the screen")]
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Animator infoTextAnimator;

    [SerializeField] private string startGameText = "Press below to play";
    [SerializeField] private string skipRevealText = "Press below when ready";
    [SerializeField] private string readyText = "Press below to check";

    [SerializeField] private string infoTextShowAnimName = "Show";
    [SerializeField] private string infoTextHideAnimName = "Hide";
    private bool infoTextDisplayed = false;

    [Header("Game objects")]
    [SerializeField] private GameObject gameView;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject skipRevealTimeButton;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject pauseButton;
    private List<Cube> path = new List<Cube>();           // Used to keep track of the path route
    private List<Cube> playerPath = new List<Cube>();   // Used to keep track of players chosen path

    [SerializeField] private LineRenderer pathLine;

    [Header("Game flow")]
    private string winGameEventString = "Win";
    private string loseGameEventString = "Lose";
    private string showAdString = "Ad";

    [Header("Scene fading and loading")]
    [SerializeField] private Fader fader;
    [SerializeField] private PersonalSceneLoader sceneLoader;
    private float fadeSpeed = 5f;

    [Header("Cubes")]
    [SerializeField] private Cube[] allCubes;
    private float timeBetweenPathReveals = 0.2f;
    private float timeBetweenPathChecks = 0.15f;
    private float timeAfterCheck = 1.5f;

    [Header("Music")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SFXPlayer sfxPlayer;

    [Header("Level timer")]
    [SerializeField] Timer timer;

    [Header("Ads & Tips")]
    [SerializeField] private AdManager adManager;
    [SerializeField] private TipManager tipManager;

    [Header("Upgrades")]
    [SerializeField] private UpgradesManager upgradesManager;

    [Header("Level management")]
    [SerializeField] private LevelManager levelManager;

    private int numberOfLevelsInEachMapSize = 100;

    /***/

    void Awake()
    {
        // Loading game data in level manager

        levelManager.SetMapSize((int)Mathf.Sqrt(allCubes.Length));

        SetGameLost(false);
        SetGameWon(false);
        SetCountTime(false);

        startGameButton.SetActive(false);
        skipRevealTimeButton.SetActive(false);
        readyButton.SetActive(false);
        pathLine.gameObject.SetActive(false);

        if (infoText)
            infoText.enabled = false;   // disabling not to flash when fading in canvas
    }

    private void Start() {
        float startGameButtonRevealDelay = 3f;
        StartCoroutine(TriggerButtonAnimationWithDelay(startGameButton, buttonEntryAnimationName, startGameButtonRevealDelay));
        StartCoroutine(ChangeTextInfoWithDelay(startGameText, infoTextShowAnimName, startGameButtonRevealDelay));
    }

    /***/

    public void StartGame() {
        RevealPath();
        // after revealed the reveal timer will be triggered
        // after timer reaches 0, hide path and start game timer
        // when path found before end of time, game won
        // when reaches 0, game lost
    }

    private void IndicateWin() {
        FindObjectOfType<BestTimeDisplay>().AdjustBestTime(timer.GetTimeOfSearch());
        sfxPlayer.PlayLevelWonSFX();
        GameEventMessage.SendEvent(winGameEventString);
        SetGameWon(true);
        SetCountTime(false);
        coinsManager.AddLevelReward();
        StartCoroutine(audioManager.ToneMusicDown(audioManager.GetPauseVolume(), audioManager.GetMusicFadeSpeed()));
        levelManager.UnlockNextLevel(levelManager.GetCurrentLevelNumber());    // Saving progress in player prefs
    }

    public void IndicateLoss() {
        SetGameLost(true);
        SetCountTime(false);
        FailedToShowAd();   //TODO maybe change name cause not always trying to show ad
    }
    
    // Called inside ad manager
    public void GiveRewardForAd() {
        tipManager.ActivateTip();
        Retry();
    }

    public void RejectedTip() {
        GameEventMessage.SendEvent(loseGameEventString);
        StartCoroutine(audioManager.ToneMusicDown(audioManager.GetPauseVolume(), audioManager.GetMusicFadeSpeed()));
    }

    public void FailedToShowAd() {
        GameEventMessage.SendEvent(loseGameEventString);
        StartCoroutine(audioManager.ToneMusicDown(audioManager.GetPauseVolume(), audioManager.GetMusicFadeSpeed()));
    }

    public void Retry() {
        StartCoroutine(fader.FadeOutAndReload());
    }

    public void Pause() {
        if (timer.gameObject.activeInHierarchy == true)
        {
            // Doozy handling canvas management
            timer.PauseTimer();
            StartCoroutine(audioManager.ToneMusicDown(audioManager.GetPauseVolume(), audioManager.GetMusicFadeSpeed()));
        }
    }

    public void Unpause() {
        // Doozy handling canvas management
        StartCoroutine(UnpauseTimer());
        StartCoroutine(audioManager.BringMusicUp(audioManager.GetMusicFadeSpeed()));
    }

    private IEnumerator UnpauseTimer()
    {
        yield return new WaitUntil(() => timer.gameObject.activeInHierarchy == true);
        timer.UnpauseTimer();
    }

    // Called by a button
    public void RevealPath() {
        sfxPlayer.PlayNextPhaseSFX();
        StartCoroutine(TriggerButtonAnimationWithDelay(startGameButton, buttonLeaveAnimationName, 0, true, 2));
        StartCoroutine(ChangeTextInfoWithDelay(startGameText, infoTextHideAnimName, 0));
        StartCoroutine(RevealPathCo());
        tipManager.DisableTipButton();
    }

    private IEnumerator RevealPathCo() {
        yield return new WaitForSeconds(2f);    // Little delay to make things smoother, considering outro anim of button
        yield return new WaitUntil(() => !timer.IsPause());

        sfxPlayer.PlayShowPathSFX();

        int index = 0;
        foreach (Cube c in path)
        {
            c.ChangeIntoPath();
            index++;
            yield return new WaitForSeconds(timeBetweenPathReveals);
        }
        
        StartCoroutine(TriggerButtonAnimationWithDelay(skipRevealTimeButton, buttonEntryAnimationName, 0)); // Skip reveal time button + text
        StartCoroutine(ChangeTextInfoWithDelay(skipRevealText, infoTextShowAnimName, 0));

        sfxPlayer.PlayShowPathSFX();

        SetupLine(GetPathLength());

        yield return new WaitUntil(()=> timer.gameObject.activeInHierarchy == true);   // Trigerring reveal timer (time to memorise path)
        yield return new WaitForSeconds(2f);                                // small delay to let button & text appear
        timer.StartRevealTimer();
    }

    // Button
    public void SkipRevealTime() {
        StartCoroutine(TriggerButtonAnimationWithDelay(skipRevealTimeButton, buttonLeaveAnimationName, 0, true, 2));
        StartCoroutine(ChangeTextInfoWithDelay(skipRevealText, infoTextHideAnimName, 0));
        StartCoroutine(ResetTimer());
    }

    private IEnumerator ResetTimer() {
        yield return new WaitUntil(() => IsRevealPhase()); // Delay to ensure it will be already set to new value, prevents bugging of timer
        if (timer.gameObject.activeInHierarchy == true)
            timer.SkipRevealTimer();
    }

    // Called by timer .. the bool is used to prevent double sfx when skipping reveal time
    public void BeginSearch(bool skippedReveal) {
        if (skipRevealTimeButton.activeInHierarchy && skipRevealTimeButton.GetComponent<Button>().interactable)
        {
            StartCoroutine(TriggerButtonAnimationWithDelay(skipRevealTimeButton, buttonLeaveAnimationName, 0, true, 2));
            StartCoroutine(ChangeTextInfoWithDelay(skipRevealText, infoTextHideAnimName, 0));
        }
        StartCoroutine(BeginSearchCo(skippedReveal));

        FadeLineOut(FindObjectOfType<LineRenderer>());
    }

    private IEnumerator BeginSearchCo(bool skippedReveal) {
        if (!skippedReveal)
            sfxPlayer.PlayNextPhaseSFX();

        foreach (Cube c in path)
        {
            c.ChangeIntoNormal();
            yield return 0;
        }
        
        yield return new WaitForSeconds(1f);    // Little delay to make things smoother, considering outro anim of button

        StartCoroutine(TriggerButtonAnimationWithDelay(readyButton, buttonEntryAnimationName, 0));  // Ready button + text
        StartCoroutine(ChangeTextInfoWithDelay(readyText, infoTextShowAnimName, 0));
        
        tipManager.HandleTip();    // Activating tip if player has watched an ad or bought it
        
        yield return new WaitUntil(() => timer.gameObject.activeInHierarchy == true);   // Trigerring time available to find path
        yield return new WaitForSeconds(1f);                                            // delay to smooth things out
        timer.StartSearchTimer();
    }

    // Called by timer or button
    public void AnimatedPathCheck() {
        StartCoroutine(AnimatedPathCheckCo());
    }

    private IEnumerator AnimatedPathCheckCo() {
        sfxPlayer.PlayNextPhaseSFX();

        if (IsSearchPhase())
            SetSearchPhase(false);  // if search phase is not indicated as finished changing it so to prevent bug
        
        if (Checking()) // Leaving if one check is already ongoing; else indicating check
            yield break;
        else
            SetCheck(true);

        if (timer.gameObject.activeInHierarchy == true)
        {
            timer.StopSearch(); // keeping track of time of searching
            timer.PauseTimer(); // pausing level time flow
        }
        
        if (pauseButton && pauseButton.GetComponent<Button>().interactable) // Disabling pause possibility
            DisablePausePossibility();
        
        if (readyButton.activeInHierarchy && readyButton.GetComponent<Button>().interactable) // Disabling button enabling to trigger this function
        {
            StartCoroutine(TriggerButtonAnimationWithDelay(readyButton, buttonLeaveAnimationName, 0, true, 2));
            StartCoroutine(ChangeTextInfoWithDelay(readyText, infoTextHideAnimName, 0));
        }

        if (tipManager.IsTipThisTurn()) // Fading out line once again only if it has been displayed as a tip
            FadeLineOut(pathLine);
        
        foreach (Cube c in playerPath) // Resetting colour of all cubes
            c.ChangeIntoNormal();
        
        bool correct = true;        // defines correctness of checked path
        
        yield return new WaitForSeconds(2); // Delay after color reset for smoothness

        // Checking each element of player path - must be in the same order of initial path
        // j is a helping variable in case a cube from tip has not been selected
        int j = 0;
        for (int i = 0; i < path.Count; i++) 
        {
            if (j >= playerPath.Count) break;

            if (path[i] != playerPath[j] && tipManager.TipContains(path[i]))
                continue;
            else
            {
                playerPath[j].ChangeIntoPath();
                yield return new WaitForSeconds(timeBetweenPathChecks);

                if (path[i] == playerPath[j]) // correct
                {
                    if (i == 0 || i == path.Count - 1)  // correct sfx at the beginning & end of path, analogically to reveal
                        sfxPlayer.PlayGoodTileChoiceSFX();

                    bool bonus;
                    if (i == 0)
                        bonus = false;  // no bonus on first tile - this prevents sfx's from overlapping with each other
                    else
                        bonus = coinsManager.TileMoneyChance(playerPath[j].transform); // Bonus possibility

                    playerPath[j].ChangeIntoCorrectPath(bonus);
                    correct = true;
                }
                else // wrong
                {
                    playerPath[j].ChangeIntoWrongPath();
                    correct = false;
                }

                if (!correct) // Breaking as soon as there was a mistake
                    break;

                // yield return new WaitForSeconds(timeBetweenPathReveals / 2); // delay for smoothness

                j++; // player path moves by 1
            }
        }

        if (correct) {
            foreach(Cube c in playerPath)   // Too much might have been chosen
            {
                if (!path.Contains(c))
                {
                    c.ChangeIntoWrongPath();
                    correct = false;
                }
            }
        }

        if (correct) {
            foreach (Cube c in path) // Checking if all cubes have been selected (accepted if the cube was a tip)
            {
                if (!playerPath.Contains(c) && !tipManager.TipContains(c))
                {
                    c.ChangeIntoWrongPath();
                    correct = false;
                }
                else if (tipManager.TipContains(c) && !c.IsMarkedCorrect())
                    c.ChangeIntoCorrectPath(false); // no bonus if tip
            }
        }
        
        yield return new WaitForSeconds(timeAfterCheck); // 1s for smoothness

        // Defining end state of the game
        if (correct)
            IndicateWin();
        else
            IndicateLoss();
    }


    public Cube GetPathAt(int index) {
        if (index < path.Count)
            return path[index];
        else
            return path[0];
    }

    // Used to draw a path on top of the cubes to show exactly the way it goes
    public void SetupLine(int count) {
        pathLine.gameObject.SetActive(true);
        Vector3[] positions = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = path[i].transform.position + new Vector3(0, 2f, -0.5f);
        }
        pathLine.positionCount = count;
        pathLine.SetPositions(positions);
        FadeLineIn(pathLine);
    }
    
    private void FadeLineIn(LineRenderer line) { line.GetComponent<Animator>().SetTrigger("In"); }
    private void FadeLineOut(LineRenderer line) { line.GetComponent<Animator>().SetTrigger("Out"); }

    public void SetPath(List<Cube> p) { path = p; }

    public void ResetPath() { path.Clear(); }

    public Cube[] GetAllCubes() { return allCubes; }
    
    private Cube GetCubeAt(int i, int j) {
        return allCubes[i * levelManager.GetMapSize() + j];
    }

    public int GetPathLength() { return path.Count; }

    public bool IsRevealPhase() { return revealPhase; }
    public bool IsSearchPhase() { return searchPhase; }

    public void SetRevealPhase(bool b) { revealPhase = b; }
    public void SetSearchPhase(bool b) { searchPhase = b; }

    private bool Checking() { return checking; }
    private void SetCheck(bool b) { checking = b; }

    public bool IsGameView() { return gameView.activeInHierarchy; }

    public void SetGameLost(bool b) { gameLost = b; }

    public void SetGameWon(bool b) { gameWon = b; }

    public bool IsGameLost() { return gameLost; }

    public bool IsGameWon() { return gameWon; }

    public void SetCountTime(bool b) { countTime = b; }

    public bool CountTime() { return countTime; }

    public void EnablePausePossibility() { pauseButton.GetComponent<Button>().interactable = true; }
    public void DisablePausePossibility() { pauseButton.GetComponent<Button>().interactable = false; }

    
    private IEnumerator TriggerButtonAnimationWithDelay(GameObject button, string animation, float delay, bool disable = false, float disableDelay = 2) {
        yield return new WaitForSeconds(delay);
        
        if (!button.activeInHierarchy)
            button.SetActive(true);
        
        Animator animator = button.GetComponent<Animator>();
        animator.SetTrigger(animation);

        // Disabling interaction if the button is to be disabled to prevent overclicking & Deactivating after animation (delay) if desired
        if (disable)
        {
            button.GetComponent<Button>().interactable = false;
            yield return new WaitForSeconds(disableDelay);
            button.SetActive(false);
        }
    }

    private IEnumerator ChangeTextInfoWithDelay(string text, string animation, float delay) {
        yield return new WaitForSeconds(delay);
        
        if (!infoText.enabled)
            infoText.enabled = true;
        
        if (animation == infoTextShowAnimName)  // If showing, need to adjust text
            infoText.text = text;
        
        infoTextAnimator.SetTrigger(animation);
    }

    public int GetNumberOfLevelsInEachMapSize() { return numberOfLevelsInEachMapSize; }


    // EDITOR

    public void AddCubeToPath(Cube c) {
        if (!path.Contains(c))
            path.Add(c);
    }

    public void RemoveCubeFromPath(Cube c) {
        if (path.Contains(c))
            path.Remove(c);
    }

    public void AddCubeToPlayerPath(Cube c) {
        if (!playerPath.Contains(c))
            playerPath.Add(c);
    }

    public void RemoveCubeFromPlayerPath(Cube c) {
        if (playerPath.Contains(c))
            playerPath.Remove(c);
    }

    public void ShowLevelPath() {
        foreach (Cube c in path)
        {
            c.ChangeIntoPathQuickly();
        }
    }

    public void HideLevelPath() {
        foreach (Cube c in path)
        {
            c.ResetMaterialAndColor();
        }
    }

    public void ResetAllCubesColor() {
        foreach (Cube c in allCubes)
        {
            c.ResetMaterialAndColor();
        }
    }
}
