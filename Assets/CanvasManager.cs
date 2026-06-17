using System;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        
        ActiveGroup = CanvasGroupType.None;
    }
    
    [SerializeField] private CanvasGroup puzzleGroup;
    
    private CanvasGroupType _activeGroup;
    public CanvasGroupType ActiveGroup
    {
        get => _activeGroup;
        set
        {
            _activeGroup = value;

            puzzleGroup.gameObject.SetActive(false);
            
            switch (value)
            {
                case CanvasGroupType.None:
                    break;
                case CanvasGroupType.Puzzle:
                    puzzleGroup.gameObject.SetActive(true);
                    break;
            }
        }
    }
}

public enum CanvasGroupType
{
    Puzzle,
    None
}
