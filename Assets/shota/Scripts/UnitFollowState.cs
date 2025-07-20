using UnityEngine;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;

    UnityEngine.AI.NavMeshAgent agent;
    public float attackingDistance = 1f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
        attackController.SetFollowMaterial();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       //Should Unit Transition to Attack State
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }else
        {
            if (animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                // Moving Unit towards Enemy
                      agent.SetDestination(attackController.targetToAttack.position);
                      animator.transform.LookAt(attackController.targetToAttack);

                      // Shold Unit Transition to Attack State ?
                      float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
                      if (distanceFromTarget < attackingDistance)
                      {
                         agent.SetDestination(animator.transform.position);
                         animator.SetBool("isAttacking", true);
                      }
            }
        }


    }

    

}
