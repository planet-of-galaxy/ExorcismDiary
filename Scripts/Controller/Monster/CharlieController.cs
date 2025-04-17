using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharlieController : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public float move_speed = 8;
    public float speed_scale = 1;
    public ChairlieFsm fsm;
    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        // ��ʼ��״̬�� �����Զ�ִ��Init ���л�����һ��״̬
        fsm = new ChairlieFsm(gameObject);

    }

    public void AttackEvent()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward,1,LayerMask.GetMask("Player"));

        if (colliders.Length == 0)
            return;

        colliders[0].transform.GetComponent<PlayerControllerBase>()?.Wound();
    }

    private void OnDestroy()
    {
        if (MonoMgr.isInstantiated)
        {
            fsm.Destory();
        }
        fsm = null;
    }
}
