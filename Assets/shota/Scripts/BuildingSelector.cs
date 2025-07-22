using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    public static BuildingSelector Instance;

    public GameObject SelectedBuilding { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectBuilding(GameObject building)
    {
        SelectedBuilding = building;
        // UIManagerに通知して、対応するUIを開かせる（後で実装）
        UIManager.Instance.ShowProductionUI(SelectedBuilding);
    }

    public void DeselectBuilding()
    {
        if (SelectedBuilding != null)
        {
            UIManager.Instance.HideProductionUI();
        }
        SelectedBuilding = null;
    }
}