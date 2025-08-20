// TitleManager.cs (全体を書き換え)

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("シーン設定")]
    public string mainGameSceneName = "save20250701"; 

    [Header("UIパネル設定")]
    public GameObject titleClickPanel; // クリックしてメニューへ進むためのパネル
    public GameObject mainMenuPanel;   // メインメニューのパネル
    public GameObject manualPanel;     // マニュアル表示用のパネル

    void Start()
    {
        // ゲーム起動時は、タイトルクリック画面のみを表示する
        titleClickPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        manualPanel.SetActive(false);
    }

    void Update()
    {
        // マニュアル表示中にESCキーが押されたらメニューに戻る
        if (manualPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMainMenu();
        }
    }

    /// <summary>
    /// タイトル画面クリック時や、マニュアルから戻る時にメインメニューを表示するメソッド
    /// </summary>
    public void ShowMainMenu()
    {
        titleClickPanel.SetActive(false); // タイトルクリック画面を非表示
        manualPanel.SetActive(false);     // 念のためマニュアル画面も非表示
        mainMenuPanel.SetActive(true);      // メインメニューを表示
    }

    // ゲーム開始ボタン用メソッド (変更なし)
    public void StartGame()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }

    // マニュアル表示ボタン用メソッド (変更なし)
    public void ShowManual()
    {
        mainMenuPanel.SetActive(false);
        manualPanel.SetActive(true);
    }

    // ゲーム終了ボタン用メソッド (変更なし)
    public void QuitGame()
    {
        // エディタ実行の場合は再生を停止、ビルドしたアプリの場合はアプリケーションを終了
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}