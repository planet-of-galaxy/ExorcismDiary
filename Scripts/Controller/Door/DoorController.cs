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

    // 所需钥匙 默认-1表示不需要钥匙
    public int keyID = -1;

    // 门的行为
    // 门的开关状态
    private bool isOpen = false;
    // 左侧门的transform
    private Transform leftDoorTransform;
    // 右侧门的transform
    private Transform rightDoorTransform;
    // 左侧门锁
    private Transform leftDoorLockTransform;
    // 右侧门锁
    private Transform rightDoorLockTransform;
    // 是否上锁
    private bool isLocked = false;
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
        // 获取门锁的transform
        leftDoorLockTransform = transform.Find("LeftDoor/LeftLock");
        rightDoorLockTransform = transform.Find("RightDoor/RightLock");
        // 门锁中有一个不为空 那么就是上锁状态
        if (leftDoorLockTransform != null || rightDoorLockTransform != null) {
            isLocked = true;
        }
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

        // 先判断是否上锁
        if (isLocked) {
            if (!isFront) {
                UIManager.Instance.ShowHint("需要从另一侧打开!");
                return;
            }
            Unlock();
            return;
        }

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

    // 返回false表示开锁失败 返回true表示开锁成功
    private bool Unlock() {
        if (keyID != -1 && !PackageManager.Instance.ContainsClub(keyID)) {
            UIManager.Instance.ShowHint("需要钥匙!");
            return false;
        }
        // 如果需要钥匙则消耗钥匙
        if (keyID != -1 && PackageManager.Instance.ContainsClub(keyID))
        {
            PackageManager.Instance.ConsumeClub(keyID); // 消耗钥匙
        }
        // 销毁门锁
        if (leftDoorLockTransform != null)
            Destroy(leftDoorLockTransform.gameObject);
        if (rightDoorLockTransform != null)
            Destroy(rightDoorLockTransform.gameObject);

        // 设置为未上锁状态
        isLocked = false;

        UIManager.Instance.ShowHint("开锁成功!");
        return true;
    }
}
