using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleRotateState : GameState, PlayerInputActions.IPuzzleActions
{
    private PlayerInputActions _actions;
    
    private Sequence _rotate;

    private Transform _pivot;

    private CanvasGroup _winBackground;
    
    private bool _canReadInput = true;
    
    public bool IsWin { get; protected set; }

    public PuzzleRotateState(PlayerInputActions actions, PuzzleManager manager) : base()
    {
        _actions = actions;
        _pivot = manager.Pivot;
        _winBackground = manager.WinBackground;
    }
    
    public override void Enter()
    {
        _canReadInput = true;
        _actions.Puzzle.SetCallbacks(this);
        CheckForInput();
    }

    public void OnHint(InputAction.CallbackContext context)
    {
        
    }

    public void OnRotateX(InputAction.CallbackContext context)
    {
        if (!_canReadInput) return;
        GameManager.Instance.puzzleInput = new Vector3(context.ReadValue<float>(), 0, 0);
    }

    public void OnRotateY(InputAction.CallbackContext context)
    {
        if (!_canReadInput) return;
        GameManager.Instance.puzzleInput = new Vector3(0, context.ReadValue<float>(), 0);
    }

    public void OnRotateZ(InputAction.CallbackContext context)
    {
        return;
        /*if (!_canReadInput) return;
        GameManager.Instance.puzzleInput = new Vector3(0, 0, context.ReadValue<float>());*/
    }

    private void RotateBy(Vector3 input)
    {
        SoundManager.Instance.PlayEvent(SoundManager.Instance.playConstMove);

        if (input.x == 0)
        {
            RotateAroundY(input.y);
        }
        else if (input.y == 0)
        {
            RotateAroundZ(input.x);
        }
        
        Quaternion targetRotation = Quaternion.Euler(_pivot.localRotation.eulerAngles);
        
        //  + input * 30
        
        //_pivot.DOLocalRotateQuaternion(targetRotation, 1f).OnComplete(CheckForInput);
    }

    private void RotateAroundZ(float x)
    {
        var angles = x * 30;
        _pivot
            .DORotateQuaternion(_pivot.rotation * Quaternion.AngleAxis(angles, Vector3.forward), 1)
            .OnComplete(CheckForInput);
    }

    private void RotateAroundY(float y)
    {
        var angles = y * 30;
        _pivot
            .DOBlendableRotateBy(new Vector3(0, angles, 0), 1)
            .OnComplete(CheckForInput);
    }

    private void CheckForInput()
    {

        if (GameManager.Instance.puzzleInput == Vector3.zero)
        {
            IsWin = _pivot.localRotation.eulerAngles is { x: < 15 and > -15, y: < 15 and > -15, z: < 15 and > -15 };
            
            if (IsWin)
            {       
                _canReadInput = false;
                SoundManager.Instance.PlayEvent(SoundManager.Instance.playWin);
                _winBackground.DOFade(1, 0.5f).OnComplete(Complete);
            }
            else
            {
                Complete();
            }
        }
        else
        {
            RotateBy(GameManager.Instance.puzzleInput);
        }
    }
    
    public override void Exit()
    {
        IsWin = false;
        _actions.Puzzle.RemoveCallbacks(this);
    }
}