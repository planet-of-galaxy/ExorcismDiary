using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ChairlieStateBase : StateBase<ChairlieStateBase, ChairlieFsm>
{
    protected Vector3 target;

    // Ѱ��Ŀ����ر�����ǰ����
    private Ray agentToPlayer;
    private float cos_angle;
    private float distance;
    private RaycastHit hitInfo;
    private LayerMask mask = ~(1 << 7); // �����Լ���һ��
    protected virtual bool FindTarget() {
        target = MainController.playerTransform.position;

        distance = Vector3.Distance(target, agent.transform.position);
        if (distance < 10) {
            cos_angle = Vector3.Dot((target - agent.transform.position).normalized, agent.transform.forward);
            if ((cos_angle > 0 || distance < 1) && Physics.Raycast(agent.transform.position, target - agent.transform.position, out hitInfo, 20, mask)) {
                if (hitInfo.collider.tag == "Player")
                    return true; 
            }
        }
        return false;
    }
}
