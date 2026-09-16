using UnityEngine.InputSystem;

public class DialogueWriteState : GameState, PlayerInputActions.IDialogueActions
{
        private PlayerInputActions _actions;
        private DialogueManager _manager;
        
        public DialogueWriteState(PlayerInputActions actions, DialogueManager manager) : base()
        {
                _actions = actions;
                _manager = manager;
        }

        public override void Enter()
        {
                _actions.Dialogue.AddCallbacks(this);
                _manager.OnLineComplete = Complete;
                _manager.NextLine();
        }

        public void OnNext(InputAction.CallbackContext context)
        {
                if (!context.started) return;
                _manager.SkipAnimation();
        }

        public override void Exit()
        {
                _actions.Dialogue.RemoveCallbacks(this);
        }
}
