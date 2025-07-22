using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabseSO database;

    [SerializeField] private GridData floorData, furnitureData; // floor things like roads, furniture change to "buildings"

    [SerializeField] private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    int selectedID;

    IBuildingState buildingState;

    private void Start()
    {

        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        Debug.Log("Should Start Placement");

        selectedID = ID;

        Debug.Log("Placement ID: " + ID);


        StopPlacement();

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();

        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        GameObject previewObject = null;
        // C#のリフレクション（GetType().GetField）を使ってpreviewObjectを取得
        var field = typeof(PreviewSystem).GetField("previewObject", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            previewObject = field.GetValue(previewSystem) as GameObject;
        }

        // プレビューオブジェクトが存在すれば、UIチェックの前に一時的に非表示にする
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }

        if (inputManager.IsPointerOverUI())
        {
            Debug.Log("Pointer was over UI - Returned");
            
            // 処理を中断する場合でも、プレビューオブジェクトを再表示する
            if (previewObject != null)
            {
                previewObject.SetActive(true);
            }
            return;
        }

        // チェックが終わったら、すぐにプレビューオブジェクトを再表示する
        if (previewObject != null)
        {
            previewObject.SetActive(true);
        }
        // ▼▼▼ 以下のロジックを修正 ▼▼▼

        // 1. これから建てる建物のデータを取得
        ObjectData ob = database.GetObjectByID(selectedID);

        // 2. 資源が本当に足りているか最終チェック
        bool canAfford = true;
        foreach (BuildRequirement req in ob.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                canAfford = false;
                break;
            }
        }

        // 3. もし足りていれば、建物を設置して資源を減らす
        if (canAfford)
        {
            // 建物を設置
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            buildingState.OnAction(gridPosition);

            // 資源を消費する
            ResourceManager.Instance.DecreaseResourcesBasedOnRequirements(ob);

            // ---- Add Buildable Benifits ---- // 
            foreach (BuildBenefits bf in ob.benefits)
            {
                CalculateAndAddBenefit(bf);
            }
        }
        else
        {
            Debug.Log("Not enough resources!");
        }

        // ---- Stop the placement after every build ---- // 
        StopPlacement();
    }

    private void CalculateAndAddBenefit(BuildBenefits bf)
    {
        switch (bf.benefitType)
        {
            case BuildBenefits.BenefitType.Housing:
             //   StatusManager.Instance.IncreaseHousing(bf.benefitAmount);
                break;
        }
    }

    public void StopPlacement()
    {
        if (buildingState == null)
            return;
       
        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    private void Update()
    {
        // We return because we did not selected an item to place (not in placement mode)
        // So there is no need to show cell indicator
        if (buildingState == null)
            return;
      
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }
}
