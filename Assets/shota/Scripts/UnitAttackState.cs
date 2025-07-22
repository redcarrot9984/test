using UnityEngine;

public class UnitAttackState : StateMachineBehaviour
{
   UnityEngine.AI.NavMeshAgent agent;
   AttackController attackController;

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
              // 射程範囲外に出たら追跡状態に戻る
              if (distanceFromTarget > attackController.attackRange + 0.2f || attackController.targetToAttack == null)
              {
                animator.SetBool("isAttacking", false);
              }
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private void Attack()
    {
        if (attackController.isRanged)
        {
            // 遠距離攻撃：弾を発射する
            GameObject projectile = Object.Instantiate(attackController.projectilePrefab, agent.transform.position, Quaternion.identity);
            // ★★ この行のコメントを解除します ★★
            projectile.GetComponent<Projectile>().Initialize(attackController.targetToAttack, attackController.unitDamage);
        }
        else
        {
            // 近距離攻撃：直接ダメージを与える
            var damageToInflict = attackController.unitDamage;
            attackController.targetToAttack.GetComponent<Unit>().TakeDamage(damageToInflict);
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}