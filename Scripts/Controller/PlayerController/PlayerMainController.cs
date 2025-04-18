using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainController : PlayerControllerBase
{
    public CharacterController character_controller;

    public override E_Player_ID ID => E_Player_ID.E_MAIN_PLAYER;

    // �ƶ���ز�����ǰ���� Move()
    private float horizontal;
    private float vertical;
    Vector3 move_direction;

    // ת����ز�����ǰ���� Rotate()
    private float camera_angle;
    private float x; // ���ˮƽ����λ��
    private float y; // �����ֱ����λ��
    private float angle_min = -75f;
    private float angle_max = 75f;

    // ģ��������ز�����ǰ���� simulateGravity()
    private Vector3 velocity = Vector3.zero;
    private float gravity = -24f;
    private float defualt_speed = -2f; // ���ڵ���ʱĬ���ٶȣ�ȷ���ȿ�ʱֱ������

    // ��ץסʱ�ƶ��������
    public Transform catched_point;
    private Coroutine position_coroutine;
    private Coroutine rotation_coroutine;

    // ֹͣUpdate��־
    private bool stop_update = false;


    protected override void Init()
    {
        if (character_controller == null)
            character_controller = GetComponent<CharacterController>();

        PlayerControllerManager.Instance.SetController(this);
        EventCenter.Instance.Subscribe(E_EventType.E_Player_Dead, GameOver);
    }

    // Update is called once per frame
    void Update()
    {
        if (stop_update)
            return;

        Move();
        Rotate();
        simulateGravity();
    }

    private void Move()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        move_direction = (transform.forward * vertical + transform.right * horizontal) * player_data.move_speed;

        if (move_direction != Vector3.zero)
            character_controller.Move(move_direction * Time.deltaTime);
    }

    private void Rotate() {
        x = Input.GetAxis("Mouse X") * player_data.rotate_speed;
        transform.Rotate(Vector3.up * x);

        // ��ֱ����ת�ӽ�
        y = Input.GetAxis("Mouse Y");
        camera_angle -= y * player_data.rotate_speed;
        camera_angle = Mathf.Clamp(camera_angle, angle_min, angle_max);

        camera_point.localRotation = Quaternion.Euler(Vector3.right * camera_angle);
    }

    private void simulateGravity() {
        if (!character_controller.isGrounded)
        {
            character_controller.Move(velocity * Time.deltaTime);
            velocity.y +=  gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = defualt_speed;
        }
    }

    private void GameOver() {
        // �Ժ����һ��ָ��ʱ��������ת����Э��
        //position_coroutine = MonoMgr.Instance.ChangeVector3Gradually(transform.position, catched_point.position, SetPosition, 100);
        //rotation_coroutine =  MonoMgr.Instance.ChangeQuaternionGradually(transform.rotation, catched_point.rotation, SetRotation, 100);
        transform.position = catched_point.position;
        transform.rotation = catched_point.rotation;
        camera_point.localRotation = Quaternion.identity;
        stop_update = true;
    }

    private void SetPosition(Vector3 position) {
        transform.position = position;
    }

    private void SetRotation(Quaternion rotation) {
        transform.rotation = rotation;
    }

    private void OnDestroy()
    {
        if (MonoMgr.isInstantiated) {
            if (position_coroutine != null)
                MonoMgr.Instance.StopCoroutine(position_coroutine);
            if (rotation_coroutine != null)
                MonoMgr.Instance.StopCoroutine(rotation_coroutine);
        }
    }
}
