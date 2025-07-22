using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Wave
{
    public string waveName;
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnInterval;
}

public class WaveManager : MonoBehaviour
{
    [Header("Waveの構成リスト")]
    public List<Wave> waves;

    [Header("次のWaveまでの待機時間")]
    public float timeBetweenWaves = 30f;

    [Header("参照するオブジェクト")]
    public Transform spawnPoint;
    public TextMeshProUGUI nextWaveCountdownText;

    private int currentWaveIndex = -1;
    private float countdown;
    private WaveState currentState = WaveState.COUNTING_DOWN;

    // ★★ 生き残っている敵の数をカウントするリスト ★★
    private List<GameObject> aliveEnemies = new List<GameObject>();

    private enum WaveState
    {
        SPAWNING,
        WAITING,
        COUNTING_DOWN
    }

    void Start()
    {
        countdown = timeBetweenWaves;
        UpdateCountdownUI();
    }

    void Update()
    {
        // 状態がカウントダウン中の場合
        if (currentState == WaveState.COUNTING_DOWN)
        {
            countdown -= Time.deltaTime;
            UpdateCountdownUI();
            if (countdown <= 0)
            {
                StartNextWave();
            }
        }
        // ★★ Waveの敵が全滅するのを待機中の場合 ★★
        else if (currentState == WaveState.WAITING)
        {
            // 生き残りの敵がいなくなったかチェックする
            if (AllEnemiesDefeated())
            {
                // 次のWaveへのカウントダウンを開始する
                StartCountdown();
            }
        }
    }

    void StartNextWave()
    {
        // Wave番号を次に進める
        currentWaveIndex++;
        // ★★ 状態を「生成中」に変更 ★★
        currentState = WaveState.SPAWNING;

        Debug.Log("Wave " + (currentWaveIndex + 1) + " を開始します！");

        // ★★ 現在のWave情報を元に、敵の生成コルーチンを開始 ★★
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    // ★★ 敵を生成するコルーチン（非同期処理）★★
    IEnumerator SpawnWave(Wave wave)
    {
        // Waveに設定された数の敵を一体ずつ生成
        for (int i = 0; i < wave.enemyCount; i++)
        {
            GameObject enemyInstance = Instantiate(wave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            // ★★ 生成した敵をリストに追加 ★★
            aliveEnemies.Add(enemyInstance);
            
            // 設定された時間だけ待機
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // 全ての敵を生成し終えたら、状態を「待機中」に変更
        currentState = WaveState.WAITING;
        // このWaveの生成処理は完了
        yield break;
    }

    // ★★ 次のWaveへのカウントダウンを開始する処理 ★★
    void StartCountdown()
    {
        // 全てのWaveをクリアした場合
        if (currentWaveIndex >= waves.Count - 1)
        {
            Debug.Log("全てのWaveをクリアしました！ゲームクリア！");
            // TODO: ゲームクリア処理を呼び出す
            enabled = false; // WaveManagerを停止
            return;
        }

        currentState = WaveState.COUNTING_DOWN;
        countdown = timeBetweenWaves;
    }

    // ★★ 全ての敵が倒されたかチェックする処理 ★★
    bool AllEnemiesDefeated()
    {
        // リスト内のnull（破壊されたオブジェクト）を掃除する
        aliveEnemies.RemoveAll(enemy => enemy == null);
        // リストの数が0になれば、敵は全滅したと判断
        return aliveEnemies.Count == 0;
    }


    void UpdateCountdownUI()
    {
        if (nextWaveCountdownText != null)
        {
            int minutes = Mathf.FloorToInt(countdown / 60);
            int seconds = Mathf.FloorToInt(countdown % 60);
            nextWaveCountdownText.text = $"Next Wave: {minutes:00}:{seconds:00}";
        }
    }
}