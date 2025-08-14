// code/Unit.cs

using UnityEngine;

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