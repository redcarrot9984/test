using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitMovement : MonoBehaviour
{
	Camera cam;
	UnityEngine.AI.NavMeshAgent agent;
	public LayerMask ground;

    public bool isCommandedToMove;
    
    DirectionIndicator directionIndicator;
    
    Animator animator;

	private void Start()
	{
		cam = Camera.main;
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		animator = GetComponent<Animator>();
		directionIndicator = GetComponent<DirectionIndicator>();
	}
	
	private void Update()
	{
		if(Input.GetMouseButtonDown(1))
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
			{
			    isCommandedToMove = true;
				agent.SetDestination(hit.point);
				animator.SetBool("isMoving",true);
				directionIndicator.DrawLine(hit);
			}
		}

		if(agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        {
            isCommandedToMove = false;
            animator.SetBool("isMoving",false);
        } 
        else{
            animator.SetBool("isMoving",true);
        }


	}
}