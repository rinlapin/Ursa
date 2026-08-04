using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NorthStar : MonoBehaviour
{
    private List<Interactable> _interactables;
    private Interactable _closestInteractable;
    private Camera _mainCamera;
    
    private Interactable GetClosestInteractable()
    {
        if (_interactables == null || _interactables.Count == 0)
        {
            return null;
        }
        
        var closestInteractable = _interactables[0];

        foreach (var interactable in _interactables)
        {
            if (interactable == null)
            {
                continue;
            }
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
        _interactables = FindObjectsByType<Interactable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        Reorient();
    }

    private void Reorient()
    {
        _closestInteractable = GetClosestInteractable();
        if (!_closestInteractable)
        {
            gameObject.SetActive(false);
            return;
        }
        transform.rotation = Quaternion.LookRotation(_closestInteractable.transform.position - transform.position, Vector3.up);
    }
}
