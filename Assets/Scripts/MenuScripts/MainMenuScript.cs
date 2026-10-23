using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    public GameObject creditsText;

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void Credits()
    {
        creditsText.SetActive(!creditsText.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
