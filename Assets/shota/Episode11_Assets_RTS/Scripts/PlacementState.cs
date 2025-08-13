// Code/PlacementState.cs

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
    private LayerMask placementCheckMask; 

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        
        placementCheckMask = ~LayerMask.GetMask("Preview");

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
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
            Debug.LogWarning("その場所には設置できません。");
            return;
        }
        
        int placedObjectIndex = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition)
        );
        
        ObjectData placedObjectData = database.objectsData[selectedObjectIndex];
        BuildingType buildingType = placedObjectData.thisBuildingType;

        if (buildingType == BuildingType.Castle)
        {
            GameObject placedObject = objectPlacer.placedGameObjects[placedObjectIndex];
            
            if (placedObject.CompareTag("Castle"))
            {
                Debug.Log("<color=green>SUCCESS:</color> 城が設置されたため、Waveシステムを開始します。");
                GameManager.Instance.RegisterCastle(placedObject.transform);
            }
            else
            {
                Debug.LogError($"設定エラー: 種類が「Castle」の建物プレハブに 'Castle' タグが設定されていません。現在のタグ: {placedObject.tag}");
            }
        }
        
        ResourceManager.Instance.DecreaseResourcesBasedOnRequirements(placedObjectData);
        ResourceManager.Instance.UpdateBuildingChanged(buildingType, true);

        // GridDataの選択ロジックを簡略化（床かそれ以外か）
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 11 ? floorData : furnitureData; // '11'は床IDの例
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    // ▼▼▼ このメソッドのロジックを修正しました ▼▼▼
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        ObjectData objectData = database.objectsData[selectedObjectIndex];
        GridData selectedData = objectData.ID == 11 ? floorData : furnitureData; // '11'は床IDの例

        // グリッドデータ上に既に他のオブジェクトがないかチェック
        if (!selectedData.CanPlaceObjectAt(gridPosition, objectData.Size))
        {
            return false;
        }
        
        // 物理的な干渉がないかチェック
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        // オブジェクトのサイズに合わせて当たり判定の中心と大きさを調整
        Vector3 boxCenter = worldPosition + new Vector3(objectData.Size.x * 0.5f, 0.5f, objectData.Size.y * 0.5f);
        Vector3 halfExtents = new Vector3(objectData.Size.x * 0.45f, 0.5f, objectData.Size.y * 0.45f); // 少し小さくして判定を緩やかに
        
        Collider[] colliders = Physics.OverlapBox(boxCenter, halfExtents, Quaternion.identity, placementCheckMask);

        // 衝突したオブジェクトを一つずつチェックする
        foreach (var collider in colliders)
        {
            // 障害物（ユニットや他の建物など）のタグが付いていれば設置不可
            if(collider.CompareTag("Unit") || collider.CompareTag("Building") || collider.CompareTag("Enemy") || collider.CompareTag("Castle"))
            {
                // デバッグ用に何と衝突したかログに出す
                // Debug.LogWarning("設置不可: " + collider.name + " と衝突しています。");
                return false;
            }
        }

        // ループを抜けた場合、障害物はなかったということなので設置可
        return true;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}