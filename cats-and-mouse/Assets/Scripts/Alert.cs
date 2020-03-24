using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : MonoBehaviour
{

    [SerializeField] float lifetime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        Color tmp = sr.color;
        tmp.a = lifetime/2;
        sr.color = tmp;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }
}
