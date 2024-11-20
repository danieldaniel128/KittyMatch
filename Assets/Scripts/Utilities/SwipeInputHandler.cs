using UnityEngine;
using UnityEngine.Events;

public class SwipeInputHandler : MonoBehaviour
{
    public UnityEvent<Vector2Int> OnSwipe; // Event for swiping, sends direction (Vector2Int)
    public UnityEvent OnTap;     // Event for tapping, sends world position (Vector3)

    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private bool _isTouching = false;

    [SerializeField] private float swipeThreshold = 50f; // Minimum distance for swipe detection

    private void Update()
    {
        DetectInput();
    }

    private void DetectInput()
    {
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startTouchPosition = touch.position;
                _isTouching = true;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _endTouchPosition = touch.position;
                _isTouching = false;
                ProcessInput();
            }
        }
        // Handle mouse input (for testing on PC)
        else if (Input.GetMouseButtonDown(0))
        {
            _startTouchPosition = Input.mousePosition;
            _isTouching = true;
        }
        else if (Input.GetMouseButtonUp(0) && _isTouching)
        {
            _endTouchPosition = Input.mousePosition;
            _isTouching = false;
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        if (Vector2.Distance(_startTouchPosition, _endTouchPosition) < swipeThreshold)
        {
            // Treat it as a tap
            OnTap?.Invoke();
            return;
        }
        Vector2 swipeDirection = _endTouchPosition - _startTouchPosition;
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            // Horizontal swipe
            if (swipeDirection.x > 0)
                OnSwipe?.Invoke(Vector2Int.right); // Swipe right
            else
                OnSwipe?.Invoke(Vector2Int.left); // Swipe left
        }
        else
        {
            // Vertical swipe
            if (swipeDirection.y < 0)
                OnSwipe?.Invoke(Vector2Int.up); // Swipe up
            else
                OnSwipe?.Invoke(Vector2Int.down); // Swipe down
        }
    }
}
