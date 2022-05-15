using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] viewsCanvasGroups;

    [SerializeField] private Image image;
    [SerializeField] private Animator animator;
    [SerializeField] private PersonalSceneLoader sceneLoader;
    [SerializeField] private AudioManager audioManager;

    public static float fadeSpeed = 1.5f;

    /***/

    void Start() {
        StartCoroutine(FadeSceneIn());
    }
    

    private IEnumerator FadeSceneIn()
    {
        image.transform.gameObject.SetActive(true);
        animator.SetTrigger("FadeIn");
        yield return new WaitUntil(() => image.color.a == 0);
    }

    public IEnumerator FadeOutAndLoad(int targetScene) {
        yield return new WaitForSeconds(0.1f);  // Little wait to prevent game view from causing bugs

        // Fading out all views and waiting for its end
        FadeViewsOut();
        yield return new WaitUntil(() => AreViewsHidden());

        if(audioManager)
            StartCoroutine(audioManager.FadeMusicOut());
        animator.SetTrigger("FadeOut");
        yield return new WaitUntil(() => image.color.a == 1);
        if (audioManager)
            yield return new WaitUntil(() => audioManager.FadedOut());
        sceneLoader.LoadScene(targetScene);
    }

    public IEnumerator FadeOutAndLoad(string targetScene) {
        // Fading out all views and waiting for its end
        FadeViewsOut();
        yield return new WaitUntil(() => AreViewsHidden());

        if (audioManager)
            StartCoroutine(audioManager.FadeMusicOut());
        animator.SetTrigger("FadeOut");
        yield return new WaitUntil(() => image.color.a == 1);
        if (audioManager)
            yield return new WaitUntil(() => audioManager.FadedOut());
        sceneLoader.LoadScene(targetScene);
    }

    public IEnumerator FadeOutAndReload()
    {
        // Fading out all views and waiting for its end
        FadeViewsOut();
        yield return new WaitUntil(() => AreViewsHidden());

        if (audioManager)
            StartCoroutine(audioManager.FadeMusicOut());
        animator.SetTrigger("FadeOut");
        yield return new WaitUntil(() => image.color.a == 1);
        yield return new WaitUntil(() => audioManager.FadedOut());
        sceneLoader.ReloadScene();
    }

    private void FadeViewsOut() {
        foreach (CanvasGroup g in viewsCanvasGroups)
            if (g.gameObject.activeInHierarchy)
                StartCoroutine(FadeViewOutCo(g));
    }

    private IEnumerator FadeViewOutCo(CanvasGroup g) {
        while (g.alpha > 0)
        {
            g.alpha -= fadeSpeed * Time.deltaTime;
            yield return 0;
        }
    }

    private bool AreViewsHidden() {
        bool hidden = true;
        foreach (CanvasGroup g in viewsCanvasGroups)
            if (g.gameObject.activeInHierarchy && g.alpha > 0.05f)
                hidden = false;

        return hidden;
    }
}
