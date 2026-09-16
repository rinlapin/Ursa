using UnityEngine;

public class FiniteStateMachine : GameState
{
    public GameState CurrentState { get; protected set; }

    public FiniteStateMachine() : base()
    {
        
    }

    public virtual void ChangeState(GameState newState)
    {
        CurrentState?.Exit();
        Debug.Log(this + ": " + newState);
        CurrentState = newState;
        CurrentState?.Enter();
    }
}
