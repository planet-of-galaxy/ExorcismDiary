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

    // ����������һ������������
    // ����Ϊ����ui ���ڵ�λ��
    public InteractableListener CreateInteractableListener(IInteractable source) {
        // ��ǰ��������ֵ
        InteractableListener listner;

        // �½�һ��gameObject����װ��Image��Sprite
        GameObject ui_gameObject = new GameObject();
        // �Ѳ㼶���ó�WorldUI ���Բ����� ֻҪCanvas��WorldUI�Ϳ���
        // ui_gameObject.layer = LayerMask.NameToLayer("WorldUI");
        // ���һ��Image�ű�
        Image img = ui_gameObject.AddComponent<Image>();
        // ����ͼƬ��С
        img.rectTransform.sizeDelta = new Vector3(0.3f, 0.3f, 0);
        // ���ø����� ȷ��UI�ܱ���ȷ��Canvas��Ⱦ
        ui_gameObject.transform.SetParent(canvas.transform, false);


        // ���3DUI�߼�
        listner = ui_gameObject.AddComponent<InteractableListener>();
        // Ϊ������ָ��uiͼ���Թ��л�ͼƬ
        listner.SetAtlas(atlas);
        // �󶨽���Դ�뽻��������
        listner.Bind(source);
        return listner;
        
    }

    // ɾ���������� ��ɾ������޵ĸ����� �Խ�ʡ����
    public void RemoveInteractableListner(InteractableListener listner) {
        GameObject.Destroy(listner.gameObject);
    }
}
