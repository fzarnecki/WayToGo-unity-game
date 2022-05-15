using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [SerializeField] private AudioClip chooseTileSFX;
    [SerializeField] private AudioClip goodTileChoiceSFX;
    [SerializeField] private AudioClip wrongTileChoiceSFX;
    [SerializeField] private AudioClip levelWonSFX;
    [SerializeField] private AudioClip nextPhaseSFX;
    [SerializeField] private AudioClip showPathSFX;
    [SerializeField] private AudioClip uiClickSFX;
    [SerializeField] private AudioClip levelChosenSFX;
    [SerializeField] private AudioClip lastSecondsSFX;
    [SerializeField] private AudioClip coinSFX;
    [SerializeField] private AudioClip purchaseSFX;

    private bool canPlayNextPhaseSFX = true;
    private bool canPlayWrongTileSFX = true;
    private float nextPhaseSFXDelay = 1f;
    private float wrongTileSFXDelay = 2f;

    /***/

    public void PlayChooseTileSFX() {
        audioSource.PlayOneShot(chooseTileSFX);
    }

    public void PlayGoodTileChoiceSFX() {
        audioSource.PlayOneShot(goodTileChoiceSFX);
    }

    public void PlayWrongTileChoiceSFX() {
        audioSource.PlayOneShot(wrongTileChoiceSFX);
    }

    public void PlayLevelWonSFX() {
        // audioSource.PlayOneShot(levelWonSFX);
        StartCoroutine(TestLevelWonSFX());
    }

    public void PlayNextPhaseSFX() {
        if (!canPlayNextPhaseSFX) return;
        
        audioSource.PlayOneShot(nextPhaseSFX);
        StartCoroutine(ResetNextPhaseSFX());
    }

    public void PlayShowPathSFX() {
        audioSource.PlayOneShot(showPathSFX);
    }

    public void PlayUIClickSFX() {
        audioSource.PlayOneShot(uiClickSFX);
    }
    
    public void PlayLevelChosenSFX() {
        audioSource.PlayOneShot(levelChosenSFX);
    }

    public void PlayLastSecondsSFX() {
        audioSource.PlayOneShot(lastSecondsSFX);
    }

    public void PlayCoinSFX() {
        audioSource.PlayOneShot(coinSFX);
    }

    public void PlayPurchaseSFX() {
        audioSource.PlayOneShot(purchaseSFX);
    }

    public IEnumerator TestLevelWonSFX() {
        for (int i = 0; i <= 4; i++)
        {
            yield return new WaitForSeconds(0.15f);
            audioSource.PlayOneShot(goodTileChoiceSFX);
        }
    }

    private IEnumerator ResetNextPhaseSFX() {
        canPlayNextPhaseSFX = false;
        yield return new WaitForSeconds(nextPhaseSFXDelay);
        canPlayNextPhaseSFX = true;
    }

    private IEnumerator ResetWrongTileSFX() {
        canPlayWrongTileSFX = false;
        yield return new WaitForSeconds(wrongTileSFXDelay);
        canPlayWrongTileSFX = true;
    }
}
