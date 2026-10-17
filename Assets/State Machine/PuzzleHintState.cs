using DG.Tweening;
using UnityEngine;

public class PuzzleHintState : GameState
{
        private Transform _pivot;
        private CanvasGroup _hint;

        private Sequence _showHint;
        
        public PuzzleHintState(PuzzleManager puzzleManager) : base()
        {
                _pivot = puzzleManager.Pivot;
                _hint = puzzleManager.Hint;
        }

        public override void Enter()
        {
                _showHint = DOTween.Sequence();
                _showHint.Append(_hint.DOFade(1, 0.5f));
                SoundManager.Instance.PlayEvent(SoundManager.Instance.playHint);
                
                _showHint
                        .AppendInterval(1f)
                        .Append(_hint.DOFade(0, 0.5f))
                        .OnComplete(Complete);
        }
}
