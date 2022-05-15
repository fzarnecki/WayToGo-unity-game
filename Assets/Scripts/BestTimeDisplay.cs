using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BestTimeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private float noBestTimeValue = 999;

    [SerializeField] private Toggle settingsToggle;

    /***/

    private void Start() {
        if (text && GetBestTimeDisplayToggle() == 0) // adjusts the display in game scene; off if disabled else setting proper text and trigerring intro animation
            gameObject.SetActive(false);
        else if (text)
        {
            SetBestTimeText();
            StartCoroutine(TriggerAnimationWithDelay(2f, "In"));
        }
    }


    private IEnumerator TriggerAnimationWithDelay(float delay, string anim) {
        yield return new WaitForSeconds(delay);
        GetComponent<Animator>().SetTrigger(anim);
    }

    private void SetBestTimeText() {
        float bt = GetBestTime();
        string s;
        if (bt == noBestTimeValue)
            s = "Best time: -";
        else
            s = "Best time: " + bt.ToString("F2");

        text.text = s;
    }

    public void AdjustBestTime(float t) {
        float bt = GetBestTime();

        if (t < bt && t != 0)
        {
            PlayerPrefs.SetFloat(GetBestTimePrefsKey(), t);
            SetBestTimeText();
        }
        else
            return;
    }

    private string GetBestTimePrefsKey() {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        string prefsKey = "BestTime" + levelManager.GetMapSize().ToString() + levelManager.GetCurrentLevelNumber().ToString();

        return prefsKey;
    }

    private float GetBestTime() {
        return PlayerPrefs.GetFloat(GetBestTimePrefsKey(), noBestTimeValue);
    }


    private string displayPrefsKey = "DisplayBestTime";

    // called by a button in settings
    public void ToggleBestTimeDisplay() {
        bool change = settingsToggle.isOn;
        int current = (change == true ? 1 : 0);
        PlayerPrefs.SetInt(displayPrefsKey, current);
    }

    public void AdjustSettingsToggle() {
        StartCoroutine(AdjustSettingsToggleCo());
    }

    private IEnumerator AdjustSettingsToggleCo() {
        yield return new WaitUntil(() => settingsToggle.isActiveAndEnabled && gameObject.activeInHierarchy);

        if (GetBestTimeDisplayToggle() == 1)
            settingsToggle.isOn = true;
        else
            settingsToggle.isOn = false;

        yield return null;
    }

    private int GetBestTimeDisplayToggle() {
        return PlayerPrefs.GetInt(displayPrefsKey, 1);
    }


    // debugging
    private void ResetBestTime() {
        PlayerPrefs.SetFloat(GetBestTimePrefsKey(), noBestTimeValue);
    }
}
