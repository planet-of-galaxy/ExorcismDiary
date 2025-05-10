using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperManController : MonoBehaviour, IControlable
{
    // 相机机位
    private Transform camPoint = null;
    // 是否正在被控制
    private bool isOnControll = false;
    // 相机
    private Camera my_camera = null;
    private CharacterController characterController;
    public Transform CameraPoint => camPoint;

    public GameObject Owner => gameObject;

    public bool IsOnControl => isOnControll;

    // Move相关参数提前声明
    private float horizontal;
    private Vector3 move_direction = Vector3.zero;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
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
        Move();
    }

    private void Move() {
        horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0)
        {
            move_direction = -transform.right * horizontal;
            characterController.Move(move_direction * Time.deltaTime);
        }
    }

    public void SetCamPoint(Transform camPoint) {
        this.camPoint = camPoint;
    }
}
