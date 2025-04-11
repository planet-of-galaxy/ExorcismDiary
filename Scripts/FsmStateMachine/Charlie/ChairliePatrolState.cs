//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//public class ChairliePatrolState : ChairlieStateBase
//{
//    private NavMeshAgent navMeshAgent;
//    private Transform[] patrol_point;
//    private int current_patrol_index = 1;

//    public override void Init(FiniteStateMachineBase<ChairlieStateBase> fsm, GameObject agent)
//    {
//        base.Init(fsm, agent);
//        navMeshAgent = agent.GetComponent<NavMeshAgent>();
//        patrol_point = GameObject.Find("ChairliePatrolPoint").GetComponentsInChildren<Transform>();

//        for (int i = 0;i< patrol_point.Length;i++) {
//            Debug.Log(patrol_point[i].name);
//        }
//    }
//    public override void OnStateEnter()
//    {
//        Debug.Log("巡逻状态开始");
//        navMeshAgent.isStopped = false;
//        // 设置为巡逻速度
//        // navMeshAgent.speed = fsm.move_speed;
//        // 设置新的巡逻点
//        Debug.Log("查理要去：" + patrol_point[current_patrol_index].name);
//        navMeshAgent.SetDestination(patrol_point[current_patrol_index].position);
//        animator.SetFloat("MoveSpeed", 0.5f);
//    }

//    public override void OnStateExit()
//    {
//        navMeshAgent.isStopped = true;
//        animator.SetFloat("MoveSpeed", 0f);
//        // 更新巡逻点索引
//        if (current_patrol_index >= patrol_point.Length - 1)
//            current_patrol_index = 1;
//        else
//            current_patrol_index++;
//    }

//    public override void OnStateUpdate()
//    {
//        // 当到达巡逻点时，切换到idle状态发呆一会儿
//        if (navMeshAgent.velocity == Vector3.zero &&
//            Vector3.Distance(agent.transform.position, patrol_point[current_patrol_index].position) < 2)
//            fsm.ChangeToState<ChairlieIdleState>();

//    }


//}
