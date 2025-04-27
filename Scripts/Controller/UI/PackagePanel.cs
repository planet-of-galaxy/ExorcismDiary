using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackagePanel : UIBaseController
{
    // �������
    public GameObject propPanel;
    public Toggle propToggle;
    // �������
    public GameObject clubPanel;
    public Toggle clubToggle;
    // �ļ����
    public GameObject FilePanel;
    public Toggle fileToggle;
    protected override void Init()
    {
        // ���¼�
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
