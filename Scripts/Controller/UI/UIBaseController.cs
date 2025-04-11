using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_UIState {
    E_NONE = 0,
    E_SHOWING,
    E_FADE_IN,
    E_FADE_OUT,
    E_HIDING,
}
public abstract class UIBaseController : MonoBehaviour
{
    private CanvasGroup canvas_group;

    private float fade_time = 0.5f; // ����ʱ��
    private Action<UIBaseController> callback = null; // ��������ص�
    private Coroutine fade_change;
    protected E_UIState current_state = E_UIState.E_NONE; // ��ǰ״̬
    void Awake()
    {
        Init();
        // ����ΪUI�㼶
        gameObject.layer = LayerMask.NameToLayer("UI");
        canvas_group = gameObject.AddComponent<CanvasGroup>();
        ChangeState(E_UIState.E_HIDING);
    }

    protected abstract void Init();
    void Start()
    {
        
    }

    public virtual void ShowMe() {
        ChangeState(E_UIState.E_SHOWING);
    }
    public virtual void HideMe() {
        ChangeState(E_UIState.E_HIDING);
    }
    public virtual void FadeIn(Action<UIBaseController> callback) {
        ChangeState(E_UIState.E_FADE_IN, callback);
    }
    public virtual void FadeOut(Action<UIBaseController> callback) {
        ChangeState(E_UIState.E_FADE_OUT, callback);
    }
    // ���ý���ʱ��
    public void SetFadeTime(float fade_time) {
        this.fade_time = fade_time; 
    } 

    protected virtual void ChangeState(E_UIState new_state, Action<UIBaseController> callback = null)
    {
        switch ((current_state, new_state)) {
            case (_, E_UIState.E_HIDING): // ����״̬ -> ���� ֱ����������
                if (fade_change != null)
                    StopCoroutine(fade_change);

                canvas_group.alpha = 0;
                canvas_group.interactable = false; // ���ý���
                gameObject.SetActive(false);
                break;
            case (_, E_UIState.E_SHOWING): // ����״̬ -> ��ʾ ֱ����ʾ����
                if (fade_change != null)
                    StopCoroutine(fade_change);

                gameObject.SetActive(true);
                canvas_group.interactable = true; // �ָ�����
                canvas_group.alpha = 1;
                break;
            case (E_UIState.E_SHOWING, E_UIState.E_FADE_IN): // ��ʾ -> ���� ֱ�ӵ��ûص�
                callback?.Invoke(this);
                break;
            case (E_UIState.E_FADE_IN, E_UIState.E_FADE_IN): // ���� -> ���� ����µĶ���
                this.callback += callback;
                break;
            case (_, E_UIState.E_FADE_IN): // ����״̬ -> ���� ����Э��
                this.callback += callback;
                gameObject.SetActive(true);

                if (fade_change != null)
                {
                    StopCoroutine(fade_change);
                }

                fade_change = StartCoroutine(FadeChange(canvas_group.alpha, 1)); // ����
                break;
            case (E_UIState.E_NONE, E_UIState.E_FADE_OUT):
            case (E_UIState.E_HIDING, E_UIState.E_FADE_OUT): // ���� -> ���� ֱ�ӵ��ûص�
                callback?.Invoke(this);
                break;
            case (E_UIState.E_FADE_OUT, E_UIState.E_FADE_OUT): // ���� -> ���� ����µĶ���
                this.callback += callback;
                break;
            case (_, E_UIState.E_FADE_OUT): // ����״̬ -> ���� ����Э��
                this.callback += callback;
                if (fade_change != null)
                {
                    StopCoroutine(fade_change);
                }
                fade_change = StartCoroutine(FadeChange(canvas_group.alpha, 0)); // ����
                break;

        }
    }

    private IEnumerator FadeChange(float start_alpha, float end_alpha) {
        float time = 0.0f;
        float incre_time = 1 /(fade_time / 0.1f); // ����ʱ�����0.1s�����ѭ������ ����1����ѭ�����������ÿ�ε�����
        canvas_group.interactable = false; // ���뽥��ʱ�����ý���
        while (Mathf.Abs(end_alpha - canvas_group.alpha) > 0.1) {
            canvas_group.alpha = Mathf.Lerp(start_alpha, end_alpha, time);
            time += 1/ incre_time;
            yield return new WaitForSeconds(0.1f); // ÿ0.1s����һ��
        }
        canvas_group.alpha = end_alpha; // ȷ�����һ�θ��µ�Ŀ��ֵ
        callback?.Invoke(this); // ���ûص�����
        callback = null; // ��ջص�����
        fade_change = null; // ���Э��
        canvas_group.interactable = true; // �ָ�����
        yield break;
    }
}
