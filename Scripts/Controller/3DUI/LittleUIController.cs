using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LittleUIController : MonoBehaviour
{
    public Transform ui_position;
    public GameObject ui_gameObject;
    void Awake() {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            print("检测到玩家进入");
            ui_gameObject= LittleUIManager.Instance.Show(ui_position, "this");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("检测到玩家离开");
            if (ui_gameObject != null) {
                LittleUIManager.Instance.Remove(ui_gameObject);
            }
        }
    }
}
