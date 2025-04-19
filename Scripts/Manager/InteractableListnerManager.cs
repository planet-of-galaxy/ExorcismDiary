using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class InteractableListnerManager : Singleton<InteractableListnerManager>
{
    private SpriteAtlas atlas;
    private GameObject canvas;
    public override void Init()
    {
        atlas = ABMgr.Instance.LoadRes <SpriteAtlas>("3dui", "3DUI");
        canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/WorldUICanvas"));
        GameObject.DontDestroyOnLoad(canvas);
    }

    // 创建并返回一个交互监听者
    // 参数为监听ui 所在的位置
    public InteractableListener CreateInteractableListener(IInteractable source) {
        // 提前声明返回值
        InteractableListener listner;

        // 新建一个gameObject用来装载Image和Sprite
        GameObject ui_gameObject = new GameObject();
        // 把层级设置成WorldUI 可以不设置 只要Canvas是WorldUI就可以
        // ui_gameObject.layer = LayerMask.NameToLayer("WorldUI");
        // 添加一个Image脚本
        Image img = ui_gameObject.AddComponent<Image>();
        // 设置图片大小
        img.rectTransform.sizeDelta = new Vector3(0.3f, 0.3f, 0);
        // 设置父物体 确保UI能被正确的Canvas渲染
        ui_gameObject.transform.SetParent(canvas.transform, false);


        // 添加3DUI逻辑
        listner = ui_gameObject.AddComponent<InteractableListener>();
        // 为监听器指定ui图集以供切换图片
        listner.SetAtlas(atlas);
        // 绑定交互源与交互监听器
        listner.Bind(source);
        return listner;
        
    }

    // 删除掉监听者 并删除其寄宿的父对象 以节省性能
    public void RemoveInteractableListner(InteractableListener listner) {
        GameObject.Destroy(listner.gameObject);
    }
}
