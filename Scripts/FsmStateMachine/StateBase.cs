using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase<T_State, U_Fsm>
    where T_State : StateBase<T_State, U_Fsm>
    where U_Fsm : FiniteStateMachineBase<T_State, U_Fsm>
{
    protected U_Fsm fsm;
    protected GameObject agent;

    public virtual void Init(U_Fsm fsm,  GameObject agent) {
        this.fsm = fsm;
        this.agent = agent;
    }
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
