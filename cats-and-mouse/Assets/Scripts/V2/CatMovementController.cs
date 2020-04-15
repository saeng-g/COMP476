using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementBehaviorState
{
    ARRIVE, // arrives at location
    PURSUE, // goes towards location
    WANDER, // "patrol" or move around
    GUARD // stay still in a particular direction
}

public class CatMovementController : MonoBehaviour
{
    // Attributes relevant to path following
    Pathfinding_V2.Path pathToFollow;
    [Tooltip("Pathfinding script")]
    [SerializeField] Pathfinding_V2 pathfinder;
    [Tooltip("How close to the target before updating pathIndex")]
    [SerializeField] float epsilon;
    [Tooltip("To easily check what state the cat is in (for testing")]
    [SerializeField] MovementBehaviorState movementState;
    [Tooltip("To see how far along the path the cat is (for testing")]
    [SerializeField] int pathIndex;
    [Tooltip("To see where the cat is targetting next (for testing")]
    [SerializeField] Vector2 targetLocation;
    [Tooltip("Target Location to compute the path for")]
    [SerializeField] Vector2 targetForPathfinding;

    // Movement scripts
    [Tooltip("Script for arrive behaviour")]
    [SerializeField] CatMovementScript steeringArriveScript;
    [Tooltip("Script for pursue behaviour")]
    [SerializeField] CatMovementScript steeringPursueScript;
    // TODO: Wander and Guard movements

    // RefreshTimer
    [Tooltip("time between recalculating path")]
    [SerializeField] float recomputePathTime;
    [SerializeField] private float recomputePathTimer;


    // Start is called before the first frame update
    void Start()
    {
        movementState = MovementBehaviorState.PURSUE;
        targetLocation = transform.position;
        recomputePathTimer = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        //For Testing
        try {
            targetForPathfinding = FindObjectOfType<PlayerMovement>().transform.position;
            UpdatePath();
            if (movementState == MovementBehaviorState.ARRIVE)
                Arrive();
            else if (movementState == MovementBehaviorState.PURSUE)
                Pursue();
            else if (movementState == MovementBehaviorState.WANDER)
                Wander();
            else
                Guard();
        }        
        catch (NullReferenceException e) {
            print("There is no more mouse; he was likely eaten.");
        }       
    }

    private void UpdatePath()
    {
        recomputePathTimer -= Time.deltaTime;
        if (recomputePathTimer <= 0)
        {
            GetNewPath();
            movementState = MovementBehaviorState.PURSUE;
            recomputePathTimer = recomputePathTime;
        }

        float distanceToTarget = Vector2.Distance(targetLocation, transform.position);
        if (distanceToTarget <= epsilon && pathToFollow != null)
        {
            if (pathIndex >= pathToFollow.trimmedPathCoordList.Count - 1)
            {
                movementState = MovementBehaviorState.ARRIVE;
                targetLocation = transform.position;
                return;
            }
            pathIndex++;
            targetLocation = pathToFollow.trimmedPathCoordList[pathIndex];
        }
    }

    public void ForceUpdatePath(Vector2 target)
    {
        targetForPathfinding = target;
        GetNewPath();
        recomputePathTimer = recomputePathTime;
    }

    private void GetNewPath()
    {
        pathToFollow = pathfinder.GetPathForChars(this.transform.position, targetForPathfinding);
        pathIndex = 0;
        targetLocation = pathToFollow.trimmedPathCoordList[pathIndex];
        if (pathToFollow == null)
            Debug.Log("no path was returned from pathfinder");
    }


    // Enables the arrive script to the targetLocation and disables all else
    private void Arrive()
    {
        steeringArriveScript.enabled = true;
        steeringPursueScript.enabled = false;

        steeringArriveScript.SetTarget(targetLocation);
    }

    // Enables the pursue script to the targetLocation and disables all else
    private void Pursue()
    {
        steeringArriveScript.enabled = false;
        steeringPursueScript.enabled = true;

        steeringPursueScript.SetTarget(targetLocation);
    }

    // TODO:
    private void Wander()
    {
        steeringArriveScript.enabled = false;
        steeringPursueScript.enabled = false;

        //wander
    }

    // Disables all scripts (and remains stationary)
    private void Guard()
    {
        steeringArriveScript.enabled = false;
        steeringPursueScript.enabled = false;

        //guard
    }

    public bool IsMoving()
    {
        if (steeringArriveScript.enabled)
            return steeringArriveScript.IsMoving();
        else if (steeringPursueScript.enabled)
            return steeringPursueScript.IsMoving();
        else if (movementState == MovementBehaviorState.WANDER)
            return true;
        else
            return false;
    }

    public void ChangeMovementState(MovementBehaviorState state)
    {
        this.movementState = state;
    }

    private void OnDrawGizmos()
    {
        if (pathToFollow == null || !this.enabled)
            return;
        Gizmos.color = Color.red;
        foreach (var e in pathToFollow.trimmedPathEntryList)
        {
            if (e.from != null && e.node != null)
            {
                Gizmos.DrawLine(e.from.wp.transform.position, e.node.wp.transform.position);
            }
        }
    }
}
