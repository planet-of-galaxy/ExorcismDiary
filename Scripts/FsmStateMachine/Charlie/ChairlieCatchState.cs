using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairlieCatchState : ChairlieStateBase
{
    private Animator animator;
    public override void OnStateEnter()
    {
        if (animator == null)
            animator = agent.GetComponent<Animator>();

        animator.SetTrigger("Howl");
        fsm.DelayInvoke(2, ShowEnd);
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {

    }

    private void ShowEnd() {
        UIManager.Instance.ShowPanel<EndPanel>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
