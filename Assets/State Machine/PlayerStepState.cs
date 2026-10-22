using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStepState : FiniteStateMachine, PlayerInputActions.IPlayerActions
{
        
        public ExitStatePlayer ExitState { get; protected set; }
        private PlayerInputActions _actions;
        
        public PlayerStepState(PlayerInputActions actions) : base()
        {
                _actions = actions;
        }
        
        public override void Enter()
        {
                _actions.Player.Enable();
                _actions.Player.SetCallbacks(this);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
                if (context.started)
                {
                        SoundManager.Instance.PlayEvent(SoundManager.Instance.playFootsteps);
                }
                else if (context.canceled)
                {
                        SoundManager.Instance.PlayEvent(SoundManager.Instance.stopFootsteps);
                }
                GameManager.Instance.walkInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
                if (!context.started) return;
                if (GameManager.Instance.ClosestInteractable == null) return;
                ExitState = ExitStatePlayer.Interaction;
                Complete();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
                if (!context.started) return;
                PlayerMovement.Instance.Jump();
        }

        public void OnHint(InputAction.CallbackContext context)
        {
                if (!context.started) return;
                ExitState = ExitStatePlayer.Hint;
                Complete();
        }
        
        public override void Exit()
        {
                ExitState = ExitStatePlayer.None;   
                GameManager.Instance.walkInput = Vector2.zero;
                _actions.Player.RemoveCallbacks(this);
                _actions.Player.Disable();
        }
}

public enum ExitStatePlayer
{
    None,
        Interaction,
        Hint
}
