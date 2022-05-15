// Script for the purpose of tag screen displayed at the beginning

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TagSceneFader : MonoBehaviour
{
    [Tooltip("Canvas with the tag")]
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject tag;

    private float fadeSpeed = 0.7f;
    private float zoom = 0.00012f;

    /***/

    void Start()
    {
        StartCoroutine(FadeUnfadeCo());
    }

    private IEnumerator FadeUnfadeCo()
    {
        // Make invisible
        canvas.alpha = 0;

        // Fade in
        while (canvas.alpha < 1)
        {
            canvas.alpha += fadeSpeed * Time.deltaTime;
            ScaleUp();
            yield return new WaitForEndOfFrame();
        }

        // Delay
        // yield return new WaitForSeconds(0.5f);
        float t = 0;
        while (t < 0.5f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            ScaleUp();
            t += Time.deltaTime;
        }

        // Fade out
        while (canvas.alpha > 0)
        {
            canvas.alpha -= fadeSpeed * 1.5f * Time.deltaTime;
            ScaleUp();
            yield return new WaitForEndOfFrame();
        }

        // Load game(next) scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ScaleUp() {
        var scale = tag.transform.localScale;
        tag.transform.localScale = new Vector3(scale.x + zoom, scale.y + zoom, scale.z + zoom);
    }
}
