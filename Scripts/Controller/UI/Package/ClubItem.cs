using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClubItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private IInPackagable item;
    private IShowItem father;
    public Image icon;
    public TextMeshProUGUI itemName;
    public GameObject back;

    public void Init(IInPackagable item, IShowItem father)
    {
        this.item = item;
        icon.sprite = Resources.Load<Sprite>(item.Icon);
        itemName.text = item.Name.ToString();
        this.father = father;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("OnPointerExit");
        back.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("OnPointerEnter");
        father.ShowItem(item.ID);
        back.SetActive(true);
    }

    void OnDestroy()
    {
        print("ClubItem Destroyed");
        item = null;
        father = null;
        icon.sprite = null;
        itemName.text = string.Empty;
        Destroy(gameObject);
    }
}
