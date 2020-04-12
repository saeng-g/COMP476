using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    [SerializeField] List<GameObject> cats;
    [SerializeField] GameObject mouse;
    [SerializeField] Camera cam;

    [SerializeField] float catHearRadius = 10f;
    [SerializeField] float catSmellRadius = 15f;
    [SerializeField] float mouseHearRadius = 10f;
    [SerializeField] float mouseSmellRadius = 15f;

    [SerializeField] float soundOffset = 3f;
    [SerializeField] float smellOffset = 7f;

    [SerializeField] GameObject soundAlert;
    [SerializeField] GameObject smellAlert;

    [SerializeField] float maxCatHearAlertTimer;
    [SerializeField] float maxCatSmellAlertTimer;
    [SerializeField] float maxMouseHearAlertTimer;
    [SerializeField] float maxMouseSmellAlertTimer;

    private float catHearAlertTimer;
    private float catSmellAlertTimer;
    private float mouseHearAlertTimer;
    private float mouseSmellAlertTimer;

    // Start is called before the first frame update
    void Start()
    {
        if (cats == null || cats.Count < 1)
            cats = new List<GameObject>(GameObject.FindGameObjectsWithTag("Cat"));
        if (mouse == null)
            mouse = GameObject.FindGameObjectWithTag("Player");
        if (cam == null)
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        TickTimer();
        bool resetMouseHearAlertTimer = false;
        bool resetMouseSmellAlertTimer = false;
        foreach (GameObject cat in cats)
        {
            float d = Vector2.Distance(cat.transform.position, mouse.transform.position);
            // ALERT PLAYER
            if (d <= mouseHearRadius
                && cat.GetComponent<CatMovementController>().IsMoving()
                && mouseHearAlertTimer <= 0)
            {
                // alert player of cat sounds
                Vector2 vaguePosition = RandomOffsetPosition(cat.transform.position, soundOffset);
                Vector2 onScreenPosition = GetPositionInViewport(vaguePosition);
                Instantiate<GameObject>(soundAlert, onScreenPosition, Quaternion.identity);
                resetMouseHearAlertTimer = true;
            }
            else if (d <= mouseSmellRadius && mouseSmellAlertTimer <= 0)
            {
                // alert player of cat smell
                Vector2 vaguePosition = RandomOffsetPosition(cat.transform.position, smellOffset);
                Vector2 onScreenPosition = GetPositionInViewport(vaguePosition);
                Instantiate<GameObject>(smellAlert, onScreenPosition, Quaternion.identity);
                resetMouseSmellAlertTimer = true;
            }
            // ALERT AI
            if (d <= catHearRadius
                && mouse.GetComponent<PlayerMovement>().isMoving
                && catHearAlertTimer <= 0)
            {
                // alert cats of mouse sounds
                Vector2 vaguePosition = RandomOffsetPosition(mouse.transform.position, soundOffset);
                //cat.GetComponent<SteeringArrive>().SetTarget(vaguePosition); // TODO: adjust to strategy
                ResetCatHearTimer();
            }
            else if (d <= catSmellRadius && catSmellAlertTimer <= 0)
            {
                // alert cats of mouse smell
                Vector2 vaguePosition = RandomOffsetPosition(mouse.transform.position, smellOffset);
                //cat.GetComponent<SteeringArrive>().SetTarget(vaguePosition); // TODO: adjust to strategy
                ResetCatSmellTimer();
            }
        }
        if (resetMouseHearAlertTimer)
            ResetMouseHearTimer();
        if (resetMouseSmellAlertTimer)
            ResetMouseSmellTimer();
    }

    Vector2 RandomOffsetPosition(Vector2 position, float offset)
    {
        Vector2 vaguePosition = position + new Vector2(
            Random.Range(-offset, offset), Random.Range(-offset, offset)
        );
        return vaguePosition;
    }

    Vector2 GetPositionInViewport(Vector2 position)
    {
        Vector2 camPosition = cam.WorldToViewportPoint(position);
        if (camPosition.x >= 0 && camPosition.x <= 1
            && camPosition.y >= 0 && camPosition.y <= 1)
            return position;
        float onScreenX = Mathf.Clamp(camPosition.x, 0, 1);
        float onScreenY = Mathf.Clamp(camPosition.y, 0, 1);
        return new Vector2(onScreenX, onScreenY);
    }

    void TickTimer()
    {
        // in case no alert in a long time, avoid underflow
        catHearAlertTimer = Mathf.Max(-1, catHearAlertTimer - Time.deltaTime);
        catSmellAlertTimer = Mathf.Max(-1, catSmellAlertTimer - Time.deltaTime);
        mouseHearAlertTimer = Mathf.Max(-1, mouseHearAlertTimer - Time.deltaTime);
        mouseSmellAlertTimer = Mathf.Max(-1, mouseSmellAlertTimer - Time.deltaTime);
    }

    void ResetCatHearTimer()
    {
        catHearAlertTimer = maxCatHearAlertTimer;
    }

    void ResetCatSmellTimer()
    {
        catSmellAlertTimer = maxCatSmellAlertTimer;
    }

    void ResetMouseHearTimer()
    {
        mouseHearAlertTimer = maxMouseHearAlertTimer;
    }

    void ResetMouseSmellTimer()
    {
        mouseSmellAlertTimer = maxMouseSmellAlertTimer;
    }
}
