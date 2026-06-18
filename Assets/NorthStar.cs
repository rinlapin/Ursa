using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NorthStar : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Interactable[] _interactables;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private Interactable GetClosestInteractable()
    {
        _interactables = FindObjectsByType<Interactable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
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
    
    public void Reorient()
    {
        //Camera.main.ScreenToWorldPoint(GetClosestInteractable().transform.position);
        var screenPoint = Camera.main.WorldToScreenPoint(GetClosestInteractable().transform.position);
        Debug.Log($"pointing at {screenPoint}");
    }
}
