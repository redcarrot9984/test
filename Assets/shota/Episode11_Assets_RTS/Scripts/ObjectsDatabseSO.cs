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

    [field: SerializeField]
    [TextArea(3, 10)]
    public string description;

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
    // ▼▼▼ 以下を追加 ▼▼▼
    [field: Header("Unit Production")]
    [field: SerializeField]
    public bool IsUnit { get; private set; } = false;

    [field: SerializeField]
    public GameObject UnitPrefab { get; private set; }
  
    // ▼▼▼ このフィールドを追加 ▼▼▼
    [field: Tooltip("このユニットを生産するために必要な建物の種類")]
    [field: SerializeField]
    public BuildingType producingBuilding { get; private set; } = BuildingType.None;
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