using System;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;
    
    Dictionary<CanvasGroupType, GameObject> _groups = new Dictionary<CanvasGroupType, GameObject>();
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    [SerializeField] private CanvasGroup puzzleGroup;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        foreach (UIGroup group in GetComponentsInChildren<UIGroup>(true))
        {
            _groups.Add(group.type, group.gameObject);
        }

        ActiveGroupType = CanvasGroupType.None;
    }
    
    private CanvasGroupType _activeGroupType;
    public CanvasGroupType ActiveGroupType
    {
        get => _activeGroupType;
        set
        {
            _activeGroupType = value;

            foreach (var type in _groups.Keys)
            {
                _groups[type].SetActive(_activeGroupType == type);
            }
        }
    }
}

public enum CanvasGroupType
{
    Puzzle,
    Star,
    None
}
