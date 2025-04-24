using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Speed of the paddle movement
    [SerializeField] private float XLimit = 7.5f; // X-axis limit for the paddle movement

    private Vector3 initialPosition;

    public bool isAllowedToMove;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if(!isAllowedToMove) return; // Early exit if not allowed to move

        Vector2 moveDirection = Vector2.zero;

#if UNITY_ANDROID || UNITY_EDITOR                              
        // Android Controls (Touch)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            float touchX = Touchscreen.current.primaryTouch.position.x.ReadValue();
            moveDirection = (touchX < Screen.width / 2f) ? Vector2.left : Vector2.right;
        }
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        // PC Controls
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            moveDirection = Vector2.left;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveDirection = Vector2.right;
        }
#endif


        transform.Translate(moveDirection * speed * Time.deltaTime);
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp(newPos.x, -XLimit, XLimit);
        transform.position = newPos;
    }

    public void ResetPosition()
    {
        isAllowedToMove = false;
        transform.position = initialPosition;
    }
}
