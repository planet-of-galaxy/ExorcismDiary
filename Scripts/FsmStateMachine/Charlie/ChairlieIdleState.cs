using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChairlieIdleState : ChairlieStateBase
{
    // 延迟切换状态协程
    private Coroutine change_state_coroutine;

    public override void OnStateEnter()
    {
        Debug.Log("查理进入Idle状态");
        fsm.SetSpeedScale(0f); // 设置速度缩放为0
        change_state_coroutine = fsm.DelayInvoke(5f, () =>
        {
            // 5秒后切换到巡逻状态
            fsm.ChangeToState<ChairliePatrolState>();
        });
    }

    public override void OnStateExit()
    {
        // 停止协程
        if (change_state_coroutine != null)
        {
            if (change_state_coroutine != null)
                fsm.StopCoroutine(change_state_coroutine);
        }
    }

    public override void OnStateUpdate()
    {
    }
}
