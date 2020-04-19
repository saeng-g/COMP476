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
    [SerializeField] float pursueEpsilon;
    [Tooltip("How close to the target before updating pathIndex")]
    [SerializeField] float arriveEpsilon;
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
    [Tooltip("Script for wandering behaviour")]
    [SerializeField] CatWandering wanderingScript;

    // TODO: Wander and Guard movements

    // RefreshTimer
    [Tooltip("time between recalculating path")]
    [SerializeField] float recomputePathTime;
    [SerializeField] private float recomputePathTimer;

    private PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        movementState = MovementBehaviorState.WANDER;
        targetLocation = transform.position;
        recomputePathTimer = 3f;
        targetForPathfinding = transform.position;
        player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<PlayerMovement>() == null) {
            print("There is no more mouse; he was likely eaten.");
            return;
        }

        RefocusToVision();
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

    private void UpdatePath()
    {
        recomputePathTimer -= Time.deltaTime;
        if (recomputePathTimer <= 0 && movementState != MovementBehaviorState.WANDER)
        {
            GetNewPath();
            movementState = MovementBehaviorState.PURSUE;
            recomputePathTimer = recomputePathTime;
        }

        float distanceToTarget = Vector2.Distance(targetLocation, transform.position);
        float epsilon = movementState == MovementBehaviorState.ARRIVE ? arriveEpsilon : pursueEpsilon;
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
        wanderingScript.enabled = false;

        steeringArriveScript.SetTarget(targetLocation);
    }

    // Enables the pursue script to the targetLocation and disables all else
    private void Pursue()
    {
        steeringArriveScript.enabled = false;
        steeringPursueScript.enabled = true;
        wanderingScript.enabled = false;

        steeringPursueScript.SetTarget(targetLocation);
    }

    // TODO:
    private void Wander()
    {
        steeringArriveScript.enabled = false;
        steeringPursueScript.enabled = false;
        wanderingScript.enabled = true;
        //wander
    }

    // Disables all scripts (and remains stationary)
    private void Guard()
    {
        steeringArriveScript.enabled = false;
        steeringPursueScript.enabled = false;
        wanderingScript.enabled = false;

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

    public void RefocusToVision()
    {
        if (IsPlayerSeen() && movementState != MovementBehaviorState.ARRIVE)
        {
            ForceUpdatePath(player.transform.position);
            movementState = MovementBehaviorState.PURSUE;
        }
    }

    private bool IsPlayerSeen()
    {
        if (Vector2.Distance(player.transform.position, this.transform.position) <= 5)
        {
            return true;
        }
        else return false;
    }

    public Pathfinding_V2.Path GetPath()
    {
        return pathToFollow;
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

    public bool Equals(CatMovementController other)
    {
        return this.GetInstanceID() == other.GetInstanceID();
    }
}
