using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ChairlieStateBase : StateBase<ChairlieStateBase>
{
    protected Animator animator;
    public override void Init(FiniteStateMachineBase<ChairlieStateBase> fsm, GameObject agent)
    {
        base.Init(fsm as ChairlieFsm, agent);
        animator = agent.GetComponent<Animator>();
    }
}
