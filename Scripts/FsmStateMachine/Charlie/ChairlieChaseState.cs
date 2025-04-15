using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChairlieChaseState : ChairlieStateBase
{
    private Coroutine change_speed_coroutine;
    private NavMeshAgent navMeshAgent;
    private float memory_time;
    public override void OnStateEnter()
    {
        Debug.Log("进入追击模式！！");
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
        AudioManager.Instance.PlaySafely("战斗", E_AudioType.E_MUSIC);
        fsm.SetSpeedScaleGradiently(1f);
        fsm.StartRepeatingAction(0.2f, Pursuit);
        memory_time = 1f;
    }

    public override void OnStateExit()
    {

        AudioManager.Instance.StopSafely("战斗");
        navMeshAgent.ResetPath();
    }

    public override void OnStateUpdate()
    {
        if (!FindTarget())
        {
            memory_time -= Time.deltaTime;
        }
        else {
            memory_time = 1f;
        }

        if (memory_time <= 0) {
            fsm.ChangeToState<ChairlieIdleState>();
        }
    }

    private void Pursuit() {
        navMeshAgent.SetDestination(target);
    }
}
