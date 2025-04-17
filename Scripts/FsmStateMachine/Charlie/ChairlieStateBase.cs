using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ChairlieStateBase : StateBase<ChairlieStateBase, ChairlieFsm>
{
    protected GameObject player;
    protected Vector3 target;

    // Ѱ��Ŀ����ر�����ǰ����
    private Ray agentToPlayer;
    private float cos_angle;
    private RaycastHit hitInfo;
    protected virtual bool FindTarget() {
        player = PlayerControllerManager.Instance.GetController();
        target = player.transform.position;

        if (Vector3.Distance(target, agent.transform.position) < 10) {
            cos_angle = Vector3.Dot((target - agent.transform.position).normalized, agent.transform.forward);
            if (cos_angle > 0 && Physics.Raycast(agent.transform.position, target - agent.transform.position, out hitInfo, 20)) {
                if (hitInfo.collider.tag == "Player")
                    return true; 
            }
        }
        return false;
    }
}
