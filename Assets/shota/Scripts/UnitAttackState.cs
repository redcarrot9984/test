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
        if (unit.unitData.AttackSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(unit.unitData.AttackSound);
        }

        // ▼▼▼ データベースの値に応じて処理を分岐 ▼▼▼
        if (unit.unitData.IsAreaOfEffect)
        {
            PerformAreaAttack();
        }
        else
        {
            PerformSingleTargetAttack();
        }
    }

    /// <summary>
    /// 単体攻撃を実行する（従来の処理）
    /// </summary>
    private void PerformSingleTargetAttack()
    {
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

    /// <summary>
    /// 範囲攻撃を実行する（新しい処理）
    /// </summary>
    private void PerformAreaAttack()
    {
        // 攻撃の中心点をメインターゲットの位置にする
        Vector3 attackCenter = attackController.targetToAttack.position;

        // 指定した半径内のすべてのコライダーを取得する
        Collider[] hitColliders = Physics.OverlapSphere(attackCenter, unit.unitData.AreaOfEffectRadius);

        foreach (var hitCollider in hitColliders)
        {
            // 自分自身は攻撃対象外
            if (hitCollider.gameObject == unit.gameObject) continue;

            // 敵かどうかをタグで判定してダメージを与える
            bool isPlayerUnit = unit.CompareTag("Unit");
            if ((isPlayerUnit && hitCollider.CompareTag("Enemy")) ||
                (!isPlayerUnit && (hitCollider.CompareTag("Unit") || hitCollider.CompareTag("Castle"))))
            {
                Unit targetUnit = hitCollider.GetComponent<Unit>();
                if (targetUnit != null)
                {
                    targetUnit.TakeDamage(unit.unitData.Damage);
                }
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