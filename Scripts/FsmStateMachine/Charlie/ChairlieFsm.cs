using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairlieFsm : FiniteStateMachineBase<ChairlieStateBase>
{
    public float move_speed = 10;
    public float speed_scale = 0;
    private Coroutine speed_scale_gradient;
    public ChairlieFsm(GameObject agent) : base(agent)
    {
    }

    public void SetSpeedScale(float scale)
    {
        Coroutine coroutine = TimeManager.Instance.DelayInvoke(0.1f, () =>
        {
            speed_scale = scale;
            agent.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = move_speed * speed_scale;
        });
    }
}