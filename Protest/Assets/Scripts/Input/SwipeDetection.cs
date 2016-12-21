using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : Base
{

    public enum SwipeDirections { left, up, down, right, none };
    public SwipeDirections swipeDirection;

    public float sensitivityTime = 0.8f;
    public float sensitivity = 90;

    public Vector2 beginPos, endPos;

    private float beginTime;

    public static SwipeDetection instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        swipeDirection = SwipeDirections.none;
#if !UNITY_EDITOR
        if (Input.touchCount <= 0)
            return;
#endif


#if !UNITY_EDITOR
        if (Input.touches[0].phase == TouchPhase.Began)
        {
            beginTime = Time.time;
            beginPos = Input.touches[0].position;
        }

        if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
        {
            endPos = Input.touches[0].position;

            if (Time.time - beginTime < sensitivityTime)
            {
                if(Mathf.Abs(endPos.x - beginPos.x) >= sensitivity)
                    swipeDirection = (endPos.x - beginPos.x < 0) ? SwipeDirections.left : SwipeDirections.right;
                if (Mathf.Abs(endPos.y - beginPos.y) >= sensitivity)
                    swipeDirection = (endPos.y - beginPos.y < 0) ? SwipeDirections.down : SwipeDirections.up;
            }
        }
#endif

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            beginTime = Time.time;
            beginPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPos = Input.mousePosition;

            if (Time.time - beginTime <= sensitivityTime)
            {
                if(Mathf.Abs(endPos.x - beginPos.x) >= sensitivity)
                    swipeDirection = (endPos.x - beginPos.x < 0) ? SwipeDirections.left : SwipeDirections.right;
                if (Mathf.Abs(endPos.y - beginPos.y) >= sensitivity)
                    swipeDirection = (endPos.y - beginPos.y < 0) ? SwipeDirections.down : SwipeDirections.up;
            }
        }
#endif
    }
}
