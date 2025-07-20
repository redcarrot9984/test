using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Unit : MonoBehaviour
{
    private float unitHealth;
    public float unitMaxHealth;

    public HealthTracker healthTracker;

    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);

        unitHealth = unitMaxHealth;
        UpdateHealthUI();
    }

   private void OnDestroy()
   {
   		if(UnitSelectionManager.Instance != null)
   		{
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
        }
   }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if (unitHealth <= 0)
        {
            //Dying logic
            Destroy(gameObject);
        }
    }



   internal void TakeDamage(int damageToInflict)
   {
       unitHealth -= damageToInflict;
       UpdateHealthUI();
   }

}