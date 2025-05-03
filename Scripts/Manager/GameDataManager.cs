using System.Collections.Generic;

/// <summary>
/// 负责游戏数据的管理
/// 游戏开始时，读取配置文件，初始化游戏数据
/// </summary>
public class GameDataManager : Singleton<GameDataManager>
{
    private List<ItemProp> props;
    private List<ItemClub> clubs;
    public override void Init()
    {

    }

    public IInPackagable GetPropByID(int id) {
        if (props == null)
        {
            LoadProps();
        }

        return props[id];
    }
    public IInPackagable GetClubByID(int id)
    {
        if (clubs == null)
        {
            LoadClubs();
        }

        return clubs[id];
    }
    private void LoadProps() {
        props = JsonMgr.Instance.LoadData<List<ItemProp>>("ItemProp");
    }
    private void LoadClubs()
    {
        clubs = JsonMgr.Instance.LoadData<List<ItemClub>>("ItemClub");
    }
}
