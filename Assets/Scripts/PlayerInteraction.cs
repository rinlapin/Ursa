using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider c)
    {
        if (c.GetComponentInParent<Interactable>() == null) return;
        GameManager.Instance.ClosestInteractable = c.GetComponentInParent<Interactable>();
    }

    private void OnTriggerExit(Collider c)
    {
        if (c.GetComponentInParent<Interactable>() == null) return;
        GameManager.Instance.ClosestInteractable = null;
    }
}
