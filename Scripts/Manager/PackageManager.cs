
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

    public List<IInPackagable> GetProps() {
        return package.GetProps();
    }
}
