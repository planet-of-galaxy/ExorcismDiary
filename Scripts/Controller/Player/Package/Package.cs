using System.Collections.Generic;
using UnityEngine;
public class Package
{
    private List<IInPackagable> props = new();
    private List<IInPackagable> clubs = new();

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

    public void ConsumeProp(int id)
    {
        int index = foundPropByID(id);
        if (index != -1) // 如果背包中有该物品
        {
            props[index].Num -= 1;
            Debug.Log(props[index].Name + "道具数量减少" + 1);
            if (props[index].Num <= 0)
            {
                props.RemoveAt(index);
            }
        }
        else // 如果背包中没有该物品
        {
            Debug.Log("背包中没有该道具");
        }
    }

    public void ConsumeClub(int id)
    {
        int index = foundClubByID(id);
        if (index != -1) // 如果背包中有该物品
        {
            clubs[index].Num -= 1;
            if (clubs[index].Num <= 0)
            {
                clubs.RemoveAt(index);
            }
        }
        else // 如果背包中没有该物品
        {
            Debug.Log("背包中没有该道具");
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
    public int foundPropByID(int id) {
        for (int i = 0;i<props.Count;i++) {
            if (props[i].ID == id)
            {
                return i;
            }
        }
        return -1;
    }

    // 拾取道具逻辑
    public void AddClub(int id, int num)
    {
        int index = foundClubByID(id);
        if (index != -1) // 如果背包中有该物品
        {
            clubs[index].Num += num;
            Debug.Log(clubs[index].Name + "道具数量增加" + num);
        }
        else // 如果背包中没有该物品
        {
            IInPackagable club = GameDataManager.Instance.GetClubByID(id);
            club.Num = num; // 设置物品数量为num
            if (club != null)
            {
                clubs.Add(club);
            }
            Debug.Log("拾取道具" + club.Name);
        }
    }
    public List<IInPackagable> GetClubs()
    {
        return clubs;
    }
    /// <summary>
    /// 寻找背包中是否有该物品
    //  如果有，返回物品在背包中的位置
    /// 如果没有，返回-1
    /// </summary>
    public int foundClubByID(int id)
    {
        for (int i = 0; i < clubs.Count; i++)
        {
            if (clubs[i].ID == id)
            {
                return i;
            }
        }
        return -1;
    }
}