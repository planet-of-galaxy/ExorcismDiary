using UnityEngine;

public class PropController : MonoBehaviour, IInteractable
{
    public InteractableListener interactable_listner;
    public Vector3 ui_position { get => transform.position; }
    public string listner_name { get => gameObject.name + "_listner"; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            print("检测到玩家进入");
            interactable_listner = InteractableListnerManager.Instance.CreateInteractableListener(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("检测到玩家离开");
            if (interactable_listner != null)
            {
                InteractableListnerManager.Instance.RemoveInteractableListner(interactable_listner);
                interactable_listner = null;
            }
        }
    }
    public void Interact()
    {
        print("物品已经被拾取");
        InteractableListnerManager.Instance.RemoveInteractableListner(interactable_listner);
        Destroy(gameObject);
    }
}
