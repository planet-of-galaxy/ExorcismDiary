using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharlieController : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public float move_speed = 10;
    public float speed_scale = 1;
    public ChairlieFsm fsm;
    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        fsm = new ChairlieFsm(gameObject);
        fsm.ChangeToState<ChairlieIdleState>();
    }

    // Update is called once per frame
    void Update()
    {
        //navMeshAgent.SetDestination(target.position);
        //navMeshAgent.speed = move_speed * speed_scale;
        //animator.SetFloat("MoveSpeed", speed_scale);

        //if (Vector3.Distance(transform.position,target.position) < 3)
        //{
        //    animator.SetTrigger("Attack");
        //}
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
        fsm?.Destory();
        fsm = null;
    }
}
