
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PropItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerExit(PointerEventData eventData)
    {
        print("OnPointerExit");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("OnPointerEnter");
        father.ShowItem(item.ID);
    }
}