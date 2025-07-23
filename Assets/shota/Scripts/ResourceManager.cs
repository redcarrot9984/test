using System;
using System.Collections.Generic; // Dictionaryを使うために必要
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }

    // ▼▼▼ 新しい資源タイプを定義 ▼▼▼
    public enum ResourcesType
    {
        Food,
        Wood,
        Stone,
        Gold
    }

    // ▼▼▼ Dictionaryを使って4つの資源を管理する ▼▼▼
    private Dictionary<ResourcesType, int> resourceAmounts;

    public event Action<ResourcesType> OnResourceChanged;
    public event Action OnBuildingsChanged;

    // ▼▼▼ 各資源を表示するUIテキスト（インスペクターから設定） ▼▼▼
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI goldText;

    public List<BuildingType> allExistingBuildings;

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

        // ▼▼▼ Dictionaryを初期化し、各資源の初期値を設定 ▼▼▼
        resourceAmounts = new Dictionary<ResourcesType, int>();
        resourceAmounts[ResourcesType.Food] = 100;
        resourceAmounts[ResourcesType.Wood] = 100;
        resourceAmounts[ResourcesType.Stone] = 50;
        resourceAmounts[ResourcesType.Gold] = 20;
    }

    private void Start()
    {
        UpdateAllUI();
    }
    
    // ▼▼▼ 資源を増減させるメソッド群 ▼▼▼
    public void IncreaseResource(ResourcesType resource, int amount)
    {
        resourceAmounts[resource] += amount;
        OnResourceChanged?.Invoke(resource); // どの資源が変わったかを通知
    }

    public void DecreaseResource(ResourcesType resource, int amount)
    {
        resourceAmounts[resource] -= amount;
        OnResourceChanged?.Invoke(resource);
    }

    public int GetResourceAmount(ResourcesType resource)
    {
        return resourceAmounts.ContainsKey(resource) ? resourceAmounts[resource] : 0;
    }

    // ▼▼▼ UI更新処理 ▼▼▼
    private void UpdateAllUI()
    {
        UpdateUI(ResourcesType.Food);
        UpdateUI(ResourcesType.Wood);
        UpdateUI(ResourcesType.Stone);
        UpdateUI(ResourcesType.Gold);
    }

    private void UpdateUI(ResourcesType resource)
    {
        int amount = GetResourceAmount(resource);
        switch (resource)
        {
            case ResourcesType.Food:
                if (foodText != null) foodText.text = $"{amount}";
                break;
            case ResourcesType.Wood:
                if (woodText != null) woodText.text = $"{amount}";
                break;
            case ResourcesType.Stone:
                if (stoneText != null) stoneText.text = $"{amount}";
                break;
            case ResourcesType.Gold:
                if (goldText != null) goldText.text = $"{amount}";
                break;
        }
    }

    // ▼▼▼ イベントの登録・解除 ▼▼▼
    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }
    
    // ResourceManager.cs のクラス内に、このメソッドを追加してください。

    public void DecreaseResourcesBasedOnRequirements(ObjectData objectData)
    {
        if (objectData == null) return;
    
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
    }
    

    // (以下、既存のメソッドは変更なし、またはこのスクリプト内では不要)
    public void UpdateBuildingChanged(BuildingType buildingType, bool isNew)
    {
        // ★★ ここからが修正・追加箇所 ★★

        if (isNew)
        {
            // 新しい建物がリストになければ追加する
            if (!allExistingBuildings.Contains(buildingType))
            {
                allExistingBuildings.Add(buildingType);
            }
        }
        else
        {
            // 建物が破壊された場合の処理（今回は不要ですが将来のために）
            if (allExistingBuildings.Contains(buildingType))
            {
                allExistingBuildings.Remove(buildingType);
            }
        }

        // ★★ 建物リストに変更があったことを、全てのUIボタンに通知する！ ★★
        OnBuildingsChanged?.Invoke();

        // ★★ ここまで ★★
    }
    internal void DecreaseRemoveResourcesBasedOnRequirement(ObjectData objectData) { /* ... */ }
}