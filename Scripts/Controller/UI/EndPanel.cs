using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : UIBaseController
{
    public Button continue_btn;
    protected override void Init()
    {
        continue_btn.onClick.AddListener(() =>
        {
            // ¼ÌĞøÓÎÏ·
            UIManager.Instance.RemovePanel<EndPanel>(true);
            SceneMgr.Instance.LoadScene("GameScene");
        });
    }
}
