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
    // ui能用到的图集
    public SpriteAtlas atlas;
    // 交互源 当检测到玩家按下交互键时 执行这个接口
    public IInteractable source;
    // 记录ui原始大小
    private Vector3 default_scale;
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
    // 判断物品是否可被交互
    private bool isCanInteract = false;

    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        // 获取WorldUI摄像机 可优化成WorldUI摄像机而不是主摄像机
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
        // 计算距离
        distance = Vector3.Distance(transform.position, cam.transform.position);
        // 计算scale
        current_scale = default_scale * distance * SCALE_PARAM;
        if (current_scale.x > 1) {
            current_scale = default_scale;
        }
        // 设置缩放
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
