
using System.Collections.Generic;
using UnityEngine;

public class PropPanel : MonoBehaviour, IShowItem
{
    private List<IInPackagable> props;
    public Transform propsContainer;
    private int index = 0; // 当前物品索引
    private void Awake()
    {
        props = PackageManager.Instance.GetProps();
    }

    private void Start()
    {
        foreach (var prop in props)
        {
            GameObject item = Instantiate(Resources.Load<GameObject>("UI/Item"), propsContainer);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            item.GetComponent<Item>().Init(prop, this);
            SetItemPosition(item.transform as RectTransform);
        }
    }

    public void ShowItem(int id)
    {
        throw new System.NotImplementedException();
    }

    private void SetItemPosition(RectTransform rectTransform)
    {
        int row = index / 3; // 每行3个物品
        int column = index % 3; // 每列3个物品
        rectTransform.anchoredPosition = new Vector2((column - 1) * 125, (1 - row) * 125); // 设置物品位置
        index++;
    }
}
