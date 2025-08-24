// Code/EnemyAI.cs (全体を書き換え)

using UnityEngine;
using UnityEngine.AI;
using System.Collections; // コルーチンを使用するために必要

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetCastle;

    // ★★ここから追加：経路更新の最適化用変数★★
    [Tooltip("ターゲット（城）への経路を再計算する頻度（秒）")]
    public float pathUpdateRate = 0.5f;
    // ★★ここまで追加★★

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // ★★変更点：Updateではなく、StartCoroutineで最適化された更新処理を呼び出す★★
        StartCoroutine(UpdatePathCoroutine());
    }

    /// <summary>
    /// 一定間隔でターゲットへの経路を更新するコルーチン
    /// </summary>
    private IEnumerator UpdatePathCoroutine()
    {
        // このユニットが生きている間、ずっと繰り返す
        while (true)
        {
            // まだ城を見つけていない場合
            if (targetCastle == null)
            {
                // GameManagerに城の場所を問い合わせる
                if (GameManager.Instance != null)
                {
                    targetCastle = GameManager.Instance.GetCastleTransform();
                }
            }

            // 城が見つかっていたら、目的地を設定
            if (targetCastle != null)
            {
                // agentがアクティブで、NavMesh上にいることを確認
                if (agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(targetCastle.position);
                }
            }
            
            // 指定した時間だけ待ってから、次の更新を行う
            yield return new WaitForSeconds(pathUpdateRate);
        }
    }
}