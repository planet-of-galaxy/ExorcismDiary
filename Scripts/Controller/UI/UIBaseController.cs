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

    private float fade_time = 0.5f; // 渐变时间
    private Action<UIBaseController> callback = null; // 渐变结束回调
    private Coroutine fade_change;
    protected E_UIState current_state = E_UIState.E_NONE; // 当前状态
    void Awake()
    {
        Init();
        // 设置为UI层级
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
    // 设置渐变时间
    public void SetFadeTime(float fade_time) {
        this.fade_time = fade_time; 
    } 

    protected virtual void ChangeState(E_UIState new_state, Action<UIBaseController> callback = null)
    {
        switch ((current_state, new_state)) {
            case (_, E_UIState.E_HIDING): // 其他状态 -> 隐藏 直接隐藏物体
                if (fade_change != null)
                    StopCoroutine(fade_change);

                canvas_group.alpha = 0;
                canvas_group.interactable = false; // 禁用交互
                gameObject.SetActive(false);
                break;
            case (_, E_UIState.E_SHOWING): // 其他状态 -> 显示 直接显示物体
                if (fade_change != null)
                    StopCoroutine(fade_change);

                gameObject.SetActive(true);
                canvas_group.interactable = true; // 恢复交互
                canvas_group.alpha = 1;
                break;
            case (E_UIState.E_SHOWING, E_UIState.E_FADE_IN): // 显示 -> 渐入 直接调用回调
                callback?.Invoke(this);
                break;
            case (E_UIState.E_FADE_IN, E_UIState.E_FADE_IN): // 渐入 -> 渐入 添加新的订阅
                this.callback += callback;
                break;
            case (_, E_UIState.E_FADE_IN): // 其他状态 -> 渐入 开启协程
                this.callback += callback;
                gameObject.SetActive(true);

                if (fade_change != null)
                {
                    StopCoroutine(fade_change);
                }

                fade_change = StartCoroutine(FadeChange(canvas_group.alpha, 1)); // 渐入
                break;
            case (E_UIState.E_NONE, E_UIState.E_FADE_OUT):
            case (E_UIState.E_HIDING, E_UIState.E_FADE_OUT): // 隐藏 -> 渐出 直接调用回调
                callback?.Invoke(this);
                break;
            case (E_UIState.E_FADE_OUT, E_UIState.E_FADE_OUT): // 渐出 -> 渐出 添加新的订阅
                this.callback += callback;
                break;
            case (_, E_UIState.E_FADE_OUT): // 其他状态 -> 渐出 开启协程
                this.callback += callback;
                if (fade_change != null)
                {
                    StopCoroutine(fade_change);
                }
                fade_change = StartCoroutine(FadeChange(canvas_group.alpha, 0)); // 渐出
                break;

        }
    }

    private IEnumerator FadeChange(float start_alpha, float end_alpha) {
        float time = 0.0f;
        float incre_time = 1 /(fade_time / 0.1f); // 渐变时间除以0.1s计算出循环次数 再用1除以循环次数，算出每次的增量
        canvas_group.interactable = false; // 渐入渐出时，禁用交互
        while (Mathf.Abs(end_alpha - canvas_group.alpha) > 0.1) {
            canvas_group.alpha = Mathf.Lerp(start_alpha, end_alpha, time);
            time += 1/ incre_time;
            yield return new WaitForSeconds(0.1f); // 每0.1s更新一次
        }
        canvas_group.alpha = end_alpha; // 确保最后一次更新到目标值
        callback?.Invoke(this); // 调用回调函数
        callback = null; // 清空回调函数
        fade_change = null; // 清空协程
        canvas_group.interactable = true; // 恢复交互
        yield break;
    }
}
