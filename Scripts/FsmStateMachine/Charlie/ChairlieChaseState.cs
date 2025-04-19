using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChairlieChaseState : ChairlieStateBase
{
    private Coroutine change_speed_coroutine;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private float memory_time = 5; // ����ʱ�� ��ʧȥĿ��� ������ֹͣ׷�� ���Ż��ɴ������ļ��ж�ȡ

    private float attack_backswing = 0; // ������ҡ ���Ż��ɴ������ļ��ж�ȡ 

    // ��ת��ز�����ǰ����
    private float rotation_speed = 90;
    private Quaternion target_quaternion;
    private float distance; // Ŀ�굽����ľ���
    private float maxDegrees; // ÿ֡��ת���Ƕ�
    public override void OnStateEnter()
    {
        Debug.Log("����׷��ģʽ����");
        if (navMeshAgent == null)
            navMeshAgent = agent.GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = agent.GetComponent<Animator>();

        AudioManager.Instance.PlaySafely("ս��", E_AudioType.E_MUSIC);
        fsm.SetSpeedScaleGradiently(1f);
        fsm.StartRepeatingAction(0.2f, Pursuit);
    }

    public override void OnStateExit()
    {

        AudioManager.Instance.StopSafely("ս��");
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

        // ִ�й�����ҡ
        if (attack_backswing > 0)
        {
            attack_backswing -= Time.deltaTime;
        }

        if (distance > 2)
            return; // ̫Զ�˾Ͳ�������

        // ������ҡ��ȫ�������ܼ�������
        if (attack_backswing <= 0)
        {
            animator.SetTrigger("Attack");
            attack_backswing = 1;
        }

        // ����Ŀ��
        // ̫���˾Ͳ�ת��
        if (distance < 0.01) return;

        // ����Ŀ����ת��Ԫ��
        target_quaternion = Quaternion.LookRotation(target - agent.transform.position);

        maxDegrees = rotation_speed * Time.deltaTime;
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, target_quaternion, maxDegrees);

    }
}
