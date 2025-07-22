using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    // インスペクターからこの建物が生産する資源の種類を設定
    public ResourceManager.ResourcesType resourceType;
    
    // 1秒あたりに生産する量
    public int amountPerSecond = 1;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        // 1秒経過ごとに
        if (timer >= 1f)
        {
            // 資源を増やす
            ResourceManager.Instance.IncreaseResource(resourceType, amountPerSecond);
            // タイマーをリセット
            timer -= 1f;
        }
    }
}