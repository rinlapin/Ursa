using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    
    public Interactable activeInteractable;
    
    public void Exit()
    {
        activeInteractable = null;
        CanvasManager.instance.ActiveGroupType = CanvasGroupType.None;
        Player.state = PlayerState.Idle;
    }

    public void Solve()
    {
        if (!activeInteractable) return;
        activeInteractable.Solve();
        PlayerInteraction.instance.closestInteractable = null;
        Exit();
    }
}
