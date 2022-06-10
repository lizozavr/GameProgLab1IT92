using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeManager : Singleton<SwipeManager>
{
    public enum Direction { Left, Right, Up, Down};
    bool[] swipe = new bool[4];
    Vector2 startTouch;
    bool touchMoved;
    Vector2 swipeDelta;

    const float SWIPE_THRESHOLD = 50;
    public delegate void MoveDelegate(bool[] swipes);
    public MoveDelegate MoveEvent;

    public delegate void ClickDelegate(Vector2 pos);
    public ClickDelegate ClickEvent;
    Vector2 TouchPosition() { return (Vector2)Input.mousePosition; }
    bool TouchBegan() { return Input.GetMouseButtonDown(0); }
    bool TouchEnded() { return Input.GetMouseButtonUp(0); }
    bool GetTouch() { return Input.GetMouseButton(0); }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(TouchBegan())
        {
            startTouch = TouchPosition();
            touchMoved = true;
        }
        else if(TouchEnded() && touchMoved == true)
        {
            SendSwipe();
            touchMoved = false;
        }
        swipeDelta = Vector2.zero;
        if(touchMoved && GetTouch())
        {
            swipeDelta = TouchPosition() - startTouch;
        }

        if(swipeDelta.magnitude > SWIPE_THRESHOLD)
        {
            if(Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                swipe[(int)Direction.Left] = swipeDelta.x < 0;
                swipe[(int)Direction.Right] = swipeDelta.x > 0;
            }
            else
            {
                swipe[(int)Direction.Down] = swipeDelta.y < 0;
                swipe[(int)Direction.Up] = swipeDelta.y > 0;
            }
            SendSwipe();
        }
    }
    void SendSwipe()
    {
        if(swipe[0] || swipe[1] || swipe[2] || swipe[3])
        {
            Debug.Log(swipe[0] + "|"+swipe[1] + "|" + swipe[2] + "|" + swipe[3]);
            if(MoveEvent != null) MoveEvent(swipe);
        }
        else
        {
            Debug.Log("Click");
            ClickEvent?.Invoke(TouchPosition());
        }
        Reset();
    }
    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        touchMoved = false;
        for( int i = 0; i < 4; i++) { swipe[i] = false; }
    }
}
