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
    private AudioSource chase_music;

    public override void Init(ChairlieFsm fsm, GameObject agent)
    {
        base.Init(fsm, agent);
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
        patrol_point = GameObject.Find("ChairliePatrolPoint").GetComponentsInChildren<Transform>();
        chase_music = agent.GetComponent<AudioSource>();

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
        // 使用SetSpeedScaleGradiently设置速度缩放 缩放会逐渐扩大到0.6 而不是一瞬间变成0.6 这样看起来更自然
        // 动画状态机的speed_scale和navMeshAgent的speed都会在这个协程中被设置 所以我们只需管理好这个协程的生命周期即可
        // 第二个参数是缩放速度 默认值为1 缩放值越大，速度变化越快
        change_speed_coroutine = fsm.SetSpeedScaleGradiently(0.6f);

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

        // 停止协程 无需手动停止协程 Fsm会帮我们处理
    }

    public override void OnStateUpdate()
    {
        if (FindTarget())
        {
            fsm.ChangeToState<ChairlieChaseState>();
        }

        // 即将到达巡逻点时，先减速 再延迟切换状态 减速期间把prepare_to_change设置为true 不再继续检测是否到达巡逻点
        // 优化点： 这个减速距离最终要从配置文件中读取
        if (!prepare_to_change && Vector3.Distance(agent.transform.position, patrol_point[current_patrol_index].position) < 2)
        {
            // 先判断速度渐变协程是否存在 如果存在则停止
            if (change_speed_coroutine != null)
            {
                fsm.StopFsmCoroutine(change_speed_coroutine);
                change_speed_coroutine = null;
            }
            // 慢慢停下来
            change_speed_coroutine = fsm.SetSpeedScaleGradiently(0f, 5f);
            change_state_coroutine = fsm.DelayInvoke(1f, () =>
            {
                // 指定秒后切换到Idle状态
                fsm.ChangeToState<ChairlieIdleState>();
            });
            prepare_to_change = true;
        }
    }
}
