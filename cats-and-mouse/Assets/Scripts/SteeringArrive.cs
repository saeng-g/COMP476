using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringArrive : MonoBehaviour
{
    [SerializeField] Vector2 target;
    [SerializeField] float radiusSlow;
    [SerializeField] float radiusArrive;
    [SerializeField] float maxVelocity;
    [SerializeField] float maxAcceleration;
    [SerializeField] float t2t;
    [SerializeField] bool isTrueTopDown;
    bool randomBehavior;

    private Vector2 velocity;
    private Vector2 acceleration;

    private float changeDirectionTimer;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
        Debug.Log(velocity.magnitude);
        acceleration = Vector2.zero;
        target = this.transform.position;
        randomBehavior = true;
        ResetChangeDirectionTimer();
    }

    // Update is called once per frame
    void Update()
    {
        // change direction if within timer
        // TODO: implement more robust behaviour
        if (randomBehavior)
        {
            changeDirectionTimer -= Time.deltaTime;
        }
        if (changeDirectionTimer <= 0)
        {
            SetRandomTarget();
            ResetChangeDirectionTimer();
        }

        // move
        Reorient();
        Arrive();
    }

    private void ResetChangeDirectionTimer()
    {
        changeDirectionTimer = 0.5f;
    }

    // Set target to a random location
    // adjust the values in Random.Range() to adjust possible values
    private void SetRandomTarget()
    {
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-5, 5);
        target = new Vector2(x, y);
    }

    // Update gameobject's position based on velocity
    // p = p + delta_v
    private void UpdatePosition(float time)
    {
        Vector3 deltaPos = time * velocity;
        transform.position += deltaPos;
    }

    // Update gameobject's velocity based on acceleration
    // v = v + delta_a
    private void UpdateVelocity(float time)
    {
        this.velocity += time * acceleration;
        if (this.velocity.magnitude > this.maxVelocity)
            this.velocity = this.velocity.normalized * this.maxVelocity;
    }

    // Steering Arrive
    private void Arrive()
    {
        Vector2 displacementVector = target - (Vector2)transform.position;
        float magnitude = displacementVector.magnitude;
        Vector2 directionalVector = displacementVector.normalized;

        if (magnitude > radiusSlow)
        {
            this.acceleration = maxAcceleration * directionalVector;
        }
        else if (magnitude > radiusArrive)
        {
            float goalSpeed = this.maxVelocity * (magnitude / radiusSlow);
            Vector2 goalVelocity = directionalVector * goalSpeed;
            this.acceleration = (goalVelocity - this.velocity) / this.t2t;
        }
        else
        {
            this.acceleration = Vector2.zero;
            this.velocity = Vector2.zero;
            return;
        }

        if (this.acceleration.magnitude > this.maxAcceleration)
            this.acceleration = this.acceleration.normalized * this.maxAcceleration;
        UpdateVelocity(Time.fixedDeltaTime);
        UpdatePosition(Time.fixedDeltaTime);
    }

    // Reorient to face the moving direction
    private void Reorient()
    {
        if (velocity.magnitude > 0)
        {
            if (isTrueTopDown)
            {
                float angle = Mathf.Atan2(-velocity.x, velocity.y) * 180 / Mathf.PI;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.fixedDeltaTime);
            }
            else
            {
                float rotationAngle = (velocity.x < 0) ? 180 : 0;
                transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
            }
        }
    }

    // Set Target to a particular position.
    public void SetTarget(Vector2 position)
    {
        this.target = position;
        this.randomBehavior = false;
    }

    public void SetRandomBehavior(bool val)
    {
        this.randomBehavior = val;
    }
}
