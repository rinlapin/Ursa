using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] GameObject puzzleGroup;
    private Camera _mainCamera;

    public static PuzzleManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        _mainCamera = Camera.main;
    }

    public Interactable ActiveInteractable => _activeInteractable;

    private Interactable _activeInteractable;

    public void Setup(Interactable newInteractable)
    {
        _activeInteractable = newInteractable;
        CanvasManager.instance.ActiveGroupType = CanvasGroupType.Puzzle;
        _mainCamera.gameObject.SetActive(false);
        puzzleGroup.SetActive(true);
    }

    public void Exit()
    {
        _mainCamera.gameObject.SetActive(true);
        puzzleGroup.SetActive(false);

        _activeInteractable = null;
        CanvasManager.instance.ActiveGroupType = CanvasGroupType.None;
        Player.state = PlayerState.Idle;
    }

    public void Solve()
    {
        if (!_activeInteractable) return;
        _activeInteractable.Solve();
        PlayerInteraction.instance.closestInteractable = null;
        Exit();
    }
}
