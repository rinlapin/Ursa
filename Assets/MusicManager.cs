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
                newMusicEvent = mainMenu;
                break;
            case "BearDenTutorial":
                inCave.SetValue(gameObject);
                newMusicEvent = gameplay;
                break;
            case "MainGame":
                outside.SetValue(gameObject);
                break;
            case "tbc_scene":
                newMusicEvent = toBeContinued;
                break;
            default:
                newMusicEvent = openingCutscene;
                break;
        }

        newMusicEvent?.Post(gameObject);
    }

    public void OnMenuExitPress()
    {
        exit.SetValue();
    }
}
