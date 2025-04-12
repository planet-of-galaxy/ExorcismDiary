using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 提供方法：
/// ChangeToState<T>() 作用： 切换状态 管理状态的生命周期
/// 延迟方法 周期方法 逼近方法等返回值为Coroutine的方法 状态机会自动管理协程的生命周期 调用者也可以手动调用StopFsmCoroutine方法或StopAllCoroutine停止协程
/// DelayInvoke(float delay, Action action) 作用：延迟调用一个委托的方法
/// ChangeFloatGradually(float start, float target, Action<float> action) 作用：让一个float值逐渐变化到目标值
/// StopCoroutine(Coroutine coroutine) 作用： 有加就有减 提供停止协程的方法
/// </summary>
/// <typeparam name="T_StateBase">该状态机管理的状态类型</typeparam>
/// <typeparam name="U_Fsm">该状态机管理的状态机类型</typeparam>
public class FiniteStateMachineBase<T_StateBase, U_Fsm>
    where T_StateBase : StateBase<T_StateBase, U_Fsm>
    where U_Fsm : FiniteStateMachineBase<T_StateBase, U_Fsm>
{
    public T_StateBase current_state;
    protected GameObject agent;
    // 状态机的状态字典
    private Dictionary<string, T_StateBase> states = new();
    // 管理状态申请的协程 状态切换时会自动停止
    private List<Coroutine> coroutines = new();
    private bool startUpdate = false;

    // 参数为调用者的gameObject对象 你得告诉我 这是谁的状态机
    public FiniteStateMachineBase(GameObject agent)
    {
        this.agent = agent;
    }

    public virtual void ChangeToState<T>() where T : T_StateBase, new()
    {
        // 获取目标状态的TAG
        string TAG = typeof(T).Name;

        // 如果当前状态和目标状态相同，则不进行切换
        if (current_state != null && TAG == current_state.GetType().Name)
        {
            return;
        }

        // 如果还没有开始更新状态机，则添加到MonoManager的Update中
        if (!startUpdate)
        {
            MonoMgr.Instance.AddUpdate(this.Update);
            startUpdate = true;
        }

        // 如果当前状态不为空，则先退出当前状态
        current_state?.OnStateExit();
        // 停止所有协程
        StopAllCoroutine();

        // 如果状态机中已经存在目标状态，则直接使用
        if (states.ContainsKey(TAG))
        {
            current_state = states[TAG];
        }
        // 否则创建新的状态实例 进行初始化 并添加到状态机中
        else
        {
            current_state = new T();
            current_state.Init(this as U_Fsm, agent);
            states.Add(TAG, current_state as T_StateBase);
        }

        // 进入目标状态
        current_state.OnStateEnter();
    }

    private void Update() {
        // 把当前状态的Update方法放到MonoManager的Update中
        current_state?.OnStateUpdate();
    }

    // 提供延迟调用一个委托的方法
    public Coroutine DelayInvoke(float delay, Action action) {
        // 返回一个委托
        Coroutine crtine = MonoMgr.Instance.DelayInvoke(delay, action);
        // 把协程添加到状态机中
        coroutines.Add(crtine);
        return crtine;
    }
    // 让一个float值逐渐变化到目标值
    public Coroutine ChangeFloatGradually(float start, float target, Action<float> action, float scale = 1) {
        Coroutine crtine = MonoMgr.Instance.ChangeFloatGradually(start, target, action, scale);
        // 把协程添加到状态机中
        coroutines.Add(crtine);
        return crtine;
    }
    // 让一个Vector3值逐渐变化到目标值
    public Coroutine ChangeVector3Gradually(Vector3 start, Vector3 target, Action<Vector3> action, float scale = 1)
    {
        Coroutine crtine = MonoMgr.Instance.ChangeVector3Gradually(start, target, action, scale);
        // 把协程添加到状态机中
        coroutines.Add(crtine);
        return crtine;
    }
    // 让一个Quaternion值逐渐变化到目标值
    public Coroutine ChangeQuaternionGradually(Quaternion start, Quaternion target, Action<Quaternion> action, float scale = 1)
    {
        Coroutine crtine = MonoMgr.Instance.ChangeQuaternionGradually(start, target, action, scale);
        // 把协程添加到状态机中
        coroutines.Add(crtine);
        return crtine;
    }
    // 提供周期调用一个委托的方法
    public Coroutine StartRepeatingAction(float interval, Action action) {
        Coroutine crtion = MonoMgr.Instance.StartRepeatingAction(interval, action);
        // 把协程添加到状态机中
        coroutines.Add(crtion);
        return crtion;
    }

    // 提供停止协程的方法
    public void StopFsmCoroutine(Coroutine coroutine)
    {
        for (int i = 0; i < coroutines.Count; i++)
        {
            if (coroutines[i] == coroutine)
            {
                MonoMgr.Instance.StopCoroutine(coroutine);
                coroutines.RemoveAt(i);
                break;
            }
        }
    }
    // 停止所有协程
    public void StopAllCoroutine()
    {
        for (int i = 0; i < coroutines.Count; i++)
        {
            MonoMgr.Instance.StopCoroutine(coroutines[i]);
        }
        coroutines.Clear();
    }

    // 请优先使用此Destory方法释放资源 效率比析构函数更高
    // 忘记了也没关系，会自动调用析构函数 但是效率会低一些
    public void Destory() {
        // 释放状态机资源
        if (startUpdate) {
            // 停止所有协程
            StopAllCoroutine();
            current_state.OnStateExit();
            MonoMgr.Instance.RemoveUpdate(this.Update);
            states.Clear();
            startUpdate = false;
        }
    }

    // 用来兜底 防止有人忘记调用Destory
    ~FiniteStateMachineBase()
    {
        // 析构函数
        Debug.Log("FiniteStateMachineBase被销毁");
        Destory();
    }
}
