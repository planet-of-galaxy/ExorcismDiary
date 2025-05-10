using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintController : UIBaseController
{
    public TextMeshProUGUI hintText;
    private float timer;
    private float duringTime = 2f; // ��ʾ��Ϣ����ʱ��
    private float fadeTime = 0.5f; // ����Ч������ʱ��

    protected override void Init()
    {
        hintText.text = "";
        gameObject.SetActive(false);
    }

    // ��ʾ��ʾ��Ϣ 2�������ʧ
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
