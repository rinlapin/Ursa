using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NorthStar : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    private Interactable[] _interactables;
    private Interactable _closestInteractable;
    private Camera _mainCamera;
    
    private Interactable GetClosestInteractable()
    {
        return _interactables[0];
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
        var closest = _interactables[0];
        //Camera.main.ScreenToWorldPoint(GetClosestInteractable().transform.position);
        var screenPoint = new Vector2(
            _mainCamera.WorldToScreenPoint(closest.transform.position).x,
            _mainCamera.WorldToScreenPoint(closest.transform.position).y);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, _mainCamera, out var localPoint);
        
        
        
        var rotationAngle = Mathf.Abs(localPoint.x) < Screen.width * 2 ? 45f / Screen.width * localPoint.x : 90 / Mathf.Sign(localPoint.x);

        transform.eulerAngles = new Vector3(0, 0, rotationAngle);
    }
}
