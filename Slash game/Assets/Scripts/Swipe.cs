using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 startTouch, swipeDelta;
    private Vector3 savedMousePosition;
    private Vector2 savedSwipeDirection;

    private bool isDragging;
    private bool crossedDeadZone;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(1))
        {
            tap = true;
            isDragging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
            Reset();
        }
        #endregion

        #region Mobile Inputs
        if(Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                isDragging = true;
                startTouch = Input.touches[0].position;
            }
            else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDragging = false;
                Reset();
            }
        }
        #endregion

        CalculateSwipeDistance();
        CheckDeadZoneInput();

        savedMousePosition = Input.mousePosition;
    }

    private void CalculateSwipeDistance()
    {
        //calculate the distance
        swipeDelta = Vector2.zero;
        if (isDragging)
        {
            if (Input.touches.Length > 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            else if (Input.GetMouseButton(1))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }
    }

    private void CheckDeadZoneInput()
    {
        //Did we cross the deadzone ?
        if (swipeDelta.magnitude > 125)
        {
            crossedDeadZone = true;
            //which direction ?
            float x = swipeDelta.x;
            float y = swipeDelta.y;


            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //horizontal
                if (x < 0)
                {
                    swipeLeft = true;
                }
                else
                {
                    swipeRight = true;
                }
            }
            else
            {
                //vertical
                if (y < 0)
                {
                    swipeDown = true;
                }
                else
                {
                    swipeUp = true;
                }
            }

            Reset();
            //savedSwipeDirection = swipeDelta;

            //Debug.Log("DEPOIS   Swipe magnitude: " + swipeDelta.normalized + " magnitude do swipe salva: " + savedSwipeDirection.normalized);
            //KeepDraggingReset();
        }
    }

    private void Reset()
    {
        startTouch = Vector2.zero;
        isDragging = false;
        crossedDeadZone = false;
    }

    private void KeepDraggingReset()
    {
        crossedDeadZone = false;
        startTouch = Input.mousePosition;
        Debug.Log("this was called");
    }

    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public Vector3 SavedMousePosition { get { return savedMousePosition; } }
    public bool Tap { get { return tap; } }
    public bool CrossedDeadZone { get { return crossedDeadZone; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
}
