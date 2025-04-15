using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChairlieChaseState : ChairlieStateBase
{
    private Coroutine change_speed_coroutine;
    private NavMeshAgent navMeshAgent;
    public override void OnStateEnter()
    {
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
        AudioManager.Instance.PlaySafely("Õ½¶·", E_AudioType.E_MUSIC);
        fsm.SetSpeedScaleGradiently(1f);
        fsm.StartRepeatingAction(0.2f, Pursuit);
    }

    public override void OnStateExit()
    {

        AudioManager.Instance.StopSafely("Õ½¶·");
    }

    public override void OnStateUpdate()
    {
        FindTarget();
    }

    private void Pursuit() {
        navMeshAgent.SetDestination(target);
    }
}
