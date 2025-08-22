// UnitAttackState.cs (全体を書き換え)

using UnityEngine;

public class UnitAttackState : StateMachineBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private AttackController attackController;
    private Unit unit;

    private float attackTimer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        unit = animator.GetComponent<Unit>();
        
        attackController.SetAttackMaterial();
        attackTimer = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack != null && !animator.GetComponent<UnitMovement>().isCommandedToMove)
        {
            LookAtTarget();

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = 1f / unit.unitData.AttackRate;
            }

            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
            if (distanceFromTarget > unit.unitData.AttackRange + 0.2f)
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
        // ★★ここから追加：攻撃SEを再生★★
        if (unit.unitData.AttackSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(unit.unitData.AttackSound);
        }
        // ★★ここまで追加★★

        if (unit.unitData.IsRanged)
        {
            if (unit.unitData.ProjectilePrefab == null) return;
            GameObject projectileGO = Object.Instantiate(unit.unitData.ProjectilePrefab, agent.transform.position, Quaternion.identity);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(attackController.targetToAttack, unit.unitData.Damage);
            }
        }
        else
        {
            Unit targetUnit = attackController.targetToAttack.GetComponent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(unit.unitData.Damage);
            }
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        if (direction != Vector3.zero)
        {
            agent.transform.rotation = Quaternion.LookRotation(direction);
            var yRotation = agent.transform.eulerAngles.y;
            agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}