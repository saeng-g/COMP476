using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [Tooltip("Reference to all the cat characters in the game")]
    [SerializeField] GameObject[] cats;
    [Tooltip("Reference to the Player character in the game")]
    [SerializeField] PlayerMovement player;
    [Tooltip("The smell alerts received by every cat at every computation")]
    [SerializeField] Stack<Vector2> smellAlerts;
    [Tooltip("The sound alerts received by every cat at every computation")]
    [SerializeField] Stack<Vector2> hearAlerts;
    [Tooltip("The ratio of weight put on smell alerts to weight put on sound alerts. Smell should be weighted less than sound as there is a larger degree of uncertainty")]
    [SerializeField] [Range(0.01f, 1)] float smellToHearRatio;

    public const int SMELL_ALERT = 0;
    public const int HEAR_ALERT = 1;

    [SerializeField] bool playerCornered;

    void Start()
    {
        playerCornered = false;
        smellAlerts = new Stack<Vector2>();
        hearAlerts = new Stack<Vector2>();
        gizmoEstimate = Vector2.zero;
    }
    // Update is called once per frame
    void Update()
    {
        CheckPlayer();
    }

    void CheckPlayer()
    {
        CheckPlayerCornered();
    }

    void CheckPlayerCornered()
    {
        Vector2? targetPos = CalculateGlobalLocationEstimate();

        //TODO: Might wanna make all cats just wander
        if (targetPos == null)
        {
            return;
        }

        GameObject corner = player.getCorner(targetPos.Value);
        if (corner != null)
        {

            //if corner was already computed, do nothing cats shoul already be cornering
            if (playerCornered)
            {
                return;
            }

            playerCornered = true;
            CornerPlayer(corner);
        }
        else
        {
            playerCornered = false;

            //check for closest cat and go to mouse estimate location
            //other cats will wander
            CatMovementController closest = null;
            float min_dist = float.MaxValue;
            for (int i = 0; i < cats.Length; ++i)
            {
                CatMovementController cmc = cats[i].GetComponent<CatMovementController>();
                cmc.ForceUpdatePath(targetPos.Value);
                float curr_dist = cmc.GetPath().GetPathLength();

                if (closest == null)
                {
                    cmc.ChangeMovementState(MovementBehaviorState.PURSUE);
                    min_dist = curr_dist;
                    closest = cmc;
                } else if (curr_dist < min_dist)
                {
                    closest.ChangeMovementState(MovementBehaviorState.WANDER);

                    cmc.ChangeMovementState(MovementBehaviorState.PURSUE);
                    min_dist = curr_dist;
                    closest = cmc;
                } else
                {
                    cmc.ChangeMovementState(MovementBehaviorState.WANDER);
                }

            }

            //debugging
            gizmoEstimate = targetPos.Value;
        }
    }

    //Corners the player in the passed tactical region
    private void CornerPlayer(GameObject corner)
    {
        List<Vector2> tacticalLocations = corner.GetComponent<TacticalRegion>().getTacticalWaypointLocations();
        List<int> indexes_unused = Enumerable.Range(0, cats.Length).ToList();

        foreach (Vector2 loc in tacticalLocations)
        {
            int ind_used = -1;
            float min_dist = float.MaxValue;
            for (int i = 0; i < cats.Length; ++i)
            {
                if (!indexes_unused.Contains(i))
                {
                    continue;
                }

                float dist = Vector2.Distance(cats[i].transform.position, loc);
                if (dist < min_dist)
                {
                    min_dist = dist;
                    ind_used = i;
                }
            }

            if (ind_used == -1)
            {
                return;
            }

            CatMovementController cmc = cats[ind_used].GetComponent<CatMovementController>();
            cmc.ForceUpdatePath(loc);
            cmc.ChangeMovementState(MovementBehaviorState.ARRIVE);

            indexes_unused.Remove(ind_used);
        }

        foreach (int unused in indexes_unused)
        {
            CatMovementController cmc = cats[unused].GetComponent<CatMovementController>();
            cmc.ChangeMovementState(MovementBehaviorState.WANDER);
        }
    }

    public void AddAlert(Vector2 alertPos, int smellOrHearAlert)
    {
        if (smellOrHearAlert == SMELL_ALERT)
            //update hear alert stack
            smellAlerts.Push(alertPos);
        else
            //update hear alert stack
            hearAlerts.Push(alertPos);
    }

    //calculates player location based on smell and sound
    public Vector2? CalculateGlobalLocationEstimate()
    {
        //Debug.Log("Starting Global Location Estimate Calculation");
        int nbSmellAlerts = smellAlerts.Count;
        //Debug.LogFormat("There are {0} nbSmellAlerts", nbSmellAlerts);
        Vector2 averageSmellPosition = Vector2.zero;
        //Debug.Log("Initiallized averageSmellPosition: " + averageSmellPosition);

        if (nbSmellAlerts > 0)
        {
            //debugging
            smellAlertsCopy = smellAlerts.ToArray();

            averageSmellPosition = smellAlerts.Pop();
            //Debug.LogFormat("Reinitializing averageSmellPosition: {0}", averageSmellPosition);
            while (smellAlerts.Count != 0)
            {
                //Debug.LogFormat("Adding {0} alert to average", smellAlerts.Peek());
                //Debug.LogFormat("Old average: {0}", averageSmellPosition);
                averageSmellPosition += smellAlerts.Pop();
                //Debug.LogFormat("New average: {0}", averageSmellPosition);
            }
            averageSmellPosition /= nbSmellAlerts;

            //Debug.LogFormat("Average Post division: {0}", averageSmellPosition);
        }

        int nbHearAlerts = hearAlerts.Count;

        Vector2 averageHearPosition = Vector2.zero;
        if (nbHearAlerts > 0)
        {
            //debugging
            hearAlertsCopy = hearAlerts.ToArray();

            averageHearPosition = hearAlerts.Pop();
            while (hearAlerts.Count != 0)
            {
                averageHearPosition += hearAlerts.Pop();
            }
            averageHearPosition /= nbHearAlerts;
        }

        if (nbSmellAlerts > 0 && nbHearAlerts > 0)
            return (smellToHearRatio * averageSmellPosition + averageHearPosition) / (smellToHearRatio + 1);
        else if (nbSmellAlerts > 0)
            return averageSmellPosition;
        else if (nbHearAlerts > 0)
            return averageHearPosition;
        return null;
    }

    Vector2[] smellAlertsCopy;
    Vector2[] hearAlertsCopy;
    Vector2 gizmoEstimate;

    //Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(cats[0].transform.position, cats[1].transform.position);
        Gizmos.DrawLine(cats[1].transform.position, cats[2].transform.position);
        Gizmos.DrawLine(cats[2].transform.position, cats[0].transform.position);

        if (smellAlertsCopy != null)
        {
            foreach (Vector2 alert in smellAlertsCopy)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(player.transform.position, alert);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(alert, gizmoEstimate);
            }
        }

        if (hearAlertsCopy != null)
        {
            foreach (Vector2 alert in hearAlertsCopy)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(player.transform.position, alert);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(alert, gizmoEstimate);
            }
        }
    }

}
