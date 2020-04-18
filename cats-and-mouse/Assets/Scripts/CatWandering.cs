using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatWandering : MonoBehaviour
{
    // Use this for initialization
    public float speed;
  
    int flag = 3;
    int coll = 0;
   float interval=10;
    float RandomTime = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        
       //BoxCollider2D bc = gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        
      // this.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (RandomTime == 0)
        {
            int WhatDirection = Random.Range(1, 5);
            switch (WhatDirection)
            {

                case 1:

                    flag = 1; coll = 0;
                    break;

                case 2:

                    flag = 2; coll = 0;


                    break;
                case 3:

                    flag = 3; coll = 0;
                    break;
                case 4:


                    flag = 4; coll = 0;

                    break;

            }
        }
        if (interval > 0)
        {
            RandomTime = 1;
            if (flag == 1 && coll == 0)
            {
                Vector2 v = new Vector2(1, 0);
                transform.Translate(v * speed * Time.deltaTime);
            }
            if (flag == 1 && coll == 1)
            {
                coll = 0; flag = 2;
            }
            if (flag == 2 && coll == 0)
            {
                Vector2 v = new Vector2(1, 0);
                transform.Translate(-v * speed * Time.deltaTime);
            }
            if (flag == 2 && coll == 1)
            {
                coll = 0; flag = 1;
            }
            ////
            if (flag == 3 && coll == 0)
            {
                Vector2 v = new Vector2(0, 1);
                transform.Translate(v * speed * Time.deltaTime);
            }
            if (flag == 3 && coll == 1)
            {
                coll = 0; flag = 4;
            }
            if (flag == 4 && coll == 0)
            {
                Vector2 v = new Vector2(0, 1);
                transform.Translate(-v * speed * Time.deltaTime);
            }
            if (flag == 4 && coll == 1)
            {
                coll = 0; flag = 3;
            }
            interval -= Time.deltaTime;
        }
        else
        { RandomTime = 0; interval = 10; }



    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.gameObject.tag == "Wall" || col.gameObject.tag == "Cat") && coll == 0)
        {


            coll = 1;
        }

       
     


    }
}
