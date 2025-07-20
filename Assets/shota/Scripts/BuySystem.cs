using System;
using UnityEngine;
using UnityEngine.UI;

public class BuySystem : MonoBehaviour
{
    public GameObject buildingsPanel;
    public GameObject unitsPanel;
    
    public Button buildingsButton;
    public Button unitsButton;

    public PlacementSystem placementSystem;
    private void Start()
    {
        unitsButton.onClick.AddListener(UnitsCategorySelected);
        buildingsButton.onClick.AddListener(BuildingsCategorySelected);
        
        unitsPanel.SetActive(false);
        buildingsPanel.SetActive(true);
    }

    private void UnitsCategorySelected()
    {
        unitsPanel.SetActive(true);
        buildingsPanel.SetActive(false);
    }

    private void BuildingsCategorySelected()
    {
        unitsPanel.SetActive(false);
        buildingsPanel.SetActive(true);
    }

}
