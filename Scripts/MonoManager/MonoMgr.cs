using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : SingletonMono<MonoMgr>
{
    public event UnityAction update;
    public event UnityAction lateUpdate;
    public event UnityAction fixUpdate;
    public event UnityAction onGUI;

    // Update is called once per frame
    private void Update()
    {
        update?.Invoke();
    }

    private void FixedUpdate()
    {
        fixUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdate?.Invoke();
    }

    private void OnGUI()
    {
        onGUI?.Invoke();
    }

    public void AddUpdate(UnityAction fun) {
        update += fun;
    }

    public void RemoveUpdate(UnityAction fun) {
        update -= fun;
    }

    public void AddLateUpdate(UnityAction fun) {
        lateUpdate += fun;
    }

    public void RemoveLateUpdate(UnityAction fun) {
        lateUpdate -= fun;
    }

    public void AddFixUpdate(UnityAction fun)
    {
        fixUpdate += fun;
    }

    public void RemoveFixUpdate(UnityAction fun)
    {
        fixUpdate -= fun;
    }

    public void AddOnGUI(UnityAction fun)
    {
        onGUI += fun;
    }

    public void RemoveOnGUI(UnityAction fun)
    {
        onGUI -= fun;
    }
}
