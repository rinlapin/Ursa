using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Unfocus()
    {
        
    }
    
    public virtual void Focus()
    {
        
    }

    public virtual void Interact()
    {
        
    }
}

public enum InteractableState
{
    Forgotten,
    Restored
}
