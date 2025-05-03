
using System.Collections.Generic;

public class PackageManager : Singleton<PackageManager>
{
    private Package package;

    public override void Init()
    {
        package = new();
    }

    public void AddProp(int id, int num) {
        package.AddProp(id, num);
    }

    public IInPackagable GetPropByID(int id)
    {
        return GameDataManager.Instance.GetPropByID(id);
    }
    public List<IInPackagable> GetProps() {
        return package.GetProps();
    }

    public void AddClub(int id, int num)
    {
        package.AddClub(id, num);
    }

    public IInPackagable GetClubByID(int id)
    {
        return GameDataManager.Instance.GetClubByID(id);
    }
    public List<IInPackagable> GetClubs()
    {
        return package.GetClubs();
    }
}
