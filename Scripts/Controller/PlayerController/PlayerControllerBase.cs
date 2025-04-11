using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerControllerBase : MonoBehaviour      
{
    public Transform camera_point;
    protected abstract void Init();
    private void Awake()
    {
        player_data = PlayerControllerManager.Instance.GetPlayerData((int)ID);

        if (camera_point == null)
            camera_point = transform.Find("camera_point");
        if (camera_point == null)
            Debug.LogError("请添加相机位置");

        SetActive(false, null);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Init();
    }

    public abstract E_Player_ID ID { get; }
    protected PlayerData player_data;

    public virtual void SetActive(bool isActive, Camera camera)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
            camera.transform.SetParent(camera_point);
            camera.transform.localPosition = Vector3.zero;
        }
        else {
            camera?.transform.SetParent(null);
        }
    }

    public void Wound() {
        // 游戏结束
        UIManager.Instance.ShowPanel<EndPanel>();
        AudioManager.Instance.CloseBackMusic();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
