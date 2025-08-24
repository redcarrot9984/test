using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    // 難易度の種類を定義
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Lunatic
    }

    // 選択された難易度を保存する変数
    public Difficulty selectedDifficulty = Difficulty.Normal; // デフォルトはNormal

    private void Awake()
    {
        // シーンを切り替えてもこのオブジェクトが破壊されないようにするシングルトン設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}