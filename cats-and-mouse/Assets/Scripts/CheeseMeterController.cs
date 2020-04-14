using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheeseMeterController : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = player.score / PlayerMovement.MAXCHEESE;
    }
}
