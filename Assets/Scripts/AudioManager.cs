using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private float pauseVolume;
    private float pauseVolumeDecrease = 0.6f;
    private float minPauseVolume = 0.15f;
    private float musicFadeSpeed = 0.03f;

    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource soundAudioSource;
    private float musicVolume = .7f;
    private float soundVolume = .7f;
    [SerializeField] private float fadeInSpeed = .2f;
    [SerializeField] private float fadeOutSpeed = .8f;

    private bool fadedOut = true;

    private string playerPrefsMusicVolume = "musicVolume";
    private string playerPrefsSoundVolume = "soundVolume";

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;

    /***/

    void Start()
    {
        UpdateVolumes();
        UpdatePauseVolume();
        UpdateVolumeSliders();
        
        if (musicAudioSource)   // fading music in slowly
        {
            musicAudioSource.volume = 0;
            StartCoroutine(FadeMusicIn());
        }
    } 


    private IEnumerator FadeMusicIn()
    {
        while (musicAudioSource.volume < musicVolume)
        {
            musicAudioSource.volume += fadeInSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FadeMusicOut()
    {
        SetFadedOut(false);
        while (musicAudioSource.volume > 0.01f)
        {
            musicAudioSource.volume -= fadeOutSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SetFadedOut(true);
    }
    
    private void SetFadedOut(bool b) { fadedOut = b; }

    public bool FadedOut() { return fadedOut; }
    
    public IEnumerator ToneMusicDown(float vol, float toneSpeed)
    {
        while (musicAudioSource.volume > vol)
        {
            musicAudioSource.volume -= toneSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator BringMusicUp(float toneSpeed)
    {
        while (musicAudioSource.volume < musicVolume)
        {
            musicAudioSource.volume += toneSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void SetMusicVolume(float vol) {
        musicVolume = vol;
        musicAudioSource.volume = musicVolume;
        PlayerPrefs.SetFloat(playerPrefsMusicVolume, musicVolume);

        UpdatePauseVolume();
    }

    public void SetSoundVolume(float vol) {
        soundVolume = vol;
        soundAudioSource.volume = soundVolume;
        PlayerPrefs.SetFloat(playerPrefsSoundVolume, soundVolume);
    }

    private void UpdateVolumes() {
        musicVolume = PlayerPrefs.GetFloat(playerPrefsMusicVolume, 1);
        soundVolume = PlayerPrefs.GetFloat(playerPrefsSoundVolume, 1);
        soundAudioSource.volume = soundVolume;
    }

    private void UpdatePauseVolume() {
        if (musicVolume < minPauseVolume)
            pauseVolume = 0;
        else
        {
            pauseVolume = musicVolume - pauseVolumeDecrease;
            pauseVolume = Mathf.Clamp(pauseVolume, minPauseVolume, musicVolume);
        }
    }

    public void UpdateVolumeSliders() {
        if (musicVolumeSlider && musicVolumeSlider.IsActive())
            musicVolumeSlider.value = musicVolume;

        if (soundVolumeSlider && soundVolumeSlider.IsActive())
            soundVolumeSlider.value = soundVolume;
    }

    public float GetPauseVolume() { return pauseVolume; }

    public float GetMusicFadeSpeed() { return musicFadeSpeed; }
}
