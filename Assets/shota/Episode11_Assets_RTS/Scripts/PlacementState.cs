// Code/PlacementState.cs

using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
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
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        // ▼▼▼ ここからが診断箇所 ▼▼▼
        Debug.Log("--- PlacementState: OnAction called ---"); // 1. メソッドが呼ばれたか

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            Debug.LogWarning("Placement was not valid. Aborting."); // 設置場所が不正
            return;
        }
        
        int placedObjectIndex = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition)
        );
        
        GameObject placedObject = objectPlacer.placedGameObjects[placedObjectIndex];

        if (placedObject == null)
        {
            Debug.LogError("CRITICAL: placedObject is NULL. Check ObjectPlacer.cs"); // 2. オブジェクトが取得できたか
            return;
        }

        Debug.Log("Placed Object Name: " + placedObject.name + " | Tag: " + placedObject.tag); // 3. オブジェクトの名前とタグを確認

        if (placedObject.CompareTag("Castle"))
        {
            Debug.Log("<color=green>SUCCESS:</color> Tag matched! Calling RegisterCastle..."); // 4. タグが一致したか
            GameManager.Instance.RegisterCastle(placedObject.transform);
        }
        else
        {
            Debug.LogError("FAILED: Tag did NOT match 'Castle'. Please check the prefab's tag."); // 5. タグが一致しなかった場合
        }
        // ▲▲▲ ここまで ▲▲▲


        // 資源を消費するなどの後続処理
        ResourceManager.Instance.DecreaseRemoveResourcesBasedOnRequirement(database.objectsData[selectedObjectIndex]);
        BuildingType buildingType = database.objectsData[selectedObjectIndex].thisBuildingType;
        ResourceManager.Instance.UpdateBuildingChanged(buildingType, true);

        GridData selectedData = GetAllFloorIDs().Contains(database.objectsData[selectedObjectIndex].ID) ? floorData : furnitureData;
        
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private List<int> GetAllFloorIDs()
    {
        return new List<int> { 11 };
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = GetAllFloorIDs().Contains(database.objectsData[selectedObjectIndex].ID) ? floorData : furnitureData;

        if (!selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size))
        {
            return false;
        }
        
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Collider[] colliders = Physics.OverlapBox(worldPosition, new Vector3(0.5f,0.5f,0.5f),Quaternion.identity, placementCheckMask);

        foreach (var collider in colliders)
        {
            if(collider.CompareTag("Unit") || collider.CompareTag("Building") || collider.CompareTag("Enemy")|| collider.CompareTag("Castle"))
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
