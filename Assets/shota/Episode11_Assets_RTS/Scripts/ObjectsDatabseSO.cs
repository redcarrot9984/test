using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabseSO : ScriptableObject
{
    public List<ObjectData> objectsData;


    public ObjectData GetObjectByID(int id)
    {
        foreach (ObjectData obj in objectsData)
        {
            if (obj.ID == id)
            {
                return obj;
            }
        }

        return new(); // This cannot happen
    }

}

public enum BuildingType
{
    None,
    Muscat,
    Wood,
    Castle,
    Stone,
    Gold,
    Heisha
    
}

[System.Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }
    
    [field: SerializeField]
    public BuildingType thisBuildingType { get; private set; }

    // ▼▼▼ この部分を変更 ▼▼▼
    [field: Header("UI 表示")]
    [field: SerializeField]
    [field: Tooltip("購入ボタンにマウスを乗せた時に表示される説明文")]
    [field: TextArea(5, 10)]
    public string TooltipDescription { get; private set; }
    // ▲▲▲ ここまで変更 ▲▲▲

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    [field: SerializeField]
    public List<BuildRequirement> resourceRequirements { get; private set; }

    [field: SerializeField]
    public List<BuildingType> buildDependency { get; private set; }
    
    [field: SerializeField]
    public List<BuildBenefits> benefits { get; private set; }
    
    [field: Header("Unit Production")]
    [field: SerializeField]
    public bool IsUnit { get; private set; } = false;

    [field: SerializeField]
    public GameObject UnitPrefab { get; private set; }
  
    [field: Header("サウンド設定")]
    [field: Tooltip("建物を設置した時のSE")]
    [field: SerializeField]
    public AudioClip PlacementSound { get; private set; }

    [field: Tooltip("ユニットが攻撃した時のSE")]
    [field: SerializeField]
    public AudioClip AttackSound { get; private set; }
    
    [field: Tooltip("このユニットを生産するために必要な建物の種類")]
    [field: SerializeField]
    public BuildingType producingBuilding { get; private set; } = BuildingType.None;

    [field: Header("Unit Combat Stats")]

    [field: Header("ユニット移動ステータス")]
    [field: Tooltip("ユニットの移動速度")]
    [field: SerializeField]
    public float MoveSpeed { get; private set; } = 5f;
    
    [field: Tooltip("ユニットの体力")]
    [field: SerializeField]
    public float MaxHealth { get; private set; } = 100f;

    [field: Tooltip("ユニットの攻撃力")]
    [field: SerializeField]
    public int Damage { get; private set; } = 10;

    [field: Tooltip("攻撃の射程距離")]
    [field: SerializeField]
    public float AttackRange { get; private set; } = 1.5f;

    [field: Tooltip("攻撃の速さ（1秒間に何回攻撃するか）")]
    [field: SerializeField]
    public float AttackRate { get; private set; } = 1f;

    [field: Tooltip("敵を感知する範囲。索敵コライダーの半径と合わせる")]
    [field: SerializeField]
    public float SensorRange { get; private set; } = 10f;

    [field: Tooltip("遠距離ユニットかどうか")]
    [field: SerializeField]
    public bool IsRanged { get; private set; } = false;

    [field: Tooltip("遠距離ユニットの場合の弾プレハブ")]
    [field: SerializeField]
    public GameObject ProjectilePrefab { get; private set; }
    
    [field: Header("Area of Effect")]
    [field: Tooltip("攻撃が範囲攻撃かどうか")]
    [field: SerializeField]
    public bool IsAreaOfEffect { get; private set; } = false;

    [field: Tooltip("範囲攻撃の場合の半径")]
    [field: SerializeField]
    public float AreaOfEffectRadius { get; private set; } = 2f;

    [field: Tooltip("このユニットの生産にかかる時間（秒）")]
    [field: SerializeField]
    public float productionTime { get; private set; } = 5f;

}

[System.Serializable]
public class BuildRequirement
{
    public ResourceManager.ResourcesType resource;
    public int amount;
}


[System.Serializable]
public class BuildBenefits
{
    public enum BenefitType
    {
        Housing
    }

    public string benefit;
    public Sprite benefitIcon;
    public BenefitType benefitType;
    public int benefitAmount;
}