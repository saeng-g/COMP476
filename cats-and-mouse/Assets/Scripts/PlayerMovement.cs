using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform playerCamera;
    [SerializeField] bool isTrueTopDownSprite;
    GameObject cornerTilemap;

    //score depending on amount of cheese eaten
    float score;
    //max cheese consumption
    const float MAXCHEESE = 20;
    //cheese score
    const float CHEESESCORE = 5;

    Vector2 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        cornerTilemap = null;
        currentVelocity = new Vector2(0.0f, 0.0f);
        if (speed <= 0)
            speed = 0.5f;
        if (playerCamera == null)
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0) //if moving
        {
            if (isTrueTopDownSprite)
            {
                float angle = Mathf.Atan2(-x, y) * 180 / Mathf.PI;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.fixedDeltaTime);
            }
            else
            {
                float rotationAngle = (x < 0) ? 180 : 0;
                transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
            }
        }

        currentVelocity = speed * new Vector2(x, y) * Time.fixedDeltaTime;
        currentVelocity = ComputeVelocityWithCheese(currentVelocity);
        transform.position += (Vector3) currentVelocity;
        playerCamera.position = transform.position + (Vector3.forward * -10);
    }

    Vector2 getNextPosition()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        return currentPos + currentVelocity;
    }

    public GameObject getCorner()
    {
        return cornerTilemap;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Corner"))
        {
            cornerTilemap = collision.gameObject;
        }

        if (collision.name.Contains("Cheese"))
        {
            Destroy(collision.gameObject);

            //TODO: Determine what we want score to be
            score += CHEESESCORE; 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Contains("Corner"))
        {
            cornerTilemap = null;
        }
    }

    private Vector2 ComputeVelocityWithCheese(Vector2 velocity)
    {
        float ratio =  1 - (score / MAXCHEESE);
        ratio = Mathf.Clamp(ratio, 0, 0.8f) + 0.2f;
        return velocity * ratio;
    }
}
