using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintController : MonoBehaviour, IInteractable
{
    public string listner_name => "PaintListener";

    public Vector3 ui_position => this.transform.position;
    private InteractableListener interactable_listner;
    private GameObject paperMan;
    public Transform camPoint;
    public Transform clubPoint;

    // 是否在交互中
    private bool isInteracting = false;

    public void Interact()
    {
        // 进入交互
        if (!isInteracting)
        {
            // 先检查背包中是否有纸人
            if (paperMan != null || PackageManager.Instance.ContainsClub(2))
            {
                // 生成纸人
                if (paperMan == null)
                    CreatClubMan();
                paperMan.GetComponent<PaperManController>().SetCamPoint(camPoint);
                ControllerManager.Instance.ChangeController(paperMan.GetComponent<PaperManController>() as IControlable);
                isInteracting = true;
                PackageManager.Instance.ConsumeClub(2); // 消耗纸人
            }
            else
            {
                UIManager.Instance.ShowHint("似乎缺少什么");
            }
        }
        // 退出交互
        else
        {
            ControllerManager.Instance.ComeBackController();
            isInteracting = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            interactable_listner = InteractableListnerManager.Instance.CreateInteractableListener(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (interactable_listner != null)
            {
                InteractableListnerManager.Instance.RemoveInteractableListner(interactable_listner);
                interactable_listner = null;
            }
        }
    }

    private void CreatClubMan() {
        paperMan = Instantiate(Resources.Load<GameObject>("Paint/Man"), clubPoint);
        paperMan.transform.localPosition = Vector3.zero;
        paperMan.transform.localRotation = Quaternion.identity;
    }
}
