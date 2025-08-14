using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuySlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
   public Sprite availableSprite;
   public Sprite unavailableSprite;
   
   public bool isAvailable;

   public BuySystem buySystem;

   public int databaseItemID;

   private void Start()
   {
      // イベントリスナーの登録
      ResourceManager.Instance.OnResourceChanged += HandleResourceChanged; // メソッド名を単数形に変更
      ResourceManager.Instance.OnBuildingsChanged += HandleBuildingsChanged;
      
      // 初期の利用可能性をチェック
      CheckAvailability();
   }

   private void OnDestroy()
   {
        // オブジェクトが破棄される際にリスナーを解除（メモリリーク防止）
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= HandleResourceChanged;
            ResourceManager.Instance.OnBuildingsChanged -= HandleBuildingsChanged;
        }
   }

   public void ClickedOnSlot()
   {
       if (!isAvailable) return;

       // ▼▼▼ ここから修正 ▼▼▼

       // 1. データベースからこのスロットのオブジェクト情報を取得
       ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);

       // 2. それがユニットかどうかを判定
       if (objectData.IsUnit)
       {
           // ★ユニットの場合：新しく作ったUnitProductionCoordinatorを呼び出す
           UnitProductionCoordinator.Instance.ProduceUnit(databaseItemID);
       }
       else
       {
           // ★建物の場合：これまで通りPlacementSystemを呼び出す
           buySystem.placementSystem.StartPlacement(databaseItemID);
       }
    
       // ▲▲▲ ここまで修正 ▲▲▲
   }

    // ▼▼▼ 以下のメソッドを全面的に修正 ▼▼▼

    // 特定の資源が変更されたときに呼び出される
    private void HandleResourceChanged(ResourceManager.ResourcesType resourceType)
    {
        // 自分のコストに関係する資源の変動があった場合のみ、チェックを更新する
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            if (req.resource == resourceType)
            {
                CheckAvailability();
                return; // 一致するものが見つかったらチェックして終了
            }
        }
    }

    private void HandleBuildingsChanged()
    {
        CheckAvailability();
    }

    // 利用可能かどうかをチェックするメインの処理
    private void CheckAvailability()
    {
        bool requirementsMet = CheckResourceRequirements() && CheckBuildingDependencies();
        isAvailable = requirementsMet;
        UpdateAvailabilityUI();
    }

    // 資源要件をチェックする
    private bool CheckResourceRequirements()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            // プレイヤーの所持量が要件より少ない場合はfalseを返す
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                return false;
            }
        }
        // 全ての資源要件を満たしていればtrueを返す
        return true;
    }

    // 建物依存関係をチェックする
    private bool CheckBuildingDependencies()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
        foreach (BuildingType dependency in objectData.buildDependency)
        {
            if (dependency == BuildingType.None)
            {
                continue; // 依存関係なし
            }
            // 必要な建物が存在しない場合はfalseを返す
            if (!ResourceManager.Instance.allExistingBuildings.Contains(dependency))
            {
                gameObject.SetActive(false); // UI自体を非表示
                return false;
            }
        }
        gameObject.SetActive(true); // UIを表示
        return true;
    }

   private void UpdateAvailabilityUI()
   {
      if (isAvailable)
      {
         GetComponent<Image>().sprite = availableSprite;
         GetComponent<Button>().interactable = true;
      }
      else
      {
         GetComponent<Image>().sprite = unavailableSprite;
         GetComponent<Button>().interactable = false;
      }
   }
   
   public void OnPointerEnter(PointerEventData eventData)
   {
       // データベースから情報を取得
       ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
       if (objectData == null) return;

       // 表示するテキストを組み立てる
       string content = $"<size=24>{objectData.Name}</size>\n<size=18>{objectData.description}</size>\n\n"; // 名前と説明

       // 資源コストを追加
       if (objectData.resourceRequirements.Count > 0)
       {
           content += "<b>Cost:</b>\n";
           foreach (var req in objectData.resourceRequirements)
           {
               content += $"{req.resource}: {req.amount}\n";
           }
       }

       // ★ユニットの場合、生産時間を追加
       if (objectData.IsUnit)
       {
           // ObjectDataにproductionTimeフィールドを追加する必要があります（後述）
           // content += $"<b>生産時間:</b> {objectData.productionTime}秒\n";
       }

       // ツールチップシステムを呼び出して表示
       TooltipSystem.Instance.Show(content);
   }

   public void OnPointerExit(PointerEventData eventData)
   {
       // マウスが離れたら隠す
       TooltipSystem.Instance.Hide();
   }
}