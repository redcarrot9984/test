using System.Collections;
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
    // ★★ チェック対象のレイヤーマスクをインスペクターから設定できるようにする ★★
    [SerializeField] private LayerMask placementCheckMask; 

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
        
        // ★★ レイヤーマスクを初期化（"Preview"レイヤー以外を全て含める） ★★
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
        // 建築可能かどうかの最終チェック
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            return;
        }

        // ★★ objectPlacerを呼び出し、設置したオブジェクトの「管理番号(index)」を受け取る ★★
        int placedObjectIndex = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition)
        );

        // ★★ 受け取った管理番号を使って、リストから実際のGameObjectを取得 ★★
        GameObject placedObject = objectPlacer.placedGameObjects[placedObjectIndex];

        // 設置が成功したかチェック
        if (placedObject == null)
        {
            Debug.LogError("オブジェクトの設置に失敗しました。");
            return;
        }

        // 設置したオブジェクトのタグが "Castle" かどうかをチェック
        if (placedObject.CompareTag("Castle"))
        {
            GameManager.Instance.RegisterCastle(placedObject.transform);
           // GameManager.castleTransform = placedObject.transform;
            Debug.Log("<color=green>SUCCESS:</color> CastleがGameManagerに登録されました。");
        }

        // 資源を消費するなどの後続処理
        ResourceManager.Instance.DecreaseRemoveResourcesBasedOnRequirement(database.objectsData[selectedObjectIndex]);
        BuildingType buildingType = database.objectsData[selectedObjectIndex].thisBuildingType;
        ResourceManager.Instance.UpdateBuildingChanged(buildingType, true);

        GridData selectedData = GetAllFloorIDs().Contains(database.objectsData[selectedObjectIndex].ID) ? floorData : furnitureData;

        // ★★ GridDataへの登録には、受け取った管理番号をそのまま使う ★★
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedObjectIndex); // ここが重要！

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    // When you have more floor objects add their id here
    private List<int> GetAllFloorIDs()
    {
        return new List<int> { 11 }; // These are all the ids of floor items - For now its only the grass
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
        // Show the player if he can place the item
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
