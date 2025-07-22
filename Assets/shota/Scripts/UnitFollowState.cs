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

        // ★★ NavMeshAgentの停止距離を、そのユニットの攻撃射程に自動で設定する ★★
        if (agent != null && attackController != null)
        {
            agent.stoppingDistance = attackController.attackRange;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            if (animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                // Moving Unit towards Enemy
                agent.SetDestination(attackController.targetToAttack.position);
                animator.transform.LookAt(attackController.targetToAttack);

                // NavMeshAgentが停止距離に到達したかどうかで判断する
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    animator.SetBool("isAttacking", true);
                }
            }
        }
    }
}