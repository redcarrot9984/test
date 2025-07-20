using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    public Material idleStateMaterial;
    public Material attackStateMaterial;
    public Material followStateMaterial;

    public int unitDamage;
    public bool isPlayer;


    private void OnTriggerEnter(Collider other)
    {
      if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
      {
        targetToAttack = other.transform;
      }
    }



    private void OnTriggerStay(Collider other)
    {
      if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
      {
        targetToAttack = other.transform;
      }
    }



    private void OnTriggerExit(Collider other)
     {
 if (isPlayer && other.CompareTag("Enemy") && targetToAttack != null)
      {
        targetToAttack = null;
      }
     }
    public void SetIdleMaterial()
    {
      // GetComponent<Renderer>().material = idleStateMaterial;
    }
    public void SetAttackMaterial()
    {
         //GetComponent<Renderer>().material = attackStateMaterial;
     }
    public void SetFollowMaterial()
    {
       // GetComponent<Renderer>().material = followStateMaterial;
    }

    private void OnDrawGizmos()
    {
        // Follow Distance /Area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f*0.2f);

        // Attack Distance /Area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
        // Stop  Attack Distance /Area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1.2f);



    }


}
