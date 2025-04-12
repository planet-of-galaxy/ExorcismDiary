using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ṩ������
/// ChangeToState<T>() ���ã� �л�״̬ ����״̬����������
/// �ӳٷ��� ���ڷ��� �ƽ������ȷ���ֵΪCoroutine�ķ��� ״̬�����Զ�����Э�̵��������� ������Ҳ�����ֶ�����StopFsmCoroutine������StopAllCoroutineֹͣЭ��
/// DelayInvoke(float delay, Action action) ���ã��ӳٵ���һ��ί�еķ���
/// ChangeFloatGradually(float start, float target, Action<float> action) ���ã���һ��floatֵ�𽥱仯��Ŀ��ֵ
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
    // ״̬����״̬�ֵ�
    private Dictionary<string, T_StateBase> states = new();
    // ����״̬�����Э�� ״̬�л�ʱ���Զ�ֹͣ
    private List<Coroutine> coroutines = new();
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
        // ֹͣ����Э��
        StopAllCoroutine();

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
        Coroutine crtine = MonoMgr.Instance.DelayInvoke(delay, action);
        // ��Э����ӵ�״̬����
        coroutines.Add(crtine);
        return crtine;
    }
    // ��һ��floatֵ�𽥱仯��Ŀ��ֵ
    public Coroutine ChangeFloatGradually(float start, float target, Action<float> action, float scale = 1) {
        Coroutine crtine = MonoMgr.Instance.ChangeFloatGradually(start, target, action, scale);
        // ��Э����ӵ�״̬����
        coroutines.Add(crtine);
        return crtine;
    }
    // ��һ��Vector3ֵ�𽥱仯��Ŀ��ֵ
    public Coroutine ChangeVector3Gradually(Vector3 start, Vector3 target, Action<Vector3> action, float scale = 1)
    {
        Coroutine crtine = MonoMgr.Instance.ChangeVector3Gradually(start, target, action, scale);
        // ��Э����ӵ�״̬����
        coroutines.Add(crtine);
        return crtine;
    }
    // ��һ��Quaternionֵ�𽥱仯��Ŀ��ֵ
    public Coroutine ChangeQuaternionGradually(Quaternion start, Quaternion target, Action<Quaternion> action, float scale = 1)
    {
        Coroutine crtine = MonoMgr.Instance.ChangeQuaternionGradually(start, target, action, scale);
        // ��Э����ӵ�״̬����
        coroutines.Add(crtine);
        return crtine;
    }
    // �ṩ���ڵ���һ��ί�еķ���
    public Coroutine StartRepeatingAction(float interval, Action action) {
        Coroutine crtion = MonoMgr.Instance.StartRepeatingAction(interval, action);
        // ��Э����ӵ�״̬����
        coroutines.Add(crtion);
        return crtion;
    }

    // �ṩֹͣЭ�̵ķ���
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
    // ֹͣ����Э��
    public void StopAllCoroutine()
    {
        for (int i = 0; i < coroutines.Count; i++)
        {
            MonoMgr.Instance.StopCoroutine(coroutines[i]);
        }
        coroutines.Clear();
    }

    // ������ʹ�ô�Destory�����ͷ���Դ Ч�ʱ�������������
    // ������Ҳû��ϵ�����Զ������������� ����Ч�ʻ��һЩ
    public void Destory() {
        // �ͷ�״̬����Դ
        if (startUpdate) {
            // ֹͣ����Э��
            StopAllCoroutine();
            current_state.OnStateExit();
            MonoMgr.Instance.RemoveUpdate(this.Update);
            states.Clear();
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
