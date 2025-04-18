using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, UIBaseController> UIs = new();
    private const string PATH = "UI/";
    private GameObject canvas;

    public void ShowPanel<T>(bool isImmediately = false) where T : UIBaseController
    {
        string TAG = typeof(T).Name;

        if (!UIs.ContainsKey(TAG)) {
            GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(PATH + TAG),canvas.transform);
            UIs.Add(TAG, ui.GetComponent<T>());
        }

        if (isImmediately)
            UIs[TAG].ShowMe();
        else
            UIs[TAG].FadeIn(null);
    }
    public void RemovePanel<T>(bool isImmediately = false) where T : UIBaseController
    {
        string TAG = typeof(T).Name;
        if (!UIs.ContainsKey(TAG))
        {
            Debug.LogWarning($"UIManager: {TAG} not found");
            return;
        }

        if (isImmediately)
        {
            GameObject.Destroy(UIs[TAG].gameObject);
            UIs.Remove(TAG);
        }
        else {
            UIs[TAG].FadeOut(RemovePanelCallBack);
            UIs.Remove(TAG);
        }
    }

    public void RemovePanelCallBack(UIBaseController ui) {
        GameObject.Destroy(ui.gameObject);
    }

    public override void Init()
    {
        canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        GameObject.DontDestroyOnLoad(canvas);
    }
}
