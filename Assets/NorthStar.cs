using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NorthStar : MonoBehaviour
{
    private Interactable[] _interactables;
    private Interactable _closestInteractable;
    private Camera _mainCamera;
    
    private Interactable GetClosestInteractable()
    {
        if (_interactables == null || _interactables.Length == 0)
        {
            return null;
        }
        
        var closestInteractable = _interactables[0];

        foreach (var interactable in _interactables)
        {
            if (Vector3.Distance(interactable.transform.position, Player.instance.transform.position)
                >= Vector3.Distance(closestInteractable.transform.position, Player.instance.transform.position))
            {
                continue;
            }

            closestInteractable = interactable;
        }
        return closestInteractable;
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _interactables = FindObjectsByType<Interactable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

    private void Update()
    {
        Reorient();
    }

    private void Reorient()
    {
        _closestInteractable = GetClosestInteractable();
        transform.rotation = Quaternion.LookRotation(_closestInteractable.transform.position - transform.position, Vector3.up);
    }
}
