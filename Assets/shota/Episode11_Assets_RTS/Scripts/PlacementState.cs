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

        // ▼▼▼ 修正箇所 ▼▼▼
        // 中心の計算をせず、グリッドセルの角の座標をそのまま使用します
        int placedObjectIndex = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition)
        );
        // ▲▲▲ 修正ここまで ▲▲▲

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

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 11 ? floorData : furnitureData;
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedObjectIndex);
    }

    // このメソッドは、建物の占有エリアを正しくチェックしているため変更しません
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        ObjectData objectData = database.objectsData[selectedObjectIndex];
        GridData selectedData = objectData.ID == 11 ? floorData : furnitureData;

        if (!selectedData.CanPlaceObjectAt(gridPosition, objectData.Size))
        {
            return false;
        }

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 boxCenter = worldPosition + new Vector3(objectData.Size.x * grid.cellSize.x * 0.5f, 0.5f, objectData.Size.y * grid.cellSize.z * 0.5f);
        Vector3 halfExtents = new Vector3(objectData.Size.x * grid.cellSize.x * 0.45f, 0.5f, objectData.Size.y * grid.cellSize.z * 0.45f);

        Collider[] colliders = Physics.OverlapBox(boxCenter, halfExtents, Quaternion.identity, placementCheckMask);

        foreach (var collider in colliders)
        {
            if(collider.CompareTag("Unit") || collider.CompareTag("Building") || collider.CompareTag("Enemy") || collider.CompareTag("Castle"))
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        // ▼▼▼ 修正箇所 ▼▼▼
        // こちらも同様に、グリッドセルの角の座標をそのままプレビュー位置として使用します
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        // ▲▲▲ 修正ここまで ▲▲▲
    }
}