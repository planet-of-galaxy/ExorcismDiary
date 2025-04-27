using UnityEngine;

public class MainController : MonoBehaviour, IControlable, ICatchable
{
    private Camera my_camera;
    // 设置相机安放位置
    public Transform camPoint;
    public Transform CameraPoint => camPoint;
    // controller拥有者 返回自己
    public GameObject Owner => gameObject;
    // 是否正在被控制 默认为false
    private bool isOnControll = false;
    public bool IsOnControl => isOnControll;

    // 玩家数据
    private PlayerData playerData;

    // 移动相关参数提前声明 Move()
    private CharacterController characterController;
    private float horizontal;
    private float vertical;
    Vector3 move_direction;

    // 模拟重力相关参数提前声明 simulateGravity()
    private Vector3 velocity = Vector3.zero;
    private float gravity = -24f;
    private float defualt_speed = -2f; // 当在地面时默认速度，确保踩空时直接下落

    // 转向相关参数提前声明 Rotate()
    private float camera_angle;
    private float x; // 鼠标水平方向位移
    private float y; // 鼠标竖直方向位移
    private float angle_min = -75f;
    private float angle_max = 75f;

    // 暴露位置
    public static Transform playerTransform;
    // 失控
    private bool isOutOfControl = false;
    public bool IsOutOfControl => isOutOfControl;

    // 背包控制
    private bool isPackageOn = false;

    void Awake() {
        // 获取玩家信息
        playerData = PlayerControllerManager.Instance.GetPlayerData(0);

        if (characterController == null)
            characterController = gameObject.GetComponent<CharacterController>();

        playerTransform = gameObject.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
    void Start() {
        // 申请控制权
        ControllerManager.Instance.ChangeController(this);
    }

    void LateUpdate() {
        if (isOutOfControl) return;
        simulateGravity();
    }

    public void OnControlEnter(Camera camera)
    {
        isOnControll = true;
        my_camera = camera;
    }

    public Camera OnControlExit()
    {
        isOnControll = false;
        return my_camera;
    }
     
    public void UpdateControl()
    {
        PackageListner();
        if (isOutOfControl) return;
        Move();
        Rotate();
    }

    private void Move()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        move_direction = (transform.forward * vertical + transform.right * horizontal) * playerData.move_speed;

        if (move_direction != Vector3.zero)
            characterController.Move(move_direction * Time.deltaTime);
    }

    private void Rotate()
    {
        x = Input.GetAxis("Mouse X") * playerData.rotate_speed;
        transform.Rotate(Vector3.up * x);

        // 垂直方向转视角
        y = Input.GetAxis("Mouse Y");
        camera_angle -= y * playerData.rotate_speed;
        camera_angle = Mathf.Clamp(camera_angle, angle_min, angle_max);

        camPoint.localRotation = Quaternion.Euler(Vector3.right * camera_angle);
    }

    private void simulateGravity()
    {
        if (!characterController.isGrounded)
        {
            characterController.Move(velocity * Time.deltaTime);
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = defualt_speed;
        }
    }

    private void PackageListner() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (isPackageOn)
            {
                // 关闭道具面板
                UIManager.Instance.RemovePanel<PackagePanel>();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                isPackageOn = false;
                isOutOfControl = false;
            }
            else {
                // 打开道具面板
                UIManager.Instance.ShowPanel<PackagePanel>();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                isPackageOn = true;
                isOutOfControl = true;
            }
        }
    }

    public Transform Catched()
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_Player_Dead);
        return transform;
    }

    public void OutOfControl(bool isOutOfControl) {
        this.isOutOfControl = isOutOfControl;
        camPoint.localRotation = Quaternion.identity;
        camPoint.localPosition = Vector3.zero;
    }
}
