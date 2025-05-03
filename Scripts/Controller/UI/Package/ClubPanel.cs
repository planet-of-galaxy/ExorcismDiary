
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClubPanel : MonoBehaviour, IShowItem
{
    private List<IInPackagable> clubs;
    public Transform clubsContainer;
    private int index = 0; // 当前物品索引
    private void Awake()
    {
        clubs = PackageManager.Instance.GetClubs();
    }

    private void Start()
    {
        foreach (var club in clubs)
        {
            GameObject item = Instantiate(Resources.Load<GameObject>("UI/ClubItem"), clubsContainer);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            item.GetComponent<ClubItem>().Init(club, this);
            SetItemPosition(item.transform as RectTransform);
        }
    }

    public void ShowItem(int id)
    {

    }

    private void SetItemPosition(RectTransform rectTransform)
    {
        rectTransform.anchoredPosition = new Vector2(0, - index * 125); // 设置物品位置
        index++;
    }
}

