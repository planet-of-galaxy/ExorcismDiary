using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    // �����߼�
    public string listner_name => "DoorListener";
    // �ɽ���ʱ UI���ڵ�λ��
    public Vector3 ui_position => this.transform.position;

    // �ſ��ض���
    // �ŵĿ���״̬
    private bool isOpen = false;
    // ������״̬
    private bool isLocked = false;
    // ����ŵ�transform
    private Transform leftDoorTransform;
    // �Ҳ��ŵ�transform
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
