// Unit.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    private float unitHealth;
    public float unitMaxHealth;
    private GameManager gameManager;

    public HealthTracker healthTracker;

    // ★★ これが建物かどうかを判定するフラグを追加 ★★
    public bool isBuilding = false;

    void Start()
    {
        // ★★ シーン内のGameManagerを検索して保持しておく ★★
        gameManager = FindObjectOfType<GameManager>();
        // ★★ 建物でない場合のみ、選択リストに追加する ★★
        if (!isBuilding && UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        }

        unitHealth = unitMaxHealth;
        UpdateHealthUI();
    }

    private void OnDestroy()
    {
        if(UnitSelectionManager.Instance != null)
        {
            if (!isBuilding) // 建物でない場合のみ
            {
                UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
                UnitSelectionManager.Instance.unitsSelected.Remove(gameObject);
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (healthTracker != null)
        {
            healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);
        }

        if (unitHealth <= 0)
        {
            // ★★ もしこのオブジェクトがお城なら、ゲームオーバー処理を呼び出す（後で実装） ★★
            if (isBuilding && CompareTag("Castle"))
            {
                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
                Debug.Log("GAME OVER"); // とりあえずログを出す
                // FindObjectOfType<GameManager>().GameOver(); // 最終形
            }
            
            Destroy(gameObject);
        }
    }

    internal void TakeDamage(int damageToInflict)
    {
        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }
}