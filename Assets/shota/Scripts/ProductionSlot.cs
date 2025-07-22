// ProductionSlot.cs

using UnityEngine;
using UnityEngine.UI;

public class ProductionSlot : MonoBehaviour
{
    public int unitID;

    public void OnClick()
    {
        // 選択中の建物を取得
        GameObject selectedBuilding = BuildingSelector.Instance.SelectedBuilding;
        if (selectedBuilding == null) return;

        UnitProducer producer = selectedBuilding.GetComponent<UnitProducer>();
        if (producer == null) return;

        // ★★ 資源チェックと消費処理を追加 ★★
        ObjectData unitData = DatabaseManager.Instance.databaseSO.GetObjectByID(unitID);
        if (CanAfford(unitData))
        {
            // 資源を消費
            ResourceManager.Instance.DecreaseResourcesBasedOnRequirements(unitData);
            // 生産キューに追加
            producer.AddToQueue(unitID);
        }
        else
        {
            Debug.Log("資源が足りません！");
            // TODO: プレイヤーに資源不足を知らせるUI表示（音を鳴らすなど）
        }
    }

    // 生産コストが足りているかチェックするヘルパーメソッド
    private bool CanAfford(ObjectData unitData)
    {
        foreach (var req in unitData.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                return false; // 1つでも足りなければfalse
            }
        }
        return true; // 全て足りていればtrue
    }
}