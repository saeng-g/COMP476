using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightArea : MonoBehaviour
{
    [SerializeField] Collider2D visionDetector;
    [SerializeField] SpriteRenderer sprite;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.Equals(visionDetector))
            sprite.enabled = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.Equals(visionDetector))
            sprite.enabled = false;
    }

}
