using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.Editor;
#endif

public class Test : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_EDITOR
        GameObject go = EditorResMgr.Instance.LoadEditorRes<GameObject>("Cube");
        Instantiate(go);
#endif
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
