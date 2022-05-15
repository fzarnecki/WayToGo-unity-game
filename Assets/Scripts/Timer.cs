using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private bool pause = false;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private UpgradesManager upgradesManager;
    [SerializeField] private SFXPlayer sfxPlayer;

    [SerializeField] private TextMeshProUGUI text;
    private float revealTime;
    private float searchTime;
    private float time = 0;

    private float timeOfSearching;

    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color urgentTextColor = Color.red;

    private bool urgentNotif = true;

    private bool skippedReveal = false; // used to prevent double sfx

    /***/

    private void Start() {
        SetRevealTime();
        SetSearchTime();
    }

    private void Update() {
        if (pause) return;

        if (gameManager.CountTime())
        {
            time -= Time.deltaTime;
            DisplayTime();
        }

        if (time <= 0 && gameManager.IsRevealPhase())
        {
            gameManager.SetRevealPhase(false);
            gameManager.SetCountTime(false);
            gameManager.BeginSearch(skippedReveal);
        }

        if (time <= 0 && gameManager.IsSearchPhase())
        {
            gameManager.SetSearchPhase(false);
            gameManager.SetCountTime(false);
            gameManager.AnimatedPathCheck();
        }

        if (time <= 1f && (gameManager.IsRevealPhase() || gameManager.IsSearchPhase()))
            gameManager.DisablePausePossibility();
    }

    public void StartRevealTimer() {
        gameManager.SetRevealPhase(true);
        SetTimer(GetRevealTime());
        gameManager.SetCountTime(true);
        gameManager.EnablePausePossibility();
    }

    public void StartSearchTimer() {
        gameManager.SetSearchPhase(true);
        SetTimer(GetSearchTime());
        gameManager.SetCountTime(true);
        gameManager.EnablePausePossibility();
    }

    private void DisplayTime() {
        AdjustTimerTextColor();
        int roundedTime = Mathf.RoundToInt(time);
        text.text = roundedTime.ToString();
    }

    private void SetTimer(float t) {
        time = t;
    }

    public void SkipRevealTimer() {
        time = 0.5f;
        skippedReveal = true;
    }

    private void AdjustTimerTextColor() {
        if (time <= 3.5f)
            text.color = urgentTextColor;
        else
            text.color = normalTextColor;

        // urgent sfx
        if (time <= 3.5f && time >= 1f && urgentNotif)
        {
            sfxPlayer.PlayLastSecondsSFX();
            StartCoroutine(UrgentNotifDelay());
        }
    }

    // pause variable is not currently used, the coroutine is canceled when turning off canvas object; then revived by UnpauseTimer
    public void PauseTimer() {
        if (pause) return;
        pause = true;
    }

    public void UnpauseTimer() {
        if (!pause) return;
        pause = false;
    }

    public bool IsPause() {
        return pause;
    }

    private IEnumerator UrgentNotifDelay() {
        if (!urgentNotif) yield break;

        urgentNotif = false;
        yield return new WaitForSeconds(1f);
        urgentNotif = true;
    }

    private float timeDisplayAdjust = 0.5f;

    private void SetSearchTime() { searchTime = gameManager.GetPathLength() + timeDisplayAdjust; }   // 1s of thinking for each cube in path
    private void SetRevealTime() { revealTime = Mathf.Ceil(gameManager.GetPathLength() / 2) + timeDisplayAdjust; }   // 0.5s of remembering for each cube in path
    
    public float GetSearchTime() { return searchTime + upgradesManager.GetAdditionalSearchTime(); }
    public float GetRevealTime() { return revealTime + upgradesManager.GetAdditionalMemoryTime(); }

    public void StopSearch() { timeOfSearching = GetSearchTime() - time - timeDisplayAdjust; }
    public float GetTimeOfSearch() { return timeOfSearching; }
}
