using UnityEngine;
using AK.Wwise;
using Event = AK.Wwise.Event;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Event playFootsteps, stopFootsteps, playConstMove, playIceSmash, playNavStar, stopNavStar, playRebirth, playHint, playWin;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlayEvent(AK.Wwise.Event audioEvent) => PlayEvent(audioEvent, gameObject);

    public void PlayEvent(AK.Wwise.Event audioEvent, GameObject source)
    {
        audioEvent.Post(source);
    }

    public void SetMusic(MusicMode mode)
    {
        
    }
}

public enum MusicMode
{
    None,
    MainMenu,
    OpeningCutscene,
    BearDen,
    Gameplay
}
