using UnityEngine;

public class UnitAttackState : StateMachineBehaviour
{
   UnityEngine.AI.NavMeshAgent agent;
   AttackController attackController;

   public float stopAttackingDistance = 1.2f;

    public float attackRate = 2f;
    private float attackTimer;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        attackController.SetAttackMaterial();
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(attackController.targetToAttack != null && animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
        {
              LookAtTarget();

              //keel moving towards enemy
                //agent.SetDestination(attackController.targetToAttack.position);

              if (attackTimer <= 0)
              {
                    Attack();
                    attackTimer = 1f / attackRate;
              }
              else
                {
                        attackTimer -= Time.deltaTime;
                }

              // should unit still attack
              float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
              if (distanceFromTarget > stopAttackingDistance || attackController.targetToAttack == null)
              {
                animator.SetBool("isAttacking", false);
              }




        }else{
            animator.SetBool("isAttacking", false);
        }
    }

    private void Attack()
    {
        var damageToInflict = attackController.unitDamage;

        attackController.targetToAttack.GetComponent<Unit>().TakeDamage(damageToInflict);

    }


    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }



    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }







}
