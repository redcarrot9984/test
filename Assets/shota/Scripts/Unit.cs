// Unit.cs (全体を書き換え)

using UnityEngine;
using UnityEngine.AI; // NavMeshAgentを使用するために必要

public class Unit : MonoBehaviour
{
    [Tooltip("このユニットのデータベース上のID")]
    [SerializeField]
    private int databaseID; 
    public ObjectData unitData { get; private set; }

    private float currentHealth;
    private GameManager gameManager;
    public HealthTracker healthTracker;
    public bool isBuilding = false;

    void Awake()
    {
        // データベースから自身の全データを取得
        unitData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseID);
        if (unitData == null)
        {
            Debug.LogError($"{gameObject.name} : ID {databaseID} のデータがデータベースに見つかりません！", this);
            return;
        }

        // 体力をデータベースの値で初期化
        currentHealth = unitData.MaxHealth;

        // ★★ここから追加：NavMeshAgentにデータベースの値を設定★★
        // isBuildingがfalse（つまりユニット）の場合のみ速度を設定
        if (!isBuilding)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // データベースから読み込んだ移動速度をagentのspeedに設定
                agent.speed = unitData.MoveSpeed;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} にNavMeshAgentコンポーネントが見つかりません。", this);
            }
        }
        // ★★ここまで追加★★
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // 建物でない場合のみ、選択リストに追加する
        if (!isBuilding && UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        }

        UpdateHealthUI();
    }

    private void OnDestroy()
    {
        if(UnitSelectionManager.Instance != null && !isBuilding)
        {
            UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
            UnitSelectionManager.Instance.unitsSelected.Remove(gameObject);
        }
    }

    public void TakeDamage(int damageToInflict)
    {
        currentHealth -= damageToInflict;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthTracker != null)
        {
            // 体力バーを更新
            healthTracker.UpdateSliderValue(currentHealth, unitData.MaxHealth);
        }

        if (currentHealth <= 0)
        {
            // 城ならゲームオーバー
            if (isBuilding && CompareTag("Castle"))
            {
                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
            }
            
            Destroy(gameObject);
        }
    }
}