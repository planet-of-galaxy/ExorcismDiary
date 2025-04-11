using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ChairliePatrolState : ChairlieStateBase
{
    private NavMeshAgent navMeshAgent;
    private Transform[] patrol_point;
    private int current_patrol_index = 1;
    private Coroutine change_speed_coroutine;
    private Coroutine change_state_coroutine;
    private bool prepare_to_change = false;

    public override void Init(ChairlieFsm fsm, GameObject agent)
    {
        base.Init(fsm, agent);
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
        patrol_point = GameObject.Find("ChairliePatrolPoint").GetComponentsInChildren<Transform>();

        for (int i = 0; i < patrol_point.Length; i++)
        {
            Debug.Log(patrol_point[i].name);
        }
    }
    public override void OnStateEnter()
    {
        Debug.Log("巡逻状态开始");
        // 进来的时候先把prepare_to_change设置为false 这样就会开始检测是否到达巡逻点
        prepare_to_change = false;
        // 使用SetSpeedScaleGradiently设置速度缩放 缩放会逐渐扩大到0.8 而不是一瞬间变成0.8 这样看起来更自然
        // 动画状态机的speed_scale和navMeshAgent的speed都会在这个协程中被设置 所以我们只需管理好这个协程的生命周期即可
        change_speed_coroutine = fsm.SetSpeedScaleGradiently(0.8f);

        // 设置新的巡逻点
        Debug.Log("查理要去：" + patrol_point[current_patrol_index].name);
        navMeshAgent.SetDestination(patrol_point[current_patrol_index].position);
    }

    public override void OnStateExit()
    {
        // 更新巡逻点索引
        if (current_patrol_index >= patrol_point.Length - 1)
            current_patrol_index = 1;
        else
            current_patrol_index++;

        // 停止协程 即使协程还没有执行完毕
        if (change_speed_coroutine != null)
        {
            fsm.StopCoroutine(change_speed_coroutine);
            change_speed_coroutine = null;
        }
        // 延迟切换协程 最好也停一下 虽然不释放也不会有内存泄漏的问题 但是change_speed_coroutine这个协程必须停止哦
        if (change_state_coroutine != null)
        {
            fsm.StopCoroutine(change_state_coroutine);
            change_state_coroutine = null;
        }
    }

    public override void OnStateUpdate()
    {
        // 即将到达巡逻点时，先减速 再延迟切换状态 减速期间把prepare_to_change设置为true 不再继续检测是否到达巡逻点
        // 优化点： 这个减速距离最终要从配置文件中读取
        if (!prepare_to_change && Vector3.Distance(agent.transform.position, patrol_point[current_patrol_index].position) < 2)
        {
            // 先判断速度渐变协程是否存在 如果存在则停止
            if (change_speed_coroutine != null)
            {
                fsm.StopCoroutine(change_speed_coroutine);
                change_speed_coroutine = null;
            }
            // 慢慢停下来
            change_speed_coroutine = fsm.SetSpeedScaleGradiently(0f);
            change_state_coroutine = fsm.DelayInvoke(1f, () =>
            {
                // 指定秒后切换到Idle状态
                fsm.ChangeToState<ChairlieIdleState>();
            });
            prepare_to_change = true;
        }
    }
}
