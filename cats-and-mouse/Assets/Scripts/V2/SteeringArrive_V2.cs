using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringArrive_V2 : CatMovementScript
{
    [SerializeField] float radiusSlow;
    [SerializeField] float radiusArrive;
    [SerializeField] float maxVelocity;
    [SerializeField] float maxAcceleration;
    [SerializeField] float t2t;

    public Vector2 velocity;
    public Vector2 acceleration;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
        SetTarget(this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Reorient();
        Arrive();
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
            float rotationAngle = (velocity.x < 0) ? 180 : 0;
            transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }

    // Check if character is moving
    public override bool IsMoving()
    {
        Debug.Log(velocity.magnitude == 0);
        return velocity.magnitude == 0;
    }

    private void OnDrawGizmos()
    {
        if (!this.enabled)
            return;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(this.transform.position, target);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Ray(this.transform.position, velocity));
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Ray(this.transform.position, acceleration));
    }
}
