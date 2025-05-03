
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private IInPackagable item;
    private IShowItem father;
    public Image icon;
    public TextMeshProUGUI num;

    public void Init(IInPackagable item, IShowItem father) {
        this.item = item;
        icon.sprite = Resources.Load<Sprite>(item.Icon);
        num.text = item.Num.ToString();
        this.father = father;
    }
}