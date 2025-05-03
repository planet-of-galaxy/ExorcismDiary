using UnityEngine;

public class PropController : MonoBehaviour, IInteractable
{
    public InteractableListener interactable_listner;
    public Vector3 ui_position { get => transform.position; }
    public string listner_name { get => gameObject.name + "_listner"; }
    // ��Ҫ���� �õ��ߵ�ID������
    public int id;
    public int num;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
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
    public void Interact()
    {
        InteractableListnerManager.Instance.RemoveInteractableListner(interactable_listner);
        if (num != 0) {
            PackageManager.Instance.AddProp(id, num);
            Debug.Log("ʰȡ����" + id + "����" + num);
        }
        Destroy(gameObject);
        return;
    }
}
