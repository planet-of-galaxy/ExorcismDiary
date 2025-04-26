using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    // �����߼�
    public string listner_name => "DoorListener";
    // �ɽ���ʱ UI���ڵ�λ��
    public Vector3 ui_position => this.transform.position + Vector3.up;

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
    // Ŀ����Ԫ��
    Quaternion targetLeftRotation;
    Quaternion targetRightRotation;

    // ����������
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
        // ��ȡ���λ��
        Transform camTrans = Camera.main.transform;
        Vector3 direction = Vector3.Cross(Camera.main.transform.position - transform.position, transform.right);
        // �ж�����Ƿ����ŵ�ǰ��
        bool isFront = direction.y > 0 ? true : false;

        // ������ת�Ƕ�
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

        // �Ƚ�����һ��Э��
        if (leftDoorCoroutine != null) StopCoroutine(leftDoorCoroutine);
        if (rightDoorCoroutine != null) StopCoroutine(rightDoorCoroutine);
        // ������Э��
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
        // ���������ӿ��Ŷ���
        while (myDoor.localRotation != target) {
            myDoor.localRotation = Quaternion.Slerp(myDoor.localRotation, target, Time.deltaTime * 2f);
            yield return null;
        }
        myDoor.localRotation = target;
    }
}
