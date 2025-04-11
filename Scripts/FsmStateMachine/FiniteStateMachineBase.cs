using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ṩ������
/// ChangeToState<T>() ���ã� �л�״̬ ����״̬����������
/// DelayInvoke(float delay, Action action) ���ã��ӳٵ���һ��ί�еķ��� �������Լ������ص�Э�̵���������
/// ChangeFloatGradually(float start, float target, Action<float> action) ���ã���һ��floatֵ�𽥱仯��Ŀ��ֵ �������Լ������ص�Э�̵���������
/// StopCoroutine(Coroutine coroutine) ���ã� �мӾ��м� �ṩֹͣЭ�̵ķ���
/// </summary>
/// <typeparam name="T_StateBase">��״̬�������״̬����</typeparam>
/// <typeparam name="U_Fsm">��״̬�������״̬������</typeparam>
public class FiniteStateMachineBase<T_StateBase, U_Fsm>
    where T_StateBase : StateBase<T_StateBase, U_Fsm>
    where U_Fsm : FiniteStateMachineBase<T_StateBase, U_Fsm>
{
    public T_StateBase current_state;
    protected GameObject agent;
    private Dictionary<string, T_StateBase> states = new();
    private bool startUpdate = false;

    // ����Ϊ�����ߵ�gameObject���� ��ø����� ����˭��״̬��
    public FiniteStateMachineBase(GameObject agent)
    {
        this.agent = agent;
    }

    public virtual void ChangeToState<T>() where T : T_StateBase, new()
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
            states.Add(TAG, current_state as T_StateBase);
        }

        // ����Ŀ��״̬
        current_state.OnStateEnter();
    }

    private void Update() {
        // �ѵ�ǰ״̬��Update�����ŵ�MonoManager��Update��
        current_state?.OnStateUpdate();
    }

    // �ṩ�ӳٵ���һ��ί�еķ���
    public Coroutine DelayInvoke(float delay, Action action) {
        // ����һ��ί��
        return MonoMgr.Instance.StartCoroutine(DelayInvokeCoroutine(delay, action));
    }
    // ��һ��floatֵ�𽥱仯��Ŀ��ֵ
    public Coroutine ChangeFloatGradually(float start, float target, Action<float> action) {
        return MonoMgr.Instance.StartCoroutine(ChangeFloatGraduallyCorouutine(start, target, action));
    }

    // �ṩֹͣЭ�̵ķ���
    public void StopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
            MonoMgr.Instance.StopCoroutine(coroutine);
    }

    // float�����Э�� �߼�ʵ��
    private IEnumerator ChangeFloatGraduallyCorouutine(float start, float target, Action<float> action) {
        while (start != target) {
            start = Mathf.Lerp(start, target, Time.deltaTime);
            action?.Invoke(start);
            yield return null;
        }
    }

    // �ӳٵ��õ�Э�� �߼�ʵ��
    private IEnumerator DelayInvokeCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    // ������ʹ�ô�Destory�����ͷ���Դ Ч�ʱ�������������
    // ������Ҳû��ϵ�����Զ������������� ����Ч�ʻ��һЩ
    public void Destory() {
        // �ͷ�״̬����Դ
        if (startUpdate) {
            current_state.OnStateExit();
            MonoMgr.Instance.RemoveUpdate(this.Update);
            current_state = null;
            states.Clear();
            states = null;
            agent = null;
            startUpdate = false;
        }
    }

    // �������� ��ֹ�������ǵ���Destory
    ~FiniteStateMachineBase()
    {
        // ��������
        Debug.Log("FiniteStateMachineBase������");
        Destory();
    }
}
