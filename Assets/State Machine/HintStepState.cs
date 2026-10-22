public class HintStepState : GameState
{
        public HintStepState() : base()
        {
                
        }

        public override void Enter()
        {
                HintGraphic.instance.Show(Complete);
        }
}
