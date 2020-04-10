using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField] GameObject[] cats;
    [SerializeField] PlayerMovement player;
    bool playerCornered;

    void Start()
    {
        playerCornered = false;    
    }
    // Update is called once per frame
    void Update()
    {
        CheckPlayer();
    }

    void CheckPlayer()
    {
        if (!playerCornered)
        {
            CheckPlayerCornered();
        } else if (player.getCorner() == null)
        {
            foreach (GameObject cat in cats)
            {
                cat.GetComponent<SteeringArrive>().SetRandomBehavior(true);
            }
            playerCornered = false;
        }
    }

    void CheckPlayerCornered()
    {
        GameObject corner = player.getCorner();
        if (player.getCorner() != null)
        {
            playerCornered = true;
            Vector2 entry1 = corner.transform.GetChild(0).transform.position;
            Vector2 entry2 = corner.transform.GetChild(1).transform.position;
            GameObject closest1 = null;
            GameObject closest2 = null;
            float min1 = float.MaxValue;
            float min2 = float.MaxValue;

            foreach (GameObject cat in cats)
            {
                float mag1 = (entry1 - (Vector2)(cat.transform.position)).magnitude;
                float mag2 = (entry2 - (Vector2)(cat.transform.position)).magnitude;
                if (mag1 < min1)
                {
                    if (closest1 != null && closest2 == null)
                    {
                        min2 = min1;
                        closest2 = closest1;
                    }
                    min1 = mag1;
                    closest1 = cat;
                    continue;
                }

                if (mag2 < min2)
                {
                    min2 = mag2;
                    closest2 = cat;
                }
            }

            closest1.GetComponent<SteeringArrive>().SetTarget(entry1);
            closest2.GetComponent<SteeringArrive>().SetTarget(entry2);
            //TODO: Once they arrive they can approach towards the player
        }
    }
}
