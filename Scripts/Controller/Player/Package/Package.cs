using System.Collections.Generic;

public class ClubItem {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }

}
public class PropItem { }
public class FileItem { }
public class Package
{
    private List<ClubItem> ClubItems { get; set; }
    private List<PropItem> PropItems { get; set; }
    private List<FileItem> FileItems { get; set; }
    public Package()
    {
        ClubItems = new List<ClubItem>();
        PropItems = new List<PropItem>();
        FileItems = new List<FileItem>();
    }


}