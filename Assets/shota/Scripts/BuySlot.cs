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
      ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
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

       // 1. データベースからこのスロットのオブジェクト情報を取得
       ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);

       // 2. それがユニットかどうかを判定
       if (objectData.IsUnit)
       {
           // ユニットの場合：UnitProductionCoordinatorを呼び出す
           UnitProductionCoordinator.Instance.ProduceUnit(databaseItemID);
       }
       else
       {
           // 建物の場合：これまで通りPlacementSystemを呼び出す
           buySystem.placementSystem.StartPlacement(databaseItemID);
       }
   }

    private void HandleResourceChanged(ResourceManager.ResourcesType resourceType)
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            if (req.resource == resourceType)
            {
                CheckAvailability();
                return;
            }
        }
    }

    private void HandleBuildingsChanged()
    {
        CheckAvailability();
    }

    private void CheckAvailability()
    {
        bool requirementsMet = CheckResourceRequirements() && CheckBuildingDependencies();
        isAvailable = requirementsMet;
        UpdateAvailabilityUI();
    }

    private bool CheckResourceRequirements()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckBuildingDependencies()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
        foreach (BuildingType dependency in objectData.buildDependency)
        {
            if (dependency == BuildingType.None)
            {
                continue;
            }
            if (!ResourceManager.Instance.allExistingBuildings.Contains(dependency))
            {
                gameObject.SetActive(false);
                return false;
            }
        }
        gameObject.SetActive(true);
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
   
   // ▼▼▼ このメソッドを修正 ▼▼▼
   public void OnPointerEnter(PointerEventData eventData)
   {
       // データベースから情報を取得
       ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);
       if (objectData == null) return;

       // データベースに記述されたテキストをそのまま表示する
       string content = objectData.TooltipDescription;

       // ツールチップシステムを呼び出して表示
       TooltipSystem.Instance.Show(content);
   }
   // ▲▲▲ ここまで修正 ▲▲▲

   public void OnPointerExit(PointerEventData eventData)
   {
       // マウスが離れたら隠す
       TooltipSystem.Instance.Hide();
   }
}