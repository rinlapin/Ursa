using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Interactable closestInteractable;

    private NorthStar _northStar;
    
    public static PlayerInteraction instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        _northStar = FindAnyObjectByType<NorthStar>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (closestInteractable == null) return;
        if (Player.state != PlayerState.Idle) return;
        
        closestInteractable.TryInteract();
    }

    public void OnObserve(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (Player.state != PlayerState.Idle) return;

        CanvasManager.instance.ActiveGroupType = CanvasGroupType.Star;
        _northStar.Reorient();
        
        Debug.Log("Observing star");
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponentInParent<Interactable>() == null) return;
        closestInteractable = collider.GetComponentInParent<Interactable>();
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponentInParent<Interactable>() == null) return;
        closestInteractable = null;
    }
}
