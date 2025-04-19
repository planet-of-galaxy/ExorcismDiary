using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LittleUIManager : Singleton<LittleUIManager>
{
    private SpriteAtlas atlas;
    private GameObject canvas;
    public override void Init()
    {
        atlas = ABMgr.Instance.LoadRes <SpriteAtlas>("3dui", "3DUI");
        canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/WorldUICanvas"));
        GameObject.DontDestroyOnLoad(canvas);
    }

    public void Show() {
        Sprite[] sprites = new Sprite[5] ;
        int num = atlas.GetSprites(sprites);
        Debug.Log("共有" + num + "个元素");
        for (int i = 0; i < num && i < sprites.Length; i++) {
            Debug.Log(sprites[i].name);
        }

        Debug.Log("canvas加载结果： " + canvas?.name);
    }

    public GameObject Show(Transform trans, string name) {
        // 获得目标精灵图片
        Sprite ui_sprite = atlas.GetSprite(name);

        // 如果该精灵图片存在
        if (ui_sprite != null) {
            // 新建一个gameObject用来装载Image和Sprite
            GameObject ui_gameObject = new GameObject(name);
            // 添加一个Image脚本
            Image img = ui_gameObject.AddComponent<Image>();
            // 设置精灵图片
            img.sprite = ui_sprite;
            // 设置图片大小
            img.rectTransform.sizeDelta = new Vector3(0.3f, 0.3f, 0);
            //img.rectTransform.localScale = new Vector3(0.005f,0.005f,0);
            // 设置父物体
            ui_gameObject.transform.SetParent(canvas.transform, false);
            // 设置位置
            ui_gameObject.transform.position = trans.position;
            // 添加3DUI逻辑
            ui_gameObject.AddComponent<LittleUILogic>();
            return ui_gameObject;
        } else {
            Debug.Log("不存在： " + name);
            return null;
        }
    }

    public void Remove(GameObject obj) {
        GameObject.Destroy(obj);
    }

    public Sprite GetSprite(string name) {
        return atlas.GetSprite(name);
    }
}
