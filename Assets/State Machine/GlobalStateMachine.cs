using UnityEngine;

public class GlobalStateMachine : FiniteStateMachine
{

        public PlayerStepState PlayerStepState { get; protected set; }
        public PuzzleStepState PuzzleStepState { get; protected set; }
        public DialogueStepState DialogueStepState { get; protected set; }
        
        public GlobalStateMachine(PlayerInputActions actions, PuzzleManager puzzleManager, DialogueManager dialogueManager) : base()
        {
                PlayerStepState = new PlayerStepState(actions)
                {
                        OnComplete = HandlePlayerComplete
                };
                PuzzleStepState = new PuzzleStepState(actions, puzzleManager)
                {
                        OnComplete = HandlePuzzleComplete
                };
                DialogueStepState = new DialogueStepState(actions, dialogueManager)
                {
                        OnComplete = HandleDialogueComplete
                };
        }
        
        public override void Enter()
        {
                ChangeState(PlayerStepState);
        }

    private void HandlePlayerComplete()
    {
        if (PlayerStepState.ExitState == ExitStatePlayer.Interaction)
        {
            if (GameManager.Instance.ClosestInteractable.GetComponent<ForgottenInteractable>())
            {
                ChangeState(PuzzleStepState);
            }
            else if (GameManager.Instance.ClosestInteractable.GetComponent<SolvedInteractable>())
            {
                ChangeState(DialogueStepState);
            }
        }
    }

        private void HandlePuzzleComplete()
        {
                ChangeState(PlayerStepState);
        }

        private void HandleDialogueComplete()
        {
                
                ChangeState(PlayerStepState);
        }
}
