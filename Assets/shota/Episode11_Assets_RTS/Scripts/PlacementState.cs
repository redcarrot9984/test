// PlacementState.cs (全体を書き換え)

using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private int ID;
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectsDatabseSO database;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;

    // ★★変更点：障害物レイヤーをインスペクターから設定できるように変更★★
    private LayerMask obstacleLayerMask; 

    public PlacementState(int iD, Grid grid, PreviewSystem previewSystem, ObjectsDatabseSO database, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        
        // ★★修正点：障害物レイヤーを直接指定★★
        // "Unit", "Building", "Enemy", "Castle" レイヤーを障害物とみなす
        // LayerMask.GetMask() は、指定した名前のレイヤーマスクを生成する
        obstacleLayerMask = LayerMask.GetMask("Unit", "Building", "Enemy", "Castle");

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"ID {iD} を持つオブジェクトはデータベースに存在しません。");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            return;
        }
        
        int placedObjectIndex = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition)
        );
        
        ObjectData placedObjectData = database.objectsData[selectedObjectIndex];
        
        if (placedObjectData.PlacementSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(placedObjectData.PlacementSound);
        }

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 11 ? floorData : furnitureData;
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedObjectIndex);
        
        BuildingType buildingType = placedObjectData.thisBuildingType;
        if (buildingType == BuildingType.Castle)
        {
            GameObject placedObject = objectPlacer.placedGameObjects[placedObjectIndex];
            if (placedObject.CompareTag("Castle"))
            {
                GameManager.Instance.RegisterCastle(placedObject.transform);
            }
        }
        
        ResourceManager.Instance.DecreaseResourcesBasedOnRequirements(placedObjectData);
        ResourceManager.Instance.UpdateBuildingChanged(buildingType, true);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    // ▼▼▼ このメソッドの判定ロジックを修正しました ▼▼▼
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        ObjectData objectData = database.objectsData[selectedObjectIndex];
        GridData selectedData = objectData.ID == 11 ? floorData : furnitureData;

        // 1. グリッドデータ上に既に他のオブジェクトがないかチェック
        if (!selectedData.CanPlaceObjectAt(gridPosition, objectData.Size))
        {
            return false;
        }
        
        // 2. 物理的な干渉がないかチェック (OverlapBoxを使用)
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        
        // 当たり判定の中心と大きさを、建物のサイズに合わせて正確に計算
        Vector3 boxCenter = worldPosition + new Vector3(objectData.Size.x * grid.cellSize.x * 0.5f, 0.5f, objectData.Size.y * grid.cellSize.z * 0.5f);
        Vector3 halfExtents = new Vector3(objectData.Size.x * grid.cellSize.x * 0.45f, 0.5f, objectData.Size.y * grid.cellSize.z * 0.45f);
        
        // ★★修正点：インスペクターで指定した障害物レイヤーとのみ衝突判定を行う★★
        Collider[] colliders = Physics.OverlapBox(boxCenter, halfExtents, Quaternion.identity, obstacleLayerMask);

        // 障害物が見つかったら設置不可
        if (colliders.Length > 0)
        {
            return false;
        }

        // 全てのチェックを通過したら設置可
        return true;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}