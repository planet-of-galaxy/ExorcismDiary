using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���������״̬��
/// �ṩ�ķ����У�
/// SetSpeedScaleGradiently ���ã��𽥱任�ٶ����� ����ֵ��Э�̶��� ��������Ҫ�Լ������Э�̵���������
/// SetSpeedScale ���ã�ֱ�ӱ任�ٶ����� ע�⣺������Ѿ������˽����ٶ����ŵķ���������ֹͣ�Ǹ�Э�̣��ٵ��ô˷���
/// </summary>
public class ChairlieFsm : FiniteStateMachineBase<ChairlieStateBase, ChairlieFsm>
{
    public const float move_speed = 10;
    public float speed_scale = 0;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public ChairlieFsm(GameObject agent) : base(agent)
    {
        animator = agent.GetComponent<Animator>();
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
        // ��ʼ��״̬ ��ʼ״̬ΪIdle
        ChangeToState<ChairlieIdleState>();
    }

    // �𽥱任 �ٶ����� ��Ŀ��ֵ ����һ��Э�̶��� ��������ع���ø�Э�̵���������
    public Coroutine SetSpeedScaleGradiently(float scale)
    {
        return ChangeFloatGradually(speed_scale, scale, ChangeSpeedGradually);
    }

    // ֱ�ӱ任 �ٶ����� ��Ŀ��ֵ
    public void SetSpeedScale(float scale)
    {
        speed_scale = scale;
        navMeshAgent.speed = move_speed * speed_scale;
        animator.SetFloat("SpeedScale", speed_scale);
    }
    private void ChangeSpeedGradually(float target) {
        SetSpeedScale(target);
    }
}