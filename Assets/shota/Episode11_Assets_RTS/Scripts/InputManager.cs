using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask; // 地面用のレイヤーマスク
    [SerializeField] private LayerMask clickableLayerMask; // 建物用のレイヤーマスク

    [SerializeField] private BuildingSelector buildingSelector;
    [SerializeField] private PlacementSystem placementSystem; // PlacementSystemへの参照を追加

    public event Action OnClicked, OnExit;

    private void Start()
    {
        // もしインスペクターから設定されていなければ、シーン内から探す
        if (buildingSelector == null) buildingSelector = FindObjectOfType<BuildingSelector>();
        if (placementSystem == null) placementSystem = FindObjectOfType<PlacementSystem>();
    }

    private void Update()
    {
        // 左クリック時の処理
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        // Escapeキーが押された時の処理
        if (Input.GetKeyDown(KeyCode.Escape))
        {
             OnExit?.Invoke();
        }
    }

   /* private void HandleLeftClick()
    {
        // UIの上なら何もしない
        if (IsPointerOverUI()) return;

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 【優先度1】まず、クリック可能な建物（Buildingレイヤー）に当たったかチェック
        if (Physics.Raycast(ray, out hit, 2000, clickableLayerMask))
        {
            UnitProducer producer = hit.collider.GetComponentInParent<UnitProducer>();
            if (producer != null)
            {
                // もし建築モード中だったら、それをキャンセルする
                if (placementSystem != null)
                {
                    placementSystem.StopPlacement();
                }
                
                // 建物を選択する
                buildingSelector.SelectBuilding(producer.gameObject);
                
                // 建物をクリックしたので、今回の処理はここで終わり
                return; 
            }
        }
           // 【優先度2】次に、地面（Placementレイヤー）がクリックされたかチェック
        if (Physics.Raycast(ray, out hit, 2000, placementLayerMask))
        {
            // もし何か建物が選択されていたら、選択を解除する
            if (buildingSelector != null)
            {
                buildingSelector.DeselectBuilding();
            }

            // 建築システムに「地面がクリックされた」と通知する
            OnClicked?.Invoke();
        }
    }
    */
   private void HandleLeftClick()
   {
       // UIの上なら何もしない
       if (IsPointerOverUI())
       {
           Debug.Log("クリックを無視: ポインターがUIの上にあります。");
           return;
       }

       Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
       RaycastHit hit;

       // 【調査A】まず、Buildingレイヤーにレイが当たったか？
       if (Physics.Raycast(ray, out hit, 2000, clickableLayerMask))
       {
           Debug.Log("レイキャスト成功: '" + hit.collider.name + "' というオブジェクト（Buildingレイヤー）にヒットしました。");

           // UnitProducerを持っているか試す
           UnitProducer producer = hit.collider.GetComponentInParent<UnitProducer>();
           
           // producerがnullでなければ（＝生産機能を持つ建物なら）選択処理を行う
           if (producer != null)
           {
               Debug.Log("成功: UnitProducerを発見しました。建物を選択します。");
               if (placementSystem != null) placementSystem.StopPlacement();
               buildingSelector.SelectBuilding(producer.gameObject);
               return; // 建物選択が成功したので、ここで処理を終了
           }
           
           // producerがnullの場合（＝生産機能を持たない建物）は、エラーを出さずに何もしない
       }
       else
       {
           Debug.Log("レイキャスト失敗: Buildingレイヤーのオブジェクトにはヒットしませんでした。");
       }
    
       // 【調査B】次に、地面レイヤーにレイが当たったか？
       if (Physics.Raycast(ray, out hit, 2000, placementLayerMask))
       {
           Debug.Log("レイキャスト成功: 地面にヒットしました。");
           if (buildingSelector != null) buildingSelector.DeselectBuilding();
        
           // 建築システムに通知
           OnClicked?.Invoke();
       }
       else
       {
           Debug.Log("レイキャスト失敗: 地面（Placementレイヤー）にもヒットしませんでした。");
       }
   }

     
    public bool IsPointerOverUI()
    {
        /*PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
    
        // マウスポインターの下にあるUI要素をすべて取得
        EventSystem.current.RaycastAll(eventData, results);

        // ヒットしたUI要素を一つずつチェック
        foreach (RaycastResult result in results)
        {
            // そのオブジェクトのレイヤーが「UI」レイヤーであれば、それは本物のUIと判断
            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                // UIがクリックされたのでtrueを返して処理を終了
                return true;
            }
        }

        // UIレイヤーのオブジェクトが一つも見つからなければ、UIはクリックされていないと判断
        return false;*/
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            // ヒットしたUI要素を一つずつチェック
            foreach (RaycastResult result in results)
            {
                // ★★ デバッグログを追加して、ヒットしたUIオブジェクト名を確認 ★★
                Debug.Log("UI Raycast hit: " + result.gameObject.name);

                // 元のレイヤーチェック
                if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    return true;
                }
                if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    return true; // "UI"レイヤーのみをブロックし、"InWorldUI"レイヤーは無視する
                }
            }
        }
        return false;
    }

    public Vector3 GetSelectedMapPosition()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2000, placementLayerMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}