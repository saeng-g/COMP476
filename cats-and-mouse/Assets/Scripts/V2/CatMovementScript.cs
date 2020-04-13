using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CatMovementScript : MonoBehaviour
{
    [SerializeField] protected Vector2 target;

    public void SetTarget(Vector2 target)
    {
        this.target = target;
    }

    abstract public bool IsMoving();
}
