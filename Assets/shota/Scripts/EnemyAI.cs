// Code/EnemyAI.cs
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool hasTarget = false;
    private Transform targetCastle; // ターゲットの情報を保持する変数

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // まだターゲットを見つけていない場合
        if (!hasTarget)
        {
            // GameManagerに城の場所を問い合わせる
            if (GameManager.Instance != null)
            {
                targetCastle = GameManager.Instance.GetCastleTransform();

                // 城の場所が返ってきたら（nullでなかったら）
                if (targetCastle != null)
                {
                    Debug.Log("<color=green>ENEMY:</color> " + gameObject.name + " found the castle!");
                    agent.SetDestination(targetCastle.position);
                    hasTarget = true; // ターゲットを見つけたので、もう探さない
                }
                else
                {
                    // まだ城が見つからない（建築されていない）ことを示すログ
                    Debug.Log(gameObject.name + " is searching for the castle...");
                }
            }
            else
            {
                Debug.LogError("ENEMY ERROR: GameManager.Instance is not found in the scene!");
            }
        }
    }
}