// Code/UnitFollowState.cs

using UnityEngine;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    UnityEngine.AI.NavMeshAgent agent;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
        attackController.SetFollowMaterial();

        if (agent != null && attackController != null && attackController.targetToAttack != null)
        {
            Unit targetUnit = attackController.targetToAttack.GetComponent<Unit>();
            if (targetUnit != null && targetUnit.isBuilding)
            {
                Collider targetCollider = attackController.targetToAttack.GetComponent<Collider>();
                if (targetCollider != null)
                {
                    float buildingRadius = Mathf.Max(targetCollider.bounds.extents.x, targetCollider.bounds.extents.z);
                    agent.stoppingDistance = buildingRadius + attackController.attackRange;
                }
                else
                {
                    agent.stoppingDistance = attackController.attackRange + 5f; 
                }
            }
            else
            {
                agent.stoppingDistance = attackController.attackRange;
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ▼▼▼ ここに安全確認を追加 ▼▼▼
        // NavMeshAgentが有効で、かつNavMesh上に配置されているかチェック
        if (agent == null || agent.isActiveAndEnabled == false || agent.isOnNavMesh == false)
        {
            // 準備ができていなければ、このフレームの処理を中断する
            return; 
        }
        // ▲▲▲ ここまで ▲▲▲

        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            if (animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                // 敵に向かって移動
                agent.SetDestination(attackController.targetToAttack.position);
                animator.transform.LookAt(attackController.targetToAttack);

                // NavMeshAgentが目的地（停止距離内）に到達したかチェック
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    // 到達していれば攻撃状態へ移行
                    animator.SetBool("isAttacking", true);
                }
            }
        }
    }
}
