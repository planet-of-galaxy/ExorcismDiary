using System.Collections.Generic;
using UnityEngine;
public class Package
{
    private List<IInPackagable> props = new();

    // 拾取道具逻辑
    public void AddProp(int id, int num) {
        int index = foundPropByID(id);
        if (index != -1) // 如果背包中有该物品
        {
            props[index].Num += num;
            Debug.Log(props[index].Name + "道具数量增加" + num);
        }
        else // 如果背包中没有该物品
        {
            IInPackagable prop = GameDataManager.Instance.GetPropByID(id);
            prop.Num = num; // 设置物品数量为num
            if (prop != null)
            {
                props.Add(prop);
            }
            Debug.Log("拾取道具" + prop.Name);
        }
    }
    public List<IInPackagable> GetProps()
    {
        return props;
    }
    /// <summary>
    /// 寻找背包中是否有该物品
    //  如果有，返回物品在背包中的位置
    /// 如果没有，返回-1
    /// </summary>
    private int foundPropByID(int id) {
        for (int i = 0;i<props.Count;i++) {
            if (props[i].ID == id)
            {
                return i;
            }
        }
        return -1;
    }
}