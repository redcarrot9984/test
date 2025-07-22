using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    public Material idleStateMaterial;
    public Material attackStateMaterial;
    public Material followStateMaterial;

    public int unitDamage;
    public bool isPlayer;

    // 遠距離攻撃用の弾のプレハブ
    public GameObject projectilePrefab;
    // 遠距離ユニットかどうかを判定するフラグ
    public bool isRanged;
    // 攻撃射程
    public float attackRange = 1f;


    private void OnTriggerEnter(Collider other)
    {
        // ターゲットがいない場合のみ索敵
        if (targetToAttack != null) return;

        // プレイヤーの場合、"Enemy"タグを探す
        if (isPlayer && other.CompareTag("Enemy"))
        {
            targetToAttack = other.transform;
        }
        // 敵の場合、"Unit"タグを探す
        else if (!isPlayer &&  (other.CompareTag("Unit") || other.CompareTag("Castle"))) // "PlayerUnit" から "Unit" へ変更
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // ターゲットがいない場合のみ索敵
        if (targetToAttack != null) return;

        if (isPlayer && other.CompareTag("Enemy"))
        {
            targetToAttack = other.transform;
        }
        else if (!isPlayer &&  (other.CompareTag("Unit") || other.CompareTag("Castle"))) // "PlayerUnit" から "Unit" へ変更
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }
    }
    public void SetIdleMaterial()
    {
        // GetComponent<Renderer>().material = idleStateMaterial;
    }
    public void SetAttackMaterial()
    {
        //GetComponent<Renderer>().material = attackStateMaterial;
    }
    public void SetFollowMaterial()
    {
        // GetComponent<Renderer>().material = followStateMaterial;
    }

    private void OnDrawGizmos()
    {
        // 攻撃範囲の視覚化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}