using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("サウンド設定")]
    public AudioClip inGameBGM;
    [Tooltip("ゲームクリア時に再生するSE")]
    public AudioClip gameClearSE;
    [Tooltip("ゲームオーバー時に再生するSE")]
    public AudioClip gameOverSE;
    
    // ★★ この行を追加 ★★
    [Tooltip("ゲーム終了時にBGMを停止するかどうか")]
    public bool stopBgmOnEnd = true;

    [Header("ゲーム設定")]
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;
    public WaveManager waveManager;
    
    [Header("ポーズメニュー")]
    public GameObject pauseMenuPanel;
    private bool isPaused = false;
    
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

    private void Update()
    {
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

        if (AudioManager.Instance != null)
        {
            // ▼▼▼ この部分を修正 ▼▼▼
            // stopBgmOnEndがtrueの場合のみBGMを停止する
            if (stopBgmOnEnd)
            {
                AudioManager.Instance.StopBGM();
            }
            AudioManager.Instance.PlaySE(gameOverSE);
        }
    }
    
    public void GameClear()
    {
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
        }
        Time.timeScale = 0f;
        
        if (AudioManager.Instance != null)
        {
            // ▼▼▼ この部分を修正 ▼▼▼
            // stopBgmOnEndがtrueの場合のみBGMを停止する
            if (stopBgmOnEnd)
            {
                AudioManager.Instance.StopBGM();
            }
            AudioManager.Instance.PlaySE(gameClearSE);
        }
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
    
    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }
    }
}