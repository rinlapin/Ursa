using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Interactable closestInteractable;
    
    public static PlayerInteraction instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (closestInteractable == null) return;
        if (Player.state != PlayerState.Idle) return;

        if (!PuzzleManager.instance.activeInteractable)
        {
            Player.state = PlayerState.Interacting;
            closestInteractable.TryInteract();
        }
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
