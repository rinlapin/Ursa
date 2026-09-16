using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleStepState : FiniteStateMachine
{
        private PlayerInputActions _actions;
        public PuzzleIdleState PuzzleIdleState { get; protected set; }
        public PuzzleRotateState PuzzleRotateState { get; protected set; }
        public PuzzleHintState PuzzleHintState { get; protected set; }
        public PuzzleWinState PuzzleWinState { get; protected set; }

        private PuzzleManager _manager;
        private ForgottenInteractable _interactable;
        
        public PuzzleStepState(PlayerInputActions actions, PuzzleManager manager) : base()
        {
                _actions = actions;
                _manager = manager;

                PuzzleIdleState = new PuzzleIdleState(actions, manager)
                {
                        OnComplete = HandleIdleStateComplete
                };

                PuzzleRotateState = new PuzzleRotateState(actions, manager)
                {
                        OnComplete = HandleRotateStateComplete
                };

                PuzzleHintState = new PuzzleHintState(manager)
                {
                        OnComplete = HandleHintStateComplete
                };

                PuzzleWinState = new PuzzleWinState(manager)
                {
                        OnComplete = HandleWinStateComplete
                };
        }

        public override void Enter()
        {
                _interactable = GameManager.Instance.ClosestInteractable as ForgottenInteractable;
                _manager.SetupPuzzle(_interactable);
                _actions.Puzzle.Enable();
                ChangeState(PuzzleIdleState);
        }

        private void HandleIdleStateComplete()
        {
                GameState nextState = PuzzleIdleState.ExitState switch
                {
                        ExitStatePuzzle.Hint => PuzzleHintState,
                        ExitStatePuzzle.Rotating => PuzzleRotateState,
                        _ => null
                };
                
                ChangeState(nextState);
        }

        private void HandleRotateStateComplete()
        {
                ChangeState(PuzzleIdleState);
        }

        private void HandleHintStateComplete()
        {
                if (PuzzleHintState.IsWin)
                {
                        PuzzleWinState.Interactable = _interactable;
                        ChangeState(PuzzleWinState);
                }
                else
                {
                        ChangeState(PuzzleIdleState);
                }
        }

        private void HandleWinStateComplete()
        {
                Complete();
        }
}