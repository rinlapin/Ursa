using System;
using UnityEngine;
using AK.Wwise;
using Event = AK.Wwise.Event;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Event playFootsteps, stopFootsteps, playConstMove, playIceSmash, playNavStar, stopNavStar, playRebirth, playHint, playWin;
    
    private string _environmentSwitchGroup = "EnvironmentSwitchGroup";

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

    public void PlayEvent(string eventName)
    {
        if (eventName == String.Empty) return;
        AkUnitySoundEngine.PostEvent(eventName, gameObject);
    }
    
    public void PlayEvent(AK.Wwise.Event audioEvent) => PlayEvent(audioEvent, gameObject);

    public void PlayEvent(AK.Wwise.Event audioEvent, GameObject source)
    {
        audioEvent.Post(source);
    }
}
