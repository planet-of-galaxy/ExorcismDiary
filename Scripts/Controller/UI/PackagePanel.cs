using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackagePanel : UIBaseController
{
    // 道具面板
    public GameObject propPanel;
    public Toggle propToggle;
    // 线索面板
    public GameObject clubPanel;
    public Toggle clubToggle;
    // 文件面板
    public GameObject FilePanel;
    public Toggle fileToggle;
    protected override void Init()
    {
        // 绑定事件
        propToggle.onValueChanged.AddListener((value) => {
            propPanel.SetActive(value);
        });
        clubToggle.onValueChanged.AddListener((value) => {
            clubPanel.SetActive(value);
        });
        fileToggle.onValueChanged.AddListener((value) => {
            FilePanel.SetActive(value);
        });
    }
}
