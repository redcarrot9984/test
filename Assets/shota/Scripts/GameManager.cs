using UnityEngine;
using UnityEngine.SceneManagement; // シーンをリロードするために必要

public class GameManager : MonoBehaviour
{
    // ゲームオーバー画面のUIパネルをインスペクターから設定する
    public GameObject gameOverPanel;

    // ゲームオーバー処理
    public void GameOver()
    {
        Debug.Log("GAME OVER - called");

        // ゲームオーバーパネルをアクティブにする
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // ゲームの時間を止める
        Time.timeScale = 0f;
    }

    // ゲームをリスタートするメソッド（UIボタンから呼び出す用）
    public void RestartGame()
    {
        // 時間の流れを元に戻す
        Time.timeScale = 1f;
        // 現在のシーンをリロードする
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}