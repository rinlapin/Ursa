using UnityEngine;
using AK.Wwise;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AK.Wwise.Event musicEvent, hint, win;
    
    private void Awake()
    {
        instance = this;

        musicEvent.Post(gameObject);
        AkUnitySoundEngine.SetState("gameplay", "wandering");
    }

    public void PlayHint()
    {
        hint.Post(gameObject);
    }

    public void PlayWin()
    {
        win.Post(gameObject);
    }

}
