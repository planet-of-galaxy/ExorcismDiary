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
    public const float move_speed = 10;
    public float speed_scale = 0;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public ChairlieFsm(GameObject agent) : base(agent)
    {
        animator = agent.GetComponent<Animator>();
        navMeshAgent = agent.GetComponent<NavMeshAgent>();
        // 初始化状态 初始状态为Idle
        ChangeToState<ChairlieIdleState>();
    }

    // 逐渐变换 速度缩放 到目标值 返回一个协程对象 调用者务必管理好该协程的生命周期
    public Coroutine SetSpeedScaleGradiently(float scale)
    {
        return MonoMgr.Instance.StartCoroutine(ChangeSpeedGradually(scale));
    }

    // 直接变换 速度缩放 到目标值
    public void SetSpeedScale(float scale)
    {
        speed_scale = scale;
        navMeshAgent.speed = move_speed * speed_scale;
        animator.SetFloat("SpeedScale", speed_scale);
    }
    private IEnumerator ChangeSpeedGradually(float target) {
        float speed_scale = this.speed_scale;
        while (speed_scale != target) {
            speed_scale = Mathf.Lerp(speed_scale, target, Time.deltaTime);
            SetSpeedScale(speed_scale);
            yield return null;
        }
    }
}