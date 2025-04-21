using UnityEngine;

public class ControllerManager : SingletonMono<ControllerManager>
{
    private IControlable current_controller;

    private void Update()
    {
        current_controller?.UpdateControl();
    }

    public void ChangeController(IControlable controller) {
        Camera cam;
        if (current_controller != null && current_controller == controller)
            return;

        // 指定摄像机
        if (current_controller == null)
            cam = Camera.main;
        else
        {
            cam = current_controller.OnControlExit();
            cam.transform.SetParent(null);
        }

        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;

        // 调用OnControlEnter 并安置相机
        current_controller = controller;
        current_controller.OnControlEnter(cam);
        cam.gameObject.transform.SetParent(current_controller.CameraPoint, false);

        // IsOnControl需要实现接口的类去更改它的状态
        // current_controller.IsOnControl = true;
    }
}
