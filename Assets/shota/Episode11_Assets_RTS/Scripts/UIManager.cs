using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button buildButton;
    public PlacementSystem placement;
    public static UIManager Instance; // シングルトン化
    public GameObject unitProductionPanel;
    public GameObject productionSlotPrefab; // ★★ 生産スロットのプレハブ ★★
    public Transform productionPanelContent; // ★★ スロットを配置する親オブジェクト（Layout Group付き） ★★
    // 生産パネルをインスペクターから設定
    private void Start()
    {
        
        buildButton.onClick.AddListener(() => Construct(0));
       
    }

    private void Construct(int id)
    {
        Debug.Log("clicked");
        placement.StartPlacement(id);
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowProductionUI(GameObject building)
    {
        unitProductionPanel.SetActive(true);

        // 既存のスロットをクリア
        foreach (Transform child in productionPanelContent)
        {
            Destroy(child.gameObject);
        }

        UnitProducer producer = building.GetComponent<UnitProducer>();
        if (producer == null) return;

        // 生産可能なユニットのリストを取得
        List<int> producibleUnits = producer.GetProducibleUnits();

        // ユニットごとに生産スロットを生成
        foreach (int unitID in producibleUnits)
        {
            GameObject slotGO = Instantiate(productionSlotPrefab, productionPanelContent);
            ProductionSlot slot = slotGO.GetComponent<ProductionSlot>();
            ObjectData unitData = DatabaseManager.Instance.databaseSO.GetObjectByID(unitID);

            // スロットにユニットIDと表示情報を設定
            slot.unitID = unitID;
            // 例：slotGO.GetComponent<Image>().sprite = unitData.Icon; // アイコンなどがあれば設定
            // 例：slotGO.GetComponentInChildren<Text>().text = unitData.Name;
        }
    }

    public void HideProductionUI()
    {
        // 既存のスロットをクリア
        foreach (Transform child in productionPanelContent)
        {
            Destroy(child.gameObject);
        }
        unitProductionPanel.SetActive(false);
    }
}
