// code/UnitFollowState.cs

using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    private AttackController attackController;
    private NavMeshAgent agent;
    private Unit unit;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
        agent = animator.GetComponent<NavMeshAgent>();
        unit = animator.GetComponent<Unit>();
        
        attackController.SetFollowMaterial();

        if (agent == null || attackController == null || unit == null || unit.unitData == null) return;

        // ターゲットが建物かどうかで停止距離を調整
        if (attackController.targetToAttack != null)
        {
            Unit targetUnit = attackController.targetToAttack.GetComponent<Unit>();
            if (targetUnit != null && targetUnit.isBuilding)
            {
                Collider targetCollider = attackController.targetToAttack.GetComponent<Collider>();
                if (targetCollider != null)
                {
                    float buildingRadius = Mathf.Max(targetCollider.bounds.extents.x, targetCollider.bounds.extents.z);
                    // 建物の半径 ＋ 攻撃範囲 を停止距離とする
                    agent.stoppingDistance = buildingRadius + unit.unitData.AttackRange;
                }
            }
            else
            {
                // 通常ユニット相手なら、攻撃範囲をそのまま停止距離とする
                agent.stoppingDistance = unit.unitData.AttackRange;
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
            return;
        }

        // 手動での移動命令がなく、ターゲットがいる場合
        if (!animator.GetComponent<UnitMovement>().isCommandedToMove)
        {
            // 敵に向かって移動
            agent.SetDestination(attackController.targetToAttack.position);
            animator.transform.LookAt(attackController.targetToAttack);

            // 目的地（停止距離内）に到達したら攻撃状態へ
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                animator.SetBool("isAttacking", true);
            }
        }
        else
        {
            // 移動命令が出されたら追跡を中断
            animator.SetBool("isFollowing", false);
        }
    }
}