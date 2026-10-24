using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AK.Wwise.Event gameplay, openingCutscene, mainMenu, toBeContinued;

    [SerializeField] private AK.Wwise.Switch inCave, outside;
    [SerializeField] private AK.Wwise.State puzzle, aries, wander, exit, open;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SetMusic;
    }

    private void SetMusic(Scene scene, LoadSceneMode mode)
    {
        AK.Wwise.Event newMusicEvent = null;

        switch (scene.name)
        {
            case "MainMenuScene":
                open.SetValue();
                mainMenu.Post(gameObject);
                break;
            case "BearDenTutorial":
                SetWander();
                inCave.SetValue(gameObject);
                gameplay.Post(gameObject);
                break;
            case "MainGame":
                SetWander();
                outside.SetValue(gameObject);
                gameplay.Post(gameObject);
                break;
            case "tbc_scene":
                toBeContinued.Post(gameObject);
                break;
            default:
                openingCutscene.Post(gameObject);
                break;
        }
    }

    public void OnMenuExitPress()
    {
        exit.SetValue();
    }

    public void SetSolving()
    {
        puzzle.SetValue();
    }

    public void SetAries()
    {
        aries.SetValue();
    }

    public void SetWander()
    {
        wander.SetValue();
    }
}
