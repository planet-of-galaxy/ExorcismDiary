using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintController : UIBaseController
{
    public TextMeshProUGUI hintText;
    private float timer;
    private float duringTime = 2f; // 提示信息持续时间
    private float fadeTime = 0.5f; // 淡出效果持续时间

    protected override void Init()
    {
        hintText.text = "";
        gameObject.SetActive(false);
    }

    // 显示提示信息 2秒后逐渐消失
    public void ShowHint(string hint)
    {
        hintText.text = hint;
        ShowMe();
        timer = 0;
    }

    private void Update()
    {
        if (canvas_group.alpha <= 0) {
            HideMe();
        }
        if (timer > duringTime) {
            canvas_group.alpha -= Time.deltaTime / fadeTime;
        }
        timer += Time.deltaTime;
    }
}
