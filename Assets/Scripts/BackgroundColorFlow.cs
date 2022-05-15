using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorFlow : MonoBehaviour
{
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    [SerializeField] private Material backgroundMaterial;

    private float colorTransitionSpeed = 0.20f;
    private float colorViewDelay = 1.5f;

    private Color targetColor;

    private bool change = true;

    /***/

    void Start()
    {
        backgroundMaterial.color = color1;
    }
    
    void Update()
    {
        if (change)
        {
            change = false;

            if (backgroundMaterial.color == color1)
                targetColor = color2;
            else
                targetColor = color1;
            
            StartCoroutine(LerpBetweenColors(backgroundMaterial.color, targetColor));
        }
    }

    private IEnumerator LerpBetweenColors(Color start, Color end) {
        float startTime = Time.time;
        while (backgroundMaterial.color != end)
        {
            backgroundMaterial.color = Color.Lerp(start, end, (Time.time - startTime) * colorTransitionSpeed);
            yield return 0;
        }

        // little delay to view the full color
        yield return new WaitForSeconds(colorViewDelay);

        // enabling next change
        change = true;
    }
}
