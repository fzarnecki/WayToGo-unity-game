using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersonalSceneLoader : MonoBehaviour
{
    [SerializeField] private int sceneCount = 3;

    public void LoadScene(int targetScene)
    {
        if (targetScene <= sceneCount)
            SceneManager.LoadScene(targetScene);
    }

    public void LoadScene(string targetScene) {
        SceneManager.LoadScene(targetScene);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("chosenScene", 1));
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public string GetSceneName() {
        return SceneManager.GetActiveScene().name;
    }
}
