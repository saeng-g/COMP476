using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform visionLight;

    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        if (speed <= 0)
            speed = 0.5f;
        if (playerCamera == null)
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (visionLight == null)
            visionLight = GameObject.FindGameObjectWithTag("VisionLight").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0) //if moving
        {
            isMoving = true;
            float angle = Mathf.Atan2(-x, y) * 180 / Mathf.PI;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            visionLight.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.fixedDeltaTime);
            float rotationAngle = (x < 0) ? 180 : 0;
            transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }
        else
            isMoving = false;

        transform.position += (Vector3)(speed * new Vector2(x, y) * Time.fixedDeltaTime);
        playerCamera.position = transform.position + (Vector3.forward * -10);
    }

}
