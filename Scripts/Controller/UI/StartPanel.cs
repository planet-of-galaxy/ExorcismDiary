using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPanel : UIBaseController
{
    public Button start_btn;
    public Button continue_btn;
    public Button loadGame_btn;
    public Button setting_btn;
    public Button exit_btn;
    protected override void Init()
    {
        start_btn.onClick.AddListener(() => {
            SceneMgr.Instance.LoadScene("GameScene");
            UIManager.Instance.RemovePanel<StartPanel>(true);
        });
        continue_btn.onClick.AddListener(() =>
        {
            // 继续游戏
        });
        loadGame_btn.onClick.AddListener(() =>
        {
            // 加载游戏
        });
        setting_btn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SettingPanel>();
        });
        exit_btn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
