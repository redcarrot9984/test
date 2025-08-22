// TitleManager.cs (全体を書き換え)

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

    void Start()
    {
        // AudioManagerにBGMの再生を依頼
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(titleBGM);
        }

        titleClickPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        manualPanel.SetActive(false);
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
        mainMenuPanel.SetActive(true);
    }

    public void StartGame()
    {
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