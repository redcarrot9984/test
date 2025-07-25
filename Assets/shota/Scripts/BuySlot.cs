using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour
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
      if (isAvailable)
      {
         buySystem.placementSystem.StartPlacement(databaseItemID);
      }
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
}