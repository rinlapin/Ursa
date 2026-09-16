using UnityEngine.InputSystem;

public class DialogueDisplayState : GameState, PlayerInputActions.IDialogueActions
{
        private PlayerInputActions _actions;
        private DialogueManager _manager;
        
        public DialogueDisplayState(PlayerInputActions actions, DialogueManager manager) : base()
        {
                _actions = actions;
                _manager = manager;
        }
        
        public override void Enter()
        {
                _actions.Dialogue.AddCallbacks(this);
        }

        public void OnNext(InputAction.CallbackContext context)
        {
                if (context.started)
                {
                        Complete();
                }
        }

        public override void Exit()
        {
                _actions.Dialogue.RemoveCallbacks(this);
        }
}
