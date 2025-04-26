using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    // 交互逻辑
    public string listner_name => "DoorListener";
    // 可交互时 UI所在的位置
    public Vector3 ui_position => this.transform.position + Vector3.up;

    // 门的行为
    // 门的开关状态
    private bool isOpen = false;
    // 门锁的状态
    private bool isLocked = false;
    // 左侧门的transform
    private Transform leftDoorTransform;
    // 右侧门的transform
    private Transform rightDoorTransform;
    // 开关门动画协程
    private Coroutine leftDoorCoroutine = null;
    private Coroutine rightDoorCoroutine = null;
    // 目标四元数
    Quaternion targetLeftRotation;
    Quaternion targetRightRotation;

    // 交互监听器
    InteractableListener listener = null;

    // Start is called before the first frame update
    void Start()
    {
        leftDoorTransform = transform.Find("LeftDoor");
        rightDoorTransform = transform.Find("RightDoor");
    }

    public void Interact()
    {
        print("Door Interacted!");
        // 获取玩家位置
        Transform camTrans = Camera.main.transform;
        Vector3 direction = Vector3.Cross(Camera.main.transform.position - transform.position, transform.right);
        // 判断玩家是否在门的前面
        bool isFront = direction.y > 0 ? true : false;

        // 计算旋转角度
        if (isOpen)
        {
            targetLeftRotation = Quaternion.Euler(0, 0, 0);
            targetRightRotation = Quaternion.Euler(0, 0, 0);
            isOpen = false;
        }
        else if (!isOpen && isFront)
        {
            targetLeftRotation = Quaternion.Euler(0, -90, 0);
            targetRightRotation = Quaternion.Euler(0, 90, 0);
            isOpen = true;
        }
        else {
            targetLeftRotation = Quaternion.Euler(0, 90, 0);
            targetRightRotation = Quaternion.Euler(0, -90, 0);
            isOpen = true;
        }

        // 先结束上一个协程
        if (leftDoorCoroutine != null) StopCoroutine(leftDoorCoroutine);
        if (rightDoorCoroutine != null) StopCoroutine(rightDoorCoroutine);
        // 开启新协程
        leftDoorCoroutine = StartCoroutine(OpenDoorCoroutine(leftDoorTransform, leftDoorTransform.localRotation, targetLeftRotation));
        rightDoorCoroutine = StartCoroutine(OpenDoorCoroutine(rightDoorTransform, rightDoorTransform.localRotation, targetRightRotation));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            listener = InteractableListnerManager.Instance.CreateInteractableListener(this);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractableListnerManager.Instance.RemoveInteractableListner(listener);
        }
    }

    private IEnumerator OpenDoorCoroutine(Transform myDoor, Quaternion start, Quaternion target)
    {
        // 这里可以添加开门动画
        while (myDoor.localRotation != target) {
            myDoor.localRotation = Quaternion.Slerp(myDoor.localRotation, target, Time.deltaTime * 2f);
            yield return null;
        }
        myDoor.localRotation = target;
    }
}
