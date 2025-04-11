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
        // ��ȡĿ��״̬��TAG
        string TAG = typeof(T).Name;

        // �����ǰ״̬��Ŀ��״̬��ͬ���򲻽����л�
        if (current_state != null && TAG == current_state.GetType().Name)
        {
            return;
        }

        // �����û�п�ʼ����״̬��������ӵ�MonoManager��Update��
        if (!startUpdate)
        {
            MonoMgr.Instance.AddUpdate(this.Update);
            startUpdate = true;
        }

        // �����ǰ״̬��Ϊ�գ������˳���ǰ״̬
        current_state?.OnStateExit();

        // ���״̬�����Ѿ�����Ŀ��״̬����ֱ��ʹ��
        if (states.ContainsKey(TAG))
        {
            current_state = states[TAG];
        }
        // ���򴴽��µ�״̬ʵ�� ���г�ʼ�� ����ӵ�״̬����
        else
        {
            current_state = new T();
            current_state.Init(this as U_Fsm, agent);
            states.Add(TAG, current_state as T);
        }

        // ����Ŀ��״̬
        current_state.OnStateEnter();
    }

    private void Update() {
        current_state?.OnStateUpdate();
    }

    public void Destory() {
        // �ͷ�״̬����Դ
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
