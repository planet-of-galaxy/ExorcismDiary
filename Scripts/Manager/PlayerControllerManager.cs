using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class  PlayerData
{
    public int ID;
    public string name;
    public float move_speed = 10.0f;
    public float rotate_speed = 20.0f;
}
public enum E_Player_ID
{
    E_MAIN_PLAYER = 0,
} 
public class PlayerControllerManager : Singleton<PlayerControllerManager>
{
    private PlayerControllerBase current_controller;
    private List<PlayerData> datas = new();
    public override void Init()
    {
        datas = JsonMgr.Instance.LoadData<List<PlayerData>>("PlayerData");
    }

    public void SetController(PlayerControllerBase controller) {
        if (current_controller != null)
            current_controller.SetActive(false, null);

        current_controller = controller;
        current_controller.SetActive(true, Camera.main);
    }

    public PlayerData GetPlayerData(int id)
    {
        // 先不设默认值，如果ID不存在，直接报错
        return datas[id];
    }
}
