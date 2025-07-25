// Code/GameManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 城のTransform。privateにして、外部からはメソッド経由でのみアクセスするようにします。
    private Transform castleTransform;

    public GameObject gameOverPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Playerが建物を設置した時に PlacementState から呼ばれるメソッド
    public void RegisterCastle(Transform newCastleTransform)
    {
        castleTransform = newCastleTransform;
        Debug.Log("<color=cyan>GAMEMANAGER:</color> Castle has been registered by placement system!");
    }

    // EnemyAI が城の場所を知るために呼び出すメソッド
    public Transform GetCastleTransform()
    {
        // まだ城の情報を知らない（参照がnullの）場合のみ
        if (castleTransform == null)
        {
            // シーン内から "Castle" タグを持つオブジェクトを探す
            // これにより、最初からシーンに配置されている城を見つけることができる
            GameObject castleObject = GameObject.FindGameObjectWithTag("Castle");
            if (castleObject != null)
            {
                // 見つけたら、その情報を保持する
                castleTransform = castleObject.transform;
                Debug.Log("<color=cyan>GAMEMANAGER:</color> Found pre-placed castle in the scene.");
            }
        }

        // 保持している城の情報を返す（見つからなければnullが返る）
        return castleTransform;
    }

    // ゲームオーバー処理
    public void GameOver()
    {
        Debug.Log("GAME OVER - called");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    // ゲームリスタート処理
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}