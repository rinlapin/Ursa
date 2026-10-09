public class PuzzleWinState : GameState
{
        private PuzzleManager _manager;
        public ForgottenInteractable Interactable;
        
        public PuzzleWinState(PuzzleManager manager) : base()
        {
                _manager = manager;
        }

        public override void Enter()
        {
                SoundManager.instance.PlayWin();
                _manager.Close();
                Interactable.Solve();
                Complete();
        }

        public override void Exit()
        {
                Interactable = null;
        }
}
