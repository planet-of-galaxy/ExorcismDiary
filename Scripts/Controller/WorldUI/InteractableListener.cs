using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class InteractableListener : MonoBehaviour
{
    private Image img;
    private Camera cam;
    // ui���õ���ͼ��
    public SpriteAtlas atlas;
    // ����Դ ����⵽��Ұ��½�����ʱ ִ������ӿ�
    public IInteractable source;
    // ��¼uiԭʼ��С
    private Vector3 default_scale;
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
    // �ж���Ʒ�Ƿ�ɱ�����
    private bool isCanInteract = false;

    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        // ��ȡWorldUI����� ���Ż���WorldUI������������������
        cam = Camera.main;
        default_scale = img.transform.localScale;
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
        current_scale = default_scale * distance * SCALE_PARAM;
        if (current_scale.x > 1) {
            current_scale = default_scale;
        }
        // ��������
        img.transform.localScale = current_scale;
    }

    private void SetRotation() {
        transform.LookAt(transform.position + cam.transform.forward);
    }

    private void SetSprite() {
        direction = (transform.position - cam.transform.position).normalized;
        if (distance < MAX_DISTANCE && Vector3.Dot(cam.transform.forward, direction) > 0.95)
        {
            img.sprite = atlas.GetSprite("E");
            isCanInteract = true;
        }
        else {
            img.sprite = atlas.GetSprite("this");
            isCanInteract = false;
        }
    }

    public void Bind(IInteractable source) {
        this.source = source;
        gameObject.transform.position = source.ui_position;
        gameObject.name = source.listner_name;
    }

    public void SetAtlas(SpriteAtlas atlas) {
        this.atlas = atlas;
    }

    private void LisenPickUp() {
        if (isCanInteract && Input.GetKeyDown(KeyCode.E)) {
            source.Interact();
        }
    }

}
