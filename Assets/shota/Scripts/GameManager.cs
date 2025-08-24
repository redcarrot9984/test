// GameManager.cs (全体を書き換え)

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("サウンド設定")]
    public AudioClip inGameBGM;

    [Header("ゲーム設定")]
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;
    public WaveManager waveManager;
    
    // ★★ ここから追加 ★★
    [Header("ポーズメニュー")]
    public GameObject pauseMenuPanel; // インスペクターからポーズメニューのUIパネルをここに割り当てる
    private bool isPaused = false;
    // ★★ ここまで追加 ★★
    
    private Transform castleTransform;

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

    // ★★ Updateメソッドを追加 ★★
    private void Update()
    {
        // Escapeキーが押されたらポーズメニューをトグル表示する
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void RegisterCastle(Transform newCastleTransform)
    {
        castleTransform = newCastleTransform;
     
        Debug.Log("<color=cyan>GAMEMANAGER:</color> Castle has been registered by placement system!");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(inGameBGM);
        }
        
        if (waveManager != null)
        {
            waveManager.StartWaves();
        }
        else
        {
            Debug.LogError("WaveManager is not assigned in the GameManager inspector!");
        }
    }

    public Transform GetCastleTransform()
    {
        if (castleTransform == null)
        {
            GameObject castleObject = GameObject.FindGameObjectWithTag("Castle");
            if (castleObject != null)
            {
                castleTransform = castleObject.transform;
                Debug.Log("<color=cyan>GAMEMANAGER:</color> Found pre-placed castle in the scene.");
            }
        }
        return castleTransform;
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }
    public void GameClear()
    {
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void BackToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene"); 
    }
    
    // ★★ ここから新しいメソッドを追加 ★★

    /// <summary>
    /// ゲームを再開する（UIボタンから呼び出す）
    /// </summary>
    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    /// <summary>
    /// ポーズ状態を切り替える
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // ゲームをポーズ
            Time.timeScale = 0f;
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }
        else
        {
            // ゲームを再開
            Time.timeScale = 1f;
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }
    }
    // ★★ ここまで追加 ★★
}