using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;
public class FiniteStateMachineBase<T> where T : StateBase<T>
{
    public T current_state;
    protected GameObject agent;
    private Dictionary<string, T> states = new();
    private bool startUpdate = false;

    public FiniteStateMachineBase(GameObject agent)
    {
        this.agent = agent;
        Debug.Log(this.GetType().Name);
    }
    public void SetAgent(GameObject agent)
    {
        this.agent = agent;
    }

    public virtual void ChangeToState<U>() where U : T,new()
    {
        // ��ȡĿ��״̬��TAG
        string TAG = typeof(U).Name;

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
            current_state = new U();
            current_state.Init(this, agent);
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
