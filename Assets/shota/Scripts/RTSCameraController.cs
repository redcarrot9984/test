// code/RTSCameraController.cs (全体を書き換え)

using UnityEngine;
using UnityEngine.EventSystems;

public class RTSCameraController : MonoBehaviour
{
    public static RTSCameraController instance;

    [Header("General")]
    [SerializeField] Transform cameraTransform;
    public Transform followTransform;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    [Header("Optional Functionality")]
    [SerializeField] bool moveWithKeyboad = true;
    [SerializeField] bool moveWithEdgeScrolling = true;
    [SerializeField] bool moveWithMouseDrag = true;

    [Header("Movement Speeds")]
    [Tooltip("通常時のカメラ移動速度")]
    [SerializeField] float normalSpeed = 20f;
    [Tooltip("Shiftキーを押したときの高速移動速度")]
    [SerializeField] float fastSpeed = 40f;
    
    [Header("Edge Scrolling")]
    [SerializeField] float edgeSize = 50f;

    [Header("Zoom")]
    [SerializeField] float zoomSpeed = 20f;
    [SerializeField] float minY = 15f;
    [SerializeField] float maxY = 100f;
    
    // カーソル関連の変数は変更なし
    private bool isCursorSet = false;
    public Texture2D cursorArrowUp;
    public Texture2D cursorArrowDown;
    public Texture2D cursorArrowLeft;
    public Texture2D cursorArrowRight;
    private CursorArrow currentCursor = CursorArrow.DEFAULT;
    enum CursorArrow { UP, DOWN, LEFT, RIGHT, DEFAULT }

    private void Awake() // StartからAwakeに変更
    {
        instance = this;
    }

    private void Update()
    {
        // 追従対象がいる場合は何もしない
        if (followTransform != null)
        {
            transform.position = followTransform.position;
            return;
        }

        // カメラ操作のメイン処理を呼び出す
        HandleCameraMovement();
        
        // 追従を解除する
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    void HandleCameraMovement()
    {
        // 現在のフレームで適用する移動量を計算するベクトル
        Vector3 move = Vector3.zero;
        
        // 現在の速度を決定 (Shiftキーで高速化)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        // --- キーボード入力 ---
        if (moveWithKeyboad)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                move += transform.forward;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                move -= transform.forward;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                move += transform.right;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                move -= transform.right;
        }

        // --- 画面端スクロール ---
        if (moveWithEdgeScrolling)
        {
            if (Input.mousePosition.x > Screen.width - edgeSize)
            {
                move += transform.right;
                ChangeCursor(CursorArrow.RIGHT);
            }
            else if (Input.mousePosition.x < edgeSize)
            {
                move -= transform.right;
                ChangeCursor(CursorArrow.LEFT);
            }
            else if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                move += transform.forward;
                ChangeCursor(CursorArrow.UP);
            }
            else if (Input.mousePosition.y < edgeSize)
            {
                move -= transform.forward;
                ChangeCursor(CursorArrow.DOWN);
            }
            else
            {
                ChangeCursor(CursorArrow.DEFAULT);
            }
        }
        
        // 計算した移動量をカメラの位置に直接反映
        // move.normalizedで斜め移動が速くなるのを防ぐ
        transform.position += move.normalized * currentSpeed * Time.deltaTime;

        // --- ズーム処理 ---
        float scrollInput = Input.mouseScrollDelta.y;
        if (scrollInput != 0)
        {
            Vector3 pos = transform.position;
            pos.y -= scrollInput * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos; // ズームも直接反映
        }
        
        // --- マウスドラッグ移動 ---
        if (moveWithMouseDrag)
        {
            HandleMouseDragInput();
        }

        // カーソルが画面外に出ないようにする
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    // マウスドラッグのロジックは newPosition を使わないように変更
    private void HandleMouseDragInput()
    {
        if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            Plane plane = new Plane(Vector3.up, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            Plane plane = new Plane(Vector3.up, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                // 現在位置に差分を直接加える
                transform.position += dragStartPosition - dragCurrentPosition;
            }
        }
    }
    
    // ChangeCursorメソッドは変更なし
    private void ChangeCursor(CursorArrow newCursor)
    {
        if (currentCursor == newCursor) return;
        
        isCursorSet = newCursor != CursorArrow.DEFAULT;

        switch (newCursor)
        {
            case CursorArrow.UP:    Cursor.SetCursor(cursorArrowUp, Vector2.zero, CursorMode.Auto); break;
            case CursorArrow.DOWN:  Cursor.SetCursor(cursorArrowDown, new Vector2(cursorArrowDown.width, cursorArrowDown.height), CursorMode.Auto); break;
            case CursorArrow.LEFT:  Cursor.SetCursor(cursorArrowLeft, Vector2.zero, CursorMode.Auto); break;
            case CursorArrow.RIGHT: Cursor.SetCursor(cursorArrowRight, new Vector2(cursorArrowRight.width, cursorArrowRight.height), CursorMode.Auto); break;
            default:                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); break;
        }
        currentCursor = newCursor;
    }
}