using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;
public class FiniteStateMachineBase<T_State,U_Fsm>
    where T_State : StateBase<T_State, U_Fsm>
    where U_Fsm : FiniteStateMachineBase<T_State, U_Fsm>
{
    public T_State current_state;
    protected GameObject agent;
    private Dictionary<string, T_State> states = new();
    private bool startUpdate = false;

    public FiniteStateMachineBase(GameObject agent)
    {
        this.agent = agent;
        Debug.Log(this.GetType().Name);
    }

    public virtual void ChangeToState<T>() where T : T_State,new()
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
            states.Add(TAG, current_state as T);
        }

        // 进入目标状态
        current_state.OnStateEnter();
    }

    private void Update() {
        current_state?.OnStateUpdate();
    }

    public void Destory() {
        // 释放状态机资源
        if (startUpdate) {
            MonoMgr.Instance.RemoveUpdate(this.Update);
            current_state.OnStateExit();
            current_state = null;
            states.Clear();
            states = null;
            agent = null;
            startUpdate = false;
        }
    }
}
