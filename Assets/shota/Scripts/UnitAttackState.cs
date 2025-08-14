// code/UnitAttackState.cs

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
        attackTimer = 0f; // 状態に入ったらすぐ攻撃できるようにタイマーをリセット
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ターゲットがいる、かつ移動命令が出ていない場合のみ攻撃
        if (attackController.targetToAttack != null && !animator.GetComponent<UnitMovement>().isCommandedToMove)
        {
            LookAtTarget();

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Attack();
                // データベースの値から次の攻撃までの時間を設定
                attackTimer = 1f / unit.unitData.AttackRate;
            }

            // ターゲットが射程範囲外に出たら追跡状態に戻る
            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
            if (distanceFromTarget > unit.unitData.AttackRange + 0.2f) // 0.2fは猶予
            {
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            // ターゲットを失うか、移動命令が出たら攻撃中断
            animator.SetBool("isAttacking", false);
        }
    }

    private void Attack()
    {
        if (unit.unitData.IsRanged)
        {
            // 遠距離攻撃：弾のプレハブをデータベースから取得して生成
            if (unit.unitData.ProjectilePrefab == null) return;
            GameObject projectileGO = Object.Instantiate(unit.unitData.ProjectilePrefab, agent.transform.position, Quaternion.identity);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                // ダメージ量をデータベースから渡す
                projectile.Initialize(attackController.targetToAttack, unit.unitData.Damage);
            }
        }
        else
        {
            // 近接攻撃：直接ダメージを与える
            Unit targetUnit = attackController.targetToAttack.GetComponent<Unit>();
            if (targetUnit != null)
            {
                // ダメージ量をデータベースから取得
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