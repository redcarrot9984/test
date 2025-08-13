// UnitProductionCoordinator.cs (新規作成)

using UnityEngine;

public class UnitProductionCoordinator : MonoBehaviour
{
    public static UnitProductionCoordinator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// UIボタンなどから呼び出されるユニット生産の起点
    /// </summary>
    /// <param name="unitID">生産したいユニットのID</param>
    public void ProduceUnit(int unitID)
    {
        ObjectData unitData = DatabaseManager.Instance.databaseSO.GetObjectByID(unitID);
        if (unitData == null || !unitData.IsUnit)
        {
            Debug.LogError($"ID:{unitID} はユニットデータではないか、見つかりません。");
            return;
        }

        // 1. 資源が足りているかチェック
        if (!CanAfford(unitData))
        {
            Debug.Log("資源が足りません！");
            // ここで音を鳴らすなどのフィードバックを入れると良い
            return;
        }

        // 2. 生産を担当する建物を探す
        UnitProducer producer = FindProducerFor(unitData.producingBuilding);
        if (producer == null)
        {
            Debug.LogWarning($"{unitData.producingBuilding} のタイプの建物が見つかりません。");
            // 「兵舎を建ててください」などのUIメッセージを出すと親切
            return;
        }

        // 3. 資源を消費して、生産キューに追加
        ResourceManager.Instance.DecreaseResourcesBasedOnRequirements(unitData);
        producer.AddToQueue(unitID);

        Debug.Log($"{unitData.Name} の生産を開始しました。担当: {producer.gameObject.name}");
    }

    /// <summary>
    /// 生産コストが足りているかチェックする
    /// </summary>
    private bool CanAfford(ObjectData unitData)
    {
        foreach (var req in unitData.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 指定された種類の建物をシーン内から探す
    /// </summary>
    private UnitProducer FindProducerFor(BuildingType type)
    {
        // シーンに存在する全てのUnitProducerコンポーネントを取得
        UnitProducer[] allProducers = FindObjectsOfType<UnitProducer>();
        foreach (UnitProducer producer in allProducers)
        {
            // 建物の種類が一致したら、その建物を返す
            if (producer.buildingType == type)
            {
                return producer;
            }
        }
        // 見つからなければnullを返す
        return null;
    }
}