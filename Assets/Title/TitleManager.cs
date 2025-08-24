using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("サウンド設定")]
    public AudioClip titleBGM; 

    [Header("シーン設定")]
    public string mainGameSceneName = "save20250701"; 

    [Header("UIパネル設定")]
    public GameObject titleClickPanel;
    public GameObject mainMenuPanel;
    public GameObject manualPanel;
    public GameObject difficultyPanel; // ★★ 難易度選択パネルをインスペクターから設定 ★★

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(titleBGM);
        }

        titleClickPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        manualPanel.SetActive(false);
        difficultyPanel.SetActive(false); // 最初は非表示
    }

    void Update()
    {
        if (manualPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        titleClickPanel.SetActive(false);
        manualPanel.SetActive(false);
        difficultyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ★★ `StartGame`から名前変更 ★★
    public void ShowDifficultySelect()
    {
        mainMenuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    // ★★ 難易度ボタンから呼び出す新しいメソッド ★★
    public void SelectDifficultyAndStart(int difficulty)
    {
        // GameSettingsに難易度を設定
        GameSettings.Instance.selectedDifficulty = (GameSettings.Difficulty)difficulty;
        // ゲームシーンをロード
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void ShowManual()
    {
        mainMenuPanel.SetActive(false);
        manualPanel.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}