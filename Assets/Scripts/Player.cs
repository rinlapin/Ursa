using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    
    public static PlayerState state = PlayerState.Idle;
}

public enum PlayerState
{
    Idle,
    Moving,
    Interacting
}
