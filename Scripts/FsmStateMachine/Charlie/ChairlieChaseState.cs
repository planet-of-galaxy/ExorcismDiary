using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChairlieChaseState : ChairlieStateBase
{
    private Coroutine change_speed_coroutine;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private float memory_time = 5; // 记忆时间 当失去目标后 多少秒停止追击 可优化成从配置文件中读取

    private float attack_backswing = 0; // 攻击后摇 可优化成从配置文件中读取 

    // 旋转相关参数提前声明
    private float rotation_speed = 90;
    private Quaternion target_quaternion;
    private float distance; // 目标到查理的距离
    private float maxDegrees; // 每帧旋转最大角度
    public override void OnStateEnter()
    {
        Debug.Log("进入追击模式！！");
        if (navMeshAgent == null)
            navMeshAgent = agent.GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = agent.GetComponent<Animator>();

        AudioManager.Instance.PlaySafely("战斗", E_AudioType.E_MUSIC);
        fsm.SetSpeedScaleGradiently(1f);
        fsm.StartRepeatingAction(0.2f, Pursuit);
    }

    public override void OnStateExit()
    {

        AudioManager.Instance.StopSafely("战斗");
        navMeshAgent?.ResetPath();
    }

    public override void OnStateUpdate()
    {
        if (!FindTarget())
        {
            memory_time -= Time.deltaTime;
        }
        else {
            memory_time = 5f;
            Attack();
        }

        if (memory_time <= 0) {
            fsm.ChangeToState<ChairlieIdleState>();
        }
    }

    private void Pursuit() {
        navMeshAgent.SetDestination(target);
    }

    private void Attack() {
        distance = Vector3.Distance(agent.transform.position, target);

        // 执行攻击后摇
        if (attack_backswing > 0)
        {
            attack_backswing -= Time.deltaTime;
        }

        if (distance > 2)
            return; // 太远了就不攻击了

        // 攻击后摇完全结束才能继续攻击
        if (attack_backswing <= 0)
        {
            animator.SetTrigger("Attack");
            attack_backswing = 1;
        }

        // 看向目标
        // 太近了就不转了
        if (distance < 0.01) return;

        // 计算目标旋转四元数
        target_quaternion = Quaternion.LookRotation(target - agent.transform.position);

        maxDegrees = rotation_speed * Time.deltaTime;
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, target_quaternion, maxDegrees);

    }
}
