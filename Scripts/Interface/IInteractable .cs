
using UnityEngine;

// 可交互的物品只需继承并实现这个接口
// 然后向InteractableListnerManager申请一个监听器就可以使用了
public interface IInteractable
{
    string listner_name { get; }
    Vector3 ui_position { get; } // 可交互时 UI所在的位置
    void Interact();
}
