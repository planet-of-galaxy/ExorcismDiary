using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChairlieIdleState : ChairlieStateBase
{
    private NavMeshAgent navMeshAgent;
    // 延迟切换状态协程
    private Coroutine change_state_coroutine;

    public override void Init(FiniteStateMachineBase<ChairlieStateBase> fsm, GameObject agent)
    {
        base.Init(fsm, agent);
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
    }
    public override void OnStateEnter()
    {
        Debug.Log("空闲状态开始");
        animator.SetFloat("MoveSpeed", 0f);
        // 设置延迟执行，1s后切换到巡逻状态
        change_state_coroutine = TimeManager.Instance.DelayInvoke(5f, () => {
            fsm.ChangeToState<ChairliePatrolState>();
        });
    }

    public override void OnStateExit()
    {
        Debug.Log("空闲状态结束");
        // 停止协程
        if (change_state_coroutine != null)
        {
            TimeManager.Instance.CancelInvoke(change_state_coroutine);
        }
    }

    public override void OnStateUpdate()
    {
    }
}
