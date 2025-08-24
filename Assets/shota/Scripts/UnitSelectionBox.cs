// UnitSelectionBox.cs (全体を書き換え)

using System.Collections.Generic;
using UnityEngine;
 
public class UnitSelectionBox : MonoBehaviour
{
    Camera myCam;
 
    [SerializeField]
    RectTransform boxVisual;
 
    Rect selectionBox;
 
    Vector2 startPosition;
    Vector2 endPosition;

    [Tooltip("クリックかドラッグかを判定するためのマウス移動のしきい値")]
    public float dragThreshold = 20f;
 
    private void Start()
    {
        myCam = Camera.main;
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawVisual();
    }
 
    private void Update()
    {
        // 左クリックを押した瞬間の処理
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionBox = new Rect();
        }
 
        // マウスを押している間の処理
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            DrawVisual();
            DrawSelection();
        }
 
        // 左クリックを離した瞬間の処理
        if (Input.GetMouseButtonUp(0))
        {
            // ★★ここからロジックを修正★★

            // マウスの移動距離を計算
            float mouseDragDistance = (startPosition - endPosition).magnitude;

            // 移動距離がしきい値を超えていれば「ドラッグ選択」とみなす
            if (mouseDragDistance > dragThreshold)
            {
                // Shiftキーが押されていなければ、まず現在の選択をすべて解除する
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    UnitSelectionManager.Instance.DeselectAll();
                }
                // その後、選択ボックス内のユニットを選択する
                SelectUnits();
            }
            // しきい値以下であれば「クリック」とみなされ、このスクリプトは何もしない
            // (クリックによる単体選択は UnitSelectionManager.cs が担当)

            // ★★ここまで修正★★

            // 変数と表示をリセット
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawVisual();
        }
    }
 
    void DrawVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;
        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;
        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        boxVisual.sizeDelta = boxSize;
    }
 
    void DrawSelection()
    {
        if (Input.mousePosition.x < startPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }
 
        if (Input.mousePosition.y < startPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }
 
    void SelectUnits()
    {
        foreach (var unit in UnitSelectionManager.Instance.allUnitsList)
        {
            if (unit.CompareTag("Unit") && selectionBox.Contains(myCam.WorldToScreenPoint(unit.transform.position)))
            {
                UnitSelectionManager.Instance.DragSelect(unit);
            }
        }
    }
}