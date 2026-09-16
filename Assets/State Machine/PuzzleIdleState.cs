using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleIdleState : GameState, PlayerInputActions.IPuzzleActions
{
        private PlayerInputActions _actions;
        private CanvasGroup _hint;
        public ExitStatePuzzle ExitState { get; protected set; }
        
        public PuzzleIdleState(PlayerInputActions actions, PuzzleManager manager) : base()
        {
                _actions = actions;
                _hint = manager.Hint;
        }
        
        public override void Enter()
        {
                _actions.Puzzle.SetCallbacks(this);
        }

        public void OnHint(InputAction.CallbackContext context)
        {
                if (!context.started) return;
                ExitState = ExitStatePuzzle.Hint;
                Complete();
                /*Debug.Log(_pivot.localRotation.eulerAngles);
                if (_pivot.localRotation.eulerAngles is { x: < 15 and > -15, y: < 15 and > -15, z: < 15 and > -15 })
                {
                        Debug.Log("Correct!");
                }*/
        }

        public void OnRotateX(InputAction.CallbackContext context)
        {
                GameManager.Instance.puzzleInput = new Vector3(context.ReadValue<float>(), 0, 0);
                if (context.ReadValue<float>() == 0)
                {
                        return;
                }
                ExitState = ExitStatePuzzle.Rotating;
                Complete();
        }

        public void OnRotateY(InputAction.CallbackContext context)
        {
                GameManager.Instance.puzzleInput = new Vector3(0, context.ReadValue<float>(), 0);
                if (context.ReadValue<float>() == 0)
                {
                        return;
                }
                ExitState = ExitStatePuzzle.Rotating;
                Complete();
        }

        public void OnRotateZ(InputAction.CallbackContext context)
        {
                GameManager.Instance.puzzleInput = new Vector3(0, 0, context.ReadValue<float>());
                if (context.ReadValue<float>() == 0)
                {
                        return;
                }
                ExitState = ExitStatePuzzle.Rotating;
                Complete();
        }

        public override void Exit()
        {
                _actions.Puzzle.RemoveCallbacks(this);
        }
}

public enum ExitStatePuzzle
{
        Rotating,
        Hint
}
