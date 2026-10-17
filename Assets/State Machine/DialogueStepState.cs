public class DialogueStepState : FiniteStateMachine
{
        private PlayerInputActions _actions;
        private DialogueManager _manager;
        private SolvedInteractable _interactable;
        
        public DialogueWriteState DialogueWriteState { get; protected set; }
        public DialogueDisplayState DialogueDisplayState { get; protected set; }
        
        public DialogueStepState(PlayerInputActions actions, DialogueManager manager) : base()
        {
                _actions = actions;
                _manager = manager;
                
                DialogueWriteState = new DialogueWriteState(actions, manager)
                {
                        OnComplete = HandleWriteComplete
                };
                DialogueDisplayState = new DialogueDisplayState(actions, manager)
                {
                        OnComplete = HandleDisplayComplete
                };
        }

        public override void Enter()
        {
                _actions.Dialogue.Enable();
                _interactable = GameManager.Instance.ClosestInteractable as SolvedInteractable;
                _manager.SetDialogue(_interactable);
                ChangeState(DialogueWriteState);
        }

        private void HandleWriteComplete()
        {
                ChangeState(DialogueDisplayState);
        }

        private void HandleDisplayComplete()
        {
                if (_manager.reachedEnd)
                {
                        _manager.Close();
                        Complete();
                        return;
                }
                ChangeState(DialogueWriteState);
        }
}
