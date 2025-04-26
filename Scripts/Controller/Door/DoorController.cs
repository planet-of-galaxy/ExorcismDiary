using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    // 交互逻辑
    public string listner_name => "DoorListener";
    // 可交互时 UI所在的位置
    public Vector3 ui_position => this.transform.position;

    // 门开关动画
    // 门的开关状态
    private bool isOpen = false;
    // 门锁的状态
    private bool isLocked = false;
    // 左侧门的transform
    private Transform leftDoorTransform;
    // 右侧门的transform
    private Transform rightDoorTransform;

    // Start is called before the first frame update
    void Start()
    {
        leftDoorTransform = transform.Find("LeftDoor");
        rightDoorTransform = transform.Find("RightDoor");
    }

    public void Interact()
    {

    }
}
