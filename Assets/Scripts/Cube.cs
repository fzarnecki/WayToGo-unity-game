using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cube : MonoBehaviour
{
    private bool active = true;
    private bool entryAnimation = true;

    // Tracking player choice
    private bool isChosenAsPath = false;
    
    // Tracking if its part of actual level path
    private bool isPath = false;

    [Header("Serialised")]
    [SerializeField] public GameManager gameManager;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Animator animator;
    private SFXPlayer sfxPlayer;

    [Header("Colors")]
    [SerializeField] private Material cubeMaterial;
    [SerializeField] private Color regularColor;
    [SerializeField] private Color pathColor = new Color(0, 210, 255, 255);
    [SerializeField] private Color correctPathColor;
    [SerializeField] private Color wrongPathColor;
    private float colorTransitionSpeed = 2f;
    
    [Header("Movement")]
    private float randomMovementSeed;
    [SerializeField] private float moveKick = 1.5f;
    [SerializeField] private float afterTouchKick = 3.5f;
    [SerializeField] private float moveKickDownforce = 2f;

    // Delay after clicking to prevent immediate change back
    private float afterClickDelay = 0.6f;
    private bool isDelay = false;

    /***/

    void Awake()
    {
        Activate();
        entryAnimation = true;
        ResetMaterialAndColor();

        randomMovementSeed = Random.value;  // Seed for random y slight movement

        sfxPlayer = FindObjectOfType<SFXPlayer>();
    }
    
    private void Update() {
        
        transform.position = new Vector3(transform.position.x, Mathf.PerlinNoise(randomMovementSeed, Time.time) * moveKick, transform.position.z);

        if (moveKick > 1f)
            moveKick -= Time.deltaTime * moveKickDownforce;
        else if (moveKick < 0.95f)
            moveKick += Time.deltaTime * moveKickDownforce;
    }
    

    public void HandleCubeClick()
    {
        if (active == false || !gameManager.IsGameView() || entryAnimation || !gameManager.IsSearchPhase() || isDelay) return;
        
        sfxPlayer.PlayChooseTileSFX();  // Click SFX
        
        StartCoroutine(AfterClickDelay());  // Triggering delay to prevent too rapid next click
        
        AddAfterTouchKick();    // Additonal knockdown for random movement
        
        isChosenAsPath = !isChosenAsPath;   // Negating state of the cube
        
        if (isChosenAsPath) // Carrying out actions according to state
        {
            ChangeIntoPath();   // changes cube visually
            gameManager.AddCubeToPlayerPath(this);  // adds cube to players choice list
        }
        else
        {
            ChangeIntoNormal();     // changes path visually
            gameManager.RemoveCubeFromPlayerPath(this);     // removes cube from players choice list
        }
    }

    private void AddAfterTouchKick() {
        moveKick = afterTouchKick;
    }

    private void AddAfterTouchKickNegative() {
        moveKick = -0.5f;
    }

    private IEnumerator AfterClickDelay() {
        if (isDelay)
            yield break;

        isDelay = true;
        yield return new WaitForSeconds(afterClickDelay);
        isDelay = false;
    }

    public void ChangeIntoNormal() {
        StartCoroutine(LerpBetweenColors(pathColor, regularColor));
    }

    public void ChangeIntoPath() {
        StartCoroutine(LerpBetweenColors(regularColor, pathColor));
    }

    public void ChangeIntoCorrectPath(bool bonus) {
        if (bonus)
            sfxPlayer.PlayCoinSFX();
        StartCoroutine(LerpBetweenColors(regularColor, correctPathColor));
        AddAfterTouchKick();
    }

    public void ChangeIntoWrongPath() {
        sfxPlayer.PlayWrongTileChoiceSFX();
        StartCoroutine(LerpBetweenColors(regularColor, wrongPathColor));
        AddAfterTouchKickNegative();
    }

    private IEnumerator LerpBetweenColors(Color start, Color end) {
        float startTime = Time.time;
        while (renderer.material.color != end)
        {
            renderer.material.color = Color.Lerp(start, end, (Time.time - startTime) * colorTransitionSpeed);
            yield return 0;
        }
    }

    private void ChangeColorQuickly(Color c) {
        renderer.material.color = c;
    }

    public void ChangeIntoPathQuickly() {
        ChangeColorQuickly(pathColor);
    }

    public void ResetMaterialAndColor() {
        renderer.material = cubeMaterial;
        ChangeColorQuickly(regularColor);
    }

    public bool IsPath() {
        return isPath;
    }

    public bool Active()
    {
        return active;
    }
    private void Activate()
    {
        active = true;
    }
    private void Deactivate()
    {
        active = false;
    }

    public void FinishedEntryAnimation()
    {
        entryAnimation = false;
    }

    public bool IsMarkedCorrect() {
        if (renderer.material.color == correctPathColor)
            return true;
        else
            return false;
    }



    // UNUSED
    // OLD EDITOR METHODS

    // Adds current cube to level path and shows change in editor (by changing color)
    /*public void AddThisToPath() {
        gameManager.AddCubeToPath(this);
        ChangeColorQuickly(pathColor);
        isPath = true;
    }

    // Analogical to above, but reversed
    public void RemoveThisFromPath() {
        gameManager.RemoveCubeFromPath(this);
        ChangeColorQuickly(regularColor);
        isPath = false;
    }*/
}
