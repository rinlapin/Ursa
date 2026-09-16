using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleRotateState : GameState, PlayerInputActions.IPuzzleActions
{
    private PlayerInputActions _actions;
    
    private Sequence _rotate;

    private Transform _pivot; 

    public PuzzleRotateState(PlayerInputActions actions, PuzzleManager manager) : base()
    {
        _actions = actions;
        _pivot = manager.Pivot;
    }
    
    public override void Enter()
    {
        _actions.Puzzle.SetCallbacks(this);
        CheckForInput();
    }

    public void OnHint(InputAction.CallbackContext context)
    {
        
    }

    public void OnRotateX(InputAction.CallbackContext context)
    {
        GameManager.Instance.puzzleInput = new Vector3(context.ReadValue<float>(), 0, 0);
    }

    public void OnRotateY(InputAction.CallbackContext context)
    {
        GameManager.Instance.puzzleInput = new Vector3(0, context.ReadValue<float>(), 0);
    }

    public void OnRotateZ(InputAction.CallbackContext context)
    {
        GameManager.Instance.puzzleInput = new Vector3(0, 0, context.ReadValue<float>());
    }

    private void RotateBy(Vector3 input)
    {
        Quaternion targetRotation = Quaternion.Euler(_pivot.localRotation.eulerAngles + input * 15);
        _pivot.DOLocalRotateQuaternion(targetRotation, 0.5f).OnComplete(CheckForInput);
    }

    private void CheckForInput()
    {
        if (GameManager.Instance.puzzleInput == Vector3.zero)
        {
            Complete();
        }
        else
        {
            RotateBy(GameManager.Instance.puzzleInput);
        }
    }
    
    public override void Exit()
    {
        _actions.Puzzle.RemoveCallbacks(this);
    }
}