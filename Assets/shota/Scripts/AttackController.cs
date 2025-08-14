// code/AttackController.cs

using UnityEngine;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(SphereCollider))]
public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    // マテリアル設定用
    public Material idleStateMaterial;
    public Material attackStateMaterial;
    public Material followStateMaterial;

    private Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    private void Start()
    {
        // 索敵範囲をデータベースの値に基づいて設定
        SphereCollider sensor = GetComponent<SphereCollider>();
        if (sensor != null && unit != null && unit.unitData != null)
        {
            sensor.radius = unit.unitData.SensorRange;
        }
        else
        {
            Debug.LogError("索敵範囲の設定に必要なコンポーネントが見つかりません。", this);
        }
    }

    // isPlayerの判定はタグで行う
    private bool IsPlayer() => gameObject.CompareTag("Unit");

    private void OnTriggerEnter(Collider other)
    {
        if (targetToAttack != null) return;

        if (IsPlayer() && other.CompareTag("Enemy"))
        {
            targetToAttack = other.transform;
        }
        else if (!IsPlayer() && (other.CompareTag("Unit") || other.CompareTag("Castle")))
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (targetToAttack != null) return;
        
        if (IsPlayer() && other.CompareTag("Enemy"))
        {
            targetToAttack = other.transform;
        }
        else if (!IsPlayer() && (other.CompareTag("Unit") || other.CompareTag("Castle")))
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ターゲットが索敵範囲から出たらターゲットを解除
        if (other.transform == targetToAttack)
        {
            targetToAttack = null;
        }
    }

    public void SetIdleMaterial() { /*GetComponent<Renderer>().material = idleStateMaterial;*/ }
    public void SetAttackMaterial() { /*GetComponent<Renderer>().material = attackStateMaterial;*/ }
    public void SetFollowMaterial() { /*GetComponent<Renderer>().material = followStateMaterial;*/ }

    private void OnDrawGizmosSelected()
    {
        // 攻撃範囲をシーンビューで視覚化
        if (unit != null && unit.unitData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, unit.unitData.AttackRange);
        }
    }
}