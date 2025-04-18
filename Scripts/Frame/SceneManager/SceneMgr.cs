using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{
    public override void Init()
    {
    }

    public void LoadScene(string scene_name) {
        Clear(); 
        SceneManager.LoadScene(scene_name);
    }

    public void Clear()
    {
        // 重置各大Manager
        MonoMgr.Instance.Clear();
        AudioManager.Instance.Clear();
        EventCenter.Instance.Clear();
    }
}
