using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
   public BaseState CurrentState { get; set; }
    public void Initialize(BaseState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(BaseState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();

    }

}
