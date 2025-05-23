using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 查理的有限状态机
/// 提供的方法有：
/// SetSpeedScaleGradiently 作用：逐渐变换速度缩放 返回值：协程对象 调用者需要自己管理好协程的生命周期
/// SetSpeedScale 作用：直接变换速度缩放 注意：如果你已经调用了渐变速度缩放的方法，请先停止那个协程，再调用此方法
/// </summary>
public class ChairlieFsm : FiniteStateMachineBase<ChairlieStateBase, ChairlieFsm>
{
    public const float move_speed = 5;
    public float speed_scale = 0;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public ChairlieFsm(GameObject agent) : base(agent)
    {
        animator = agent.GetComponent<Animator>();
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
        // 监听玩家死亡的消息
        EventCenter.Instance.Subscribe(E_EventType.E_Player_Dead, Catched);
        // 初始化状态 初始状态为Idle
        ChangeToState<ChairlieIdleState>();
    }

    private void Catched()
    {
        ChangeToState<ChairlieCatchState>();
    }

    // 逐渐变换 速度缩放 到目标值 返回一个协程对象
    // 第一个参数是目标值 第二个参数是缩放速度 默认值为1
    public Coroutine SetSpeedScaleGradiently(float scale, float scale_scale = 1)
    {
        return ChangeFloatGradually(speed_scale, scale, ChangeSpeedGradually, scale_scale);
    }

    // 直接变换 速度缩放 到目标值
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