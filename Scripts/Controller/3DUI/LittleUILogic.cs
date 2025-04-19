using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleUILogic : MonoBehaviour
{
    private Image img;
    private Camera cam;
    public PropController father { get; set; }
    // 记录ui原始大小
    private Vector3 defualt_scale;
    // 记录ui现在大小
    private Vector3 current_scale;

    // 当current_scale为1时 cam距ui的距离
    public const float MAX_DISTANCE = 4f;
    // 计算缩放系数
    public const float SCALE_PARAM = 1f/MAX_DISTANCE;

    // UI大小动态变换参数提前声明
    private float distance;
    // cam到UI的向量 用于计算角度
    private Vector3 direction;
    // 判断物品是否可被拾取
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
        // 计算距离
        distance = Vector3.Distance(transform.position, cam.transform.position);
        // 计算scale
        current_scale = defualt_scale * distance * SCALE_PARAM;
        if (current_scale.x > 1) {
            current_scale = defualt_scale;
        }
        // 设置缩放
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
            print("物品已被拾取！！");
            father.PickedUp();
            Destroy(gameObject);
        }
    }
}
