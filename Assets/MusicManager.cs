
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AK.Wwise.Event gameplay, openingCutscene, mainMenu, toBeContinued;
    public AK.Wwise.Event stopMusic;

    [SerializeField] private AK.Wwise.Switch inCave, outside;
    [SerializeField] private AK.Wwise.State puzzle, aries, wander, exit, open;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += SetMusic;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= SetMusic;
        }
    }

    private void SetMusic(Scene scene, LoadSceneMode mode)
    {
        // Stop the previous music before starting the new music.
        stopMusic.Post(gameObject);

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
