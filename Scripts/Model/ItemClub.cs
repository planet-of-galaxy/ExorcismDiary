public class ItemClub : IInPackagable
{
    public int id;
    public string name;
    public string description;
    public string icon_path;
    public int ID => id;

    public string Name => name;

    public string Description => description;

    public string Icon => icon_path;
    public int Num { get; set; }
}
