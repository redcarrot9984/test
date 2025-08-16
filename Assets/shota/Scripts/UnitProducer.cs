// code/UnitProducer.cs (全体を書き換え)

using System.Collections.Generic;
using UnityEngine;

public class UnitProducer : MonoBehaviour
{
    [Header("建物の種類")]
    public BuildingType buildingType;

    [Tooltip("この建物が生産できるユニットのIDリスト")]
    [SerializeField]
    private List<int> producibleUnitIDs = new List<int>();

    [Tooltip("ユニットが出現する場所")]
    [SerializeField]
    private Transform spawnPoint;

    // 生産キューとタイマー関連
    private Queue<int> productionQueue = new Queue<int>();
    private float currentProductionTime;
    private float productionTimer;
    
    // --- UIや他のシステムが参照するためのプロパティ ---
    
    /// <summary> 現在何かを生産中かどうか </summary>
    public bool IsProducing => productionQueue.Count > 0;
    
    /// <summary> 現在の生産の進捗状況 (0.0～1.0) </summary>
    public float ProductionProgress => (currentProductionTime > 0) ? (productionTimer / currentProductionTime) : 0f;
    
    /// <summary> 現在生産中のユニットのID </summary>
    public int CurrentlyProducingID => (productionQueue.Count > 0) ? productionQueue.Peek() : -1;
    
    /// <summary> 生産キューの中身をリストとして取得 </summary>
    public List<int> GetQueueContents() => new List<int>(productionQueue);


    private void Update()
    {
        // 生産キューに何か入っていれば、タイマーを進める
        if (productionQueue.Count > 0)
        {
            productionTimer += Time.deltaTime;
            
            // タイマーが生産時間に達したら
            if (productionTimer >= currentProductionTime)
            {
                SpawnUnit(); // ユニットを生成
                StartNextProduction(); // 次の生産へ移行
            }
        }
    }

    /// <summary>
    /// 生産キューに新しいユニットを追加する
    /// </summary>
    public void AddToQueue(int unitID)
    {
        productionQueue.Enqueue(unitID);
        
        // もしこれがキューに追加された最初のユニットなら、生産を開始する
        if (productionQueue.Count == 1)
        {
            StartNextProduction();
        }
    }

    /// <summary>
    /// キューの次のユニットの生産準備を開始する
    /// </summary>
    private void StartNextProduction()
    {
        // キューが空なら、タイマーをリセットして終了
        if (productionQueue.Count == 0)
        {
            productionTimer = 0;
            currentProductionTime = 0;
            return;
        }

        // キューの先頭から次に生産するユニットのIDを取得 (まだ取り出さない)
        int unitID = productionQueue.Peek();
        ObjectData unitData = DatabaseManager.Instance.databaseSO.GetObjectByID(unitID);
        
        if (unitData != null)
        {
            // ★★ データベースから生産時間を読み込んでタイマーを設定 ★★
            currentProductionTime = unitData.productionTime;
            productionTimer = 0f;
        }
    }

    /// <summary>
    /// ユニットを実際にシーンに生成する
    /// </summary>
    private void SpawnUnit()
    {
        if (productionQueue.Count == 0) return;

        // キューの先頭からユニットを取り出して処理
        int unitID = productionQueue.Dequeue();
        ObjectData unitData = DatabaseManager.Instance.databaseSO.GetObjectByID(unitID);

        if (unitData != null && unitData.UnitPrefab != null)
        {
            Instantiate(unitData.UnitPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    /// <summary>
    /// この建物が生産可能な全ユニットのIDリストを返す
    /// </summary>
    public List<int> GetProducibleUnits()
    {
        return producibleUnitIDs;
    }
}