using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitSelectionManager : MonoBehaviour
{
   public static UnitSelectionManager Instance { get; set; }
   
   public List<GameObject> allUnitsList = new List<GameObject>();
   public List<GameObject> unitsSelected = new List<GameObject>();

    public LayerMask clickable;
    public LayerMask ground;

    public LayerMask attackable;
    public bool attackCursorVisible;

    public GameObject groundMarker;

    public Camera cam;

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
  }


    private void Start()
    {
       cam = Camera.main;
    }
  public void Update()
  {
      if (Input.GetMouseButtonDown(0))
      {
         RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // If we are hitting a cluickable object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                  MultiSelect(hit.collider.gameObject);
                }
                else
                {
                  SelectByClicking(hit.collider.gameObject);
                }


            }
            else
            {
            if(Input.GetKey(KeyCode.LeftShift) ==  false)
            {
                  DeselectAll();
            }

            }
       }
    if (Input.GetMouseButtonDown(1) && unitsSelected.Count > 0)
            {
               RaycastHit hit;
                  Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                  // If we are hitting a cluickable object
                  if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                  {
                     groundMarker.transform.position = hit.point;

                     groundMarker.SetActive(false);
                     groundMarker.SetActive(true);
                  }
            }

        //attack target
 if (unitsSelected.Count >0 && AtleastOneOffensiveUnit(unitsSelected))
            {
               RaycastHit hit;
                  Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                  // If we are hitting a cluickable object
                  if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable))
                  {
                     Debug.Log("Enemy Hovered with mouse");

                     attackCursorVisible = true;
                     if (Input.GetMouseButtonDown(1))
                     {
                        Transform target = hit.transform;

                        foreach (GameObject unit in unitsSelected)
                        {
                           if (unit.GetComponent<AttackController>())
                            {
                              unit.GetComponent<AttackController>().targetToAttack = target;
                            }
                        }
                     }
                  }
                  else
                  {
                   attackCursorVisible = false;
                  }


            }

            CursorSelector();

  }

  private void CursorSelector()
  {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Selectable);
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable) && unitsSelected.Count > 0 && AtleastOneOffensiveUnit(unitsSelected))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Attackable);
        }   
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground) && unitsSelected.Count > 0)
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Walkable);
        }
        else
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.None);
        }
  }

    private bool AtleastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
         foreach (GameObject unit in unitsSelected)
         {
              if (unit.GetComponent<AttackController>())
              {
                 return true;
              }

         }
            return false;
    }


    private void MultiSelect(GameObject gameObject)
    {
       if (unitsSelected.Contains(gameObject) == false)
         {
              // If the unit is not already selected, add it to the list
              unitsSelected.Add(gameObject);
                SelectUnit(gameObject, true);
         }
         else
         {
              // If the unit is already selected, remove it from the list
              unitsSelected.Remove(gameObject);
               SelectUnit(gameObject, false);
         }

    }
   public void DeselectAll()
  {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit, false);
        }

        // Clear the list of selected units
         groundMarker.SetActive(false);
   unitsSelected.Clear();
     //throw new NotImplementedException();
  }

  internal void DragSelect(GameObject unit)
    {
            if (unitsSelected.Contains(unit) == false)
            {
                unitsSelected.Add(unit);
                SelectUnit(unit , true);
            }


    }



  private void SelectByClicking(GameObject gameObject)
  {
        DeselectAll();

        unitsSelected.Add(gameObject);
        SelectUnit(gameObject , true);
  }

   private void SelectUnit(GameObject unit, bool isSelected)
   {
    TriggerSelectionIndicator(unit, isSelected);
    EnableUnitMovement(unit, isSelected);
   }


  private void EnableUnitMovement(GameObject unit, bool shouldMove)
  {
    unit.GetComponent<UnitMovement>().enabled = shouldMove;
  }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.Find("Indicator").gameObject.SetActive(isVisible);}

}