using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleUILogic : MonoBehaviour
{
    private Image img;
    private Camera cam;
    public PropController father { get; set; }
    // ��¼uiԭʼ��С
    private Vector3 defualt_scale;
    // ��¼ui���ڴ�С
    private Vector3 current_scale;

    // ��current_scaleΪ1ʱ cam��ui�ľ���
    public const float MAX_DISTANCE = 4f;
    // ��������ϵ��
    public const float SCALE_PARAM = 1f/MAX_DISTANCE;

    // UI��С��̬�任������ǰ����
    private float distance;
    // cam��UI������ ���ڼ���Ƕ�
    private Vector3 direction;
    // �ж���Ʒ�Ƿ�ɱ�ʰȡ
    private bool isCanPick = false;

    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        cam = Camera.main;
        defualt_scale = img.transform.localScale;
    }

    void LateUpdate()
    {
        SetScale();
        SetRotation();
        SetSprite();
        LisenPickUp();
    }

    private void SetScale() {
        // �������
        distance = Vector3.Distance(transform.position, cam.transform.position);
        // ����scale
        current_scale = defualt_scale * distance * SCALE_PARAM;
        if (current_scale.x > 1) {
            current_scale = defualt_scale;
        }
        // ��������
        img.transform.localScale = current_scale;
    }

    private void SetRotation() {
        transform.LookAt(transform.position + cam.transform.forward);
    }

    private void SetSprite() {
        direction = (transform.position - cam.transform.position).normalized;
        if (distance < 3 && Vector3.Dot(cam.transform.forward, direction) > 0.9)
        {
            img.sprite = LittleUIManager.Instance.GetSprite("E");
            isCanPick = true;
        }
        else {
            img.sprite = LittleUIManager.Instance.GetSprite("this");
            isCanPick = false;
        }
    }

    private void LisenPickUp() {
        if (isCanPick && Input.GetKeyDown(KeyCode.E)) {
            print("��Ʒ�ѱ�ʰȡ����");
            father.PickedUp();
            Destroy(gameObject);
        }
    }
}
