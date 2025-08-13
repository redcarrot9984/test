using System.Collections.Generic;
using UnityEngine;

public class UnitProducer : MonoBehaviour
{
    // ▼▼▼ このフィールドを追加 ▼▼▼
    [Header("建物の種類")]
    public BuildingType buildingType;
    // この建物が生産できるユニットのIDリスト
    [SerializeField]
    private List<int> producibleUnitIDs = new List<int>();

    // ユニットが出現する場所（インスペクターから設定）
    [SerializeField]
    private Transform spawnPoint;

    private Queue<int> productionQueue = new Queue<int>();
    private float productionTimer;
    private float productionTime = 5f; // ユニット1体の生産時間

    private void Update()
    {
        // 生産キューに何か入っていれば
        if (productionQueue.Count > 0)
        {
            productionTimer += Time.deltaTime;
            if (productionTimer >= productionTime)
            {
                SpawnUnit();
                productionTimer = 0f;
            }
        }
    }

    public void AddToQueue(int unitID)
    {
        productionQueue.Enqueue(unitID);
    }

    private void SpawnUnit()
    {
        int unitID = productionQueue.Dequeue();
        ObjectData unitData = DatabaseManager.Instance.databaseSO.GetObjectByID(unitID);

        if (unitData != null && unitData.UnitPrefab != null)
        {
            Instantiate(unitData.UnitPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public List<int> GetProducibleUnits()
    {
        return producibleUnitIDs;
    }
}