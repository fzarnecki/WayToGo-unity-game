using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;

public class MenuStartButton : MonoBehaviour
{
    [SerializeField] Doozy.Engine.UI.UIButton levelChoiceButton;

    private float randomSeed;

    private Quaternion targetRotation;
    [SerializeField] private float rotation = 40f;
    [SerializeField] private float perlinYMultiplier = 0.4f;
    [SerializeField] private float angleBalancer = 20;  // makes it so that when substracted from perlin noise result makes it possible to have negative values of angles
    private float touchAddRotation = 2f;
    private float rotationDownforce = 2f;

    private float moveKick = 1.5f;
    private float afterTouchKick = 3.5f;
    private float moveKickDownforce = 2f;

    /***/

    private void Start() {
        randomSeed = Random.value;
    }

    void Update()
    {
        targetRotation = Quaternion.Euler(  Mathf.PerlinNoise(randomSeed + 0, Time.time * perlinYMultiplier) * rotation - angleBalancer,
                                            Mathf.PerlinNoise(randomSeed + 1, Time.time * perlinYMultiplier) * rotation - angleBalancer,
                                            Mathf.PerlinNoise(randomSeed + 2, Time.time * perlinYMultiplier) * rotation - angleBalancer);
        transform.rotation = targetRotation;
    }


    public void HandleClick() {
        if (!levelChoiceButton.IsActive()) return;

        // Calling those methods here, because the UI button won't be registered as clicked, only call will be sent to doozy
        FindObjectOfType<SFXPlayer>().PlayNextPhaseSFX();
        FindObjectOfType<LevelManager>().PrepareLevelChoice();
        levelChoiceButton.ExecuteClick();
    }
}
