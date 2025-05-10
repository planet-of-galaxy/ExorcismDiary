using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintController : MonoBehaviour, IInteractable
{
    public string listner_name => "PaintListener";

    public Vector3 ui_position => this.transform.position;
    private InteractableListener interactable_listner;
    public GameObject paperMan;
    public Transform camPoint;

    // �Ƿ��ڽ�����
    private bool isInteracting = false;

    public void Interact()
    {
        print("Interact with PaintController");

        // ���뽻��
        if (!isInteracting) {
            paperMan.GetComponent<PaperManController>().SetCamPoint(camPoint);
            ControllerManager.Instance.ChangeController(paperMan.GetComponent<PaperManController>() as IControlable);
            isInteracting = true;
        }
        // �˳�����
        else
        {
            ControllerManager.Instance.ComeBackController();
            isInteracting = false;
        }
        UIManager.Instance.ShowHint("��Ҫֽ��");
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
}
