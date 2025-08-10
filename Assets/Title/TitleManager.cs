// TitleManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // Inspectorでメインゲームシーンの名前を設定します
    // 例: "SampleScene", "Main" など
    public string mainGameSceneName = "save20250701"; 

    // スタートボタンが押されたときに呼び出すメソッド
    public void StartGame()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }
}