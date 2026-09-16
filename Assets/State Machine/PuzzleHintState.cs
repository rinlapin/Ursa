using DG.Tweening;
using UnityEngine;

public class PuzzleHintState : GameState
{
        public bool IsWin { get; protected set; }

        private Transform _pivot;
        private CanvasGroup _hint, _winBackground;

        private Sequence _showHint;
        
        public PuzzleHintState(PuzzleManager puzzleManager) : base()
        {
                _pivot = puzzleManager.Pivot;
                _hint = puzzleManager.Hint;
                _winBackground = puzzleManager.WinBackground;
        }

        public override void Enter()
        {
                IsWin = _pivot.localRotation.eulerAngles is { x: < 15 and > -15, y: < 15 and > -15, z: < 15 and > -15 };
                _showHint = DOTween.Sequence();
                _showHint.Append(_hint.DOFade(1, 0.5f));
                if (IsWin)
                {
                        _showHint.Join(_winBackground.DOFade(1, 0.5f));
                }
                
                _showHint
                        .AppendInterval(1f)
                        .Append(_hint.DOFade(0, 0.5f))
                        .OnComplete(Complete);
        }

        public override void Exit()
        {
                IsWin = false;
        }
}
