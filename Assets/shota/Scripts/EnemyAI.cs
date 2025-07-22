using UnityEngine;
using UnityEngine.AI; // NavMeshAgentを扱うために必要

[RequireComponent(typeof(NavMeshAgent))] // このスクリプトにはNavMeshAgentが必須であることを示す
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetCastle;

    void Start()
    {
        // このスクリプトがアタッチされているオブジェクトのNavMeshAgentコンポーネントを取得
        agent = GetComponent<NavMeshAgent>();

        // "Castle"というタグがついたオブジェクトを探して、それをターゲットに設定
        GameObject castleObject = GameObject.FindGameObjectWithTag("Castle");
        if (castleObject != null)
        {
            targetCastle = castleObject.transform;
            // ターゲット（お城）に向かって移動を開始
            agent.SetDestination(targetCastle.position);
        }
        else
        {
            // もしお城が見つからなかった場合、エラーメッセージを表示
            Debug.LogError("ターゲットとなる'Castle'タグが付いたオブジェクトが見つかりません！");
        }
    }
}