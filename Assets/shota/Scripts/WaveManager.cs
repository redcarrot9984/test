// Code/WaveManager.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

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
    public TextMeshProUGUI nextWaveCountdownText;
    
    [Header("敵の出現範囲")]
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
        for (int i = 0; i < wave.enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            if(spawnPosition == Vector3.zero)
            {
                Debug.LogError("Failed to find a valid spawn position on the NavMesh.");
                yield return null;
                i--;
                continue;
            }

            GameObject enemyInstance = Instantiate(wave.enemyPrefab, spawnPosition, Quaternion.identity);
            aliveEnemies.Add(enemyInstance);
            
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        currentState = WaveState.WAITING;
        yield break;
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

    // ★★ このメソッドをクラスの直下に移動しました ★★
    private Vector3 GetRandomSpawnPosition()
    {
        Transform castleTransform = GameManager.Instance.GetCastleTransform();
        if (castleTransform == null)
        {
            Debug.LogError("Cannot find Castle Transform!");
            return Vector3.zero;
        }

        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += castleTransform.position;
        randomDirection.y = castleTransform.position.y;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, spawnRadius, -1))
        {
            return navHit.position;
        }

        return Vector3.zero;
    }
}