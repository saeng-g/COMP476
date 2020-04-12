using System.Collections;
using System.Collections.Generic;
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
        GameObject corner = player.getCorner();
        if (corner != null)
        {
            playerCornered = true;
            List<Vector2> tacticalLocations = corner.GetComponent<TacticalRegion>().getTacticalWaypointLocations();
            Dictionary<Vector2, GameObject> catTargetDict = new Dictionary<Vector2, GameObject>();
            Dictionary<Vector2, float> minDistDict = new Dictionary<Vector2, float>();

            foreach (Vector2 loc in tacticalLocations)
            {
                List<float> distances = new List<GameObject>(cats).ConvertAll(x => Vector2.Distance(x.transform.position, loc));
                int minIndex = distances.FindIndex(x => x.Equals(Mathf.Min(distances.ToArray())));
                if (!catTargetDict.ContainsValue(cats[minIndex]))
                {
                    catTargetDict.Add(loc, cats[minIndex]);
                    minDistDict.Add(loc, distances[minIndex]);
                }
            }

            foreach(KeyValuePair<Vector2, GameObject> pair in catTargetDict)
            {
                CatMovementController cmc = pair.Value.GetComponent<CatMovementController>();
                cmc.ForceUpdatePath(pair.Key);
                cmc.ChangeMovementState(MovementBehaviorState.ARRIVE);
            }
        }
        else
        {
            playerCornered = false;
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

    public Vector2 CalculateGlobalLocationEstimate()
    {
        int nbSmellAlerts = smellAlerts.Count;
        Vector2 averageSmellPosition = Vector2.zero;
        if (nbSmellAlerts > 0)
        {
            while (smellAlerts.Count != 0)
            {
                averageSmellPosition += smellAlerts.Pop();
            }
            averageSmellPosition /= nbSmellAlerts;
        }

        int nbHearAlerts = hearAlerts.Count;

        Vector2 averageHearPosition = Vector2.zero;
        if (nbHearAlerts > 0)
        {
            while (hearAlerts.Count != 0)
            {
                averageHearPosition += smellAlerts.Pop();
            }
            averageHearPosition /= nbHearAlerts;
        }

        Vector2 finalEstimate = (smellToHearRatio * averageSmellPosition + averageHearPosition) / (smellToHearRatio + 1);
        return finalEstimate;
    }


}
