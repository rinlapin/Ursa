using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleState state = PuzzleState.Idle;
    private Camera _mainCamera;
    private GameObject _activeConstellation;
    public GameObject ActiveConstellation
    {
        get => _activeConstellation;
        set
        {
            if (_activeConstellation)
            {
                Destroy(_activeConstellation);
                _activeConstellation = null;
            }

            if (!value)
            {
                ConstellationFramework.instance.Constellation = null;
                return;
            }
            
            _activeConstellation = Instantiate(value, ConstellationFramework.instance.canvas.transform);
            ConstellationFramework.instance.Constellation = _activeConstellation.transform;
        }
    }

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
        ConstellationFramework.instance.gameObject.SetActive(true);
        
        ActiveConstellation = newInteractable.Constellation;
    }

    public void Exit()
    {
        ActiveConstellation = null;
        
        _mainCamera.gameObject.SetActive(true);
        ConstellationFramework.instance.gameObject.SetActive(false);

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

    #if UNITY_EDITOR
    public void OnSolve(InputAction.CallbackContext context)
    {
        if (!context.started || Player.state != PlayerState.Interacting)
        {
            Solve();
        }
    }
    #endif
}

public enum PuzzleState
{
    Idle,
    Rotating,
    Hinting
}
