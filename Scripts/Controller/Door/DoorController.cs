using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour, IInteractable
{
    // 交互逻辑
    public string listner_name => "DoorListener";
    // 可交互时 UI所在的位置
    private Vector3 uiPosition;
    public Vector3 ui_position => uiPosition;

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
    // 动态障碍
    private NavMeshObstacle doorObstacle;

    // 交互监听器
    InteractableListener listener = null;

    // Start is called before the first frame update
    void Start()
    {
        // 获取门的transform
        leftDoorTransform = transform.Find("LeftDoor");
        rightDoorTransform = transform.Find("RightDoor");
        // 获取动态障碍
        doorObstacle = GetComponent<NavMeshObstacle>();

        // 计算ui位置 根据它是是左侧门还是右侧门进行偏移 如果是双门则在中间
        uiPosition = transform.position + new Vector3(0, 1.5f, 0);
        if (leftDoorTransform != null)
            uiPosition += transform.right;
        if  (rightDoorTransform != null)
                uiPosition += -1 * transform.right;
    }

    public void Interact()
    {
        print("Door Interacted!");
        // 获取玩家位置
        Transform camTrans = Camera.main.transform;
        Vector3 direction = Vector3.Cross(Camera.main.transform.position - transform.position, transform.right);
        // 判断玩家是否在门的前面
        bool isFront = direction.y > 0 ? true : false;

        if (isOpen)
        {
            OpenDoor(leftDoorTransform, Quaternion.Euler(0,0,0),ref leftDoorCoroutine);
            OpenDoor(rightDoorTransform, Quaternion.Euler(0, 0, 0), ref rightDoorCoroutine);
            isOpen = false;
        }
        else {
            int flag = isFront ? -1 : 1;
            OpenDoor(leftDoorTransform, Quaternion.Euler(0, 90 * flag, 0), ref leftDoorCoroutine);
            OpenDoor(rightDoorTransform, Quaternion.Euler(0, -90 * flag, 0), ref rightDoorCoroutine);
            isOpen = true;
        }
        // 开门时关闭动态障碍 关门时打开动态障碍
        doorObstacle.enabled = !isOpen;
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

    private void OpenDoor(Transform door, Quaternion targetRotation, ref Coroutine coroutine) {
        // 单门情况 会有一个门为null
        if (door == null) return;
        // 先结束上一个协程
        if (coroutine != null) StopCoroutine(coroutine);
        // 开启新协程
        coroutine = StartCoroutine(OpenDoorCoroutine(door, door.localRotation, targetRotation));
    }
}
