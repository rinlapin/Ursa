using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private GlobalStateMachine _globalStateMachine;
    private PlayerInputActions _actions;
    private PuzzleManager _puzzleManager;
    private DialogueManager _dialogueManager;

    public Vector2 walkInput;
    public Vector3 puzzleInput;

    private Interactable _closestInteractable;
    public Interactable ClosestInteractable
    {
        get => _closestInteractable;
        set
        {
            if (value == _closestInteractable) return;
            _closestInteractable?.Unfocus();
            _closestInteractable = value;
            _closestInteractable?.Focus();
        }
    }
    
    private void Awake()
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
    
    private void OnEnable()
    {
        _actions = new PlayerInputActions();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _puzzleManager = FindAnyObjectByType<PuzzleManager>();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        _globalStateMachine = new GlobalStateMachine(_actions, _puzzleManager, _dialogueManager);
        _globalStateMachine.Enter();
    }
}
