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
        Debug.Log("����" + num + "��Ԫ��");
        for (int i = 0; i < num && i < sprites.Length; i++) {
            Debug.Log(sprites[i].name);
        }

        Debug.Log("canvas���ؽ���� " + canvas?.name);
    }

    public GameObject Show(Transform trans, string name) {
        // ���Ŀ�꾫��ͼƬ
        Sprite ui_sprite = atlas.GetSprite(name);

        // ����þ���ͼƬ����
        if (ui_sprite != null) {
            // �½�һ��gameObject����װ��Image��Sprite
            GameObject ui_gameObject = new GameObject(name);
            // ���һ��Image�ű�
            Image img = ui_gameObject.AddComponent<Image>();
            // ���þ���ͼƬ
            img.sprite = ui_sprite;
            // ����ͼƬ��С
            img.rectTransform.sizeDelta = new Vector3(0.3f, 0.3f, 0);
            //img.rectTransform.localScale = new Vector3(0.005f,0.005f,0);
            // ���ø�����
            ui_gameObject.transform.SetParent(canvas.transform, false);
            // ����λ��
            ui_gameObject.transform.position = trans.position;
            // ���3DUI�߼�
            ui_gameObject.AddComponent<LittleUILogic>();
            return ui_gameObject;
        } else {
            Debug.Log("�����ڣ� " + name);
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
