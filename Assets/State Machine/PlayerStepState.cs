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
        
        public override void Exit()
        {
                GameManager.Instance.walkInput = Vector2.zero;
                _actions.Player.RemoveCallbacks(this);
                _actions.Player.Disable();
        }
}

public enum ExitStatePlayer
{
        Interaction,
        Pause
}
