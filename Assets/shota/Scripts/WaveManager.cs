using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

// Wave内で、敵のグループを定義するためのクラス
[System.Serializable]
public class EnemyGroup
{
    [Tooltip("このグループで出現させる敵のプレハブ")]
    public GameObject enemyPrefab;
    [Tooltip("この敵を何体出現させるか")]
    public int enemyCount;
    [Tooltip("次の敵が出現するまでの時間（秒）")]
    public float spawnInterval;
}

// 1つのWaveの構成を定義するクラス
[System.Serializable]
public class Wave
{
    public string waveName;
    [Tooltip("このWaveで出現する敵グループのリスト")]
    public List<EnemyGroup> enemyGroups;
}

public class WaveManager : MonoBehaviour
{
    [Header("難易度ごとのWaveリスト")]
    [Tooltip("Easyモードの時に使用するWaveのリスト")]
    public List<Wave> easyWaves;
    [Tooltip("Normalモードの時に使用するWaveのリスト")]
    public List<Wave> normalWaves;
    [Tooltip("Hardモードの時に使用するWaveのリスト")]
    public List<Wave> hardWaves;
    [Tooltip("Lunaticモードの時に使用するWaveのリスト")]
    public List<Wave> lunaticWaves;

    // ゲーム中に実際に使用されるWaveリスト
    private List<Wave> waves;

    [Header("Wave設定")]
    [Tooltip("次のWaveが始まるまでの待機時間（秒）")]
    public float timeBetweenWaves = 30f;

    [Header("参照オブジェクト")]
    [Tooltip("Wave情報を表示するUIテキスト")]
    public TextMeshProUGUI nextWaveCountdownText;
    
    [Header("敵の出現範囲")]
    [Tooltip("城を中心とした敵の出現半径")]
    public float spawnRadius = 50f;
    
    private int currentWaveIndex = -1;
    private float countdown;
    private WaveState currentState = WaveState.COUNTING_DOWN;
    
    private List<GameObject> aliveEnemies = new List<GameObject>();

    private enum WaveState
    {
        SPAWNING,
        WAITING,
        COUNTING_DOWN,
        ALL_WAVES_CLEARED
    }
    private bool wavesActive = false; 
    
    void Awake()
    {
        // GameSettingsから選択された難易度を読み込み、使用するWaveリストを決定する
        if (GameSettings.Instance != null)
        {
            switch (GameSettings.Instance.selectedDifficulty)
            {
                case GameSettings.Difficulty.Easy:
                    waves = easyWaves;
                    break;
                case GameSettings.Difficulty.Normal:
                    waves = normalWaves;
                    break;
                case GameSettings.Difficulty.Hard:
                    waves = hardWaves;
                    break;
                case GameSettings.Difficulty.Lunatic:
                    waves = lunaticWaves;
                    break;
                default: // 念のため
                    Debug.LogWarning("不明な難易度が設定されています。Normalで開始します。");
                    waves = normalWaves;
                    break;
            }
        }
        else
        {
            // タイトルシーンを経由せずに直接ゲームシーンを実行した場合のフォールバック
            Debug.LogWarning("GameSettingsが見つかりません。難易度Normalで開始します。");
            waves = normalWaves;
        }
    }
    
    void Start()
    {
        if (nextWaveCountdownText != null)
        {
            nextWaveCountdownText.text = "Build a Castle to begin";
        }
    }
    
    public void StartWaves()
    {
        if (wavesActive) return;

        wavesActive = true;
        countdown = timeBetweenWaves;
        currentState = WaveState.COUNTING_DOWN;
        Debug.Log("Wave system has been activated by GameManager.");
    }

    void Update()
    {
        if (!wavesActive)
        {
            return;
        }

        if (currentState == WaveState.COUNTING_DOWN)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                StartNextWave();
            }
        }
        else if (currentState == WaveState.WAITING)
        {
            if (AllEnemiesDefeated())
            {
                StartCountdown();
            }
        }
        
        UpdateWaveStatusUI();
    }

    void StartNextWave()
    {
        currentWaveIndex++;
        currentState = WaveState.SPAWNING;
        Debug.Log("Wave " + (currentWaveIndex + 1) + " を開始します！");
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }
    
    IEnumerator SpawnWave(Wave wave)
    {
        // Waveに設定されているすべての敵グループを順番に処理する
        foreach (var group in wave.enemyGroups)
        {
            // グループ内の敵を、指定された数だけ出現させる
            for (int i = 0; i < group.enemyCount; i++)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                if(spawnPosition == Vector3.zero)
                {
                    Debug.LogError("Failed to find a valid spawn position on the NavMesh.");
                    yield return new WaitForSeconds(1.0f); // 1秒待ってリトライ
                    i--; // カウンターを戻して、同じ敵を再度スポーン試行
                    continue;
                }

                // groupに設定された敵プレハブをインスタンス化
                GameObject enemyInstance = Instantiate(group.enemyPrefab, spawnPosition, Quaternion.identity);
                aliveEnemies.Add(enemyInstance);
                
                // groupに設定されたインターバルだけ待つ
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        // すべてのグループのスポーンが終わったら、待機状態に移行
        currentState = WaveState.WAITING;
    }
    
    void StartCountdown()
    {
        if (currentWaveIndex >= waves.Count - 1)
        {
            Debug.Log("全てのWaveをクリアしました！ゲームクリア！");
            currentState = WaveState.ALL_WAVES_CLEARED;
            GameManager.Instance.GameClear();
            enabled = false; 
            return;
        }

        currentState = WaveState.COUNTING_DOWN;
        countdown = timeBetweenWaves;
    }
    
    bool AllEnemiesDefeated()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
        return aliveEnemies.Count == 0;
    }

    void UpdateWaveStatusUI()
    {
        if (nextWaveCountdownText == null) return;

        switch (currentState)
        {
            case WaveState.COUNTING_DOWN:
                int minutes = Mathf.FloorToInt(countdown / 60);
                int seconds = Mathf.FloorToInt(countdown % 60);
                nextWaveCountdownText.text = $"Next Wave: {minutes:00}:{seconds:00}";
                break;
                
            case WaveState.SPAWNING:
            case WaveState.WAITING:
                aliveEnemies.RemoveAll(enemy => enemy == null);
                nextWaveCountdownText.text = $"Enemies: {aliveEnemies.Count}";
                break;

            case WaveState.ALL_WAVES_CLEARED:
                nextWaveCountdownText.text = "All Waves Cleared!";
                break;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Transform castleTransform = GameManager.Instance.GetCastleTransform();
        if (castleTransform == null)
        {
            Debug.LogError("Cannot find Castle Transform!");
            return Vector3.zero;
        }

        for (int i = 0; i < 30; i++) // 30回までリトライ
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection += castleTransform.position;
            randomDirection.y = castleTransform.position.y;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, spawnRadius, -1))
            {
                return navHit.position;
            }
        }
        
        Debug.LogError("NavMesh上で有効なスポーン地点を見つけられませんでした。");
        return Vector3.zero;
    }
}