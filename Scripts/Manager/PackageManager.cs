
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

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

    public bool ContainsProp(int id)
    {
        int ret = package.foundPropByID(id);

        return ret == -1 ? false : true;
    }

    public bool ContainsClub(int id)
    {
        int ret = package.foundClubByID(id);

        return ret == -1 ? false : true;
    }
    public void ConsumeProp(int id)
    {
        package.ConsumeProp(id);
    }

    public void ConsumeClub(int id)
    {
        package.ConsumeClub(id);
    }
}
