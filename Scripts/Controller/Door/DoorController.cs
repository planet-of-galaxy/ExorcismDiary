using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour, IInteractable
{
    // �����߼�
    public string listner_name => "DoorListener";
    // �ɽ���ʱ UI���ڵ�λ��
    private Vector3 uiPosition;
    public Vector3 ui_position => uiPosition;

    // �ŵ���Ϊ
    // �ŵĿ���״̬
    private bool isOpen = false;
    // ������״̬
    private bool isLocked = false;
    // ����ŵ�transform
    private Transform leftDoorTransform;
    // �Ҳ��ŵ�transform
    private Transform rightDoorTransform;
    // �����Ŷ���Э��
    private Coroutine leftDoorCoroutine = null;
    private Coroutine rightDoorCoroutine = null;
    // ��̬�ϰ�
    private NavMeshObstacle doorObstacle;

    // ����������
    InteractableListener listener = null;

    // Start is called before the first frame update
    void Start()
    {
        // ��ȡ�ŵ�transform
        leftDoorTransform = transform.Find("LeftDoor");
        rightDoorTransform = transform.Find("RightDoor");
        // ��ȡ��̬�ϰ�
        doorObstacle = GetComponent<NavMeshObstacle>();

        // ����uiλ�� ��������������Ż����Ҳ��Ž���ƫ�� �����˫�������м�
        uiPosition = transform.position + new Vector3(0, 1.5f, 0);
        if (leftDoorTransform != null)
            uiPosition += transform.right;
        if  (rightDoorTransform != null)
                uiPosition += -1 * transform.right;
    }

    public void Interact()
    {
        print("Door Interacted!");
        // ��ȡ���λ��
        Transform camTrans = Camera.main.transform;
        Vector3 direction = Vector3.Cross(Camera.main.transform.position - transform.position, transform.right);
        // �ж�����Ƿ����ŵ�ǰ��
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
        // ����ʱ�رն�̬�ϰ� ����ʱ�򿪶�̬�ϰ�
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
        // ���������ӿ��Ŷ���
        while (myDoor.localRotation != target) {
            myDoor.localRotation = Quaternion.Slerp(myDoor.localRotation, target, Time.deltaTime * 2f);
            yield return null;
        }
        myDoor.localRotation = target;
    }

    private void OpenDoor(Transform door, Quaternion targetRotation, ref Coroutine coroutine) {
        // ������� ����һ����Ϊnull
        if (door == null) return;
        // �Ƚ�����һ��Э��
        if (coroutine != null) StopCoroutine(coroutine);
        // ������Э��
        coroutine = StartCoroutine(OpenDoorCoroutine(door, door.localRotation, targetRotation));
    }
}
