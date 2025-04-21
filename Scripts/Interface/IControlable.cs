using UnityEngine;

public interface IControlable
{
    Transform CameraPoint { get;}
    GameObject Owner { get;}
    bool IsOnControl { get;}
    void UpdateControl();
    void OnControlEnter(Camera camera);
    Camera OnControlExit();
}
