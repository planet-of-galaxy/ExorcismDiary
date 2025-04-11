using System;
using System.Collections;
using UnityEngine;
public class TimeManager : SingletonMono<TimeManager>
{
    private IEnumerator DelayFunc(float delay, Action action) {
        print("延迟" + delay + "秒执行");
        yield return new WaitForSeconds(delay);
        print("执行");
        action?.Invoke();
    }

    public Coroutine DelayInvoke(float delay, Action action)
    {
        return StartCoroutine(DelayFunc(delay, action));
    }

    public void CancelInvoke(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}