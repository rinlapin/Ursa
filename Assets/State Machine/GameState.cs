using System;

public abstract class GameState
{
    public Action OnComplete;
    
    public virtual void Enter()
    {
    }

    /*public virtual void Update()
    {
    }*/

    public virtual void Exit()
    {
        
    }
    
    public virtual void Complete()
    {
        OnComplete?.Invoke();
    }
}
