// code/FaceCamera.cs (全体を書き換え)

using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform localTrans;
    private Camera mainCamera; // カメラを保持する変数をprivateに

    private void Start()
    {
        localTrans = GetComponent<Transform>();
        
        // ★★ ゲーム開始時に、自動でメインカメラを探して設定する ★★
        mainCamera = Camera.main;
    }

    private void LateUpdate() // UpdateからLateUpdateに変更
    {
        // カメラが見つかっていれば、その方向を向く
        if (mainCamera != null)
        {
            // カメラの方向を向くように回転させる
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
}