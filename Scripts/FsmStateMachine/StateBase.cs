using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase<T> where T : StateBase<T>
{
    protected FiniteStateMachineBase<T> fsm;
    protected GameObject agent;

    public virtual void Init(FiniteStateMachineBase<T> fsm,  GameObject agent) {
        this.fsm = fsm;
        this.agent = agent;
    }
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
